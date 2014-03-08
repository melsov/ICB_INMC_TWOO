#define NEW_PATCH_READY
//#define TERRAIN_TEST

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
	public static Coord BLOCKSPERNOISEPATCH = NoisePatch.PATCHDIMENSIONSCHUNKS * new Coord((int)ChunkManager.CHUNKLENGTH, 
	                                                                                                   (int) ChunkManager.CHUNKHEIGHT, 
	                                                                                                   (int)ChunkManager.CHUNKLENGTH);
	private const string NOISE_PATCHES_SAVE_NAME = "SavedNoisePatches";
	
	private Quad domain = Quad.theErsatzNullQuad();
	
	// TODO: set up a save system where different sets of noise patches are saved by
	// seed. basically, there should be a 'world' object.
	// it knows its seed(s)
	// it can have a handy name, maybe a screen shot (? why not.)
	// and it knows its noise_patch_save_name.
	
	// basically, there's a PlayerPrefs key: "SavedGames"
	// and this holds a list of 'World' objects.
	// and these objects have the nec info to load the world in question, etc...

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
			
			updateDomain();
		} 
	}
	
	private void updateDomain()
	{
		foreach(KeyValuePair<NoiseCoord,NoisePatch> keyVal in noisePatches)
		{
			NoiseCoord nco = keyVal.Key;
			if (domain.isErsatzNull())
			{
				domain = Quad.UnitQuadWithPoint(PTwo.PTwoXZFromNoiseCoord(nco));
			} else {
				domain.expandedToContainPoint(PTwo.PTwoXZFromNoiseCoord(nco));
			}
		}
		
		//debug
		ChunkManager.debugLinesAssistant.debugQuad = domain;
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

#if TERRAIN_TEST
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
#endif

	public NoisePatch noisePatchAtWorldCoord (Coord woco) {
		NoiseCoord nco = noiseCoordForWorldCoord (woco);
		if (!noisePatches.ContainsKey(nco)) 
			return null;
		return noisePatches [nco];
	}
	
	public NoisePatch noisePatchAtNoiseCoord (NoiseCoord nco) {
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
	
	public static NoiseCoord noiseCoordForWorldCoord(Coord woco){
		
		return CoordUtil.NoiseCoordForWorldCoord(woco);
//		woco =  woco - (woco.booleanNegative () * (BLOCKSPERNOISEPATCH  - 1)) ; // (shift neg coords by -1)
//		return new NoiseCoord (woco / BLOCKSPERNOISEPATCH);
	}
	
	public void destroyPatchAt(NoiseCoord nco) 
	{
		if (noisePatches.ContainsKey(nco)) {
			NoisePatch npToDestroy = noisePatches[nco];
			if (npToDestroy.hasStarted || !npToDestroy.IsDone)
				return;

			bool destroyed = noisePatches.Remove(nco);	
			if (!destroyed) {
				throw new Exception("failed to destroy..." + nco.toString());
			}	
		} else {
			b.bug("we don't have the key: " + nco.toString());	
		}
	}
	
	public bool noisePatchAtCoordIsReady(NoiseCoord nco) {
#if NEW_PATCH_READY
		if (noisePatchAtNoiseCoordHasBuiltAtleastOnce(nco))
		{
			return noisePatches[nco].neighborsHaveAllBuiltAtLeastOnce;	
		}
		return false;
#endif
		
		if (!noisePatches.ContainsKey(nco))
			return false;
#if NEW_PATCH_READY
		
#else
		return noisePatches[nco].generatedBlockAlready;
#endif
	}
	
	public void addNoisePatchAt(NoiseCoord nco, NoisePatch _npatch) {
		noisePatches.Add(nco, _npatch);
		//domain limits
		if (Quad.Equal(this.domain, Quad.theErsatzNullQuad())) {
			//first time
			domain = new Quad(new PTwo(nco.x, nco.z), PTwo.PTwoOne());
		} else {
			domain = domain.expandedToContainPoint(new PTwo(nco.x, nco.z));	
		}
		
		ChunkManager.debugLinesAssistant.debugQuad = domain;
	}
	
	// TODO: rewrite to return just one far away nco?
	public List<NoiseCoord> furthestNoiseCoords() {
		
		List<NoiseCoord> result = new List<NoiseCoord>();
		
		if (domain.dimensions.s < 3 || domain.dimensions.t < 3)
			return result;
		
		NoiseCoord xnudge = new NoiseCoord(1,0);
		NoiseCoord znudge = new NoiseCoord(0,1);
		
		//mins (traverse min edge of domain on the z and x sides until we find a coord contained in noisePatches)
		if (noisePatches.ContainsKey(NoiseCoord.NoiseCoordWithPTwo(domain.origin)))
		{	
			result.Add(NoiseCoord.NoiseCoordWithPTwo(domain.origin));
		} else {
			// xmin z
			NoiseCoord xminz = NoiseCoord.NoiseCoordWithPTwo(domain.origin);
			while (xminz.z < domain.extent().t)
			{
				xminz += znudge;
				if (noisePatches.ContainsKey(xminz)) {
					result.Add(xminz);
					break;
				}
				if (xminz.z == domain.extent().t - 1) {
					domain.origin.s++;	
					domain.dimensions.s--;
					
				}
			}
			NoiseCoord zminx = NoiseCoord.NoiseCoordWithPTwo(domain.origin);
			while (zminx.x < domain.extent().s)
			{
				zminx += xnudge;
				if (noisePatches.ContainsKey(zminx)) {
					result.Add(zminx);
					break;
				}
				if (zminx.x == domain.extent().s - 1) {
					domain.origin.t++;	
					domain.dimensions.t--;
					
				}
			}
		}
		
		//maxes
		NoiseCoord extentCoord = NoiseCoord.NoiseCoordWithPTwo(domain.extent());
		if (noisePatches.ContainsKey(extentCoord))
		{
			result.Add(extentCoord);	
		} else {
			
			NoiseCoord xmaxz = extentCoord;
			while(xmaxz.z >= domain.origin.t)
			{
				xmaxz -= znudge;
				if (noisePatches.ContainsKey(xmaxz)) {
					result.Add(xmaxz);
					break;
				}
				if (xmaxz.z == domain.origin.t) {
					domain.dimensions.s--;
				}
			}
			
			NoiseCoord zmaxx = extentCoord;
			while(zmaxx.x >= domain.origin.s)
			{
				zmaxx -= xnudge;
				if (noisePatches.ContainsKey(zmaxx)) {
					result.Add(zmaxx);
					break;
				}
				if (zmaxx.x == domain.origin.s) {
					domain.dimensions.t--;
				}
			}
		}
		
		return result;
	}
	
	public bool noisePatchAtNoiseCoordHasBuiltAtleastOnce(NoiseCoord nco) {
		if (!noisePatches.ContainsKey(nco))
			return false;

		return (!noisePatches[nco].hasStarted && noisePatches[nco].IsDone);
	}
	
	public bool noisePatchAtNoiseCoordHasBuiltOrIsBuildingCurrently(NoiseCoord nco) {
		if (!noisePatches.ContainsKey(nco))
			return false;

		return (noisePatches[nco].hasStarted || noisePatches[nco].IsDone);
	}
	
	public List<Range1D> heightsListAtWorldCoord(Coord woco) {
		NoisePatch np = noisePatchAtWorldCoord(woco);
		
		if (np == null) {
			throw new Exception("trying to get a heights list for which we don't have a noise patch");	
			return null;
		}
		
		return np.heightsListAtWorldCoord(woco);
			
	}
	
	public CoordSurfaceStatus worldCoordIsAboveSurface(Coord woco) {
		NoisePatch np = noisePatchAtWorldCoord(woco);
		
		if (np == null) {
			throw new Exception("trying to get a heights list for which we don't have a noise patch");	
			return CoordSurfaceStatus.ABOVE_SURFACE;
		}
		
		return np.worldCoordIsAboveSurface(woco);
			
	}
	
	public float ligtValueAtWorldCoord(Coord woco, Direction dir)
	{
		NoisePatch np = noisePatchAtWorldCoord(woco);
		
		if (np == null) {
			throw new Exception("trying to get a heights list for which we don't have a noise patch");	
			return 0f; 
		}
		
		return np.lightValueAtPatchRelativeCoord(CoordUtil.PatchRelativeBlockCoordForWorldBlockCoord(woco), dir);
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

