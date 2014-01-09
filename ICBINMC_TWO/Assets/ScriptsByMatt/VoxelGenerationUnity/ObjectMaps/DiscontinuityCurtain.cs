using UnityEngine;
using System.Collections;

public class DiscontinuityCurtain 
{
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
