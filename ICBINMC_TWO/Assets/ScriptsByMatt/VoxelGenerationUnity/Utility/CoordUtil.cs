using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class CoordUtil 
{
	public static Coord WorldCoordForChunkCoord(Coord chunkCo)
	{
		return new Coord ((int)(chunkCo.x * ChunkManager.CHUNKLENGTH),
							0, 
							(int) (chunkCo.z * ChunkManager.CHUNKLENGTH));	//CHUNKS ALWAYS Y ZERO
	}
	
	public static Coord WorldCoordFromNoiseCoord(NoiseCoord nCo)
	{
		return new Coord(nCo.x, 0, nCo.z) * NoisePatch.patchDimensions;
	}
	
	public static Coord WorldCoordFromNoiseCoordAndPatchRelativeOffset(NoiseCoord nco, Coord offset)
	{
		return CoordUtil.WorldCoordFromNoiseCoord(nco) + offset;
	}
	
	public static Coord PatchRelativeCoordWithWorldChunkCoordOffset(Coord chCoord, Coord offset)
	{
//		chCoord = PatchRelativeChunkCoordForChunkCoord(chCoord);
//		Coord patchCoord = chCoord * ChunkManager.CHUNKLENGTH;	
//		return patchCoord + offset;
		
		return PatchRelativeBlockCoordForWorldBlockCoord(WorldCoordForChunkCoord(chCoord) + offset);
	}
	
	public static Coord WorldRelativeChunkCoordForNoiseCoord(NoiseCoord nco)
	{
		return new Coord(nco.x * NoisePatch.CHUNKDIMENSION, 0 , nco.z * NoisePatch.CHUNKDIMENSION);	
	}
	
	public static Coord PatchRelativeChunkCoordForChunkCoord(Coord chunkCo) 
	{
		// pos chunk coords are 'array index friendly' in their current state, because they start at 0,0,0 .
		// not so neg chunk coords.
		// neg chunk coords start at -1 (globally) so we have to shift them by pos one (-1 becomes 0)
		// and then flip the array index that they then indicate (0 is the 4th elem in patch at -1,1. so 0 -> (chunkDim - 1 - 0) -> 3
		// put another way: chunk co (-1,0,0) is at patch Rel Co (3, 0, 0) (for a 4x4 patch whose x,z lower left corner == the xz ll
		// corner of chunk at co (-4,0,0) )

		// use booleanNeg func. as a mask to massage neg chunk coords
		Coord boolNeg = chunkCo.booleanNegative ();
		chunkCo = chunkCo + boolNeg; // e.g. x at -7 (+1) becomes -6
		chunkCo = chunkCo % NoisePatch.PATCHDIMENSIONSCHUNKS; //  CHUNKDIMENSION; (NOTE: SWAPPED OUT CHDIMS FOR NEW TALL CHUNKS) // x becomes -2

		// - 2 (plus chunkDim) becomes 2 (minus boolNeg) becomes 1 
		// then take a mod again because positive chunk dims will have gone out of bounds
		// then multiply by chunklength (blocks per chunk is a dimension) to get the rel block coord of the chunk (i.e. 
		// the coord of its 'lower left bottom' block -- i.e. block at the chunk's 0,0,0 coord).
		return ((chunkCo + NoisePatch.PATCHDIMENSIONSCHUNKS - boolNeg ) % NoisePatch.PATCHDIMENSIONSCHUNKS); //massage negative coords
	}
	
	
	public static Coord PatchRelativeBlockCoordForWorldBlockCoord(Coord woco) 
	{
		// this function parallels the patchRelBlockCoord function above

		// example woco = (-1,0,0)
		Coord boolNeg = woco.booleanNegative ();
		woco = woco + boolNeg; // x at -1 becomes 0
		woco = woco % NoisePatch.patchDimensions; //  BLOCKSPERPATCHLENGTH; // 0 -> 0

		// 0 -> (BPPL = 64) 64 -> (minus booleanNeg) 63 (mod again to put pos coords back where they belong)
		return ((woco + NoisePatch.patchDimensions - boolNeg)) % NoisePatch.patchDimensions; // BLOCKSPERPATCHLENGTH;
	}
	
	public static List<Coord> WorldRelativeChunkCoordsWithinNoiseCoord(NoiseCoord nco)
	{
		Coord chCoOrigin = WorldRelativeChunkCoordForNoiseCoord(nco);
		
		List<Coord> result = new List<Coord>();
		
		for(int i = 0; i < NoisePatch.CHUNKDIMENSION; ++i)
		{
			for(int j = 0; j < NoisePatch.CHUNKDIMENSION; ++j)
			{
				result.Add(chCoOrigin + new Coord(i, 0, j));
			}
		}
		return result;
	}
	
	public static Coord ChunkCoordContainingBlockCoord(Coord co)
	{
		// if a coord member is negative, member - CHUNKLENGTH will give us the chunkCoord we want.
		// E.G. -2, 3, 4 is inside chunkCoord -1, 0, 0. so (-2 - 16, 3, 4) / 16 = (-1,0,0)
		// (-2, 3, 4) / 16 = 0,0,0 (!not what we want!)
		
		Coord chcoAdjustNeg = co.booleanNegative () * Chunk.DIMENSIONSINBLOCKS; // CHUNKLENGTH;
		return (co + co.booleanNegative() - chcoAdjustNeg)  / Chunk.DIMENSIONSINBLOCKS; //  CHUNKLENGTH;
	}
	
	public static NoiseCoord NoiseCoordForWorldCoord(Coord woco)
	{
//		return BlockCollection.noiseCoordForWorldCoord(woco);
		woco =  woco - (woco.booleanNegative () * (BlockCollection.BLOCKSPERNOISEPATCH  - 1)) ; // (shift neg coords by -1)
		return new NoiseCoord (woco / BlockCollection.BLOCKSPERNOISEPATCH);
	}
	

	public static Coord ChunkRelativeCoordForWorldCoord(Coord worldBlockCoord) 
	{
		return (worldBlockCoord + worldBlockCoord.booleanNegative() ) % Chunk.DIMENSIONSINBLOCKS + worldBlockCoord.booleanNegative() * (Chunk.DIMENSIONSINBLOCKS - 1);
	}	
	
	public static List<NoiseCoord> NoiseCoordsWithingQuad(Quad area)
	{
		SimpleRange sRange = area.sSimpleRange();
		SimpleRange tRange = area.tSimpleRange();
		List<NoiseCoord> result = new List<NoiseCoord>();
		
		for(int i = sRange.start; i < sRange.extent() ; ++i)
		{
			for(int j = tRange.start; j < tRange.extent() ; ++j)
			{
				result.Add(new NoiseCoord(i,j));
			}			
		}
		
		return result;
	}
	
	public static List<PTwo> PointsJustInsideBorderOfQuad(Quad area)
	{
		return PointsInsideBorderOfQuad(area, 0);
	}
	
	public static List<PTwo> PointsInsideBorderOfQuad(Quad area, int inset)
	{
		List<PTwo> result = new List<PTwo>();
		SimpleRange sRange = area.sSimpleRange();
		SimpleRange tRange = area.tSimpleRange();
		
		if (inset > sRange.range/2 || inset > tRange.range/2)
			return result;
		
		int outerNudge = inset + 1;
		
		int start = sRange.start + inset;
		int otherDim = tRange.start + inset;
		int otherDimOuter = tRange.extent() - outerNudge;
		
		for(int i = start; i < sRange.extent() - outerNudge; ++i)
		{
			result.Add(new PTwo(i, otherDim));
			result.Add(new PTwo(i, otherDimOuter));
		}
		
		start = tRange.start + inset + 1;
		otherDim = sRange.start + inset;
		otherDimOuter = sRange.extent() - outerNudge;
		
		for(int i = start; i < tRange.extent() - outerNudge - 1; ++i)
		{
			result.Add(new PTwo(otherDim, i));
			result.Add(new PTwo(otherDimOuter, i));
		}
		
		return result;
		
	}
}
