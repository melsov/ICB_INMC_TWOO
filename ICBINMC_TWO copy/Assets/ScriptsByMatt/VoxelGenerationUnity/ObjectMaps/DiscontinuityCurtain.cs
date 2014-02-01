using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Wintellect.PowerCollections;

using System;

//public struct Box3
//{
//	public Coord origin;
//	public Coord range;
//	
//	public Box3( Coord _or, Coord _ra) {
//		origin = _or; range = _ra;	
//	}
//	
//	public Coord extent() {
//		return origin + range;
//	}
//	
//	public Coord extentMinusOne() {
//		return origin + range - 1;
//	}
//}

public struct ZCurtainUnit
{
//	SimpleRange startHeightRange;
//	SimpleRange endHeightRange;
	public SimpleRange zRange;
	
	public bool startIsOpen, endIsOpen;
	
//	public ZCurtain(SimpleRange start_height_range, SimpleRange end_height_range, SimpleRange z_range, bool startOpen, bool endOpen) {
//		startHeightRange = start_height_range; end_height_range = end_height_range; zRange = z_range;
//		startIsOpen = startOpen; endIsOpen = endOpen;
//	}
	
	public ZCurtainUnit(int z_start) {
		zRange = new SimpleRange(z_start, 1);
		startIsOpen = false; endIsOpen = false;
	}
	
	public ZCurtainUnit(SimpleRange z_range) {
		zRange = z_range;
		startIsOpen = false; endIsOpen = false;
	}
	
	public ZCurtainUnit(SimpleRange z_range, bool startOpen, bool endOpen) {
		zRange = z_range;
		startIsOpen = startOpen; endIsOpen = endOpen;
	}
	
	public ZCurtainUnit(int z_start, bool startOpen) {
		this = new ZCurtainUnit(new SimpleRange(z_start, 1), startOpen, false);
	}
}

public class ZCurtain
{
	private List<ZCurtainUnit> sections = new List<ZCurtainUnit>();
	
	public ZCurtain(int woco_z_start)
	{
		ZCurtainUnit first = new ZCurtainUnit(woco_z_start);
		sections.Add(first);
	}
	
	public int worldStartZ {
		get {
			ZCurtainUnit start_curtainu = sections[0];
			return start_curtainu.zRange.start;	
		}
	}
	
	public int worldEndZ {
		get {
			return sections[sections.Count - 1].zRange.extent();	
		}
	}
	
	public bool startIsOpen {
		get {
			if (sections.Count > 0) {
				ZCurtainUnit first = sections[0];
				return first.startIsOpen;
			}
			return false;	
		}
	}
	
	public bool endIsOpen {
		get {
			if (sections.Count > 0) {
				ZCurtainUnit last = sections[sections.Count - 1];
				return last.endIsOpen;
			}
			return false;	
		}
	}
	
	public ZDiscontinuityPointCoverageStatus z_contains(int woco_z) {
		if (woco_z < this.worldStartZ)
		{
			return ZDiscontinuityPointCoverageStatus.BeyondBoundsZNeg;
		}
		
		if (woco_z > this.worldEndZ)
		{
			return ZDiscontinuityPointCoverageStatus.BeyondBoundsZPos;
		}
		
		foreach (ZCurtainUnit zcu in sections) {
			if (zcu.zRange.contains(woco_z))
				return ZDiscontinuityPointCoverageStatus.WithinDiscontinuity;
			
			if (zcu.zRange.start > woco_z)
				return ZDiscontinuityPointCoverageStatus.WithinBoundsOutsideOfDiscontinuity;
		}
		
		return ZDiscontinuityPointCoverageStatus.WithinBoundsOutsideOfDiscontinuity;
	}
	
	public void addNewCurtainUnitWithZNegStartAt(int woco_z) 
	{
		ZCurtainUnit zcu = new ZCurtainUnit(woco_z);
		
		// greater than end
		if (woco_z > this.worldEndZ)
		{
			sections.Add(zcu);
			return;
		}
		
		if (woco_z < this.worldStartZ)
		{
			sections.Insert(0, zcu);
			return;
		}
		
		int insert_at = 0;
		//check contiains
		for(int i = 0; i < sections.Count; ++i) 
		{
			ZCurtainUnit _zcu = sections[i];
			if (_zcu.zRange.contains(woco_z)) {
				throw new Exception("trying to add a woco z that we already contain...");	
			}
			
			if (_zcu.zRange.start < woco_z) {
				
				if (i == 0)
					throw new Exception("confusing: why did we get here then (adding new curtain unit..)");
				
				insert_at = i - 1;
				break;
			}
		}

		sections.Insert (insert_at, zcu);
	}
	
	public void extendCurtainToIncludeNonTerminationWocoZ(int woco_z) 
	{
		extendCurtainToIncludeWocoZ(woco_z, true, Range1D.theErsatzNullRange(), Range1D.theErsatzNullRange(), false);
	}
	
	public void extendCurtainToIncludeWocoZ(int woco_z, Range1D adjacentSurfaceContinuityZPosNeg, Range1D airRange) 
	{
		extendCurtainToIncludeWocoZ(woco_z, true, adjacentSurfaceContinuityZPosNeg, airRange, true);
	}
	
	private void extendCurtainToIncludeWocoZ(int woco_z, bool extendInZPosForMiddle, Range1D adjacentSurfaceContinuityZPosNeg, Range1D airRange, bool setTerminalStatus) 
	{
		// greater than end
		if (woco_z >= this.worldEndZ)
		{
			ZCurtainUnit lastcu = sections[sections.Count - 1];
			lastcu.zRange = lastcu.zRange.extendRangeToInclude(woco_z);
			sections[sections.Count - 1] = lastcu;
			if (setTerminalStatus)
				setDiscontinuityEndWithSurfaceRange(adjacentSurfaceContinuityZPosNeg, airRange);
			return;
		}
		
		if (woco_z <= this.worldStartZ)
		{
			ZCurtainUnit firstcu = sections[0];
			firstcu.zRange = firstcu.zRange.extendRangeToInclude(woco_z);
			sections[0] = firstcu;
			if (setTerminalStatus)
				setDiscontinuityStartWithSurfaceRange(adjacentSurfaceContinuityZPosNeg, airRange);
			return;	
		}
		
		if (sections.Count == 1) {
			ZCurtainUnit zcu = sections[0];
			if (zcu.zRange.contains(woco_z))
				return;
		}
		
		if (sections.Count < 2) {
			return;
		} // can't be anthing more to do!
		
		// Between ranges
		// now we need bool extend in z pos
		
		SimpleRange zRange;
		SimpleRange nextZRange;
		bool combineWithNext = false;
		int i = 0;
		ZCurtainUnit zcurtain_unit;
		ZCurtainUnit nextZCurtain_unit;
		for(; i < sections.Count - 1; ++i)
		{
			zcurtain_unit = sections[i];
			nextZCurtain_unit = sections[i + 1];
			
			zRange = zcurtain_unit.zRange;
			nextZRange = nextZCurtain_unit.zRange;
			
			if (zRange.contains(woco_z))
				return;
			
			if (zRange.extent() <= woco_z && nextZRange.start > woco_z)
			{
				if (extendInZPosForMiddle)
				{
					zRange = SimpleRange.SimpleRangeWithStartAndExtent(zRange.start, woco_z + 1);
				} else {
					nextZRange = SimpleRange.SimpleRangeWithStartAndExtent(woco_z, nextZRange.extent());	
				}
					
				//connected up now?
				if (zRange.extent() == nextZRange.start) {
					zRange.range += nextZRange.range;
					zcurtain_unit.endIsOpen = nextZCurtain_unit.endIsOpen;
					sections.RemoveAt(i + 1); // remove next
				} else {
					nextZCurtain_unit.zRange = nextZRange;
					sections[i + 1] = nextZCurtain_unit;	
				}
				
				zcurtain_unit.zRange = zRange;
				sections[i] = zcurtain_unit;
				break;
			}
		}
	}
	
	public void setDiscontinuityStartWithSurfaceRange(Range1D lastContinuityBeforeRange, Range1D airRange_) 
	{
		setDiscontinuityWithSurfaceRange(lastContinuityBeforeRange, airRange_, true);
	}
	
	public void setDiscontinuityEndWithSurfaceRange(Range1D lastContinuityBeforeRange, Range1D airRange_) 
	{
		setDiscontinuityWithSurfaceRange(lastContinuityBeforeRange, airRange_, false);
	}
	
	private void setDiscontinuityWithSurfaceRange(Range1D lastContinuityBeforeRange, Range1D airRange_, bool wantStart) 
	{
		if (sections.Count < 1)
			return;
		
		SimpleRange surfRange = new SimpleRange(lastContinuityBeforeRange.start, lastContinuityBeforeRange.range);
		SimpleRange airRange = new SimpleRange(airRange_.start, airRange_.range);
		
		ZCurtainUnit zcu;
		if (wantStart) zcu = sections[0];
		else zcu = sections[sections.Count - 1];
		
		// whether 
		zcu.startIsOpen = SimpleRange.SimpleRangeCoversRange(surfRange, airRange);
	}
	
	public bool zCoordContainedByCurtain(int z_co) {
		return z_co >= this.worldStartZ &&	z_co < this.worldEndZ;
	}
	
	private bool theresIsAnotherElementAfter(int index) {
		return index < sections.Count - 1;	
	}
	
	public IEnumerable zCurtainsRanges() {
		foreach(ZCurtainUnit zcu in sections) {
			yield return zcu.zRange;	
		}
	}
	
}

public enum ZDiscontinuityPointCoverageStatus {
 	BeyondXDomain, BeyondBoundsZNeg, BeyondBoundsZPos,  ZAdjacentToStart, ZAdjacentToEnd, WithinBoundsOutsideOfDiscontinuity, WithinDiscontinuity
}

public enum XDiscontinuityPointCoverageStatus {
	BeyondBoundsXNeg, BeyondBoundsXPos,  XAdjacentToLowerLimit, XAdjacentToUpperLimit, WithinXDomain
}

public class DiscontinuityCurtain 
{
	Quad bounds;
	Deque<ZCurtain> z_curtains = new Deque<ZCurtain>();
	
	public DiscontinuityCurtain(PTwo xzStartCoord, Range1D continuityRangeAtZMinusOne, Range1D airRange) 
	{
		bounds = Quad.UnitQuadWithPoint(xzStartCoord);
		
		ZCurtain inital_zc = new ZCurtain(xzStartCoord.t);
		inital_zc.setDiscontinuityStartWithSurfaceRange(continuityRangeAtZMinusOne, airRange);
		
		z_curtains.Add(inital_zc);
	}
	
	#region point coverage status
	
	public ZDiscontinuityPointCoverageStatus zCoverageStatusForPoint(PTwo woco_pointxz) 
	{
		if (!bounds.sRange().contains(woco_pointxz.s)) {
			
			return ZDiscontinuityPointCoverageStatus.BeyondXDomain;
		}
		
		PTwo relCo = woco_pointxz - bounds.origin;
		return z_curtains[relCo.s].z_contains(woco_pointxz.t);
	}
	
//	public ZDiscontinuityPointCoverageStatus zCoverageStatusForPointIncludeXAdjaceny(PTwo woco_pointxz)
//	{
//		XDiscontinuityPointCoverageStatus xcoverage = this.xCoverageStatusForPoint(woco_pointxz);
//		
//		if (xcoverage <= XDiscontinuityPointCoverageStatus.BeyondBoundsXPos) //beyond bounds pos or neg
//			return false;
//		
//		if (xcoverage == XDiscontinuityPointCoverageStatus.XAdjacentToLowerLimit) {
//			woco_pointxz.s++;	
//		} else if (xcoverage == XDiscontinuityPointCoverageStatus.XAdjacentToUpperLimit) {
//			woco_pointxz.s--;	
//		}
//		
//		ZDiscontinuityPointCoverageStatus zcoverage = this.zCoverageStatusForPoint(woco_pointxz);
//		
//		if (zcoverage <= ZDiscontinuityPointCoverageStatus.BeyondBoundsZPos) {
//				
//		}
//	}
	
	public XDiscontinuityPointCoverageStatus xCoverageStatusForPoint(PTwo woco_pointxz) 
	{
		Range1D srange = bounds.sRange();
		if (woco_pointxz.s > srange.extentMinusOne())
		{
			if (srange.extent() == woco_pointxz.s)
				return XDiscontinuityPointCoverageStatus.XAdjacentToUpperLimit;
			
			return XDiscontinuityPointCoverageStatus.BeyondBoundsXPos;
		}
		if (woco_pointxz.s < srange.start)
		{
			if (srange.start - 1 == woco_pointxz.s)
				return XDiscontinuityPointCoverageStatus.XAdjacentToLowerLimit;
			return XDiscontinuityPointCoverageStatus.BeyondBoundsXNeg;
		}

		return XDiscontinuityPointCoverageStatus.WithinXDomain;
	}
	
	#endregion
	
	#region extend curtain
	
	public void extendWithXZCoordWithoutTerminatingRanges(PTwo xzco) 
	{
		extendWithXZCoord(xzco, Range1D.theErsatzNullRange(), Range1D.theErsatzNullRange(), false);	
	}
	
	public void extendWithXZCoordAndTerminatingRanges(PTwo xzco, Range1D continuityRangeAtZPlusOrMinusOne, Range1D airRange)
	{
		extendWithXZCoord(xzco, continuityRangeAtZPlusOrMinusOne, airRange, true);
	}
	
	//TODO: need extra parameters for the air and surface at z plus 1!
	private void extendWithXZCoord(PTwo xzco, Range1D continuityRangeAtZPlusOrMinusOne, Range1D airRange, bool setTerminatingDisConStatus)
	{
		Range1D x_domain = bounds.sRange();
		if (x_domain.contains(xzco.s)) 
		{
			PTwo relCo = xzco - bounds.origin;	
			ZCurtain zcur = z_curtains[relCo.s];
			if (setTerminatingDisConStatus)
				zcur.extendCurtainToIncludeWocoZ(xzco.t, continuityRangeAtZPlusOrMinusOne, airRange);
			else 
				zcur.extendCurtainToIncludeNonTerminationWocoZ(xzco.t);
			
			z_curtains[relCo.s] = zcur;
			
			bounds = bounds.expandedToContainPoint(xzco);
		}
	}
	
	#endregion
	
	#region add new curtain
	
	public bool addNewZCurtainSectionAt(PTwo xzco, Range1D continuityRangeAtZPlusOrMinusOne, Range1D airRange)
	{
		if (doAddNewZCurtainSectionAt(xzco, continuityRangeAtZPlusOrMinusOne, airRange)) {
			bounds = bounds.expandedToContainPoint(xzco);
			return true;
		}
		return false;
	}
	
	private bool doAddNewZCurtainSectionAt(PTwo xzco, Range1D continuityRangeAtZPlusOrMinusOne, Range1D airRange)
	{
		if (!thereIsAdjacencyOnOneSideOrTheOther(xzco))
			return false; 
		
		Range1D x_domain = bounds.sRange();
		if (x_domain.contains(xzco.s)) 
		{
			PTwo relCo = bounds.origin - xzco;	
			ZCurtain zcur = z_curtains[relCo.s];
			
			zcur.addNewCurtainUnitWithZNegStartAt(xzco.t);
			zcur.setDiscontinuityStartWithSurfaceRange(continuityRangeAtZPlusOrMinusOne, airRange);
			z_curtains[relCo.s] = zcur;

			return true;
		}
		
		if (x_domain.start - 1 == xzco.s)
		{
			ZCurtain zc = new ZCurtain(xzco.t);	
			zc.setDiscontinuityStartWithSurfaceRange(continuityRangeAtZPlusOrMinusOne, airRange);
			z_curtains.Insert(0, zc);
			return true;
		}
		else if (x_domain.extent() == xzco.s)
		{
			ZCurtain zc = new ZCurtain(xzco.t);	
			zc.setDiscontinuityStartWithSurfaceRange(continuityRangeAtZPlusOrMinusOne, airRange);			
			z_curtains.Add(zc);
			return true;
		}
		
		return false;
	}
	
	#endregion
	
	private bool checkAdjacencyAt(int xcoToCheckAgainst, int zco) 
	{
		if (!bounds.sRange().contains(xcoToCheckAgainst))
			return false;
		
		ZCurtain zcurtain = z_curtains[bounds.origin.s - xcoToCheckAgainst];
		foreach(SimpleRange curtainUnitRange in zcurtain.zCurtainsRanges()) 
		{
			if (curtainUnitRange.contains(zco)) {
				return true;
			}
		}
		return false;
	}
	
	private bool thereIsAdjacencyOnOneSideOrTheOther(PTwo pointToAdd)
	{
		if (checkAdjacencyAt(pointToAdd.s - 1, pointToAdd.t)) {
			return true;
		}
		return this.checkAdjacencyAt(pointToAdd.s + 1, pointToAdd.t);
	}

	
	// from a bird's eye view
	// a discontinuity curtain is a squiggle on the map
	// CORRECTION: a discontinuity curtain is a squiggle WITH RANGES STICKING OUT OF IT -- GOING IN THE Z DIRECTION on the map
	//  -- AND these are special ranges that know whether their end ends in solid or another piece of discontinuity (sort of)
	// representing boundaries in the continuity of the surface.
	
	// a curtain could be closed or open-ended
	// can only be one block in width. can traverse blocks diagonally.
	// not bounded by the dimensions of noise_patches.
	// therefore, somehow there's an independent map of them
	// and that map has to be able to answer the question: 'which curtains are within my quad? for chunks etc..'
	
	// they have sets of some curtain unit object -- that owns a range representing the curtain height...
	// may also have (sort of) surface normal representing which side is below stuff.
	// that way an xz face can know: 'how far (if at all) am I behind the curtain?'
	
	//BUILDING CURTAINS:
	/*
	 * we're building the map...we're at a given x and z
	 * we encounter some range that's on top of some other range
	 * record the point...
	 * if we're at 0,0 we don't know what was behind us (either x or z neg)
	 * for what we don't know (at the edge of a noise patch) (or the beginning of the world creation)
	 * assume temporarily that there was nothing on top of the surface range in the unknown direction.
	 * means any on-top-ness encountered at x or z = 0 border is automatically considered 'semi-legitimate' discontinuity
	 * 
	 * also, for our encountered 'on-top-ness' point, check its surrounding points (that are already known)
	 * if it is adjacent to non-on-topness it is legit.
	 * if it is surrounded on the four orthagonal sides by at least semi-legit it is illegit.
	 * --in this case, if any of the surrounding orthos are fully legit, the legit to illegit direction becomes the 'normal.'
	 * if its adjacent to a known 'legit' or 'semi-legit' point and not all of its adjacent points are known add it to that points curtain as a semi-legit
	 * */
	
	//BUILDING CURTIANS 2.0!
	/*
	 * Last way was needlessly troubled...
	 * there's a bool for 'lastZWasCovered' -- Curtain Map handles this so is noise patch independent. (except for the very first block! oh no. oh well? just handle this)
	 * we find on top ness
	 * 	--was last z covered?
	 * 	--if yes:
	 * 		--extend curtain unit's z depth by one (i.e. the range of its range) 
	 *  --if no:
	 * 		--make a new curtain unit.
	 * 		--was the last z point (the non covered) partially adjacent to the covering range at this point?
	 * 		--if yes
	 * 			--this is not the start of a curtain unit (necessarily). its the solid end of its range. (i.e. near_end_open = false)
	 * 		--if no
	 * 			--near end open = true;
	 * 		
	 * 		-- set last-z-covered to true
	 * 
	 * we find lack of on-top-ness
	 * 	--was last z covered?
	 * 	--if yes
	 * 		--is the new, not covered point 'connected to' (i.e. partially adjacent to) the last z covering range?
	 * 		--if yes 
	 * 			--this c unit ends in solidity --its extent Minus One is not another curtain start.
	 * 		--if no
	 * 			--this c unit ends in a curtain end. (it could be double ended or just this one, we don't need to know hopefully)
	 * 		--set last z covered to false
	 *  --if no
	 * 		--nothing to do here
	 * 
	 * so the curtain map takes the form of a list of 'special' ranges for each z dimension. ?? maybe not. see curtain map
	 * so some kind of table of key: z coord, value: the list of special ranges.
	 * special ranges have the same start and end as normal ranges but they have an extra enum that keeps track of FAR_END_OPEN, NEAR_END_OPEN, BOTH_ENDS_OPEN, NEITHER_END_OPEN.
	 *  -- still need some trickiness for adding c units from neg noise coords i.e. noisepatch at 1, -1...its z rel 15 is world -1, z rel 0 is world -16
	 * 
	 * curtains proper are back because its not enough to know only the z +/- openness of a curtain unit.
	 * also need to know... all of this same FAR/NEAR/BOTH/NEITHER OPEN info in the x direction...!
	 * its ok. the discont curtain class just keeps building... collecting those z curtain units.
	 * if it reaches a point when there's no more adjacency to 'be had.'
	 * it finds all the points of 'exposed ness' in its z curtains.-- the 'above and below ranges' from its neighbors on the x pos and neg sides
	 *  and measures the vertical exposed-ness at all of those points -- in other words: how much of this vertical range is not covered from the surface up?
	 * 
	 * */
	
	//CURTAIN MAPS can tell us whether a woco is in a curtain. 
	/*
	 * D CURTAIN maintenance
	 * an add solid / translucent block at coord type thing...
	 * */
	
	// maybe the container of the curtain unit things should be a deque? a list that's good at adding to both ends?
	// since, when we encounter discontinuity, we foresee only 3 options: 
	// 1) connected to the end of a curtain
	// 2) connected (i.e. one square 'away') to the beginning of a curtain
	// 3) on its own (new curtain).
	
	// curtains units are located under the above discontinuity (because ultimately there has to be lower continuity below throughout the world--bedrock)
	// in other words, a single block floating above a surface would have a curtain directly underneath it.
	
	// good if curtains can connect to other curtains.
	// the case of a curtain going underneath another curtain--e.g. two land bridges one over the other. -- curtain builders must not get fooled by these!
	// luckily, we can do a simple case for now since only one discontinuity section is possible right now...but this will probably (hopefully) change.
	// i.e. caves may not get 'proper' curtains.
	
	/*
	 * CURTAIN MAP:
	 * does the curtain unit building, being fed discontinuity by noisepatch
	 * probably noisepatch constructs a struct for each 'on topness' it encounters and passes it
	 * when it finishes building a new curtain unit--meaning it resolves both of its ends, or gets to the end of a noise patch also? (maybe no...)
	 * it checks the x pos neg dirs for adjacent curtains.
	 * d curtains are stored in a table. key some xz coord. (noise coord?). value a list of indices. containing any curtains that intersect with the noise patch
	 * curtains can be in more than one noise patch. TODO FIRST: make this kind of table and test it a lot! (it's its own class, this table is.)
	 * d curtains own a quad that describes the area they cover.
	 * that way, we can quickly check if a newly formed z curtain unit has a chance at being added to the d curtain
	 * if it has a chance, we go through the two lists (for that is what they are) of the special z curtain units on either side. 
	 * FOR A C UNIT TO JOIN A D CURTAIN IT MUST: have overlap in the Z and Y dirs.
	 * SO, a d curtain, as mentioned, owns lists of z curtain units.
	 * also, d curtains owns a separate list of lists of ranges that get added to, as c unit neighbors as are found, of the x side exposure of curtains.
	 * maybe c units should each own such a list of ranges?
	 * 
	 * */
	
	/*
	 * SOME CLASS 
	 * needs ultimately to grab any and all curtains that overlap the quad taken up by a FaceSet.
	 * thinking who other than...FaceAg or Set? (or a helper class really)
	 * don't want to recalc the light levels every time we rebuild a chunk, unless we actually need to (but then we want to only edit the light levels?)
	 * won't be so bad.. dis curtain owns YA table?...
	 * 
	 * can...OK simple case: can provide an integer (or short) for any (up to) 2 by 4 quad taken up by a half of a face set.
	 * 
	 * NOTE: no way for a faceset to be influenced by more than one d curtain
	 * 
	 * on the chunk level. there's a flag: recalculate d curtains
	 * when face Agg getsGeometry, if the recalc flag is true
	 * it accesses (from somewhere)
	 * a list of discontinuity curtains that may influence it.
	 * 
	 * OK SOME CLASS: 2.0
	 * is really no class at all. instead we must (finally)
	 * make a new struct, BlockRange.
	 * it owns a Range1D and will have to replace Range1D in a lot of code!
	 * it also owns a light level for top and bottom
	 * and a blocktype and whatever else--thus freeing range to be simple again!
	 * BlockRanges are passed to the chunk as its (now mis-named) YsurfaceMap
	 * then it can tell the meshbuilder to tell the faceAgg to tell the faceSet
	 * what the light level is at that coord.
	 * 
	 * */
	
}
