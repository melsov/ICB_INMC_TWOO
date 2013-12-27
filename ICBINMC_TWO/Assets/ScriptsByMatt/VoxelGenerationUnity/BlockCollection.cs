
using UnityEngine;
using System.Collections;

using System.ComponentModel;
using System.Threading;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;

using System.Collections.Generic;
using System.Linq;

using System.IO;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Runtime.ConstrainedExecution;
using System.Diagnostics;

using System.Runtime.Serialization.Formatters.Binary;

public class BlockCollection
{
	public Dictionary<NoiseCoord, NoisePatch> noisePatches = new Dictionary<NoiseCoord, NoisePatch>();
	private static Coord BLOCKSPERNOISEPATCH = NoisePatch.PATCHDIMENSIONSCHUNKS * new Coord((int)ChunkManager.CHUNKLENGTH, 
	                                                                                                   (int) ChunkManager.CHUNKHEIGHT, 
	                                                                                                   (int)ChunkManager.CHUNKLENGTH);
	private const string NOISE_PATCHES_SAVE_NAME = "SavedNoisePatches";

	public BlockCollection () 
	{
	}

	public void getSavedNoisePatches () 
	{
//		PlayerPrefs.DeleteAll ();
		//Get the data
		var data = PlayerPrefs.GetString(NOISE_PATCHES_SAVE_NAME);

		//If not blank then load it
		if(!string.IsNullOrEmpty(data))
		{
			//Binary formatter for loading back
			var b = new BinaryFormatter();
			//Create a memory stream with the data
			var m = new MemoryStream(Convert.FromBase64String(data));
				noisePatches = (Dictionary<NoiseCoord, NoisePatch>)b.Deserialize(m);

		} 
	}

	public void saveNoisePatchesToPlayerPrefs() 
	{
		var b = new BinaryFormatter();
		var m = new MemoryStream();
		b.Serialize(m, noisePatches);

		PlayerPrefs.SetString(NOISE_PATCHES_SAVE_NAME, 
			Convert.ToBase64String(
				m.GetBuffer()
			)
		);
	}

	public Block this[int xx, int yy, int zz]
	{
		get
		{
			return this [new Coord (xx, yy, zz)];
		}
		set
		{
			this [new Coord (xx, yy, zz)] = value;
		}
	}


	public Block this[Coord woco]
	{
		get 
		{
			NoiseCoord nco = noiseCoordForWorldCoord (woco);
			if (!noisePatches.ContainsKey(nco))
			{
				return null;
			}
			NoisePatch np = noisePatches [nco];

			return np.blockAtWorldBlockCoord (woco);
		}
		set
		{
			NoisePatch np = noisePatches [noiseCoordForWorldCoord (woco)];
			np.setBlockAtWorldCoord (value, woco);
		}
	}

	
	public Texture2D textureForTerrainAtXEqualsZero()
	{
		List<Color> colorData = new List<Color> ();

		Color[][] colorAr = new Color[ChunkManager.CHUNKHEIGHT] [];

		for (int i  = 0; i < colorAr.Length; ++i) {
			colorAr [i] = new Color[ChunkManager.CHUNKLENGTH * NoisePatch.CHUNKDIMENSION];
		}

		int lowestZCoord = 0;
		int highestZCoord = 0;

		int patchCount = 0;

		foreach(KeyValuePair<NoiseCoord, NoisePatch> npatch in noisePatches)
		{
			NoiseCoord nco = npatch.Key;

			if (nco.x != 0)
				continue;

			lowestZCoord = nco.z < lowestZCoord ? nco.z : lowestZCoord;
			highestZCoord = nco.z > highestZCoord ? nco.z : highestZCoord;
		}

		//get patch count
		for (int zi = lowestZCoord; zi <= highestZCoord; zi++ ) {
			NoiseCoord nco = new NoiseCoord (0, zi);
			if (!noisePatches.ContainsKey(nco)) {
				continue;
			}
			NoisePatch np = noisePatches [nco];
			if (np.terrainSlice != null) {
				patchCount++;

			}
		}

		bug ("patch count was: " + patchCount);
		int patchWidth = (int)(ChunkManager.CHUNKLENGTH * NoisePatch.CHUNKDIMENSION);
		int tex2DWidth = (int)patchWidth * patchCount; 
		Texture2D result = new Texture2D (tex2DWidth, (int)(ChunkManager.CHUNKHEIGHT));

		int iterations = 0;

		for (int zi = lowestZCoord; zi <= highestZCoord; zi++, iterations++ ) 
		{
			NoiseCoord nco = new NoiseCoord (0, zi);
			if (!noisePatches.ContainsKey(nco)) {
				continue;
			}
			NoisePatch np = noisePatches [nco];
			if (np.terrainSlice != null) 
			{

				int texSliceWidth = np.terrainSlice.GetLength (1);

				for (int j = 0; j < np.terrainSlice.GetLength(0); ++j) 
				{
					for (int k = 0; k < texSliceWidth; ++k) 
					{
						result.SetPixel (k + patchWidth * iterations, j, np.terrainSlice [j, k]);
					}
				}
			}
		}

		return result;
	}
	

	public NoisePatch noisePatchAtWorldCoord (Coord woco) {
		NoiseCoord nco = noiseCoordForWorldCoord (woco);
		if (!noisePatches.ContainsKey(nco)) 
			return null;
		return noisePatches [nco];
	}
	
	public bool noisePatchExistsAtNoiseCoord(NoiseCoord nco) {
		return noisePatches.ContainsKey(nco);
	}
	
	public bool noisePatchExistsAtWorldCoord(Coord woco) {
		NoiseCoord nco = noiseCoordForWorldCoord(woco);
		return noisePatches.ContainsKey(nco);
	}
	
	public NoiseCoord noiseCoordContainingWorldCoord(Coord woco) {
		return noiseCoordForWorldCoord(woco);
	}
	
	private static NoiseCoord noiseCoordForWorldCoord(Coord woco){
		woco =  woco - (woco.booleanNegative () * (BLOCKSPERNOISEPATCH  - 1)) ; // (shift neg coords by -1)
		return new NoiseCoord (woco / BLOCKSPERNOISEPATCH);
	}
	
	public bool noisePatchAtCoordIsReady(NoiseCoord nco) {
		if (!noisePatches.ContainsKey(nco))
			return false;
		
		return noisePatches[nco].generatedBlockAlready;
	}	
	
	public List<Range1D> heightsListAtWorldCoord(Coord woco) {
		NoisePatch np = noisePatchAtWorldCoord(woco);
		
		if (np == null) {
			throw new Exception("trying to get a heights list for which we don't have a noise patch");	
			return null;
		}
		
		return np.heightsListAtWorldCoord(woco);
			
	}

//	public Block specialGetBlockForTesting(Coord woco) 
//	{
//
//		NoiseCoord nco = noiseCoordForWorldCoord (woco);
//		bug ("special get block NOISE coord: " + nco.toString ());
//
//		if (!noisePatches.ContainsKey(nco))
//		{
////			throw new System.ArgumentException ("noise co not in the dictionary: " + nco.toString ());
//			return null;
//		}
//
//		NoisePatch np = noisePatches [nco];
//
//		Coord relCo = np.patchRelativeBlockCoordForWorldBlockCoord (woco);
//		bug ("special get block at world co at patch relative coo: " + relCo.toString ());
//
//		return np.blockAtWorldBlockCoord (woco);
//
//	}

	void bug (string str) {
		UnityEngine.Debug.Log (str);
	}


}

