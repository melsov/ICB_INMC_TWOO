using UnityEngine;
using System.Collections;

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
	
	public static Coord PatchRelativeCoordWithChunkCoordOffset(Coord chCoord, Coord offset)
	{
		Coord patchCoord = chCoord * NoisePatch.CHUNKDIMENSION;	
		return patchCoord + offset;
	}
}
