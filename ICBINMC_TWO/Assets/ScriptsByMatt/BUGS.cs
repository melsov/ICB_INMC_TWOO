

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

/*
 PLANNING:
 --If light columns could deal with adjacent (vertically) light columns...
 	then maybe they could also deal with torches and other light emitting blocks. 
 	CONSIDER: what this would break, if anything...
 	
 	-- REALLY MIGHT BE EASIER TO HAVE A SEPARATE SYSTEM!
 		--if we could simply know, any lightemitting blocks within a woco.
 		
 	--IF we did those fancy shadows. might be able to devise a way to know whether the air block can "see" the light block.
 	--like how many walls are nearby...
 	
 	-- or: super simple: all light blocks are just light columns: who cares if they're super tall!
 	-- make lights a little weak? (nawh). lights are just powerful!
 */