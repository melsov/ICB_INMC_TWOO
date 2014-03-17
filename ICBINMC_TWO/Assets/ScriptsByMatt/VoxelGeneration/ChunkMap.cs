using UnityEngine;
using System.Collections;


//using UnityEngine;
//using System.Collections;
using System.Collections.Generic;
using System.Linq;

using System.IO;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Runtime.ConstrainedExecution;
using System.Diagnostics;

using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Threading;
using System.Collections.Specialized;
using System.Runtime.InteropServices;

public class ChunkMap
{
	Dictionary<Coord, Chunk> chunks;

	public ChunkMap()
	{
		chunks = new Dictionary<Coord, Chunk> ();
	}
	
	public Chunk chunkContainingCoord(Coord co)
	{
		Coord chunkCo = CoordUtil.ChunkCoordContainingBlockCoord(co);
		if (!coIsOnMap (chunkCo))
			return null;

		return chunkAt(chunkCo);
	}

	public void addChunkAt(Chunk the_chunk, Coord c)
	{
		if (chunks.ContainsKey(c))
		{
			destroyChunkAt (c);
			chunks [c] = the_chunk;	
			return;
		}
		chunks.Add (c, the_chunk);
		ChunkManager.debugLinesAssistant.addFunnyChunkCoord(c);
	}

	public void destroyChunkAt(Coord c) {

		if (!chunks.ContainsKey(c))
			return;

		Chunk ch = chunks[c];
		if (ch != null) {
			destroyChunkMeshOfChunk (ch);
//			ChunkManager.debugLinesAssistant.addFunnyChunkCoord(c);
			ch.isActive = false;
			ch.resetCalculatedAlready();
			chunks.Remove(ch.chunkCoord);
		}
	}

	void destroyChunkMeshOfChunk(Chunk ch) 
	{
		GameObject chObj = ch.meshHoldingGameObject; 
		GameObject.Destroy (chObj);
		ch.destroyAndSetGameObjectToNull ();
	}

	public Chunk chunkAt(int ii, int jj, int kk){
		return chunkAt (new Coord (ii, kk, jj));
	}

	public Chunk chunkAt(Coord co) {
		if (!chunks.ContainsKey(co))
		{
			return null;
		}

		return chunks [co]; 
	}

	public Chunk chunkAtOrNullIfUnready(int ii, int jj, int kk) {
		return chunkAtOrNullIfUnready (new Coord (ii, kk, jj));
	}

	public Chunk chunkAtOrNullIfUnready(Coord coo) 
	{
		return chunkAt(coo);
	}

	public bool coIsOnMap(Coord co) {
		return chunks.ContainsKey (co);
	}
	
	public bool isChunkAtCoordCurrentlyBuilding(Coord co)
	{
		Chunk ch = chunkAt(co);
		if (ch == null) return false;
		return isChunkCurrentlyBuilding(ch);
	}
	
	public bool isChunkCurrentlyBuilding(Chunk chunk)
	{
		return (chunk.hasStarted && !chunk.IsDone);
	}

	public List<Chunk> nonNullChunksInCoRange(CoRange _corange) {
		return chunksInCoRange (_corange, true);
	}

	public List<Chunk> chunksInCoRange(CoRange _corange, bool excludeNullChunks) {
		List<Chunk> retChunks = new List<Chunk> ();
		if (!chunks.ContainsKey(_corange.start))
			return retChunks;

		Coord start = _corange.start;
		Coord range = _corange.range;
		int i = start.x;
		for (; i < start.x + range.x; ++i) {

			int j = (int) start.y;
			for (; j < start.y + range.y; ++j) {

				int k = start.z;
				for (; k < start.z + range.z; ++k) {
					Chunk chh = chunkAt (i, j, k);
					if (!excludeNullChunks || ( chh != null )) {
						retChunks.Add (chh);
					}
				}
			}
		}
		return retChunks;
	}

	void bug(string str) {
		UnityEngine.Debug.Log (str);
	}

}

