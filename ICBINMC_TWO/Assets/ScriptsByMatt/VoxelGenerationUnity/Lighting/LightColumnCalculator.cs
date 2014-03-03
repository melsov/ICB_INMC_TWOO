using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightColumnCalculator 
{
	private LightColumnMap m_lightColumnMap = new LightColumnMap();
	
	private static Coord NoisePatchDims = NoisePatch.patchDimensions;
	private static PTwo NoisePatchDimxz = PTwo.PTwoXZFromCoord(NoisePatchDims);
	private static Quad PatchQuad = new Quad(new PTwo(0), NoisePatchDimxz);
	
	public LightColumnMap lightColumnMap {
		get {
			return m_lightColumnMap;
		}
	}
	
	private List<LightColumn> surfaceExposedColumns = new List<LightColumn>();
	
	#region query

	public float ligtValueAtPatchRelativeCoord(Coord relCo) //, Direction faceDirection)
	{
		LightColumn lilc = m_lightColumnMap.columnContaining(relCo);
		if (lilc == null) {
			return 0f;
		}
		
		return (float) lilc.lightLevel;
	}
	#endregion
	
	//should be able to
	// update with new surface height...
	public void updateWindowsWithNewSurfaceHeight(int newHeight, int xx, int zz)
	{
		bool lightened = false;
		bool darkened = false;
		List<LightColumn> needUpdateCols = new List<LightColumn>();
		foreach(PTwo neighborPoint in DirectionUtil.SurroundingPTwoCoordsFromPTwo(new PTwo(xx,zz)))
		{
			//inefficient
			DiscreteDomainRangeList<LightColumn> lilcoms = m_lightColumnMap[neighborPoint];
			
			if (lilcoms == null)
				continue;
			
			for(int i = 0; i < lilcoms.Count; ++i)
			{
				LightColumn lilc = lilcoms[i];
				bool wasAboveSurface = surfaceExposedColumns.Contains(lilc);
				if (lilc.range.extent() > newHeight)
				{
					if (!wasAboveSurface)
					{
						lightened = true;
						surfaceExposedColumns.Add(lilc);
						needUpdateCols.Add(lilc);
					}
				} else {
					if (wasAboveSurface)
					{
						darkened = true;
						surfaceExposedColumns.Remove(lilc);
					}
				}
			}
		}
		
		//for now
		if (darkened)
		{
			Quad area = LightDomainAround(xx,zz);
			resetLightColumnsWithin(area);
			updateColumnsWithin(area);
		}
		else if (lightened)// || darkened)
		{
			updateColumnsWithin(LightDomainAround(xx,zz), needUpdateCols);
		}
		
	}
	
//	public
	
	public void updateWindowsWithHeightRangesAndUpdateLight(List<Range1D> heightRanges, int x, int z, SurroundingSurfaceValues ssvs, int blockUpdatedAtY, bool solidBlockRemoved)
	{
		updateWindowsWithHeightRanges(heightRanges, x,z,ssvs, blockUpdatedAtY,solidBlockRemoved, true);
	}
	
	public void updateWindowsWithHeightRanges(List<Range1D> heightRanges, int x, int z, SurroundingSurfaceValues ssvs, int blockUpdatedAtY, bool solidBlockRemoved)
	{
		updateWindowsWithHeightRanges(heightRanges, x,z,ssvs, blockUpdatedAtY,solidBlockRemoved, false);
	}
	
	// update a list of columns with a list of height ranges 
	// cribbed from window map...
	private void updateWindowsWithHeightRanges(List<Range1D> heightRanges, int x, int z, 
		SurroundingSurfaceValues ssvs, int blockUpdatedAtY, bool solidBlockRemoved, bool shouldPropagateLight)
	{
		//TODO: MAKE SHOULD PROPAGATE
		List<SimpleRange> newDiscontins = discontinuitiesFromHeightRanges(heightRanges);
		DiscreteDomainRangeList<LightColumn> discreteRangesXZ = m_lightColumnMap[x,z];
		
		if (discreteRangesXZ == null)
			return; // maybe want dif behavior later
		
//		List<LightColumn> prevLCols = prevLDiscrete.toList();
//		List<SimpleRange> prevDiscontins = simpleRangeListFromLightColumnList(prevLCols);
//		List<SimpleRange> newDiscontinsPruned = rangesRepresentingChangedLightColumnsWith(newDiscontins, prevDiscontins);
		
		//TODO: make more efficient (the above and below)
		// (GET INDICES THAT ARE IN BOTH OLD AND NEW LISTS THAT REPRESENT CHANGES)
		bool needUpdateColumnXZDarkened = false;
		bool needUpdateColumnXZLightened = false;
		
		int i = 0;
		for (;discreteRangesXZ != null && i < discreteRangesXZ.Count && newDiscontins != null && i < newDiscontins.Count; ++i)
		{
			LightColumn prevLCL = discreteRangesXZ[i];
			SimpleRange newDisRange = newDiscontins[i];
			
			if (prevLCL.range.Equals(newDisRange)) {
				continue;
			}
			
			bool prevRangeWasAboveSurface = surfaceExposedColumns.Contains(prevLCL); // RangeAboveSurface(prevRange);
			bool newRangeIsAboveSurface = RangeAboveSurface(newDisRange, ssvs);
			SimpleRange prevRange = prevLCL.range;

			// check if the new range is above surface
			if (newRangeIsAboveSurface)
			{
				if (!prevRangeWasAboveSurface)
				{
					needUpdateColumnXZLightened = true;
					prevLCL.range = newDisRange;
					discreteRangesXZ[i] = prevLCL;
					surfaceExposedColumns.Add(prevLCL);
					continue;
				}
			}
			
			//check if old range was above surface (was in exposed list)
			if (prevRangeWasAboveSurface)
			{
				if (!newRangeIsAboveSurface)
				{
					needUpdateColumnXZDarkened = true;
					bool didRemove = surfaceExposedColumns.Remove(prevLCL);
					prevLCL.range = newDisRange;
					discreteRangesXZ[i] = prevLCL;
//					AssertUtil.Assert(didRemove, "hmmm"); // should have been if we were editing
					continue;
				}
			}
			
			//for now
			needUpdateColumnXZDarkened = needUpdateColumnXZLightened = true;
			prevLCL.range = newDisRange;
//			stoppedAtIndex = i;
//			break;
			
			// if so remove from list
			// update darkened needed
			// continue
			
			// get new neighbors and old neighbors.
			// are they exactly the same neightbors?
			// are there some cases where we don't need
			// to change so much about the whole map?
			
//			foreach(Direction dir in DirectionUtil.TheDirectionsXZ()) {
//				
//			}
		}
		
		if (i < newDiscontins.Count)
		{
			for(int k = i; discreteRangesXZ != null && k < discreteRangesXZ.Count; ++k)
			{
				discreteRangesXZ.RemoveAt(k);
				k--;
			}
			// new light column
			for(int j = i; j < newDiscontins.Count; ++j)
			{
				LightColumn liCol = new LightColumn(new PTwo(x,z), 0, newDiscontins[j]);
				discreteRangesXZ.Add(liCol);
				if (RangeAboveSurface(liCol.range, ssvs))
				{
					surfaceExposedColumns.Add(liCol);
				}
			}
		}
		
		m_lightColumnMap[x,z] = discreteRangesXZ;
		
		if (!shouldPropagateLight)
			return;
		// for now
		if (needUpdateColumnXZDarkened || needUpdateColumnXZLightened)
		{
			// zero out if darken
			// update around xz
			Quad area = LightDomainAround(x,z);
			resetLightColumnsWithin(area);
			updateColumnsWithin(area);
		}
	}
	
	public void calculateLight()
	{
		updateAllNoisePatchColumns();
	}
	
	public DiscreteDomainRangeList<LightColumn> getLightColumnsAt(PTwo patchRelPoint)
	{
		return m_lightColumnMap[patchRelPoint];
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
	
	#region update columns
	
	private void updateAllNoisePatchColumns()
	{
		updateColumnsWithin(PatchQuad);
	}
	
	private void updateColumnsWithin(Quad area)
	{
		//maybe faster if we do these first?
		updateColumnsWithin(area, surfaceExposedColumns);
		
		updateWithPerimeterSurrounding(area);
	}
	
	private void resetLightColumnsWithin(Quad area)
	{
		SimpleRange srange = area.sSimpleRange();
		SimpleRange trange = area.tSimpleRange();
		for(int i = srange.start; i < srange.extent(); ++i)
		{
			for(int j = trange.start; j < trange.extent(); ++j)
			{
				DiscreteDomainRangeList<LightColumn> lilcoms = m_lightColumnMap[i,j];
				if (lilcoms == null)
					continue;
				for(int k = 0; k < lilcoms.Count; ++k)
				{
					lilcoms[k].resetLightLevel();
				}
			}
		}
	}
	
	// make all cols OUTSIDE of a perimeter 'meet up' with all cols
	// INSIDE a perimeter described by @param area
	private void updateWithPerimeterSurrounding(Quad area)
	{
		foreach(Direction dir in DirectionUtil.TheDirectionsXZ())
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
			lightcol.setLightLevelToMax();
			tookInfluence = true;
		} else {
			tookInfluence = lightcol.takeInfluenceFromColumn(influencerColumn);
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
	
	
}
