

/*
 
--find a good way to get light columns at noisepatch borders to trade with neighbors
	--CONSIDER: don't build lightcol maps until the chunk wants to build. (at this point noise patch neighbor (the relevant one)
			should be ready. we think.
			--DOWNSIDE: main thread execution time. REMEDY: go back to alt thread chunk creation? 
--GO back to alt thread chunk creation? LOOK INTO: the bugs asso with this.
 

SOLVED:
--At border of noisePatches, if a structure is a 63 of one and discontinuity in surface at 0 coord of other, discon mistakenly
thinks its exposed to the surface.

 * 
 */ 