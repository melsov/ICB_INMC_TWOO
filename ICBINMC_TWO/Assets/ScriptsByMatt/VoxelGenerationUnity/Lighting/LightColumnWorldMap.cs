using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class LightColumnWorldMap  
{
	
	private ChunkManager m_chunkManager;
	
	public LightColumnWorldMap(ChunkManager chunkManager)
	{
		m_chunkManager = chunkManager;
	}
	
	//get the light columns at any woco that exists...
	
	public DiscreteDomainRangeList<LightColumn> lightColumnsAtWoco(int x, int z)
	{
		NoiseCoord nco = CoordUtil.NoiseCoordForWorldCoord(new Coord(x,0,z));
		NoisePatch npatch = m_chunkManager.blocks.noisePatchAtNoiseCoord(nco);
		if (npatch == null)
		{
			return null;
		}
		
		Coord pRelCo = CoordUtil.PatchRelativeBlockCoordForWorldBlockCoord(new Coord(x,0,z));
		
		// noisepatch return lightcol map at rel co
		return npatch.lightColumnsAt(PTwo.PTwoXZFromCoord(pRelCo));
	}
	
//	public LightColumnMap lightColumnMapAtNoiseCoord or something...
//	maybe a static func to get a noisecoord in direction etc..
}
