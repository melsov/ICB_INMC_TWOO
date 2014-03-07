using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//public interface iLightProvider
//{
//	float ligtValueAtPatchRelativeCoord(Coord relCo);
//	void updateWindowsWithNewSurfaceHeight(int newHeight, int xx, int zz);
//}

public enum ChangeOfLightStatus
{
	NO_CHANGE, LIGHTENED, DARKENED
}

public class LightColumnCalculator 
{
	private LightColumnMap m_lightColumnMap; // = new LightColumnMap();
	
	private static Coord NoisePatchDims = NoisePatch.patchDimensions;
	private static PTwo NoisePatchDimxz = PTwo.PTwoXZFromCoord(NoisePatchDims);
	private static Quad PatchQuad = new Quad(new PTwo(0), NoisePatchDimxz);
	
	private NoisePatch m_noisePatch;
	
	public NoiseCoord noiseCoord {
		get {
			return this.m_noisePatch.coord;
		}
	}
	
	private int debugTimeDrewDebugColm = 0;
	
	private NoiseCoord debugOrigNoiseCoord;
	
	public LightColumnMap lightColumnMap {
		get {
			return m_lightColumnMap;
		}
	}
	
	public LightColumnCalculator(NoisePatch npatch)
	{
		m_noisePatch = npatch;
		debugOrigNoiseCoord = m_noisePatch.coord;
		this.m_lightColumnMap = new LightColumnMap(this);
	}
	
	private List<LightColumn> surfaceExposedColumns = new List<LightColumn>();
	
	#region query

	public float lightValueAtPatchRelativeCoord(Coord relCo)
	{
		LightColumn lilc = m_lightColumnMap.columnContaining(relCo);
		
		if (lilc == null) {
			return 0f; // Window.LIGHT_LEVEL_MAX; //TEST// 0f;
		}
//		addColumnIfNoiseCoordOO(lilc); //DBG
		
//		if (lilc.lightLevel > 17f)
//			this.addDebugColumnIfNoiseCoZIsNeg(lilc, (int)(lilc.lightLevel / 3f));

		return (float) lilc.lightLevel; //WANT
	}
	#endregion
	
	#region public add / update
	
	//should be able to
	// update with new surface height...
	public void updateWindowsWithNewSurfaceHeight(int newHeight, int xx, int zz, bool removedABlock)
	{
		ChangeOfLightStatus lightChangeStatus = ChangeOfLightStatus.NO_CHANGE;
		List<LightColumn> needUpdateCols = new List<LightColumn>();
		
		b.bug("upd with new surf height");
		//REMOVE COLUMNS IF COLUMN REVEALED
		if (removedABlock)
		{
			b.bug("remove cols above  at x: " + xx + " zz: " + zz + "height: " + newHeight);
			m_lightColumnMap.removeColumnsAbove(xx, newHeight, zz);
			ChunkManager.debugLinesAssistant.clearColumns(); //DBG
		}
		
		checkSurfaceStatusAt(newHeight, xx, zz, ref lightChangeStatus, ref needUpdateCols);
		
		//for now
		if (lightChangeStatus == ChangeOfLightStatus.DARKENED)
		{
			Quad area = LightDomainAround(xx,zz);
			b.bug("got darken by surface change. reset area" + area.toString());
			resetAndUpdateColumnsWithin(area); //TEST WANT
		}
		else if (lightChangeStatus == ChangeOfLightStatus.LIGHTENED)
		{
			b.bug("got lightened");
			updateColumnsWithin(LightDomainAround(xx,zz), needUpdateCols);
		}
	}
	
	public void updateLightWith(List<Range1D> heightRanges, 
		Coord blockChangedpRelCo, 
		SurroundingSurfaceValues ssvs,  
		bool solidBlockRemoved)
	{
		updateWindowsWithHeightRanges(heightRanges, blockChangedpRelCo ,ssvs,solidBlockRemoved, true);
	}
	
	public void replaceColumnsWithHeightRanges(List<Range1D> heightRanges, int x, int z, SurroundingSurfaceValues ssvs)
	{
		int countAt = m_lightColumnMap.countAt(x,z);
		if (countAt > 0)
		{
			//clobber columns here.
			m_lightColumnMap.clearColumnsAt(x,z);
			clearSurfaceExposedListAt(x,z);
		}

		//add new discons
		int lowestSurroundingSurfaceHeight = ssvs.lowestValue();
		
		ssvs.debugAssertLowestNonNegFound(); //DBG
		
//		AssertUtil.Assert(lowestSurroundingSurfaceHeight > 0, "weird. lowest sur value is: " + lowestSurroundingSurfaceHeight);
		
//		AssertUtil.Assert(NoiseCoord.Equal(this.debugOrigNoiseCoord, m_noisePatch.coord ), "not equal? nco " 
//			+ m_noisePatch.coord.toString() + " and orig??" + debugOrigNoiseCoord.toString()); //fails
		
		List<SimpleRange> discons = discontinuitiesFromHeightRanges(heightRanges);
		
		int light = 0;
		
		LightColumn col = null;
		bool exposedLightColumn = false;
		int countDisCon = discons.Count; //DBG
		foreach(SimpleRange discon in discons)
		{
			exposedLightColumn = discon.extent() > lowestSurroundingSurfaceHeight;
			light = 0; // discon.extent() > lowestSurroundingSurfaceHeight ? (int) Window.LIGHT_LEVEL_MAX_BYTE : 0;
			col = new LightColumn(new PTwo(x,z), (byte) light, discon);
			if (exposedLightColumn)
			{
				//DBG
//				if (countDisCon > 1) {
//					bugIf0Neg1( " discon extent: " + discon.extent() + " lowest Sur Height: " + lowestSurroundingSurfaceHeight + " x: " + x + " z: " + z);
//				}
				
				surfaceExposedColumns.Add(col);
				
//				this.addDebugColumnIfNoiseCoZIsNeg(col, 4);
			}
			m_lightColumnMap.addColumnAt(col, x,z);
		}
	}

	
	#endregion
	
	// update a list of columns with a list of height ranges 
	// cribbed from window map...
	// TODO: check surface status here as well.
	// could have changed...
	
	// many things to do:
	// check if any new ranges are cut-off or
	// exposed to:
	// other ranges
	// the surface.
	// can deal with surrounding ranges separately where surface is concerned...
	private void updateWindowsWithHeightRanges(List<Range1D> heightRanges, Coord pRelCoord, 
		SurroundingSurfaceValues ssvs,  bool solidBlockRemoved, bool shouldPropagateLight)
	{
		b.bug("update with ranges");
		if (solidBlockRemoved)
		{
			updateWindowsWithHeightRangesSolidRemoved(heightRanges, pRelCoord, ssvs, shouldPropagateLight);
			ChunkManager.debugLinesAssistant.clearColumns(); //DBG
			return;
		}
		
		updateWindowsWithHeightRangesSolidAdded(heightRanges, pRelCoord,ssvs, shouldPropagateLight);
		
		ChunkManager.debugLinesAssistant.clearColumns(); //DBG
	}
	
	#region solid block removed
	
	private void updateWindowsWithHeightRangesSolidRemoved(List<Range1D> heightRanges, Coord pRelCoord, 
		SurroundingSurfaceValues ssvs,  bool shouldPropagateLight)
	{
		int x = pRelCoord.x, y = pRelCoord.y, z = pRelCoord.z;
		DiscreteDomainRangeList<LightColumn> liCols = m_lightColumnMap[x,z];
		
		LightColumn colJustBelowYBeforeUpdate = liCols.rangeContaining(y - 1); //null if there wasn't
		
		Range1D highest = heightRanges[heightRanges.Count - 1];
		int newSurfaceHeightExtentAtXZ = highest.extent();
		
		if (colJustBelowYBeforeUpdate != null) {
			colJustBelowYBeforeUpdate = colJustBelowYBeforeUpdate.Copy(); // safe keeping
		}

		updateRangeListAtXZ(heightRanges, pRelCoord, true, ssvs);
		
		//case where y represents a removed top block 
		// where either air or solid was underneath the
		// former top block
		if (newSurfaceHeightExtentAtXZ <= y)
		{
			//no column here anymore
			//surface was revealed.
			//add any adjacent blocks to exposed list
			// if they weren't on the list before
			// add them to need update list
			// our work is done...return (put the above in a function)
			
			b.bug("got new surf less than y");
			Coord surfaceTopCoord = pRelCoord;
			if (newSurfaceHeightExtentAtXZ < y) {
				b.bug("is fully less than");
				surfaceTopCoord.y = newSurfaceHeightExtentAtXZ;
			}
			
			foreach(LightColumn col in  m_lightColumnMap.lightColumnsAdjacentToAndAtleastPartiallyAbove(surfaceTopCoord))
			{
				b.bug("updating ");
				addToSurfaceExposedColumnIfNotContains(col);
				col.lightLevel = Window.LIGHT_LEVEL_MAX_BYTE;
				updateColumnAt(col, col.coord, null, LightDomainAround(col.coord));
			}
			
			m_lightColumnMap[x,z] = liCols;
			
			return;
		}
		
		//REMAINING CASES: Y REMOVED FROM BELOW SURFACE
		
		int lowestSurroundingSurface = ssvs.lowestValue();
		
		LightColumn colAtY = liCols.rangeContaining(y);
		
		AssertUtil.Assert(colAtY != null, "(remove block. didn't get a col at y in lc calc) y was: " + y);
		
		
		if (y > lowestSurroundingSurface)	//just update it. anything new will update. anything not new won't.
		{
			//one way or another a newly exposed column here
			addToSurfaceExposedColumnIfNotContains(colAtY);
			updateColumnAt(colAtY, colAtY.coord, null, LightDomainAround(x,z));
			return;
		}
		
		//REMAINING CASES: Y WAS BELOW THE SURFACE AND NOT EXPOSED BY ADJACENT SURFACE.
		List<LightColumn> justConnectedWithColumns = columnsThatColumnJustConnectedWith(colAtY, y);
		if (justConnectedWithColumns.Count == 0)
			return;
		
		LightColumn lightest = colAtY;

		bool atYIsLightest = true;
		foreach(LightColumn justConnectedCol in justConnectedWithColumns)
		{
			if (justConnectedCol.lightLevel > lightest.lightLevel)
			{
				lightest = justConnectedCol;
				atYIsLightest = false;
			} 
		}
		
		if (atYIsLightest)
		{
			foreach(LightColumn adjCol in justConnectedWithColumns)
				updateColumnAt(adjCol, adjCol.coord, colAtY, LightDomainAround(adjCol.coord));
		} else {
			updateColumnAt(colAtY, colAtY.coord, lightest, LightDomainAround(colAtY.coord));
		}
	}
	
	#endregion
	
	#region solid block added
	
	private void updateWindowsWithHeightRangesSolidAdded(List<Range1D> heightRanges, Coord pRelCoord, 
		SurroundingSurfaceValues ssvs,  bool shouldPropagateLight)
	{
		Range1D highest = heightRanges[heightRanges.Count - 1];
		int newSurfaceHeightAtXZ = highest.extent();
		bool yIsNewHighestSurface = pRelCoord.y == newSurfaceHeightAtXZ - 1;
		
		b.bug("y is " + pRelCoord.y + " new surf height minus one: " + (newSurfaceHeightAtXZ - 1) );
		// WAS A FORMERLY SURFACE EXPOSED SURFACE COLUMN JUST COVERED?
		if (yIsNewHighestSurface )
		{
			updateColumnsWithHigherHighestSurface(heightRanges, pRelCoord, ssvs, shouldPropagateLight);
			return;
		}
		
		updateColumnsSolidAddedBelowSurface(heightRanges, pRelCoord, ssvs, shouldPropagateLight);
	}
	
	private void updateColumnsWithHigherHighestSurface(List<Range1D> heightRanges, Coord pRelCoord, SurroundingSurfaceValues ssvs, bool shouldPropagateLight)
	{
		b.bug("update w higher highest surf");
		int x = pRelCoord.x, y = pRelCoord.y, z = pRelCoord.z;
		SimpleRange justCoveredColumn = new SimpleRange(y, 1);
		Range1D highest = heightRanges[heightRanges.Count - 1];
		LightColumn newlico = null;
		int lowestSurroundingSurface = ssvs.lowestValue();
		if (highest.range == 1)
		{
			AssertUtil.Assert(heightRanges.Count > 1, "what we just 'covered' bedrock?");
			Range1D secondHighest = heightRanges[heightRanges.Count - 2 ];
			justCoveredColumn = new SimpleRange( secondHighest.extent(), y + 1 - secondHighest.extent()); //(extent - start)
			
			//new column
			SimpleRange newColRange = justCoveredColumn;
			newColRange.range--;

			newlico = new LightColumn(PTwo.PTwoXZFromCoord(pRelCoord), 0, newColRange );
			m_lightColumnMap.addColumnAt(newlico, x,z);
			
			if (newColRange.extent() > lowestSurroundingSurface)
			{
				newlico.lightLevel = Window.LIGHT_LEVEL_MAX_BYTE;
				surfaceExposedColumns.Add(newlico);
			}
		}
		
		List<LightColumn> adjColsFlushToJustCovered = m_lightColumnMap.lightColumnsAdjacentToAndFlushWithSimpleRangeAndPoint(justCoveredColumn, new PTwo(x,z));
		foreach(LightColumn adjcol in adjColsFlushToJustCovered)
		{
			//adj cols could still be above
			if (adjcol.extent() <= y)
			{
				//get surrounding surface for these?
				SurroundingSurfaceValues assvs = m_noisePatch.surfaceValuesSurrounding(adjcol.coord);
				if (adjcol.extent() <= assvs.lowestValue() )
				{
					surfaceExposedColumns.Remove(adjcol);
					adjcol.lightLevel = 0;
					// put back?
				}
			}
		}
		
		resetAndUpdateColumnsWithin(LightDomainAround(x,z));
	}
	
	private void updateColumnsSolidAddedBelowSurface(List<Range1D> heightRanges, Coord pRelCoord, 
		SurroundingSurfaceValues ssvs,  bool shouldPropagateLight)
	{
		
		b.bug("adding below surface");
		int x = pRelCoord.x, y = pRelCoord.y, z = pRelCoord.z;
		DiscreteDomainRangeList<LightColumn> licols = m_lightColumnMap[x,z];
		
		LightColumn prevColContainingY = licols.rangeContaining(y);
		
		AssertUtil.Assert(prevColContainingY != null, "confusing. how did we add a block not in the area of a light colm under the surface?");
		
		List<LightColumn> columnsJustDisconnected = this.columnsThatColumnJustConnectedWith(prevColContainingY, y);
		
		LightColumn aboveSplit = licols.Incorporate(y, false);
		if (aboveSplit != null)
		{
			aboveSplit.coord = new PTwo(x,z);
		}
				
		LightColumn newBelow = licols.rangeContaining(y - 1);
		LightColumn newAbove = licols.rangeContaining(y + 1);
		
		if (newBelow == null && newAbove == null)// COLUMNS WERE CUT OFF?
		{
			b.bug("got both y above and below null");
			// general reboot
			resetAndUpdateColumnsWithin(LightDomainAround(PTwo.PTwoXZFromCoord(pRelCoord)));
			return;
		}
		
		updateColumnOrLightestNeighbor(newBelow);
		
		updateColumnOrLightestNeighbor(newAbove);
		
		
		
		foreach(LightColumn colm in columnsJustDisconnected)
		{
			updateColumnOrLightestNeighbor(colm);
		}
		
	}
	
	#endregion
	
	private void updateColumnOrLightestNeighbor(LightColumn col)
	{
		if (col != null)
		{
			LightColumn lightestN = lightestNeighbor(col);
			if (lightestN != null)
			{
				if (lightestN.lightLevel > col.lightLevel)
				{
					updateColumnAt(col, col.coord, lightestN, LightDomainAround(col.coord));
				} else {
					updateColumnAt(lightestN, lightestN.coord, col, LightDomainAround(lightestN.coord));
				}
			} else {
				col.lightLevel = 0;
			}
			
			m_lightColumnMap.addReplaceColumn(col);
		}
	}
	
	private LightColumn lightestNeighbor(LightColumn col)
	{
		return lightestNeightbor(col.range, col.coord);
	}
	
	private LightColumn lightestNeightbor(SimpleRange colRange, PTwo co)
	{
		LightColumn lightest = null;
		
		List<LightColumn> neighbors = m_lightColumnMap.lightColumnsAdjacentToAndFlushWithSimpleRangeAndPoint(colRange, co);
		
		if (neighbors.Count == 0)
			return null;
		
		lightest = neighbors[0];
		
		//CONSIDER: could be a little faster if we iterated over directions ourselves here...
		for(int i = 1; i < neighbors.Count; ++i)
		{
			LightColumn neicol = neighbors[i];
			if (neicol.lightLevel > lightest.lightLevel)
			{
				lightest = neicol;
			}
		}
		
		return lightest;
		
	}
	
	private void addToSurfaceExposedColumnIfNotContains(LightColumn col)
	{
		if (!surfaceExposedColumns.Contains(col))
			surfaceExposedColumns.Add(col);
	}
	
	private bool columnContains(LightColumn lcol , int y)
	{
		if (lcol == null)
			return false;
		
		return lcol.contains(y);
	}
		
	private void clearSurfaceExposedListAt(int x, int z)
	{
		//TODO make faster collection? another lightColumnMap maybe?
		for(int i = 0; i < surfaceExposedColumns.Count; ++i)
		{
			LightColumn col = surfaceExposedColumns[i];
			if (col.coord.s == x && col.coord.t == z)
			{
				surfaceExposedColumns.RemoveAt(i);
				--i;
			}
		}
	}
	
		
	private List<LightColumn> columnsThatColumnJustConnectedWith(LightColumn colAtY, int y) 
	{
		List<LightColumn> result = new List<LightColumn>();
		
		bool colExtentMinusOneEqualY = colAtY.extent() - 1 == y;
		bool colStartEqualY = colAtY.startP == y;
		
		foreach (PTwo co in DirectionUtil.SurroundingPTwoCoordsFromPTwo(colAtY.coord) )
		{
			DiscreteDomainRangeList<LightColumn> licols = m_lightColumnMap[co];
			LightColumn	justConnected = null;
			if (colExtentMinusOneEqualY) {
				justConnected = licols.rangeWithStartEqual(y);
			} else if (colStartEqualY) {
				justConnected = licols.rangeWithExtentMinusOneEqual(y);
			} else {
				justConnected = licols.rangeWithStartAndExtentMinusOneEqual(y);
			}
			
			if (justConnected != null)
				result.Add(justConnected);
		}
		return result;
	}
	
	private List<LightColumn> columnsAdjacentTo(Coord pco)
	{
		List<LightColumn> result = new List<LightColumn>();
		
		foreach(Direction dir in DirectionUtil.TheDirectionsXZ())
		{
			Coord adjCo = pco + DirectionUtil.NudgeCoordForDirection(dir);
			DiscreteDomainRangeList<LightColumn> adjLicols = m_lightColumnMap[pco.x, pco.z];
			LightColumn lico = adjLicols.rangeContaining(pco.y);
			if (lico != null)
				result.Add(lico);
		}
		return result;
	}
	
	private void updateRangeListAtXZ(List<Range1D> heightRanges, Coord pRelCoord, bool solidBlockRemoved, SurroundingSurfaceValues ssvs)
	{
		DiscreteDomainRangeList<LightColumn> liCols = m_lightColumnMap[pRelCoord.x, pRelCoord.z];
		
		if(!solidBlockRemoved && heightRanges.Count <= 1) // equal zero would be down right silly.
			return;
		// y above highest extent?
		int highestDiscontinuity = liCols.highestExtent();
		
		int lowestSurroundingLevel = ssvs.lowestValue();
		//CASE WHERE A BLOCK IS ADDED TO THE TOP OF OR OVER TOP OF THE SURFACE
		if (pRelCoord.y > highestDiscontinuity && !solidBlockRemoved)
		{
			// Y MUST BE THE TOP BLOCK?
			int hRangeCount = heightRanges.Count;

			SimpleRange between = discontinuityBetween(heightRanges[hRangeCount - 2], heightRanges[hRangeCount - 1]);
			if (!between.isErsatzNull())
			{
				AssertUtil.Assert(pRelCoord.y == between.extentMinusOne(), "confused. y is " + pRelCoord.y + " between range is " + between.toString());
				int lightValue = pRelCoord.y >= lowestSurroundingLevel? (int) Window.LIGHT_LEVEL_MAX_BYTE : 0;
				LightColumn licol = new LightColumn(PTwo.PTwoXZFromCoord(pRelCoord),(byte) lightValue,between);
				liCols.Add(licol);
				return; // licol;
			}			
			throw new Exception("don't want to be here. we think a block couldn't be placed vertically in between at this point");
		}
		
		//TODO handle case where y is above surface, solid block removed
		// need to destroy some discon ranges (not incorporate y).
		Range1D highest = heightRanges[heightRanges.Count - 1];
		if (pRelCoord.y > highest.extent() )
		{
			liCols.RemoveStartAbove(highest.extent());
		}
		else
		{
			LightColumn newCol = liCols.Incorporate(pRelCoord.y, solidBlockRemoved);
			if (newCol != null)
			{
				newCol.coord = new PTwo(pRelCoord.x, pRelCoord.z);
			}
//			throw new Exception("lots of exceptions. we failed to incorp a solid block added.");
		} 
		b.bug("we incorporated a pRelCOord: " + pRelCoord.toString() + "solid removed was: "+ solidBlockRemoved);
		
		m_lightColumnMap[pRelCoord.x, pRelCoord.z ] = liCols;
	}

	
	#region calc light
	
	public void calculateLight()
	{
	
		
		updateAllNoisePatchColumns();
	}
	
	#endregion
	
	#region public lookup
	
	public DiscreteDomainRangeList<LightColumn> getLightColumnsAt(PTwo patchRelPoint)
	{
		return m_lightColumnMap[patchRelPoint];
	}
	
	#endregion
	
	private static Quad LightDomainAround(PTwo point)
	{
		return LightDomainAround(point.s, point.t);
	}
	
	private static Quad LightDomainAround(int x, int z)
	{
		PTwo midPoint = new PTwo(x,z);
		PTwo lightTravel = new PTwo((int)Window.LIGHT_TRAVEL_MAX_DISTANCE);
		return Quad.QuadWithOriginAndExtent(midPoint - lightTravel, midPoint + lightTravel);
	}
	
	private static bool RangeAboveSurface(SimpleRange range, SurroundingSurfaceValues ssvs)
	{
		foreach(Direction dir in DirectionUtil.TheDirectionsXZ())
		{
			int height = ssvs.valueForDirection(dir);
			if (height < range.extent())
			{
				return true;
			}
		}
		return false;
	}
	
	
	private static List<SimpleRange> simpleRangeListFromLightColumnList(List<LightColumn> lcolms)
	{
		List<SimpleRange> result = new List<SimpleRange>();
		foreach(LightColumn lilc in lcolms)
		{
			result.Add(lilc.range);
		}
		return result;
	}
	
	
	private static List<SimpleRange> discontinuitiesFromHeightRanges(List<Range1D> heightRanges)
	{
		List<SimpleRange> result = new List<SimpleRange>();
		for(int i = 1; i < heightRanges.Count; ++i)
		{
			SimpleRange between = discontinuityBetween(heightRanges[i - 1], heightRanges[i]);
			if (!between.isErsatzNull())
			{
				result.Add(between);
			}
		}
		return result;
	}
	
	private static SimpleRange discontinuityBetween(Range1D belowRange, Range1D aboveRange)
	{
		int gap = aboveRange.start - belowRange.extent();
		if (gap > 0)
		{
			return new SimpleRange(belowRange.extent(), gap);
		}

//		throw new Exception("good to know if this happens. discon between got a null range. no gap?");
		
		return SimpleRange.theErsatzNullRange();
	}
	
	// TWO main things to watch for:
	// 1. column/coord got edited --or conversely--
	// NO COLUMNS CHANGED.
	// meaning, for exposed columns, still exposed
	// for all columns, still connected to the same neighbor columns
	
	//privately
	// any time there's a change in the status of a lightColumn
	// it should recalc a certain region surrounding that column
	// and should update the whole noisepatchdim the first time...
	
	//efficiencies:
	// if column just turned on,
	// only update "from" it.
	
	private List<SimpleRange> rangesRepresentingChangedLightColumnsWith(List<SimpleRange> updatedColumns, List<SimpleRange> prevColumns)
	{
		//naive check
		for (int i = 0; i < prevColumns.Count && i < updatedColumns.Count; ++i)
		{
			if(updatedColumns[i].Equals(prevColumns[i]))
			{
				updatedColumns.RemoveAt(i);
				prevColumns.RemoveAt(i);
				--i;
			}
		}
		if (updatedColumns.Count <= 1)
			return updatedColumns;
		
		for (int i = 0; i < prevColumns.Count ; ++i)
		{
			for(int j = 0; j < updatedColumns.Count; ++j)
			{
				if(updatedColumns[j].Equals(prevColumns[i]))
				{
					updatedColumns.RemoveAt(j);
					prevColumns.RemoveAt(i);
					--j;
					--i;
					break;
				}
			}
		}
		return updatedColumns;
	}
	
	#region check surface status	
	// this func. represnts our muddled strategy unfortuntly.
	private void checkSurfaceStatusAt(int newHeight, int xx, int zz, ref ChangeOfLightStatus lightChangeStatus, ref List<LightColumn> needUpdateCols)	
	{
		foreach(PTwo neighborPoint in DirectionUtil.SurroundingPTwoCoordsFromPTwo(new PTwo(xx,zz)))
		{
			//inefficient?
			DiscreteDomainRangeList<LightColumn> lilcoms = m_lightColumnMap[neighborPoint];
			b.bug("checking surface...");
			if (lilcoms == null)
				continue;
			b.bug("not ull");
			for(int i = 0; i < lilcoms.Count; ++i)
			{
				LightColumn lilc = lilcoms[i];
				bool wasAboveSurface = surfaceExposedColumns.Contains(lilc);
				if (lilc.range.extent() > newHeight)
				{
					b.bug("extent was greater than new height");
					if (!wasAboveSurface)
					{
						b.bug("added a column now surf exposed");
						lightChangeStatus = ChangeOfLightStatus.LIGHTENED;
						surfaceExposedColumns.Add(lilc);
						needUpdateCols.Add(lilc);
					}
				} else {
					b.bug("below surface new height is: " + newHeight + "lilc range extent is: " + lilc.range.extent());
					if (wasAboveSurface)
					{
						lightChangeStatus = ChangeOfLightStatus.DARKENED;
						needUpdateCols.Add(lilc);
						surfaceExposedColumns.Remove(lilc);
					}
				}
			}
		}
	}
	
	#endregion
	
	#region update columns
	
	private void updateAllNoisePatchColumns()
	{
		updateColumnsWithin(PatchQuad);
	}
	
	private void resetAndUpdateColumnsWithin(Quad area)
	{
		resetLightColumnsWithin(area);
		updateColumnsWithin(area);
	}
	
	private void updateColumnsWithin(Quad area)
	{
		//maybe faster if we do these first?
		updateColumnsWithin(area, surfaceExposedColumns);
		///*
		updateWithPerimeterSurrounding(area);
		//*/ //WANT
	}
	
	private void resetLightColumnsWithin(Quad area)
	{
		SimpleRange srange = area.sSimpleRange();
		SimpleRange trange = area.tSimpleRange();
		
		ChunkManager.debugLinesAssistant.clearColumns(); //DBG
		
		for(int i = srange.start; i < srange.extent(); ++i)
		{
			for(int j = trange.start; j < trange.extent(); ++j)
			{
				DiscreteDomainRangeList<LightColumn> lilcoms = m_lightColumnMap[i,j];
				if (lilcoms == null)
					continue;
				for(int k = 0; k < lilcoms.Count; ++k)
				{
					lilcoms[k].resetLightLevel(); //WANT
				}
			}
		}
	}
	
	public void updateWithColumnsOfNoisePatchInDirection(Direction dir)
	{
		updateQuadBorderInDirection( LightColumnCalculator.PatchQuad , dir );
	}
	
	// make all cols OUTSIDE of a perimeter 'meet up' with all cols
	// INSIDE a perimeter described by @param area
	private void updateWithPerimeterSurrounding(Quad area)
	{
		foreach(Direction dir in DirectionUtil.TheDirectionsXZ())
		{
			this.updateQuadBorderInDirection(area, dir);
		}
	}
	
	private void updateQuadBorderInDirection(Quad area, Direction dir)
	{
		PTwo nudge = DirectionUtil.NudgeCoordForDirectionPTwo(dir);
		Axis axis = DirectionUtil.AxisForDirection(dir);
		PTwo startPoint = area.origin;
		if (DirectionUtil.IsPosDirection(dir) )
		{
			if (axis == Axis.X) 
				startPoint.t += area.dimensions.t;
			else 
				startPoint.s += area.dimensions.s;
		}
		PTwo iterNudge = PTwo.Abs(nudge).flipSAndT();
		int length = axis == Axis.X ? area.dimensions.t : area.dimensions.s;
		PTwo cursorPoint = startPoint;
		for(int i = 0; i < length; ++i)
		{
			DiscreteDomainRangeList<LightColumn> licols = m_lightColumnMap[cursorPoint + nudge];
			if (licols == null)
				continue;
			DiscreteDomainRangeList<LightColumn> insideCols = m_lightColumnMap[cursorPoint];
			if (insideCols == null)
				continue;
			
			for(int j = 0; j < licols.Count; ++j)
			{
				LightColumn outsideCol = licols[j];
				for(int k = 0; k < insideCols.Count; ++k)
				{
					LightColumn insideCol = insideCols[k];
					updateColumnAt(insideCol, insideCol.coord, outsideCol, area);
				}
			}
			
			cursorPoint += iterNudge;
		}
	}
	
	public void updateWithSurfaceHeightAtNeighborBorderInDirection(Direction dir)
	{
		updateSurfaceHeightsInDirection(PatchQuad, dir);
	}
	
	//COPY PASTE OF ABOVE FUNC.!
	private void updateSurfaceHeightsInDirection(Quad area, Direction dir)
	{
//		return;
		
		PTwo nudge = DirectionUtil.NudgeCoordForDirectionPTwo(dir);
		Axis axis = DirectionUtil.AxisForDirection(dir);
		PTwo startPoint = area.origin;
		if (DirectionUtil.IsPosDirection(dir) )
		{
			if (axis == Axis.X) 
				startPoint.t += area.dimensions.t;
			else 
				startPoint.s += area.dimensions.s;
		}
		PTwo iterNudge = PTwo.Abs(nudge).flipSAndT();
		int length = axis == Axis.X ? area.dimensions.t : area.dimensions.s;
		PTwo cursorPoint = startPoint;
		
		List<LightColumn> needUpdateColumns = new List<LightColumn>();
		
		for(int i = 0; i < length; ++i)
		{
//			DiscreteDomainRangeList<LightColumn> licols = m_lightColumnMap[cursorPoint + nudge];
//			if (licols == null)
//				continue;
			DiscreteDomainRangeList<LightColumn> insideCols = m_lightColumnMap[cursorPoint];
			if (insideCols == null)
				continue;
			
			int surfaceHeightJustBeyondBorder = this.m_noisePatch.highestPointAtPatchRelativeCoord(cursorPoint + nudge);
			
			for(int j = 0; j < insideCols.Count; ++j)
			{
				LightColumn insideCol = insideCols[j];
				if (insideCol.heightIsBelowExtent(surfaceHeightJustBeyondBorder) )
				{
					addToSurfaceExposedColumnIfNotContains(insideCol);
					needUpdateColumns.Add(insideCol );
				}
			}
			
			cursorPoint += iterNudge;
		}

		foreach(LightColumn col in needUpdateColumns)		
		{
			updateColumnAt(col, col.coord, null, PatchQuad);
		}
		
	}
	
	//COPY PASTE OF ABOVE FUNC.!
	private void updateSurfaceHeightsInDirection(Quad area, Direction dir)
	{
		PTwo nudge = DirectionUtil.NudgeCoordForDirectionPTwo(dir);
		Axis axis = DirectionUtil.AxisForDirection(dir);
		PTwo startPoint = area.origin;
		if (DirectionUtil.IsPosDirection(dir) )
		{
			if (axis == Axis.X) 
				startPoint.t += area.dimensions.t;
			else 
				startPoint.s += area.dimensions.s;
		}
		PTwo iterNudge = PTwo.Abs(nudge).flipSAndT();
		int length = axis == Axis.X ? area.dimensions.t : area.dimensions.s;
		PTwo cursorPoint = startPoint;

		List<PTwo> result = new List<PTwo>();
		for(int i = 0; i < length; ++i)
		{
			result.Add(cursorPoint + nudge);
			
			cursorPoint += iterNudge;
		}

	}
	
	private void updateColumnsWithin(Quad area, List<LightColumn> surafceLitColumns)
	{
		foreach(LightColumn lcol in surafceLitColumns)
		{
			updateColumnAt(lcol, lcol.coord, null, area);
		}
	}
	
	private void updateColumnAt(LightColumn lightcol, PTwo coord, LightColumn influencerColumn, Quad withinBounds)
	{
		if (!withinBounds.contains(coord))
			return;
		
		bool tookInfluence = false;
		if (influencerColumn == null)
		{
//			addColumnIfNoiseCoordOO(lightcol); //DBG
			lightcol.setLightLevelToMax();
			tookInfluence = true;
		} else {
			tookInfluence = lightcol.takeInfluenceFromColumn(influencerColumn);
//			if (tookInfluence) //DEBUG!!!
//			{
//				this.addDebugColumnIfNoiseCoZIsNeg(lightcol, 4);
//				this.addDebugColumnIfNoiseCoZIsNeg(influencerColumn, 1);
//			}
		}
		if (!tookInfluence)
			return;
		
		List<LightColumn> adjacentNeighbors = m_lightColumnMap.lightColumnsAdjacentToAndFlushWith(lightcol);
		if (adjacentNeighbors == null)
			return;
		
		for(int i = 0; i < adjacentNeighbors.Count; ++i)
		{
			LightColumn adjNeighbor = adjacentNeighbors[i];
			//TEST WANT
			updateColumnAt(adjNeighbor, adjNeighbor.coord, lightcol, withinBounds);
		}

		// touch any connected columns below this light col
		// then check this column done on check list
//		foreach(LightColumn belowCol in m_lightColumnMap[coord].toList() )
//		{
//			if (belowCol.Equals(lightcol))
//				continue;
//			foreach(LightColumn adjCol in adjacentNeighbors) {
//				belowCol.takeInfluenceFromColumn(adjCol);
//				if (belowCol.lightLevel >= lightcol.lightLevel - Window.UNIT_FALL_OFF_BYTE * 2)
//					break;
//			}
//		}
//		columnDoneCheckList[coord.s,coord.t] = true;
		
		
		
	}
	
	#endregion
	
//	private static bool getAlreadyDone(bool[,] colDoneCheckList, PTwo co)
//	{
//		AssertUtil.Assert(! (co.area() >= colDoneCheckList.GetLength(0) * colDoneCheckList.GetLength(1)), "problems with col done checklist out of bounds:" +
//		 	"" + co.toString() + " check list length: " + colDoneCheckList.Length);
//		
//		return colDoneCheckList[co.s, co.t];
//	}
//	
//	private static void setTrueAlreadyDone(ref bool[,] colDoneCheckList, PTwo co)
//	{
//		AssertUtil.Assert(! (co.area() >= colDoneCheckList.GetLength(0) * colDoneCheckList.GetLength(1)), "problems with col done checklist out of bounds:" +
//		 	"" + co.toString() + " check list length: " + colDoneCheckList.Length);
//		
//		colDoneCheckList[co.s, co.t] = true;
//	}
	
	#region debug
	
	void addColumnIfNoiseCoordOO(LightColumn lcol)
	{
		if(NoiseCoord.Equal (this.m_noisePatch.coord, new NoiseCoord(0,0) ))
		{
			ChunkManager.debugLinesAssistant.addColumn(lcol.toColumnDebug(), this.debugTimeDrewDebugColm++, m_noisePatch.coord );
		}
	}
	
		
	private void addDebugColumnIfNoiseCoZIsNeg(LightColumn col, int handyI)
	{
		if (this.m_noisePatch.coord.z < 0)
		{
			int handyInt = handyI;
			ChunkManager.debugLinesAssistant.addColumn(col.toColumnDebug(), handyInt, this.m_noisePatch.coord);
		}
	}
	
	private void bugIf00(string stir)
	{
		if (this.m_noisePatch.coord.x == 0 && m_noisePatch.coord.z == 0)
		{
			b.bug(stir);
		}
	}
	
	private void bugIf0Neg1(string stir)
	{
		if (this.m_noisePatch.coord.x == 0 && m_noisePatch.coord.z == -1)
		{
			b.bug(stir);
		}
	}
	
	#endregion
	

}

//GRAVEYARD

//
//		List<SimpleRange> newDiscontins = discontinuitiesFromHeightRanges(heightRanges);
//		DiscreteDomainRangeList<LightColumn> discreteRangesXZ = m_lightColumnMap[x,z];
//		
//		if (discreteRangesXZ == null)
//			return; // maybe want dif behavior later
//		
////		List<LightColumn> prevLCols = prevLDiscrete.toList();
////		List<SimpleRange> prevDiscontins = simpleRangeListFromLightColumnList(prevLCols);
////		List<SimpleRange> newDiscontinsPruned = rangesRepresentingChangedLightColumnsWith(newDiscontins, prevDiscontins);
//		
//		//TODO: make more efficient (the above and below)
//		// (GET INDICES THAT ARE IN BOTH OLD AND NEW LISTS THAT REPRESENT CHANGES)
//		bool needUpdateColumnXZDarkened = false;
//		bool needUpdateColumnXZLightened = false;
//		
//		int i = 0;
//		for (;discreteRangesXZ != null && i < discreteRangesXZ.Count && newDiscontins != null && i < newDiscontins.Count; ++i)
//		{
//			LightColumn prevLCL = discreteRangesXZ[i];
//			SimpleRange newDisRange = newDiscontins[i];
//			
//			if (prevLCL.range.Equals(newDisRange)) {
//				continue;
//			}
//			
//			bool prevRangeWasAboveSurface = surfaceExposedColumns.Contains(prevLCL); // RangeAboveSurface(prevRange);
//			bool newRangeIsAboveSurface = RangeAboveSurface(newDisRange, ssvs);
//			SimpleRange prevRange = prevLCL.range;
//
//			// check if the new range is above surface
//			if (newRangeIsAboveSurface)
//			{
//				if (!prevRangeWasAboveSurface)
//				{
//					needUpdateColumnXZLightened = true;
//					prevLCL.range = newDisRange;
//					discreteRangesXZ[i] = prevLCL;
//					surfaceExposedColumns.Add(prevLCL);
//					continue;
//				}
//			}
//			
//			//check if old range was above surface (was in exposed list)
//			if (prevRangeWasAboveSurface)
//			{
//				if (!newRangeIsAboveSurface)
//				{
//					needUpdateColumnXZDarkened = true;
//					bool didRemove = surfaceExposedColumns.Remove(prevLCL);
//					prevLCL.range = newDisRange;
//					discreteRangesXZ[i] = prevLCL;
////					AssertUtil.Assert(didRemove, "hmmm"); // should have been if we were editing
//					continue;
//				}
//			}
//			
//			//for now
//			needUpdateColumnXZDarkened = needUpdateColumnXZLightened = true;
//			prevLCL.range = newDisRange;
//			//DBG
//			addColumnIfNoiseCoordOO(prevLCL);
//
//			// get new neighbors and old neighbors.
//			// are they exactly the same neightbors?
//			// are there some cases where we don't need
//			// to change so much about the whole map?
//			
////			foreach(Direction dir in DirectionUtil.TheDirectionsXZ()) {
////				
////			}
//		}
//		
//		if (i < newDiscontins.Count)
//		{
//			for(int k = i; discreteRangesXZ != null && k < discreteRangesXZ.Count; ++k)
//			{
//				discreteRangesXZ.RemoveAt(k);
//				k--;
//			}
//			// new light column
//			for(int j = i; j < newDiscontins.Count; ++j)
//			{
//				LightColumn liCol = new LightColumn(new PTwo(x,z), 0, newDiscontins[j]);
//				discreteRangesXZ.Add(liCol);
//				if (RangeAboveSurface(liCol.range, ssvs))
//				{
//					surfaceExposedColumns.Add(liCol);
//				}
//				
//				//DBG
////				addColumnIfNoiseCoordOO(liCol);
//				
//			}
//		}
//		
//		m_lightColumnMap[x,z] = discreteRangesXZ;
//		
//		if (!shouldPropagateLight)
//			return;
//		// for now
//		if (needUpdateColumnXZDarkened || needUpdateColumnXZLightened)
//		{
//			// zero out if darken
//			// update around xz
//			Quad area = LightDomainAround(x,z);
//			resetLightColumnsWithin(area);
			///  new version of func. is done
//		updateColumnsWithin(area);
//		}


//		bool dontAddColAtY = !colAtYWasBelowSurface && y < lowestSurroundingSurface;
//		List<LightColumn> adjacentCols = columnsAdjacentTo(pRelCoord);
//		if (!dontAddColAtY && adjacentCols.Count == 0)
//		{
//			return;
//		}
//		
//		// an adjacent column now also above surface?
//		if (y >= ssvs.lowestValue()) {
//			foreach(LightColumn col in adjacentCols)
//			{
//				if (col.extent() - 1 == y )
//				{
//					surfaceExposedColumns.Add(col);
//					updateColumnAt(col, col.coord, null, LightDomainAround(col.coord.x, col.coord.z)); 
//				}			
//			}
//		}
//		
//		if (colAtY.extent() > ssvs.lowestValue() && colAtYWasBelowSurface)
//		{
//			surfaceExposedColumns.Add(colAtY);
//			updateColumnAt(colAtY, colAtY.coord, null, LightDomainAround(x,z));
//			return;
//		}
//		
//		updateColumnAt(colAtY, colAtY.coord, adjacentCols[0], LightDomainAround(x,z));