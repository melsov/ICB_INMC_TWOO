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
}
