using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Wintellect.PowerCollections;

public struct CoordLine
{
	public Coord start;
	public Coord end;
	
}

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
	
	
	
	public static void drawDebugCubesForAllUncreatedChunks(Set<Coord> createTheseVeryCloseAndInFrontChunks)
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
//	
//	public static void DrawDebugCubesForChunksCoords(List<Coord> chunkCos)
//	{
//		Coord nudge = new Coord(3, 0, 1); // avoid occluding create very close lines
//		foreach(Coord chco in chunkCos)
//		{
//			drawDebugForChunkCoord(chco, Color.yellow, nudge);
//		}
//	}
	
	public static void DrawDebugCubesForChunksCoords(Set<Coord> chunkCos)
	{
		Coord nudge = new Coord(3, 0, 3); // avoid occluding create very close lines
		foreach(Coord co in chunkCos)
		{
			drawDebugForChunkCoord(co, Color.red, nudge);
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
	
	public static void DrawDebugForChunkCoord(Coord chunkCo, Color color_)
	{
		drawDebugForChunkCoord(chunkCo, color_, Coord.coordZero());
	}
	
	static void drawDebugForChunkCoord(Coord chunkCo, Color color_, Coord nudge)
	{
		Coord woco = chunkCo * ChunkManager.CHUNKLENGTH + nudge;
		drawDebugCube(woco, new Coord(ChunkManager.CHUNKLENGTH, ChunkManager.CHUNKHEIGHT, ChunkManager.CHUNKLENGTH), color_);
	}
	
	public static void drawDebugLinesForChunkRange(CoRange chunkCoRange)
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
	
	public static void DrawUnitCubeAt(Coord woco)
	{
		drawDebugCube(woco, new Coord(1), Color.magenta);	
	}
	
	static void drawDebugCube(Coord start, Coord dims)
	{
		drawDebugCube(start, dims, Color.white);
	}
	
	public static void DrawWindow(Window win)
	{
		Coord origin = win.worldRelativeOrigin;
		Coord upperNeg =origin;
		upperNeg.y += win.startHeightRange.range;
		Coord lowerPos = origin;
		lowerPos.z += win.zExtent - win.patchRelativeOrigin.z;
		Coord upperPos = lowerPos;
		upperPos.y += win.endHeightRange.range;
		
		float light  = win.midPointLight.lightValue/Window.LIGHT_LEVEL_MAX;		
		
		Color winColor = new Color(light, .5f, .5f, 1f);
		if (win.xCoord == 0)
			winColor = new Color(1f, 0f, 0f, 1f);
		else if (win.xCoord == 1)
			winColor = new Color(0f, 0f, 1f, 1f);
			
		
		DebugBox(origin, upperNeg, lowerPos, upperPos, winColor , new Color(light, .5f, .5f, 1f) );
	}
	
	public static void DebugBox(Coord ll, Coord ul, Coord lr, Coord ur, Color _col, Color auxCol)
	{
		debugLine(ll, ul, _col);
		debugLine(ul, ur, _col);
		debugLine(ur, lr, _col);
		debugLine(lr, ll, _col);
//		debugLine(ll, ur, auxCol);
	}
	
	public static void DrawDebugColumn(Column colm)
	{
		Coord start = CoordUtil.WorldCoordFromNoiseCoord(colm.noiseCoord);
		
		start += new Coord(colm.xz.s, colm.range.start, colm.xz.t);
		Coord dims = new Coord(1, colm.range.range, 1);
		
		
//		Color col = new Color(1.0f*((float) colm.handyInteger/Window.LIGHT_LEVEL_MAX), .4f, 1.0f, 1.0f); //red
		Color col = new Color(1.0f, .2f, 0.0f, 1.0f); //red
		int level = colm.handyInteger; // colm.xz.s | colm.xz.t | colm.range.start | colm.range.range; //   == 0 ? 0 : 2;
		switch(level % 8) {
		case 0:
			col = new Color(1.0f, 1f, 0.0f, 1.0f); //yellow
			break;
		case 1:
			col = new Color(0f, 1f, 0f, 1.0f); //green
			break;
		case 2:
			col = new Color(0f, 1f, 1.0f, 1.0f); //cyan
			break;
		case 3:
			col = new Color(0f, 0f, 1.0f, 1.0f); //blue
			break;
		case 4:
			col = new Color(1f, 1f, 1.0f, 1.0f); //white
			break;
		case 5:
			col = new Color(.5f, .2f, .3f, 1.0f); //red brown
			break;
		case 6:
			col = new Color(.5f, .3f, .7f, 1.0f); //blue brown ?
			break;
		default:
			//red
			break;
			
		}
		
		drawDebugCube(start, dims, col);
//		drawDebugCubeJustDiagonal(start, dims, col);
	}
	
	
	public static void drawDebugQuad(Quad quad, Color col)
	{

		Coord start = CoordUtil.WorldCoordFromNoiseCoord(NoiseCoord.NoiseCoordWithPTwo(quad.origin));
		Coord outer = CoordUtil.WorldCoordFromNoiseCoord(NoiseCoord.NoiseCoordWithPTwo(quad.extent()));
		

		//lower box
		debugLine (start, new Coord (start.x, start.y, outer.z), col );
		debugLine (start, new Coord (outer.x , start.y, start.z), col );
		debugLine (new Coord (outer.x, start.y, outer.z), new Coord (start.x, start.y, outer.z), col );
		debugLine (new Coord (outer.x, start.y, outer.z), new Coord (outer.x , start.y, start.z), col );
	}
	
	
	static void drawDebugCube(Coord start, Coord dims, Color col)
	{
//		int length = drawBlock ? 1 : (int) CHUNKLENGTH;
//		Coord start = start; // * length;
		Coord outer = start + dims; // chunkCoRange.outerLimit () * length;

		//lower box
		debugLine (start, new Coord (start.x, start.y, outer.z), col );
		debugLine (start, new Coord (outer.x , start.y, start.z), col );
		debugLine (new Coord (outer.x, start.y, outer.z), new Coord (start.x, start.y, outer.z), col );
		debugLine (new Coord (outer.x, start.y, outer.z), new Coord (outer.x , start.y, start.z), col );
		
		//upperbox
		debugLine (outer, new Coord (start.x, outer.y, outer.z), col );
		debugLine (outer, new Coord (outer.x , outer.y, start.z), col );
		debugLine (new Coord (start.x, outer.y, start.z), new Coord (start.x, outer.y, outer.z), col );
		debugLine (new Coord (start.x, outer.y, start.z), new Coord (outer.x , outer.y, start.z), col );
		
		//vertical
		debugLine(start, new Coord(start.x, outer.y, start.z), col);
		debugLine(new Coord (start.x, start.y, outer.z),new Coord (start.x, outer.y, outer.z), col);
		debugLine(new Coord (outer.x , start.y, start.z),new Coord (outer.x , outer.y, start.z), col);
		debugLine(new Coord(outer.x, start.y, outer.z), outer, col);
		
		//diag on top
//		debugLine(new Coord (start.x, outer.y, start.z), outer);
		
		debugLine(start, outer, col );  // across 
	}
	
	static void drawDebugCubeJustDiagonal(Coord start, Coord dims, Color col)
	{
//		int length = drawBlock ? 1 : (int) CHUNKLENGTH;
//		Coord start = start; // * length;
		Coord outer = start + dims; // chunkCoRange.outerLimit () * length;
		
		debugLine(start, outer, col );  // across 
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
	
	public static void debugLine(Coord aa, Coord bb)
	{
		UnityEngine.Debug.DrawLine (aa.toVector3(), bb.toVector3());
	}
	
	public static void debugLine(Coord aa, Coord bb, Color color)
	{
		UnityEngine.Debug.DrawLine (aa.toVector3() - new Vector3(.5f, .5f, .5f), bb.toVector3() - new Vector3(.5f, .5f, .5f), color);
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
