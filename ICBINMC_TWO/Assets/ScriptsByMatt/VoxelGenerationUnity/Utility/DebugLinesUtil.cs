using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class DebugLinesUtil
{
	
	public static void drawDebugCubesForAllCreatedNoisePatches(NoiseCoord currentTargetedForCreationNoiseCo, Dictionary<NoiseCoord,NoisePatch> noisePatchDictionary)
	{
		if (!noisePatchDictionary.ContainsKey(currentTargetedForCreationNoiseCo))
			return;
		// current to be created n patch
		NoisePatch curTargeted = noisePatchDictionary[currentTargetedForCreationNoiseCo];
		
//		Color tarCol = curTargeted.generatedBlockAlready ? Color.gray : Color.yellow;
		Color tarCol = curTargeted.IsDone ? Color.gray : Color.yellow;
		tarCol = curTargeted.hasStarted ? new Color(.3f, 1f, .9f, 1f) : tarCol;
		
		//TODO: create a coroutine for getting rid of noisepatches that are far away?
		
		drawDebugLinesForNoisePatch(currentTargetedForCreationNoiseCo, tarCol);
		
		foreach(KeyValuePair<NoiseCoord, NoisePatch> npatch in noisePatchDictionary)
		{
			NoiseCoord nco = npatch.Key;
			if (NoiseCoord.Equal(nco, currentTargetedForCreationNoiseCo) )
				continue;
			
			NoisePatch np = npatch.Value;
			Color col = np.hasStarted ? Color.magenta : Color.cyan;
			if (np.IsDone)
				col = Color.green;
			
			drawDebugLinesForNoisePatch(nco, col );
		}
	}
	
	public static void drawDebugCubesForNoisePatchesList(List<NoisePatch> setupThesePatches, Color color, int offset)
	{
		foreach(NoisePatch np in setupThesePatches)
		{
			drawDebugLinesForNoisePatch(np.coord, color , offset);	
		}
		
	}
	
	public static void drawDebugCubesForNoiseCoordList(List<NoiseCoord> nCos, Color color, int offset)
	{
		foreach(NoiseCoord nco in nCos)
		{
			drawDebugLinesForNoisePatch(nco, color , offset);	
		}
		
	}
	
	public static void drawDebugCubesForAllUncreatedNoisePatches(List<NoisePatch> setupThesePatches)
	{
		drawDebugCubesForAllUncreatedNoisePatches(setupThesePatches, Color.green);
	}
	
	public static void drawDebugCubesForAllUncreatedNoisePatches(List<NoisePatch> setupThesePatches, Color color)
	{
		drawDebugCubesForNoisePatchesList(setupThesePatches, color, 5);

	}
	
	
	
	public static void drawDebugCubesForAllUncreatedChunks(List<Coord> createTheseVeryCloseAndInFrontChunks)
	{
		foreach(Coord chco in createTheseVeryCloseAndInFrontChunks)
		{
			drawDebugForChunkCoord(chco, Color.cyan);
		}
	}
	
	public static void drawDebugCubesForChunksOnDestroyList(List<Chunk> destroyTheseChunks)
	{
		Coord nudge = new Coord(3, 0, 3); // avoid occluding create very close lines
		foreach(Chunk ch in destroyTheseChunks)
		{
			drawDebugForChunkCoord(ch.chunkCoord, Color.red, nudge);
		}
	}
	
	public static void drawDebugCubesForChunksOnCheckASyncList(List<Chunk> checkTheseAsyncChunksDoneCalculating)
	{
		Coord nudge = new Coord(-3, 0, -3); // avoid occluding create very close lines
		foreach(Chunk ch in checkTheseAsyncChunksDoneCalculating)
		{
			drawDebugForChunkCoord(ch.chunkCoord, Color.blue, nudge);
		}
	}
	
	public static void drawDebugCubesForChunkCoordList(List<Coord> coordList, Color color, Coord nudgeCo)
	{
		foreach(Coord chco in coordList)
		{
			drawDebugForChunkCoord(chco, color, nudgeCo);
		}
	}
	
	public static void drawDebugCubesForChunkList(List<Chunk> chunkList, Color color, Coord nudgeCo)
	{
		foreach(Chunk ch in chunkList)
		{
			drawDebugForChunkCoord(ch.chunkCoord, color, nudgeCo);
		}
	}
	
	public static void drawDebugLinesForBlockAtWorldCoord(Coord woco)
	{
		CoRange blockCo = new CoRange (woco, Coord.coordOne ());
		drawDebugCube (blockCo, true);
	}
	
	static void drawDebugForChunkCoord(Coord chunkCo)
	{
		drawDebugForChunkCoord(chunkCo, Color.magenta);
	}
	
	static void drawDebugForChunkCoord(Coord chunkCo, Color color_)
	{
		drawDebugForChunkCoord(chunkCo, color_, Coord.coordZero());
	}
	
	static void drawDebugForChunkCoord(Coord chunkCo, Color color_, Coord nudge)
	{
		Coord woco = chunkCo * ChunkManager.CHUNKLENGTH + nudge;
		drawDebugCube(woco, new Coord(ChunkManager.CHUNKLENGTH, ChunkManager.CHUNKHEIGHT, ChunkManager.CHUNKLENGTH), color_);
	}
	
	static void drawDebugLinesForChunkRange(CoRange chunkCoRange)
	{
		drawDebugCube (chunkCoRange, false);
	}
	
	static void drawDebugLinesForNoisePatch(NoiseCoord nco, Color col) 
	{
		drawDebugLinesForNoisePatch(nco, col, 3);
	}
	
	static void drawDebugLinesForNoisePatch(NoiseCoord nco, Color col, int offset) 
	{
		Coord woco = worldCoordForNoiseCoord(nco) + offset;
		Coord dims = new Coord(ChunkManager.CHUNKLENGTH * NoisePatch.CHUNKDIMENSION, 
			ChunkManager.CHUNKHEIGHT, ChunkManager.CHUNKLENGTH * NoisePatch.CHUNKDIMENSION) - 3;
		
		drawDebugCube(woco, dims, col);
	}
	
	static void drawDebugCube(CoRange chunkCoRange, bool drawBlock)
	{
		int length = drawBlock ? 1 : (int) ChunkManager.CHUNKLENGTH;
		Coord start = chunkCoRange.start * length;
				
		if (drawBlock)
		{
			drawDebugBlock(start, chunkCoRange.range * length);
			return;
		}
		
		drawDebugCube(start, chunkCoRange.range * length);
	}
	
	static void drawDebugCube(Coord start, Coord dims)
	{
		drawDebugCube(start, dims, Color.white);
	}
	
	static void drawDebugCube(Coord start, Coord dims, Color col)
	{
//		int length = drawBlock ? 1 : (int) CHUNKLENGTH;
//		Coord start = start; // * length;
		Coord outer = start + dims; // chunkCoRange.outerLimit () * length;

		//upper box
		debugLine (start, new Coord (start.x, start.y, outer.z), col );
		debugLine (start, new Coord (outer.x , start.y, start.z), col );
		debugLine (new Coord (outer.x, start.y, outer.z), new Coord (start.x, start.y, outer.z), col );
		debugLine (new Coord (outer.x, start.y, outer.z), new Coord (outer.x , start.y, start.z), col );

		debugLine (outer, new Coord (start.x, outer.y, outer.z), col );
		debugLine (outer, new Coord (outer.x , outer.y, start.z), col );
		debugLine (new Coord (start.x, outer.y, start.z), new Coord (start.x, outer.y, outer.z), col );
		debugLine (new Coord (start.x, outer.y, start.z), new Coord (outer.x , outer.y, start.z), col );
		
		//diag on top
		debugLine(new Coord (start.x, outer.y, start.z), outer);
	}
	
	static void drawDebugBlock(Coord start, Coord dims)
	{
		Vector3 startV = start.toVector3 () - new Vector3 (.5f, .5f, .5f);
		Coord outer = start + dims;
		Vector3 outerV = outer.toVector3 () - new Vector3 (.5f, .5f, .5f);

		//upper box
		debugLineV (startV, new Vector3 (startV.x, startV.y, outerV.z));
		debugLineV (startV, new Vector3 (outerV.x , startV.y, startV.z));
		debugLineV (new Vector3 (outerV.x, startV.y, outerV.z), new Vector3 (startV.x, startV.y, outerV.z));
		debugLineV (new Vector3 (outerV.x, startV.y, outerV.z), new Vector3 (startV.x , startV.y, startV.z));

		debugLineV (outerV, new Vector3 (startV.x, outerV.y, outerV.z));
		debugLineV (outerV, new Vector3 (outerV.x , outerV.y, startV.z));
		debugLineV (new Vector3 (startV.x, outerV.y, startV.z), new Vector3 (startV.x, outerV.y, outerV.z));
		debugLineV (new Vector3 (startV.x, outerV.y, startV.z), new Vector3 (outerV.x , outerV.y, startV.z));
	}
	
	static void debugLine(Coord aa, Coord bb)
	{
		UnityEngine.Debug.DrawLine (aa.toVector3(), bb.toVector3());
	}
	
	static void debugLine(Coord aa, Coord bb, Color color)
	{
		UnityEngine.Debug.DrawLine (aa.toVector3(), bb.toVector3(), color);
	}

	static void debugLineV(Vector3 aa, Vector3 bb)
	{
		UnityEngine.Debug.DrawLine (aa, bb);
	}
	
	static void debugLineV(Vector3 aa, Vector3 bb, Color color)
	{
		UnityEngine.Debug.DrawLine (aa, bb, color);
	}
	
	static Coord worldCoordForNoiseCoord(NoiseCoord nco) {
		int blocksPerPatch = (int) (NoisePatch.CHUNKDIMENSION * ChunkManager.CHUNKLENGTH);
		return new Coord(nco * blocksPerPatch);
	}
	
}
