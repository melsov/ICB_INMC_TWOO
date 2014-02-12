#define GLOSS_QUAD_ARRAY_INDEX_BUG
#define LIGHT_BY_RANGE
#define UNDO_LIGHT_BY_RANGE_TEST
//#define IRREGULARITY_LOG
//#define NO_OPTIMIZATION

using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using System;



//public struct Face
//{
//	// has an xpos and zpos neighbor
////	Face * xPosNeighbor; //, zPosNeighbor; // these are unsafe (not allowed in this safe code world??)
//	
//	CTwo ctwo;
//	
//	//?? an enum (later an int to be looked at bitwise?) that tells which walls are intact
//	//for now just four bools
//	bool xnegWall, xposWall, znegWall, zposWall;
//	
//	public Face (CTwo _ctwo) {
////		xPosNeighbor = zPosNeighbor = null;
//		ctwo = _ctwo;
//		xnegWall = xposWall = znegWall = zposWall = true;
//	}
//}


public class FaceSet  
{

	public BlockType blockType;
	public Direction blockFaceDirection;
	
	private List<Strip>[] stripsArray = new List<Strip>[(int)ChunkManager.CHUNKLENGTH];
	
	private Quad faceSetLimits = Quad.theErsatzNullQuad();
#if LIGHT_BY_RANGE
	private List<LitQuad> quads = new List<LitQuad>();
#else
	private List<Quad> quads = new List<Quad>();
#endif
	
	private PTwo quadTableDimensions;
	private int[,] quadTable; 
	private const int SPECIAL_QUAD_LOOKUP_NUMBER = (int)(ChunkManager.CHUNKLENGTH * ChunkManager.CHUNKLENGTH * 805);
	public static PTwo MAX_DIMENSIONS = new PTwo(4,4);
	
	public FaceSet(BlockType _type, Direction _dir, AlignedCoord initialCoord, byte lightLevel)
	{
		blockType = _type;	
		blockFaceDirection = _dir;

//		MAX_FACES = (int)(ChunkManager.CHUNKLENGTH * ChunkManager.CHUNKLENGTH); // for now
		
		if (_dir == Direction.yneg || _dir == Direction.ypos)
		{
			quadTableDimensions = new PTwo((int) ChunkManager.CHUNKLENGTH, (int) ChunkManager.CHUNKLENGTH);
		} else {
			quadTableDimensions = new PTwo((int) ChunkManager.CHUNKLENGTH, (int) ChunkManager.CHUNKHEIGHT);
		}
		quadTable  = new int [quadTableDimensions.s, quadTableDimensions.t]; 
		
		addCoord(initialCoord, lightLevel);
	}
	
	private int quadIndexAtCoord(PTwo coord) {
		return quadTable[coord.s, coord.t] - FaceSet.SPECIAL_QUAD_LOOKUP_NUMBER;
	}
	
	private int addNewQuadAtCoord(LitQuad qq, PTwo coord) 
	{
		int curQuadCount = quads.Count + FaceSet.SPECIAL_QUAD_LOOKUP_NUMBER;
		
		try {
			quadTable[coord.s, coord.t] = curQuadCount ;
		} catch(IndexOutOfRangeException e) {
			throw new Exception("an index was out of range when trying to add a coord to table. the coord: " +coord.toString() + " table dims: " + 	quadTable.GetLength(0) +  " 2nd length: " + quadTable.GetLength(1) + "\n the quad: " + qq.quad.toString());
		}
		
		
		quads.Add(qq);
		
		return curQuadCount - FaceSet.SPECIAL_QUAD_LOOKUP_NUMBER;
	}
	
	private LitQuad quadAtCoord(PTwo co) {
		return quads[quadIndexAtCoord(co)];	
	}
	
	private void copyQuadIndexFromTo(PTwo fromCo, PTwo toCo) {
		quadTable[toCo.s, toCo.t] = quadTable[fromCo.s, fromCo.t];	
	}
	
	
	//reference
//	Range1D rOD = Range1D.theErsatzNullRange();
//		if (isAirBlock)
//		{
//			bool checkGotAContainingRange = false;
//			
//			for (; heightsIndex < heights.Count ; ++heightsIndex)
//			{
//				
//				rOD = heights[heightsIndex];
//				if (rOD.contains(relCo.y))
//				{
//					
//					checkGotAContainingRange = true;
//					if (relCo.y == rOD.start) {
//						rOD.start ++;	
//						rOD.range--; // corner case: range now zero: (check for this later)
//					} else if (relCo.y == rOD.extentMinusOne() ) {
//						rOD.range--;
//					} else {
//						int newBelowRange = relCo.y - 1 + 1 - rOD.start;
//						Range1D newAboveRange = new Range1D(relCo.y + 1, rOD.range - newBelowRange - 1);
//						rOD.range = newBelowRange;
//						heights.Insert(heightsIndex + 1, newAboveRange);
//					}
//					
//					if (rOD.range == 0) // no more blocks here
//					{
//						heights.RemoveAt(heightsIndex);	
//					}
//					else
//					{
//						// need to put back???
//						heights[heightsIndex] = rOD;
//					}
//					
//					break;
//				}
//			}
//			
//			if (!checkGotAContainingRange)
//			{
//				throw new Exception("confusing: we didn't find the height range where an air block was being added");
//			}
	// end reference
	
	private List<Strip> rangesWithRemovedFaceAt(int removeLevel, List<Strip> strips)
	{
//		TestRunner.bug("removing at remove level: " + removeLevel + "\n strips count: " + strips.Count);
		if (strips == null) {
			b.bug("this strips list was null. remove Level: " + removeLevel + "\nblock face dir of this face set: "
				+ blockFaceDirection + " block type: " + blockType + "\nface set limits: " + faceSetLimits.toString() );
			return new List<Strip>(0); //weird work around-ish procedure...
		}
		int i = 0;
		int numberOfStrips = strips == null ? 0 : strips.Count;
		for (; i < numberOfStrips; ++i)
		{
			Strip str = strips[i];
			Range1D ra = str.range;
			if (ra.contains(removeLevel) ) 
			{
				// split range	
				if (ra.start == removeLevel)
				{
					// did we erase the last tile?
					if (ra.range == 1) {
						strips.RemoveAt(i);
						break;
					}
					ra.start++;
					ra.range--;
					str.range = ra;
					strips[i] = str;
					
					
				} else if (ra.extentMinusOne() == removeLevel) {
					ra.range--;	
					if (ra.range == 0) {
						throw new Exception("whoa. range is zero after remove Level == extentMinus one??? \ndirection: " + blockFaceDirection +"\n removeLevel : " + removeLevel);	
					}
					str.range = ra;
					
					strips[i] = str;
				} else {
					
					Range1D new_above = str.range.subRangeAbove(removeLevel);
					Range1D new_below = str.range.subRangeBelow(removeLevel);
					
					//DEBUG
					if (new_above.isErsatzNull() ) {
						throw new Exception("new above is ersatz null");	
					}
					
					str.range = new_below;
					strips[i] = str;
					Strip newaboveStrip = new Strip(new_above);
					strips.Insert(i + 1, newaboveStrip);
					
				}
				break;
				
				//TODO: adjust faceSetLimits if needed.
			}
		}
		return strips;
		
	}
	
	public int vertexCount() {
		return (int)(quads.Count * 4);	
	}
	
	public void removeFaceAtCoord(AlignedCoord alco) 
	{
		List<Strip> strips = stripsArray[alco.across];
		stripsArray[alco.across] = rangesWithRemovedFaceAt(alco.up, strips);
		
		if (stripsArray != null && stripsArray[alco.across].Count == 0) {
			stripsArray[alco.across] = null;	
		}
		
//		TestRunner.bug(" face set now has this many non null strip lists: " + this.nonNullStripListCount + "\n strips with count > 0 : " + this.greaterThanZeroCountStripListCount );
	}
	
	public int nonNullStripListCount {
		get {
			int result = 0;
			foreach(List<Strip> strips in stripsArray) {
				if (strips != null) {
					result++;
				}
			}
			return result;
		}
	}
	
	public int greaterThanZeroCountStripListCount {
		get {
			int result = 0;
			foreach(List<Strip> strips in stripsArray) {
				if (strips != null && strips.Count > 0) {
					result++;
				}
			}
			return result;
		}
	}
	
	public int faceCount {
		get {
			int result = 0;
			foreach(List<Strip> strips in stripsArray) {
				if (strips != null) {
					foreach(Strip strip in strips) {
						result += strip.range.range;	
					}
				}
			}
			return result;
		}
	}
	
	public Quad getFaceSetLimits() {
		return faceSetLimits;	
	}
	
	private Range1D maxUpDomainPos {
		get {
			if (faceSetLimits.isErsatzNull()) {
				return Range1D.theErsatzNullRange();	
			}
			return new Range1D( faceSetLimits.origin.t, FaceSet.MAX_DIMENSIONS.t);
		}
	}
	private Range1D maxUpDomainNeg {
		get {
			if (faceSetLimits.isErsatzNull()) {
				return Range1D.theErsatzNullRange();	
			}
			return Range1D.RangeWithRangeAndExtent(FaceSet.MAX_DIMENSIONS.t, faceSetLimits.tRange().extent() );
		}
	}
	
	private Range1D usableRangeFromRange(Range1D range) 
	{
		if (faceSetLimits.isErsatzNull())
			return Range1D.IntersectingRange(range, new Range1D(range.start, FaceSet.MAX_DIMENSIONS.t));
		
		Range1D posSection = Range1D.IntersectingRange(maxUpDomainPos, range);
		
		if (posSection.range == FaceSet.MAX_DIMENSIONS.t)
			return posSection;
		
		Range1D negSection = Range1D.IntersectingRange(maxUpDomainNeg, range);
		if (negSection.range == FaceSet.MAX_DIMENSIONS.t)
			return negSection;
		
		if (posSection.range > negSection.range)
			return posSection;
		
		return negSection;
	}
	
	private bool canUseRange(Range1D range) {
		return !usableRangeFromRange(range).isErsatzNull();	
	}
	
	private void updateFaceSetLimits(AlignedCoord co) {
		if (faceSetLimits.isErsatzNull() ) {
			faceSetLimits = Quad.UnitQuadWithAlignedCoord(co); 
		} else {
			faceSetLimits = faceSetLimits.expandedToContainPoint(new PTwo(co.across, co.up));
		}	
	}
	
	private void updateFaceSetLimitsWithRange(Range1D range, int acrossIndex) 
	{
		Range1D withinLimitsRange = usableRangeFromRange(range); 
		this.updateFaceSetLimits( new AlignedCoord(acrossIndex, withinLimitsRange.start));
		this.updateFaceSetLimits( new AlignedCoord(acrossIndex, withinLimitsRange.extentMinusOne()));
	}
	
	public void addCoord(AlignedCoord co, byte lightLevel)
	{
		//sanity check
		if (!co.isIndexSafe(new AlignedCoord(quadTable.GetLength(0), quadTable.GetLength(1) ) ))
		{
			throw new Exception ( "why are we trying to add a coord that's beyond bounds? coord: " + co.toString() );
			return;
		}

		updateFaceSetLimits(co);
		
		int stripsIndex = co.across;
		int addAtHeight = co.up;
		
		List<Strip> strips = stripsAtIndex(stripsIndex);
		
		// no strips yet?
		if (strips.Count == 0)
		{
			
#if LIGHT_BY_RANGE
			Strip newStrip = new Strip(new Range1D(addAtHeight, 1, this.blockType, lightLevel, lightLevel));
#else
			Strip newStrip = new Strip(new Range1D(addAtHeight, 1, this.blockType));
#endif
			strips.Add(newStrip);
			stripsArray[stripsIndex] = strips;

			return;
		}
		
		for(int i = 0; i < strips.Count ; ++i)
		{
			Strip str = strips[i];
			
			//check contains (sanity check)
			if (str.range.contains(addAtHeight) )
			{
//				throw new Exception("trying to add a coord that we already have? (strip contains)");
				
				// TODO: with this and with face agg. figure out the smart thing to do when we are
				// told to add a face where there already was one...
				// here , since faces are of one type,
				// should just ignore right?
				continue;
			}
			
			if 	(str.range.isOneAboveRange(addAtHeight) )
			{
				str.range.range++; 
				
				byte bottom_light_level = lightLevel;
				// can we combine with a next range?
				if (i < strips.Count - 1) 
				{
					Strip nextStrip = strips[i + 1];
					if (nextStrip.range.isOneBelowStart(addAtHeight))
					{
						str.range = str.range.extendRangeToInclude(nextStrip.range);
#if LIGHT_BY_RANGE
						bottom_light_level = nextStrip.range.bottom_light_level;
#endif
						strips.RemoveAt(i+1);
					}
				}
				
#if LIGHT_BY_RANGE
				str.range.bottom_light_level = bottom_light_level;
#endif

				strips[i] = str; //put it back 
				break;
			}
			
			if (str.range.isOneBelowStart(addAtHeight) ) 
			{	
#if LIGHT_BY_RANGE
				str.range.top_light_level = lightLevel;
#endif
				str.range.start--;
				str.range.range++;

				strips[i] = str;
				break;
			}
			
			if (i == strips.Count - 1) //still didn't find anything? make new
			{
#if LIGHT_BY_RANGE
				Strip newStrip = new Strip(new Range1D(addAtHeight, 1, this.blockType, lightLevel, lightLevel));
#else	
				Strip newStrip = new Strip(new Range1D(addAtHeight, 1));
#endif
				strips.Add(newStrip);
				break;
			}
		}
		
		stripsArray[stripsIndex] = strips;	
	}
	
#region check range to add
	
	public bool canIncorporatePartOrAllOfRange(Range1D rangeToAdd, int acrossIndex)
	{
		if (this.blockType != rangeToAdd.blockType)
			return false;
		
		if (this.faceSetLimits.isErsatzNull()) {
			return true;
		}
		
		if(!partOrAllOfRangeIsWithinMaxDomainTRange(rangeToAdd))
			return false;
		
		if (!this.faceSetLimits.sRange().containsOrFlushWithEitherEnd(acrossIndex))
			return false;
		
		if (!this.faceSetLimits.sRange().contains(acrossIndex) && faceSetLimitsSDimensionHasReachedMaxDomainSDimension())
			return false;
		
		// skip checking (thoroughly) for adjacency? ... for now...?...do we not actually rely on it?
		
		return true;
	}
	
	private bool faceSetLimitsSDimensionHasReachedMaxDomainSDimension() {
		return this.faceSetLimits.sRange().range == FaceSet.MAX_DIMENSIONS.s;	
	}
	
	private bool partOrAllOfRangeIsWithinMaxDomainTRange(Range1D range) {
		if (faceSetLimits.isErsatzNull())
			return true;
		
		if(Range1D.RangesIntersect(this.maxUpDomainPos, range))
			return true;
		
		return Range1D.RangesIntersect(this.maxUpDomainNeg, range);
	}
	
#endregion
	
	public Range1D addRangeAtAcrossReturnAddedRange(Range1D rangeToAdd, int acrossIndex)
	{
		Range1D originalRange = rangeToAdd;
		// get the intersection of the range and possible (pos/neg) MAX DIM T RANGE
		Range1D limitedRangeToAdd = usableRangeFromRange(rangeToAdd); // Range1D.IntersectingRange(rangeToAdd, this.maxDomain.tRange()); 
		rangeToAdd.start = limitedRangeToAdd.start; 
		rangeToAdd.range = limitedRangeToAdd.range;
			
		updateFaceSetLimitsWithRange(rangeToAdd, acrossIndex);
		
		AssertUtil.Assert(!faceSetLimits.isErsatzNull(), "whoa face set limits is ersatz null at this point??");
		
		if (rangeToAdd.isErsatzNull())
			return Range1D.theErsatzNullRange();
		
		List<Strip> strips = stripsAtIndex(acrossIndex);
		
		// no strips yet?
		// if no strips here, simply add the range (as a strip)
		if (strips.Count == 0)
		{
			Strip str = new Strip (rangeToAdd);
			strips.Add(str);
			// don't need to put back?
			return rangeToAdd;
		}
		
		// range to add at least overlappingOverEnd with last strip.range?
		Strip last = strips[strips.Count - 1];
		OverlapState overlapLast = last.range.overlapWithRange(rangeToAdd);
		if (overlapLast == OverlapState.BeyondExtent) {
			strips.Add(new Strip(rangeToAdd));
			return rangeToAdd;
		}

		if (overlapLast == OverlapState.FlushWithExtent || overlapLast == OverlapState.OverlappingOverEnd) {
			last.range = Range1D.Union(last.range, rangeToAdd);
			strips[strips.Count - 1] = last;
			return rangeToAdd;
		}
		
		// before, flush with or overlapping with start?
		Strip first = strips[0];
		OverlapState overlapFirst = first.range.overlapWithRange(rangeToAdd);
		if (overlapFirst == OverlapState.BeforeStart) {
			strips.Insert(0, new Strip(rangeToAdd));
			return rangeToAdd;
		}
		if (overlapFirst == OverlapState.FlushWithStart || overlapFirst == OverlapState.OverlappingOverStart) {
			first.range = Range1D.Union(first.range, rangeToAdd);
			strips[0] = first;
			return rangeToAdd;
		}
		
		int startCombineStripsIndex = -1;
		int endCombineStripsIndex = -1;
		
		int insertAtIndex = -1;
		
		// find possible insertAtIndex, maybe find combine end
		for(int i = 0; i < strips.Count ; ++i)
		{
			Strip str = strips[i];
			Range1D currentRange = str.range;
			
			OverlapState overlapState = currentRange.overlapWithRange(rangeToAdd);
			
			if (overlapState == OverlapState.BeforeStart) {
				insertAtIndex = i;
				break;	
			}
			
			if (overlapState == OverlapState.FlushWithStart || overlapState == OverlapState.OverlappingOverStart) {
				insertAtIndex = i;
				endCombineStripsIndex = i;
				break;
			}
			
			if (overlapState == OverlapState.IContainIt) {
				return rangeToAdd;	//nothing needs to be done
			}
			
			if (Range1D.Equal(currentRange, rangeToAdd)) {
				return rangeToAdd;
			}
			//still beyond?
			if (i == strips.Count - 1) {
				AssertUtil.Assert(OverLapUtil.OverlapStateIsABeyondState(overlapState), "(in face set add range) should be beyond");
				insertAtIndex = i;
				endCombineStripsIndex = i;
			}
		}
		
		AssertUtil.Assert(insertAtIndex != -1, "insert index shouldn't be neg one anymore");
		
		//find combine start
		for(int j = insertAtIndex; j >= 0 ; --j) 
		{
			Strip str = strips[j];
			Range1D currentRange =str.range;
			
			OverlapState overlapState = currentRange.overlapWithRange(rangeToAdd);
			
			if (overlapState == OverlapState.BeyondExtent) {
				startCombineStripsIndex = j + 1;
				insertAtIndex = j + 1;
				break;
			}
			
			if (overlapState == OverlapState.FlushWithExtent || overlapState == OverlapState.OverlappingOverEnd) {
				startCombineStripsIndex = j;
				insertAtIndex = j;
				break;
			}
		}

		Range1D combinedRange = rangeToAdd;
		
		if (endCombineStripsIndex > -1)
		{
			Strip endStrip = strips[endCombineStripsIndex];
			combinedRange = Range1D.Union(endStrip.range, rangeToAdd);
		}
		
		if (startCombineStripsIndex < endCombineStripsIndex && startCombineStripsIndex > -1)
		{
			Strip startStrip = strips[startCombineStripsIndex];
			combinedRange = Range1D.Union(startStrip.range, combinedRange);
		}
		
		//any strips to remove?
		if (startCombineStripsIndex > -1)
		{
			if (startCombineStripsIndex < endCombineStripsIndex)
				strips.RemoveRange(startCombineStripsIndex, endCombineStripsIndex - startCombineStripsIndex);
			else 
				strips.RemoveAt(startCombineStripsIndex);
		} else if (endCombineStripsIndex > -1) {
			
			strips.RemoveAt(endCombineStripsIndex);
		}
		
		strips.Insert(insertAtIndex, new Strip(combinedRange));
		return rangeToAdd;
	}
	
	private void optimizeStripsSafeVersion()
	{
		
		int horizDimStart = faceSetLimits.origin.s;
		int horizDimEnd = faceSetLimits.extent().s;
		
		int horizontalDim = horizDimStart; // 0;
		List<Strip> currentStrips;
		
		if (horizDimEnd > stripsArray.Length)
			throw new Exception("the limits were greater than strips array length. what's up with that?");

		for(; horizontalDim < horizDimEnd ; ++horizontalDim)
		{
			currentStrips = stripsArray[horizontalDim];
			if (currentStrips == null)
			{
				continue;
			}
			
			foreach(Strip sturip in currentStrips){
//				Quad qq = Quad.QuadFromStrip(sturip, horizontalDim);
				LitQuad litQuad = LitQuad.LitQuadFromStrip(sturip, horizontalDim);
				addNewQuadAtCoord(litQuad, new PTwo(horizontalDim, sturip.range.start));
			}
		}
	}
	
	private void resetStripsQuadIndices() 
	{
		List<Strip> strips;
		for(int i = 0; i < stripsArray.Length; ++i) {
			strips = stripsArray[i];
			if (strips != null) 
			{
				for(int j = 0; j < strips.Count; ++j) {
					Strip ss = strips[j];
					ss.resetQuadIndex();
					strips[j] = ss;
				}
			}
		}
	}
	
	private void clearQuadTable() {
		int table_length = quadTable.GetLength(0) * quadTable.GetLength(1);
		Array.Clear(quadTable, 0, table_length);
	}
	
	// TODO: test whether we got any overlapping quads...
	private void optimizeStrips()
	{
		//clear quads
		this.quads.Clear();
		resetStripsQuadIndices(); 
		clearQuadTable(); //helpful?
		
#if NO_OPTIMIZATION
		optimizeStripsSafeVersion(); 
		return;///!!!!
#endif
		
		// deal with the case where there's only one list of strips
		if (faceSetLimits.dimensions.s == 1) {
			
//			TestRunner.bug("!! face set limits dimension was only one: " + faceSetLimits.toString() );
			
			List<Strip> the_strips = stripsArray[faceSetLimits.origin.s  ];
			int stripsCount = the_strips != null ? the_strips.Count : 0;
			for(int u = 0; u < stripsCount ; ++u) {
//			foreach(Strip stripp in the_strips) {
				
				Strip stripp = the_strips[u];
#if GLOSS_QUAD_ARRAY_INDEX_BUG
				if (stripp.range.start >= quadTableDimensions.t)
				{
					b.bug("range start greater or eq quad table . t. face set limits: " + faceSetLimits.toString() + "range: " + stripp.range.toString() );
					continue;
				}
				if (stripp.range.range < 1)
				{
					b.bug("range range less than 1. face set limits: " + faceSetLimits.toString() + "range: " + stripp.range.toString() );
					continue;
				}
#endif
				LitQuad litquadd = LitQuad.LitQuadFromStrip(stripp, faceSetLimits.origin.s); // new LitQuad(quadd, lightcorners);
				addNewQuadAtCoord(litquadd, new PTwo(faceSetLimits.origin.s, stripp.range.start) );
			}
			return;
		}
		
		
		// ENHANCED NAIVE METHOD
		// MAKE A LIST OF QUADS FROM THE LISTS OF STRIPS.
		// (ADMITTEDLY: WE GOT NOTHING OUT OF USING QUADS INSTEAD OF STRIPS WITH WIDTHS...)
		
		int horizDimStart = faceSetLimits.origin.s + 1;
		PTwo faceLimitsExtent = faceSetLimits.extent(); // + new PTwo(1,1);
		int horizDimEnd = faceLimitsExtent.s;
		
		if (horizDimEnd > stripsArray.Length )
			throw new Exception("wha extents greater than strips array length?");
		
		// Go through adjacent strips lists two at a time
		int horizontalDim = horizDimStart;
		List<Strip> currentStrips = new List<Strip>();
		List<Strip> lastStrips = new List<Strip>();
		for(; horizontalDim < horizDimEnd ; ++horizontalDim)
		{
			currentStrips = stripsArray[horizontalDim];
			lastStrips = stripsArray[horizontalDim - 1];
			
			// *isolated list of 'current Strips'?
			if (lastStrips == null && horizontalDim == horizDimEnd - 1)
			{
//				TestRunner.bug(" ^^^^ isolated last row of stips condition met.");
				if (currentStrips != null)
					foreach(Strip lastRowStrip in currentStrips) {
						LitQuad litlq = LitQuad.LitQuadFromStrip(lastRowStrip, horizontalDim);
						addNewQuadAtCoord(litlq, new PTwo(horizontalDim, lastRowStrip.range.start) );
					}
			}
			
			
			// *go through last strips
			int lastStripsCount = 0;
			if ( lastStrips != null ) //currentStrips == null) // ||
				lastStripsCount = lastStrips.Count;
			
			for(int j = 0 ; j < lastStripsCount ; ++j) 
			{
				Strip lstp = lastStrips[j];
				
				// *is this 'last strip' in the first row ('leftmost')? if so, make a quad from it.
				if (horizontalDim == horizDimStart)
				{
					LitQuad litqq = LitQuad.LitQuadFromStrip(lstp, horizontalDim - 1);
					lstp.quadIndex = addNewQuadAtCoord(litqq, new PTwo(horizontalDim - 1, lstp.range.start) );
					
					if (lstp.quadIndex < 0)
						b.bug ("the quad index less than zero: " + lstp.quadIndex + " strip is: " + lstp.toString());
					
					lastStrips[j] = lstp;
				}
				
				int curCount = currentStrips == null?  0 : currentStrips.Count;
				
				// *go through current strips for each last strip
				for(int i = 0; i < curCount ; ++i) 
				{
					Strip stp = currentStrips[i];
					
					if (Range1D.Equal(stp.range, lstp.range) ) 
					{
						// has a quad index?
						if (lstp.quadIndex != -1)
						{
							LitQuad qua;
							try {
								qua = quads[lstp.quadIndex];
							} catch(ArgumentOutOfRangeException e) {
								throw new Exception("argument out of range. index was: " + lstp.quadIndex + "\nhoriz dim: " + horizontalDim );		
							}
							
							qua.quad.dimensions.s++;
							qua.lightCorners.setRightCornersWithRange(stp.range);
							
							if (lstp.quadIndex < 0)
								bug ("the quad index less than zero: " + lstp.quadIndex + " strip is: " + lstp.toString() + "else from lstp qI == -1" + "\n BTW: the shift number is: " + FaceSet.SPECIAL_QUAD_LOOKUP_NUMBER);
							
							quads[lstp.quadIndex] = qua;
							stp.quadIndex = lstp.quadIndex;
							
						} else {
							
							//* make a new quad with these two 
							Quad doubleWidthqq = Quad.QuadFromStrip(lstp, horizontalDim - 1);
							doubleWidthqq.dimensions.s = 2;
							LightCorners lightCorners = new LightCorners(lstp.range.top_light_level, stp.range.top_light_level, lstp.range.bottom_light_level, stp.range.bottom_light_level);
							LitQuad litdoubleWidthQuad = new LitQuad(doubleWidthqq, lightCorners);
							
							lstp.quadIndex = addNewQuadAtCoord(litdoubleWidthQuad, new PTwo(horizontalDim - 1, lstp.range.start) );
							
							if (lstp.quadIndex < 0)
								bug ("the quad index less than zero: " + lstp.quadIndex + " strip is: " + lstp.toString() + "else from lstp qI == -1" + "\n BTW: the shift number is: " + FaceSet.SPECIAL_QUAD_LOOKUP_NUMBER);
							
							stp.quadIndex = lstp.quadIndex;
							
						}
					} 
					else 
					{
						// new quad with stp
						// only if its the last time around
//						if (horizontalDim == stripsArray.Length - 1)
						if (horizontalDim == horizDimEnd - 1)
						{
							Quad sq = Quad.QuadFromStrip(stp, horizontalDim);
							
							LightCorners lightCorners = LightCorners.LightCornersFromRangeTopBottom(stp.range);
							LitQuad litSQuad = new LitQuad(sq, lightCorners);
							
							stp.quadIndex = addNewQuadAtCoord(litSQuad, new PTwo(horizontalDim, stp.range.start) );
							
							if (stp.quadIndex < 0)
								bug ("the quad index less than zero: " + stp.quadIndex + " strip is: " + stp.toString());
						}
					}
					
					
					currentStrips[i] = stp;
				}
				
				if (lstp.quadIndex == -1) // still no match
				{
					Quad sq = Quad.QuadFromStrip(lstp, horizontalDim - 1);
					
					LightCorners lightCorners = LightCorners.LightCornersFromRangeTopBottom(lstp.range);
					LitQuad litSQuad = new LitQuad(sq, lightCorners);
					
					lstp.quadIndex = addNewQuadAtCoord(litSQuad, new PTwo(horizontalDim - 1, lstp.range.start) );
				}
				
				lastStrips[j] = lstp;
			}
			
			stripsArray[horizontalDim] = currentStrips;
			stripsArray[horizontalDim - 1] = lastStrips;
		}
		
	}
	
	public MeshSet CalculateGeometry(float verticalHeight) //need param y height...
	{
		optimizeStrips();
		
		TestRunner.bug("quads count after optimize strips: " + quads.Count );
		
		int t_offset = faceSetLimits.origin.t;
		
		int curTriIndex = 0;
		
		bool faceIsOnPosSide = ((int) this.blockFaceDirection % 2 == 0);
		
		bool isZFace = (blockFaceDirection >= Direction.zpos); // must flip tri indices if z face!!
		
		verticalHeight += (faceIsOnPosSide ? 0.5f : -0.5f);
		
		float uvIndex = Chunk.uvIndexForBlockTypeDirection(this.blockType, this.blockFaceDirection);
		
		Vector2 monoUVValue = new Vector2(uvIndex, 0f); 
		
		List<Vector2> returnUVS = new List<Vector2>();
		List<int> returnTriIndices = new List<int>();
		List<Vector3> returnVecs = new List<Vector3>();
		
		List<Vector2> returnColors = new List<Vector2>();
		List<Color32> returnCol32s = new List<Color32>();
		
		float half_unit = 0.5f;
		
#if LIGHT_BY_RANGE
		Color32[] cornerColors = new Color32[4] {new Color32(0,0,0,0),new Color32(0,0,0,0),new Color32(0,0,0,0),new Color32(0,0,0,0)}; 	
#endif
		
#if UNDO_LIGHT_BY_RANGE_TEST
		byte firstRowTest = (4*4*4) * 3 + (4 * 4) * 2 + (4) * 1; // last square lit?
		Color32 lightByFacesXRows = new Color32(firstRowTest,255,255,255); // each component sets the possible 4 faces in an x row
		cornerColors = new Color32[4]{ lightByFacesXRows,lightByFacesXRows,lightByFacesXRows,lightByFacesXRows };
#endif

		int i = 0;
		for(; i < quads.Count ; ++i)
		{
#if LIGHT_BY_RANGE
			LitQuad litQuad = quads[i];
			Quad quad = litQuad.quad;

			// WANT IF LIGHTING BY RANGE!
//			//CONSIDER: do we want to rotate (and/or flip?) the corners based on blockFaceDirection?
//			LightCorners lightCorners = litQuad.lightCorners;
//			cornerColors[0].r = lightCorners.oo;
//			cornerColors[1].r = lightCorners.mo;
//			cornerColors[2].r = lightCorners.om;
//			cornerColors[3].r = lightCorners.mm;
#else
			Quad quad = quads[i];
#endif
			
			int vertsAddedByStrip = 0;
			
			int curULindex = curTriIndex; // + i * 4;
			int curLLindex = curULindex + 1;
			int curURindex = curULindex + 2;
			int curLRindex = curULindex + 3;
			
			Vector3 ulVec = positionVectorForAcrossUpVertical((float)quad.origin.s - half_unit, 
				verticalHeight, 
				(float)quad.origin.t - half_unit) * Chunk.VERTEXSCALE;
			returnVecs.Add(ulVec);
			vertsAddedByStrip++;
			returnUVS.Add(monoUVValue);
			returnCol32s.Add(cornerColors[0] );
		

			Vector3 llVec = positionVectorForAcrossUpVertical((float) quad.origin.s - half_unit, 
				verticalHeight, 
				(float)quad.extent().t  - half_unit) * Chunk.VERTEXSCALE;
			returnVecs.Add(llVec);
			vertsAddedByStrip++;
			returnUVS.Add(monoUVValue);
			returnCol32s.Add(cornerColors[1]);
			
		

			Vector3 urVec = positionVectorForAcrossUpVertical((float) quad.extent().s - half_unit, 
				verticalHeight, 
				(float)quad.origin.t - half_unit) * Chunk.VERTEXSCALE;
			returnVecs.Add(urVec);
			vertsAddedByStrip++;
			returnUVS.Add( monoUVValue); 
			returnCol32s.Add(cornerColors[2]);
			

			Vector3 lrVec = positionVectorForAcrossUpVertical((float)quad.extent().s - half_unit, 
				verticalHeight, 
				(float)quad.extent().t - half_unit) * Chunk.VERTEXSCALE;
			returnVecs.Add(lrVec);
			vertsAddedByStrip++;
			returnUVS.Add(monoUVValue);
			returnCol32s.Add(cornerColors[3]);
			
			int[] tris;
			
			if (faceIsOnPosSide != isZFace) { // if Z face we want the opposite
				tris = new int[]  {curULindex, curLLindex, curURindex, 	curURindex, curLLindex, curLRindex };
			} else {
				tris = new int[]{curULindex, curURindex, curLLindex,   curURindex, curLRindex, curLLindex};
			}
			
			returnTriIndices.AddRange(tris);
			
			curTriIndex += vertsAddedByStrip;

		}
		
		//now maybe just calculate the actual v3 array and the indices....
		GeometrySet geomset = new GeometrySet(returnTriIndices, returnVecs );
		
		return new MeshSet(geomset, returnUVS, returnCol32s);
	}
	
	private Vector3 positionVectorForAcrossUpVertical(float across,  float vertical, float up)
	{
		if (blockFaceDirection <= Direction.xneg) {
			return new Vector3(vertical, up, across); // we haven't actually figured whether vertical height corresponds to the right side of the cube!	
		}
		if (blockFaceDirection <= Direction.yneg) {
			return new Vector3(across, vertical, up); // the original case	
		}
		
		return new Vector3(across, up, vertical);
	}
	
//	private List<Vector3> collectedVectors()
//	{
//		List<Vector3> resVs = new List<Vector3> ();
//				
//		int hDim = 0;
//		for(;hDim < vertexColumns.Count ; ++hDim)
//		{
//			
//		}
//		
//		return resVs;
//	}
	
//	private List<VertexTwo> vertexListAt(int index) {
//		if 	(vertexColumns[index] == null) {
//			vertexColumns [index] = new List<VertexTwo>();
//		}
//		return vertexColumns[index];
//	}
	
	private bool stripFromListWithStartEqualTo(int equalToThis, List<Strip> _strips, ref Strip out_strip) {
		return stripFromList(equalToThis, _strips, ref out_strip, false);
	}
	
	private bool stripFromListWithExtentEqualTo(int equalToThis, List<Strip> _strips, ref Strip out_strip) {
		return stripFromList(equalToThis, _strips, ref out_strip, true);
	}
	
	private bool stripFromList(int equalToThis, List<Strip> _strips, ref Strip out_strip, bool wantExtent) {
		foreach (Strip str in _strips) {
			if ((wantExtent && str.range.extentMinusOne() == equalToThis) || str.range.start == equalToThis)
			{
				out_strip = str;
				return true;
			}
			
		}
		return false; // Strip.theErsatzNullStrip();
	}
	
	private List<Strip> stripsAtIndex(int ix) {
		if (stripsArray[ix] == null) {
			stripsArray[ix] = new List<Strip>();		
		}
		return stripsArray[ix];
	}
	
	private void setStripsAtIndex(List<Strip> strips, int ix) {
		if (ix >= stripsArray.Length)
			throw new Exception("we don't have a strips list at this index: " + ix);
		stripsArray[ix] = strips;	
	}
	
	public void logStripsArray() 
	{
		int count = 0;
		string info = "";
		foreach(List<Strip> strips in stripsArray)
		{
			
			if (strips != null)
			{
				info += "" + strips.Count;
				
			
				foreach(Strip st in strips) {
	
					info += "strip: " + st.toString ();
				}
				info += "\n";
			}
			++count;
		}	
		bug (info);
	}
	
	public string getQuadsString() {
		string result = "";
		
		foreach(LitQuad qq in quads) {
			result += " | " + qq.quad.toString();	
		}
		return result;
	}
	
	public void checkForIntersectingStrips()
	{
		//TODO: this func.
		//Also, check if there's total coverage in the strips across the CHLEN
		// and if not where are the gaps?
	}
	
	private void bug(string str) {
		UnityEngine.Debug.Log(str);	
	}
}

public class FaceSetTest
{
	private List<Vector3> showInGuiVecs = new List<Vector3>();
	public FaceSetTest()
	{
		bug ("BEGIN FACE SET TEST");
		FaceSet fs = new FaceSet(BlockType.Grass, Direction.ypos, new AlignedCoord(0,0), 3);
		
		Coord[] coords = new Coord[256];
		for(int i = 0; i < 256 ; ++i)
		{
			int z = i % 16;
			int x = i / 16;
			coords[i] = new Coord(x, 2, z);
		}
		
		
		
		foreach (Coord co in coords) {
			fs.addCoord(new AlignedCoord(co.x, co.z), 3);	
		}
		
		MeshSet mset = fs.CalculateGeometry(2);
		
//		fs.logStripsArray();
		
		this.logMeshSet(mset);
		
		bug ("END FACE SET TEST");
	}
	
	private void logMeshSet( MeshSet mset) 
	{
		bug("Mesh set verts");
		string vstr = "";
		int jj = 0;
		foreach(Vector3 vert in mset.geometrySet.vertices) {
			if (jj % 4 == 0)
				vstr += "\n";
			vstr += "[" + vert.x + " , " + vert.z + "]";
			jj++;
		}
		bug (vstr);
		
		vstr="";
		jj = 0;
		foreach(Vector3 vert in mset.geometrySet.vertices) {
//			if (jj % 4 == 0)
			vstr += "*";
			if (jj % 16 == 0)
				vstr += "\n";
			jj++;
		}
		bug (vstr);
		
		showInGuiVecs.AddRange(mset.geometrySet.vertices);
		
		int vertCount = mset.geometrySet.vertices.Count;
		
		bug("VertCount: " + vertCount + " verts/indices (INDEX)[x,z]");
		string indexstr = "";
		int count = 0;
		foreach(int ii in mset.geometrySet.indices) {
			if (count % 6 == 0) 
				indexstr += "\n";
//			indexstr += "|" + ii;
			Vector3 vv = mset.geometrySet.vertices[ii];
			indexstr += "("+ii+")[" + vv.x + " , " + vv.z + "]";
			if (ii >= vertCount || ii < 0) {
				indexstr += "\n out of bounds: ? " + ii + "\n";
			}
			++count;
		}
//		bug (indexstr);
	}
	
	private void bug(string str) {
		
		UnityEngine.Debug.Log(str);
	}
	
//	public void OnGUI() 
//	{
//		foreach(Vector3 vv in showInGuiVecs) {
//			guiBlipAt(vv);	
//		}
//	}
//	
//	private void guiBlipAt(Vector3 vv) {
//		GUI.Box (new Rect (vv.x * 10, vv.z * 10 , 10, 10), "*");
//	}
}




//			int relUpperLeftTriIndex = ++internalTriIndex;
//			
////			IndexSet newIset = IndexSet(
//			// is there a strip to the left?
//			if (co.x > 0)
//			{
//				List<Strip> oneLeftStrips = stripsAtIndex(co.x - 1);
//				Strip leftStartFlushStrip = stripFromListWithSameStartEqualTo(co.z, oneLeftStrips);
//				if ( Strip.StripNotNull(leftStartFlushStrip) ) {
//					relUpperLeftTriIndex = leftStartFlushStrip.indexSet.upperRight;
//					--internalTriIndex;
//				}
//			}


// OLD calc geom

//public MeshSet CalculateGeometry(float verticalHeight) //need param y height...
//	{
//		int curTriIndex = 0;
//		int horizontalDim = 0;
//		
//		bool faceIsOnPosSide = ((int) this.blockFaceDirection % 2 == 0);
//		
//		verticalHeight += (faceIsOnPosSide ? -0.5f : 0.5f);
//		
//		Vector2 monoUVValue = Chunk.uvOriginForBlockType(this.blockType, this.blockFaceDirection);
//		Vector2 uvURTexDimTest = new Vector2(0.25f, 0f);
//		Vector2 uvLLTexDimTest = new Vector2(0f, .25f);
//		Vector2 uvLRTexDimTest = new Vector2(0.25f, 0.25f);
//		
//		List<Vector2> returnUVS = new List<Vector2>();
//		
//		List<Strip> strips;
//		List<Vector3> rightVertexColumn;
//		List<Vector3> leftVertexColumn = new List<Vector3>();
//		
//		List<int> returnTriIndices = new List<int>();
//		List<Vector3> returnVecs = new List<Vector3>();
//		
//		float half_unit = 0.5f;
//		
//		
//		for (; horizontalDim < stripsArray.Length; ++horizontalDim)
//		{
//			rightVertexColumn = new List<Vector3>(); 
//			strips = stripsArray[horizontalDim];
//			
//			float horizontalDimLessHalf = horizontalDim - half_unit;
//			
//			int strips_count = 0;
//			if (strips != null)
//				strips_count = strips.Count; //is continue ok???
//			
//			if (strips_count == 0)
//				bug ("strips count 0 (or null) at h dim: " + horizontalDim);
//			else if (strips_count > 1)
//				bug ("strips count > 1 : " + strips_count + " at h dim: " + horizontalDim);
//			
//			int vertsAddedByStrip = 0;
//			
//			for (int i = 0; i < strips_count ; ++i)
//			{
//				Strip str = strips[i];
//				int curULindex = curTriIndex + i * 2;
//				int curLLindex = curULindex + 1;
//				int curURindex = curTriIndex + strips_count * 2 + i * 2;
//				int curLRindex = curURindex + 1;;
////				int curURindex = curTriIndex + 2;
////				int curLRindex = curTriIndex + 3;
//				
//				bool addULVert = true;
//				bool addLLVert = true;
//				
//				
//#if COMBINE_FLUSH_VERTS
//
//				if (horizontalDim > 0 && stripsArray[horizontalDim - 1] != null) 
//				{
//					// if this strip is flush with the start of any strips in the col to the left...
//					Strip flushWithStartOneLeft = Strip.theErsatzNullStrip();
//					bool gotAStrip = stripFromListWithStartEqualTo(str.range.start, stripsArray[horizontalDim - 1], ref flushWithStartOneLeft);
//					if (gotAStrip) // (Strip.StripNotNull(flushWithStartOneLeft))
//					{
//						curULindex = flushWithStartOneLeft.indexSet.upperRight;
//						curLLindex--;
//						curURindex--;
//						curLRindex--;
//						addULVert = false;
//					}
//					
//					Strip flushWithExtentOneLeft = Strip.theErsatzNullStrip();
//					gotAStrip =	stripFromListWithExtentEqualTo(str.range.extentMinusOne() , stripsArray[horizontalDim - 1], ref flushWithExtentOneLeft);
//					if (gotAStrip)
//					{
//						curLLindex = flushWithExtentOneLeft.indexSet.lowerRight;
//						curURindex--;
//						curLRindex--;
//						addLLVert = false;
//					}
//				}
//#endif
//				
//				if (addULVert) {
//					Vector3 ulVec = new Vector3((float) horizontalDimLessHalf, verticalHeight, (float)str.range.start - half_unit);
//					leftVertexColumn.Add(ulVec); //LEFT COLUMN
//					vertsAddedByStrip++;
//					returnUVS.Add(monoUVValue);
//				}
//				
//				if (addLLVert) {
//					Vector3 llVec = new Vector3((float) horizontalDimLessHalf, verticalHeight, (float)str.range.extent() - half_unit);
//					leftVertexColumn.Add(llVec); //LEFT COLUMN!!
//					vertsAddedByStrip++;
//					returnUVS.Add(monoUVValue + uvLLTexDimTest);
//				}
//				
//				Vector3 urVec = new Vector3((float) horizontalDimLessHalf + 1, verticalHeight, (float)str.range.start - half_unit);
//				rightVertexColumn.Add(urVec);
//				vertsAddedByStrip++;
//				returnUVS.Add(monoUVValue + uvURTexDimTest);
//				
//				Vector3 lrVec = new Vector3((float)horizontalDimLessHalf + 1, verticalHeight, (float)str.range.extent() - half_unit);
//				rightVertexColumn.Add(lrVec);
//				vertsAddedByStrip++;
//				returnUVS.Add(monoUVValue + uvLRTexDimTest);
//				
//				//need to index sets.
//				str.indexSet = new IndexSet(curULindex, curURindex, curLLindex, curLRindex);
//				strips[i] = str;
//				
//				int[] tris;
//				
//				if (faceIsOnPosSide) {
//					tris = new int[] {curULindex, curURindex, curLLindex,   curURindex, curLRindex, curLLindex};
//				} else {
//					tris = new int[] {curULindex, curLLindex, curURindex, 	curURindex, curLLindex, curLRindex };
//				}
//				
//				returnTriIndices.AddRange(tris);
//				
//			}
//			
//			curTriIndex += vertsAddedByStrip;
//			
////			vertexColumns[horizontalDim] = leftVertexColumn;
////			vertexColumns[horizontalDim + 1] = rightVertexColumn;
//			
//			returnVecs.AddRange(leftVertexColumn);
//			
//			if (horizontalDim == stripsArray.Length - 1)
//			{
//				returnVecs.AddRange(rightVertexColumn);
//			}
//			else 
//			{
//				leftVertexColumn = rightVertexColumn;
//			}
//			
////			stripsArray[horizontalDim] = strips;
//		}
//		
//		//now maybe just calculate the actual v3 array and the indices....
//		GeometrySet geomset = new GeometrySet(returnTriIndices, returnVecs );
//		
//		return new MeshSet(geomset, returnUVS);
//		
//	}




//public MeshSet CalculateGeometry(float verticalHeight) //need param y height...
//	{

//  WORKS WITH STRIPS
//		optimizeStrips();
//		
//		int curTriIndex = 0;
//		int horizontalDim = 0;
//		
//		bool faceIsOnPosSide = ((int) this.blockFaceDirection % 2 == 0);
//		
//		verticalHeight += (faceIsOnPosSide ? -0.5f : 0.5f);
//		
//		Vector2 monoUVValue = Chunk.uvOriginForBlockType(this.blockType, this.blockFaceDirection);
//		Vector2 uvURTexDimTest = new Vector2(0.25f, 0f);
//		Vector2 uvLLTexDimTest = new Vector2(0f, .25f);
//		Vector2 uvLRTexDimTest = new Vector2(0.25f, 0.25f);
//		
//		List<Vector2> returnUVS = new List<Vector2>();
//		
//		List<Strip> strips;
//		List<Vector3> rightVertexColumn;
//		List<Vector3> leftVertexColumn = new List<Vector3>();
//		
//		List<int> returnTriIndices = new List<int>();
//		List<Vector3> returnVecs = new List<Vector3>();
//		
//		float half_unit = 0.5f;
//		
//		// NEW WAY: SIMPLY COLLECT THE VERTS AND INDICES FROM EACH STRIP IN ORDER
//		
//		for (; horizontalDim < stripsArray.Length; ++horizontalDim)
//		{
//			rightVertexColumn = new List<Vector3>(); 
//			strips = stripsArray[horizontalDim];
//			
//			float horizontalDimLessHalf = horizontalDim - half_unit;
//			
//			int strips_count = 0;
//			if (strips != null)
//				strips_count = strips.Count; //is continue ok???
//			
////			if (strips_count == 0)
////				bug ("strips count 0 (or null) at h dim: " + horizontalDim);
////			else if (strips_count > 1)
////				bug ("strips count > 1 : " + strips_count + " at h dim: " + horizontalDim);
//			
//			int vertsAddedByStrip = 0;
//			
//			for (int i = 0; i < strips_count ; ++i)
//			{
//				Strip str = strips[i];
//				int curULindex = curTriIndex + i * 4;
//				int curLLindex = curULindex + 1;
////				int curURindex = curTriIndex + strips_count * 2 + i * 2;
////				int curLRindex = curURindex + 1;;
//				int curURindex = curULindex + 2;
//				int curLRindex = curULindex + 3;
//				
//				bool addULVert = true;
//				bool addLLVert = true;
//				
//				
//#if COMBINE_FLUSH_VERTS
//
//				if (horizontalDim > 0 && stripsArray[horizontalDim - 1] != null) 
//				{
//					// if this strip is flush with the start of any strips in the col to the left...
//					Strip flushWithStartOneLeft = Strip.theErsatzNullStrip();
//					bool gotAStrip = stripFromListWithStartEqualTo(str.range.start, stripsArray[horizontalDim - 1], ref flushWithStartOneLeft);
//					if (gotAStrip) // (Strip.StripNotNull(flushWithStartOneLeft))
//					{
//						curULindex = flushWithStartOneLeft.indexSet.upperRight;
//						curLLindex--;
//						curURindex--;
//						curLRindex--;
//						addULVert = false;
//					}
//					
//					Strip flushWithExtentOneLeft = Strip.theErsatzNullStrip();
//					gotAStrip =	stripFromListWithExtentEqualTo(str.range.extentMinusOne() , stripsArray[horizontalDim - 1], ref flushWithExtentOneLeft);
//					if (gotAStrip)
//					{
//						curLLindex = flushWithExtentOneLeft.indexSet.lowerRight;
//						curURindex--;
//						curLRindex--;
//						addLLVert = false;
//					}
//				}
//#endif
//				
//				if (addULVert) {
//					Vector3 ulVec = new Vector3((float) horizontalDimLessHalf, verticalHeight, (float)str.range.start - half_unit);
////					leftVertexColumn.Add(ulVec); //LEFT COLUMN
//					returnVecs.Add(ulVec);
//					vertsAddedByStrip++;
//					returnUVS.Add(monoUVValue);
//				}
//				
//				if (addLLVert) {
//					Vector3 llVec = new Vector3((float) horizontalDimLessHalf, verticalHeight, (float)str.range.extent() - half_unit);
////					leftVertexColumn.Add(llVec); //LEFT COLUMN!!
//					returnVecs.Add(llVec);
//					vertsAddedByStrip++;
//					returnUVS.Add(monoUVValue + uvLLTexDimTest);
//				}
//				
//				Vector3 urVec = new Vector3((float) horizontalDimLessHalf + (float) str.width, verticalHeight, (float)str.range.start - half_unit);
////				rightVertexColumn.Add(urVec);
//				returnVecs.Add(urVec);
//				vertsAddedByStrip++;
//				returnUVS.Add(monoUVValue + uvURTexDimTest);
//				
//				Vector3 lrVec = new Vector3((float)horizontalDimLessHalf + (float) str.width , verticalHeight, (float)str.range.extent() - half_unit);
////				rightVertexColumn.Add(lrVec);
//				returnVecs.Add(lrVec);
//				vertsAddedByStrip++;
//				returnUVS.Add(monoUVValue + uvLRTexDimTest);
//				
//				//need to index sets.
//				str.indexSet = new IndexSet(curULindex, curURindex, curLLindex, curLRindex);
//				strips[i] = str;
//				
//				int[] tris;
//				
//				if (faceIsOnPosSide) {
//					tris = new int[] {curULindex, curURindex, curLLindex,   curURindex, curLRindex, curLLindex};
//				} else {
//					tris = new int[] {curULindex, curLLindex, curURindex, 	curURindex, curLLindex, curLRindex };
//				}
//				
//				returnTriIndices.AddRange(tris);
//				
//			}
//			
//			curTriIndex += vertsAddedByStrip;
//			
////			vertexColumns[horizontalDim] = leftVertexColumn;
////			vertexColumns[horizontalDim + 1] = rightVertexColumn;
//			
//			returnVecs.AddRange(leftVertexColumn);
//			
//			if (horizontalDim == stripsArray.Length - 1)
//			{
//				returnVecs.AddRange(rightVertexColumn);
//			}
//			else 
//			{
//				leftVertexColumn = rightVertexColumn;
//			}
//			
////			stripsArray[horizontalDim] = strips;
//		}
//		
//		//now maybe just calculate the actual v3 array and the indices....
//		GeometrySet geomset = new GeometrySet(returnTriIndices, returnVecs );
//		
//		return new MeshSet(geomset, returnUVS);
//		
//	}

//Coord[] other_coords = new Coord[] {
//			new Coord(0, 2, 0),
//			new Coord(0, 2, 1),
//			new Coord(0, 2, 2),
//			new Coord(0, 2, 3),
//			new Coord(0, 2, 4),
//			
//			new Coord(1, 2, 0),
//			new Coord(1, 2, 1),
//			new Coord(1, 2, 2),
//			new Coord(1, 2, 3),
//			new Coord(1, 2, 4),
//			new Coord(1, 2, 5),
//			
//			new Coord(2, 2, 0),
//			new Coord(2, 2, 1),
//			new Coord(2, 2, 2),
//			
//			new Coord(3, 2, 1),
//			new Coord(3, 2, 2),
//			new Coord(3, 2, 3),
////			
//			new Coord(4, 2, 0),
//			new Coord(4, 2, 1),
//			new Coord(4, 2, 2),
//		};

//	private List<Quad> quadsForArea(Quad area, ref List<Quad> out_quads) // make a quad (if possible) from a given area
//	{
//		// THIS CAUSES AN 'OUT OF MEMORY' ERROR!
////		throw new Exception("we don't want to run this func. it causes an out of memory error, it seems");
//		if (iterationSafety > 1000) {
//			return new List<Quad>();	
//		}
//		
//		iterationSafety ++;
//		
//		if (areaIsFilled(area))
//		{
//			List<Quad> ret = new List<Quad>();
//			ret.Add(area);
//			return ret;
//		} else if (area.dimensions.area() == 1) {
//			return new List<Quad>();	
//		}
//		
//		if (area.dimensions.t > 1)
//		{
//			if (area.dimensions.s > 1)
//			{
//				Quad ULQuad = area.upperLeftQuarter();
//				out_quads.AddRange(quadsForArea(ULQuad, ref out_quads) );
//			}
//			
//			Quad URQuad = area.upperRightQuarter();
//			out_quads.AddRange(quadsForArea(URQuad, ref out_quads) );
//		}
//		
//		if (area.dimensions.s > 1) {
//			Quad LLQuad = area.lowerLeftQuarter();
//			out_quads.AddRange(quadsForArea(LLQuad, ref out_quads) );
//		}
//		
//		Quad LRQuad = area.lowerRightQuarter();
//		out_quads.AddRange(quadsForArea(LRQuad, ref out_quads) );
//		
//		return out_quads;
//
//	}
//	
//	private bool areaIsFilled(Quad area) 
//	{
//		if (area.dimensions.area() == 0)
//			return false;
//		
//		int i = area.origin.s;
//		int end = area.extent().s;
//		int jend = area.extent().t;
//		int jstart = area.origin.t;
//		int j;
//		for(; i < end ; ++i)
//		{
//			j = jstart;
//			for(; j < jend ; ++j)
//			{
//				if (!filledFaces[i, j])
//					return false;
//			}
//			
//		}
//		return true;
//	}

//	
