using UnityEngine;
using System.Collections;

public class GraveYardCode {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

//
//public class ChunkMapOLD_MID_CONVERT_TO_DICTIONARY
//{
//	//	Chunk[,,] chunks;
//
//	Dictionary<Coord, Chunk> chunks;
//
//	//	private uint CHUNKWORLDLENGTH = 4;
//
//	private Coord m_mapDimensions;
//
//	private int destroyedTotalTest;
//
//	//	private ChunkCoord mapDimensions; 
//
//	public ChunkMap(Coord world_dims)
//	{
//
//		m_mapDimensions = world_dims; // new ChunkCoord( world_dims);
//		//		chunks = new Chunk[m_mapDimensions.x, m_mapDimensions.y, m_mapDimensions.z];
//		chunks = new Dictionary<Coord, Chunk> ();
//	}
//
//	public CoRange makeCoRangeSafe(CoRange _cora) {
//		//		throw 
//		return _cora.makeIndexSafeCoRange (m_mapDimensions);
//	}
//
//	public void addChunkAt(Chunk the_chunk, Coord c)
//	{
//		if (chunks.ContainsKey(c))
//		{
//			destroyChunkAt (c);
//			chunks [c] = the_chunk;	
//			return;
//		}
//
//		chunks.Add (c, the_chunk);
//
//	}
//
//	//	public void addChunkAt(Chunk the_chunk, Coord c)
//	//	{
//	//		if (!c.isIndexSafe (m_mapDimensions))
//	//			return;
//	//
//	//		destroyChunkAt (c);
//	//
//	//		chunks [c.x, c.y, c.z] = the_chunk;
//	//	}
//
//
//	public void destroyChunkAt(Coord c) {
//
//		if (!chunks.ContainsKey(c))
//			return;
//
//		Chunk ch = chunks[c];
//		if (ch != null) {
//			destroyChunkMeshOfChunk (ch);
//			ch.isActive = false;
//		}
//
//		//TODO DONE: purge (replace with an ACTIVEFLAG)
//		//chunks [c.x, c.y, c.z] = null; 
//
//	}
//
//	//	public void destroyChunkAt(Coord c) {
//	//
//	//		if (!c.isIndexSafe (m_mapDimensions))
//	//			return;
//	//
//	//		Chunk ch = chunks[c.x, c.y, c.z];
//	//		if (ch != null) {
//	//			destroyChunk (ch);
//	//			ch.isActive = false;
//	//		}
//	//
//	//		//TODO DONE: purge (replace with an ACTIVEFLAG)
//	//		//chunks [c.x, c.y, c.z] = null; 
//	//
//	//	}
//
//	void destroyChunkMeshOfChunk(Chunk ch) 
//	{
//		//		foreach (Component compo in ch.GetComponents<SphereCollider>())
//		//			GameObject.Destroy (compo);
//		//		foreach (Component compo in ch.GetComponents< MeshFilter>())
//		//			GameObject.Destroy (compo);
//		//		foreach (Component compo in ch.GetComponents< MeshRenderer>())
//		//			GameObject.Destroy (compo);
//		//		foreach (Component compo in ch.GetComponents<Component>()) //destroy the rest (yeah!)
//		//			GameObject.Destroy (compo);
//
//		GameObject chObj = ch.meshHoldingGameObject; //  ch.transform.gameObject;
//
//
//		//		GameObject.Destroy (ch );
//		GameObject.Destroy (chObj);
//		destroyedTotalTest++;
//
//		//		bug("total destroys: " +destroyedTotalTest);
//		ch.destroyAndSetGameObjectToNull ();
//	}
//
//	//	void destroyChunk(Chunk ch) //OLD WAY 
//	//	{
//	////		foreach (Component compo in ch.GetComponents<SphereCollider>())
//	////			GameObject.Destroy (compo);
//	////		foreach (Component compo in ch.GetComponents< MeshFilter>())
//	////			GameObject.Destroy (compo);
//	////		foreach (Component compo in ch.GetComponents< MeshRenderer>())
//	////			GameObject.Destroy (compo);
//	////		foreach (Component compo in ch.GetComponents<Component>()) //destroy the rest (yeah!)
//	////			GameObject.Destroy (compo);
//	//
//	//		GameObject chObj = ch.transform.gameObject;
//	//
//	//
//	////		GameObject.Destroy (ch );
//	//		GameObject.Destroy (chObj);
//	//	}
//
//
//	//	public Chunk chunkAt(ChunkCoord co) {
//	//		return chunks [co.x, co.y, co.z];
//	//	}
//
//	public Chunk chunkAt(int ii, int jj, int kk){
//		return chunkAt (new Coord (ii, kk, jj));
//	}
//
//	public Chunk chunkAt(Coord co) {
//		if (!chunks.ContainsKey(co))
//		{
//			bug ("no chunk at this co");
//			return null;
//		}
//
//		//		if (!co.isIndexSafe (m_mapDimensions))
//		//			return null;
//
//		return chunks [co]; //dictionary way
//	}
//
//	public Chunk chunkAtOrNullIfUnready(int ii, int jj, int kk) {
//		return chunkAtOrNullIfUnready (new Coord (ii, kk, jj));
//	}
//
//	public Chunk chunkAtOrNullIfUnready(Coord coo) {
//		Chunk chh = chunkAt (coo);
//
//		if (chh == null) { //TODO: change this logic later...
//			return null;
//		}
//
//		return chh;
//	}
//
//	public bool coIsOnMap(Coord co) {
//		return chunks.ContainsKey (co);
//		//		return co.isIndexSafe (m_mapDimensions);
//	}
//
//	public List<Chunk> nonNullChunksInCoRange(CoRange _corange) {
//		return chunksInCoRange (_corange, true);
//	}
//
//	public List<Chunk> chunksInCoRange(CoRange _corange, bool excludeNullChunks) {
//		List<Chunk> retChunks = new List<Chunk> ();
//		if (!chunks.ContainsKey(_corange.start))
//			return retChunks;
//
//		//		if (!coIsOnMap (_corange.start))
//		//			return retChunks;
//
//		Coord start = _corange.start;
//		Coord range = _corange.range;
//		int i = start.x;
//		for (; i < start.x + range.x; ++i) {
//
//			int j = (int) start.y;
//			for (; j < start.y + range.y; ++j) {
//
//				int k = start.z;
//				for (; k < start.z + range.z; ++k) {
//					Chunk chh = chunkAt (i, j, k);
//					if (!excludeNullChunks || ( chh != null )) {
//						retChunks.Add (chh);
//					}
//				}
//			}
//		}
//		return retChunks;
//	}
//
//	void bug(string str) {
//		UnityEngine.Debug.Log (str);
//	}
//
//}
//
//public class BlockCollection
//{
//	public Dictionary<NoiseCoord, NoisePatch> noisePatches = new Dictionary<NoiseCoord, NoisePatch>();
//	private int BLOCKSPERNOISEPATCH;
//
//	public BlockCollection(int _bpNP) {
//		BLOCKSPERNOISEPATCH = _bpNP;
//	}
//
//	public Block this[int xx, int yy, int zz]
//	{
//		get
//		{
//			return this [new Coord (xx, yy, zz)];
//		}
//		set
//		{
//			this [new Coord (xx, yy, zz)] = value;
//		}
//	}
//
//
//	public Block this[Coord woco]
//	{
//		get 
//		{
//			NoisePatch np = noisePatches [noiseCoordForWorldCoord (woco)];
//			Coord relCoord = woco % BLOCKSPERNOISEPATCH;
//			return np.blocks [relCoord.x, relCoord.y, relCoord.z];
//		}
//		set
//		{
//			NoisePatch np = noisePatches [noiseCoordForWorldCoord (woco)];
//			Coord relCoord = woco % BLOCKSPERNOISEPATCH;
//			np.blocks [relCoord.x, relCoord.y, relCoord.z] = value;
//		}
//	}
//
//	private NoiseCoord noiseCoordForWorldCoord(Coord woco){
//		return new NoiseCoord (woco / BLOCKSPERNOISEPATCH);
//	}
//}
//
//public struct NoiseCoord
//{
//	public int x; 
//	public int z;
//
//	public NoiseCoord(int xx, int zz) {
//		x = xx;
//		z = zz;
//	}
//
//	public NoiseCoord(Coord cc) {
//		x = cc.x;
//		z = cc.z;
//	}
//
//	public NoiseCoord (ChunkCoord cc) {
//		this = new NoiseCoord (new Coord (cc));
//	}
//
//	public string toString() {
//		return "Noise Coord :) x: " + x + " z: " + z;
//	}
//}

//public class NoisePatch // before threaded job...
//{
//	public const int CHUNKDIMENSION = 4;
//	private int CHUNKLENGTH;
//
//	private Coord patchDimensions;
//	public Block[,,] blocks;
//	public NoiseCoord coord;
//
//	public bool generatedNoiseAlready;
//	public bool generatedBlockAlready;
//
//	private ChunkManager m_chunkManager;
//
//	public NoisePatch(NoiseCoord _noiseCo, int _chunkLen, ChunkManager _chunkMan)
//	{
//		coord = _noiseCo;
//		CHUNKLENGTH = _chunkLen;
//		int array_dim = _chunkLen * CHUNKDIMENSION; //grab borders of neighbor blocks from the noise as well (therefor extra two elements in array in each dimension)
//
//		blocks = new Block[array_dim, array_dim, array_dim];
//		patchDimensions = new Coord (array_dim);
//		m_chunkManager = _chunkMan;
//	}
//
//	public Block blockAtPatchCoord(Coord _worldCo) {
//		return null;
//	}
//
//	public bool coordIsInBlocksArray(Coord indexCo) {
//		if (!indexCo.isIndexSafe(patchDimensions))
//		{
//			bug ("coord out of array bounds for this noise patch: coord: " + indexCo.toString() + " array bounds: " + patchDimensions.toString ());
//			return false;
//		}
//
//		return indexCo.isIndexSafe (patchDimensions);
//	}
//
//	private Coord patchRelativeBlockCoordForChunkCoord(Coord chunkCo) {
//		chunkCo = chunkCo % CHUNKDIMENSION;
//		return ((chunkCo + CHUNKDIMENSION) % CHUNKDIMENSION) * CHUNKLENGTH; //massage negative coords
//	}
//
//	public Block blockAtChunkCoordOffset(Coord chunkCo, Coord offset) 
//	{
//		Coord index = patchRelativeBlockCoordForChunkCoord(chunkCo) + offset;
//
//		if (!index.isIndexSafe(patchDimensions))
//		{
//			return m_chunkManager.blockAtChunkCoordOffset (chunkCo, offset); // looking for a block outside of this patch (will happen sometimes we think)
//		}
//
//		return blocks [index.x, index.y, index.z];
//	}
//
//	void bug(string str) {
//		UnityEngine.Debug.Log (str);
//	}
//
//	public void generateNoisePatch()
//	{
//		m_chunkManager.noiseHandler.GenerateAtNoiseCoord (coord);
//		generatedNoiseAlready = true;
//	}
//
//	public void populateBlocksFromNoise()
//	{
//		Coord start = new Coord (0);
//		Coord range = patchDimensions;
//		populateBlocksFromNoise (start, range);
//	}
//
//	public void populateBlocksFromNoise(Coord start, Coord range)
//	{
//		//...
//		int x_start = (int)( start.x);
//		int z_start = (int)( start.z);
//		int y_start = (int)( start.y);
//
//		int x_end = (int)(x_start + range.x);
//		int y_end = (int)(y_start + range.y);
//		int z_end = (int)(z_start + range.z);
//
//		int xx = x_start;
//		for (; xx < x_end; ++xx) 
//		{
//			int zz = z_start;
//			for (; zz < z_end; ++zz) 
//			{
//				float noise_val = m_chunkManager.noiseHandler [xx, zz];
//
//				int yy = y_start; // (int) ccoord.y;
//				for (; yy < y_end; ++ yy) 
//				{
//
//					BlockType btype = BlockType.Air;
//
//					//test
//					//					if (yy < 37)
//					//						btype = BlockType.Grass;
//
//
//					// no caves for now...
//					int noiseAsWorldHeight =(int) ((noise_val * .5 + .5) * patchDimensions.y * .2);
//
//					if (yy < patchDimensions.y - 4 ) // 4 blocks of air on top
//					{ 
//
//						if (false) {
//							int total = xx + yy + zz;
//							if (total < (CHUNKLENGTH - 5) * 3 && total > 4)
//								btype = BlockType.Grass;
//						} else {
//
//							if (yy == 0) {
//								btype = BlockType.BedRock;
//							} else if (noiseAsWorldHeight > yy) {
//								if (yy < CHUNKLENGTH)
//									btype = BlockType.Grass;
//								else if (yy < CHUNKLENGTH * 2)
//									btype = BlockType.Sand;
//								else if (yy < CHUNKLENGTH * 3)
//									btype = BlockType.Dirt;
//								else
//									btype = BlockType.Stone;
//							}
//						}
//					}
//					//					UnityEngine.Debug.Log (" creating block at x " + xx + " y " + yy + " z " + zz);
//					blocks [xx , yy , zz] = new Block (btype);
//				}
//
//			}
//		}
//
//		generatedBlockAlready = true;
//	}
//
//
//}


// ******* OLD NOISE HANDLER WITH MONOBEH

//using System.Collections;
//using LibNoise.Unity;
//using LibNoise.Unity.Generator;
//using LibNoise.Unity.Operator;
//using System.IO;
//using System;
//using UnityEngine;
//
////using System.Collections.Generic;
//using System.Linq;
////using System.Text;
//using System.Diagnostics;
//
//public enum NoiseType {Perlin, Billow, RiggedMultifractal, Voronoi, Mix};
//
//public class NoiseHandler : MonoBehaviour 
//{
//	private Noise2D m_noiseMap = null;
//	private Texture2D[] m_textures = new Texture2D[3];
//	public int resolutionX = 128; 
//	public int resolutionZ = 64; 
//	public NoiseType noise = NoiseType.Perlin;
//	public float zoom = 1f; 
////	public float offset = 0f; 
//	public float offset = 1f; 
//
//	public void Start() {
////		Generate();
//	}
//
////	public void OnGUI() {
////		int y = 0;
////		foreach ( string i in System.Enum.GetNames(typeof(NoiseType)) ) {
////			if (GUI.Button(new Rect(0,y,100,20), i) ) {
////				noise = (NoiseType) Enum.Parse(typeof(NoiseType), i);
////				Generate();
////			}
////			y+=20;
////		}
////	}
//
//	public void Generate() {	
//		// Create the module network
//		ModuleBase moduleBase;
//		switch(noise) {
//			case NoiseType.Billow:	
//			moduleBase = new Billow();
//			break;
//
//			case NoiseType.RiggedMultifractal:	
//			moduleBase = new RiggedMultifractal();
//			break;   
//
//			case NoiseType.Voronoi:	
//			moduleBase = new Voronoi();
//			break;             	         	
//
//			case NoiseType.Mix:            	
//			Perlin perlin = new Perlin();
//			RiggedMultifractal rigged = new RiggedMultifractal();
//			moduleBase = new Add(perlin, rigged);
//			break;
//
//			default:
//			moduleBase = new Perlin();
//			break;
//
//		}
//
//		// Initialize the noise map
//		this.m_noiseMap = new Noise2D(resolutionX, resolutionZ, moduleBase);
////		this.m_noiseMap.GeneratePlanar(
////			offset + -1 * 1/zoom, 
////			offset + offset + 1 * 1/zoom, 
////			offset + -1 * 1/zoom,
////			offset + 1 * 1/zoom);
//		this.m_noiseMap.GeneratePlanar(
//			-1.0, 
//			1.0 * 0.0, 
//			-1.0 * 0.0,
//			1.0); //TEST **** L,R,Top,Bottom
//
//
//
//
//		//Generate the textures
//		this.m_textures[0] = this.m_noiseMap.GetTexture(LibNoise.Unity.Gradient.Grayscale);
//		this.m_textures[0].Apply();
////
////
////
////		//MMP experiment
//		this.m_noiseMap.GeneratePlanar( 
//			-2.0, // * 0.0, 
//		    -1.0,
//			-1.0 * 0.0,
//			1.0);  // WoRKS! phew! (makes adjacent tiles...)
//
//		// Generate the textures
//		Texture2D adjTex = this.m_noiseMap.GetTexture(LibNoise.Unity.Gradient.Grayscale);
//		adjTex.Apply();
//		File.WriteAllBytes(Application.dataPath + "/../GrayAdjLeftL.png", adjTex.EncodeToPNG() );
//		// END MMP experiment
//
//
//
//
//		this.m_textures[1] = this.m_noiseMap.GetTexture(LibNoise.Unity.Gradient.Terrain);
//		this.m_textures[1].Apply();
//
//		this.m_textures[2] = this.m_noiseMap.GetNormalMap(3.0f);
//		this.m_textures[2].Apply();
//
//		//display on plane
//		renderer.material.mainTexture = m_textures[0];
//
//
//		//write images to disk
//		File.WriteAllBytes(Application.dataPath + "/../Gray.png", m_textures[0].EncodeToPNG() );
//		File.WriteAllBytes(Application.dataPath + "/../Terrain.png", m_textures[1].EncodeToPNG() );
//		File.WriteAllBytes(Application.dataPath + "/../Normal.png", m_textures[2].EncodeToPNG() );
//
////		Debug.Log("Wrote Textures out to "+Application.dataPath + "/../");
//
//
//	}
//
//	public void initNoiseMap() { //MMP
//		// Create the module network
//		ModuleBase moduleBase;
//		switch(noise) {
//			case NoiseType.Billow:	
//			moduleBase = new Billow();
//			break;
//
//			case NoiseType.RiggedMultifractal:	
//			moduleBase = new RiggedMultifractal();
//			break;   
//
//			case NoiseType.Voronoi:	
//			moduleBase = new Voronoi();
//			break;             	         	
//
//			case NoiseType.Mix:            	
//			Perlin perlin = new Perlin();
//			RiggedMultifractal rigged = new RiggedMultifractal();
//			moduleBase = new Add(perlin, rigged);
//			break;
//
//			default:
//			moduleBase = new Perlin();
////			break;
////
////		}
////
////		// Initialize the noise map
////		this.m_noiseMap = new Noise2D(resolutionX, resolutionZ, moduleBase);
////	}
////
////	public void GenerateAtNoiseCoord(NoiseCoord _noiseCoord)  ///MMP
////	{	
////	
////		//		this.m_noiseMap.GeneratePlanar(
////		//			offset + -1 * 1/zoom, 
////		//			offset + offset + 1 * 1/zoom, 
////		//			offset + -1 * 1/zoom,
////		//			offset + 1 * 1/zoom);
////
////
////		float left = (float)_noiseCoord.x * 2.0f;
////		float right = left + 2.0f;
////		float top = (float) _noiseCoord.z * 2.0f;
////		float bottom = top + 2.0f;
////
//////		float left = (float)_noiseCoord.x * 2.0f;
//////		float right = left + 2.0f;
//////		float top = (float) _noiseCoord.z * 2.0f;
//////		float bottom = top + 2.0f;
////
////		UnityEngine.Debug.Log ("about to gen the noise in NoiseH");
////
////		this.m_noiseMap.GeneratePlanar (
////			left,
////			right, 
////			top,
////			bottom);
////
//////		this.m_noiseMap.GeneratePlanar(
//////			-1.0, 
//////			1.0 * 0.0, 
//////			-1.0 * 0.0,
//////			1.0); //TEST **** L,R,Top,Bottom
////
////
////
////
////		//Generate the textures
//////		this.m_textures[0] = this.m_noiseMap.GetTexture(LibNoise.Unity.Gradient.Grayscale);
//////		this.m_textures[0].Apply();
//////		string fname = "_gray_patch" + _noiseCoord.x + "z" + _noiseCoord.z + ".png";
////////
////////		this.m_textures[1] = this.m_noiseMap.GetTexture(LibNoise.Unity.Gradient.Terrain);
////////		this.m_textures[1].Apply();
////////
////////		this.m_textures[2] = this.m_noiseMap.GetNormalMap(3.0f);
////////		this.m_textures[2].Apply();
//////
//////		//display on plane
////////		renderer.material.mainTexture = m_textures[0];
////////
////////
////////		//write images to disk
//////		File.WriteAllBytes(Application.dataPath + "/../" + fname, m_textures[0].EncodeToPNG() );
////////		File.WriteAllBytes(Application.dataPath + "/../Terrain.png", m_textures[1].EncodeToPNG() );
////////		File.WriteAllBytes(Application.dataPath + "/../Normal.png", m_textures[2].EncodeToPNG() );
//////
////		//		Debug.Log("Wrote Textures out to "+Application.dataPath + "/../");
////
////
////	}
////
////	public float this[int x, int y]
////	{
////		get {
////			return this.m_noiseMap [x, y];
////		}
////		set {
////			this.m_noiseMap [x, y] = value;
////		}
////	}
////
////}
//
//// **** FUNKY VERSION OF 
//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//
//using System.IO;
//using System;
//using System.Security.Cryptography.X509Certificates;
//using System.Security.Cryptography;
//using System.Runtime.ConstrainedExecution;
//using System.Diagnostics;
//
//
///*
// *MAIN CLASS FOR ICBINMC
// *To anyone looking at this code
// *there's a lot of messiness right now 
// *some variables are essential and others
// *should be purged, for example.
// *
// *CHUNK MANAGER checks for chunks that should be loaded
// *loads new chunks
// *destroys far away chunks 
// *runs a dictionary of noisePatches that
// *get more noise for new chunks...
// *
// */
//using System.ComponentModel;
//
//
//public class ChunkManager : MonoBehaviour 
//{
//
//	//	public Chunk prefabChunk;
//	public Transform prefabMeshHolder;
//
//	private ChunkMap chunkMap;
//
//	public NoiseHandler noiseHandler;
//
//	public const uint CHUNKLENGTH = 16;
//	public uint WORLD_HEIGHT_CHUNKS = 4;
//	public uint WORLD_XLENGTH_CHUNKS = 8;
//	public uint WORLD_ZLENGTH_CHUNKS = 4;
//
//	private Coord m_mapDims_blocks;
//
//	//	public Block[,,] blocks;
//
//	public Transform playerCameraTransform; 
//	private AudioSource audioSourceCamera; // = playerCameraTransform.parent.GetComponent<AudioSource> ();
//
//	private List<Chunk> activeChunks;
//	private List<Chunk> createTheseChunks;
//	private List<Chunk> createTheseVeryCloseAndInFrontChunks;
//	private List<Chunk> createTheseFurtherAwayChunks;
//	private List<Chunk> destroyTheseChunks;
//
//	private CoRange m_wantActiveRealm;
//	private CoRange m_veryCloseAndInFrontRealm;
//	private CoRange m_dontDestroyRealm;
//
//	private	List<NoisePatch> setupThesePatches;
//
//	//	private List<Chunk> inTheOvenChunks;
//
//	BakeryOfChunks bakeryOfChunks;
//
//	private Coord lastPlayerBlockCoord;
//	private Coord lastPlayerChunkCoord;
//	private NoiseCoord lastPlayerNoiseCoord;
//
//
//	private Coord m_testBreakMakeBlockWorldPos;
//
//
//	public Coord spawnPlayerAtCoord = new Coord (6, 0, 6);
//
//	Job myJob;
//
//	private FrustumChecker frustumChecker;
//	private bool gotAFarAwayChunkTest;
//	private Ray farawayRayTest;
//	private Vector3 farAwayPos;
//	private bool wasNullAndNeededToRenderTest;
//	private bool wasInactiveAndNotNullTest;
//
//	//	public Dictionary<NoiseCoord, NoisePatch> noisePatches;
//	public BlockCollection blocks;
//
//	private bool firstNoisePatchDone = false;
//
//	public ChunkManager()
//	{
//		noiseHandler = new NoiseHandler ();
//
//		m_mapDims_blocks = new Coord (WORLD_XLENGTH_CHUNKS * CHUNKLENGTH, WORLD_HEIGHT_CHUNKS  * CHUNKLENGTH, WORLD_ZLENGTH_CHUNKS  * CHUNKLENGTH);
//		blocks = new BlockCollection ((int)(CHUNKLENGTH * NoisePatch.CHUNKDIMENSION));//  new Block[m_mapDims_blocks.x, m_mapDims_blocks.y, m_mapDims_blocks.z];
//
//		activeChunks = new List<Chunk> ();
//		createTheseVeryCloseAndInFrontChunks = new List<Chunk> ();
//		createTheseChunks = new List<Chunk> ();
//		destroyTheseChunks = new List<Chunk> ();
//
//		setupThesePatches = new List<NoisePatch> ();
//
//		spawnPlayerAtCoord = new Coord (6, 0, 6);
//		//		noisePatches = new Dictionary<NoiseCoord, NoisePatch> ();
//	}
//
//
//	void populateBlocksArray(ChunkCoord start, ChunkCoord range)
//	{
//		populateBlocksArray (start, range, false);
//	}
//
//	void populateBlocksArray(ChunkCoord start, ChunkCoord range, bool SillyTest) 
//	{
//
//		int x_start = (int)( start.x * CHUNKLENGTH);
//		int z_start = (int)( start.z * CHUNKLENGTH);
//		int y_start = (int)( start.y * CHUNKLENGTH);
//
//		int x_end = (int)(x_start + range.x * CHUNKLENGTH);
//		int y_end = (int)(y_start + range.y * CHUNKLENGTH);
//		int z_end = (int)(z_start + range.z * CHUNKLENGTH);
//
//		int xx = x_start;
//		for (; xx < x_end; ++xx) 
//		{
//			int zz = z_start;
//			for (; zz < z_end; ++zz) 
//			{
//				float noise_val = this.noiseHandler [xx, zz];
//
//				int yy = y_start; // (int) ccoord.y;
//				for (; yy < y_end; ++ yy) 
//				{
//
//					BlockType btype = BlockType.Air;
//
//					//test
//					//					if (yy < 37)
//					//						btype = BlockType.Grass;
//
//
//					// no caves for now...
//					int noiseAsWorldHeight =(int) ((noise_val * .5 + .5) * m_mapDims_blocks.y * 1.0);
//
//					if (yy < m_mapDims_blocks.y - 4 || SillyTest) // 4 blocks of air on top
//					{ 
//
//						if (SillyTest) {
//							int total = xx + yy + zz;
//							if (total < (CHUNKLENGTH - 5) * 3 && total > 4)
//								btype = BlockType.Grass;
//						} else {
//
//							if (yy == 0) {
//								btype = BlockType.BedRock;
//							} else if (noiseAsWorldHeight > yy) {
//								if (yy < CHUNKLENGTH)
//									btype = BlockType.Grass;
//								else if (yy < CHUNKLENGTH * 2)
//									btype = BlockType.Sand;
//								else if (yy < CHUNKLENGTH * 3)
//									btype = BlockType.Dirt;
//								else
//									btype = BlockType.Stone;
//							}
//						}
//					}
//					blocks [xx , yy , zz] = new Block (btype);
//				}
//
//			}
//		}
//	}
//
//	public Block blockAtChunkCoordOffset(Coord cc, Coord offset)
//	{
//		Coord index = new Coord ((int)(cc.x * CHUNKLENGTH + offset.x),(int) (cc.y * CHUNKLENGTH + offset.y),(int) (cc.z * CHUNKLENGTH + offset.z));
//
//		// TODO: if cc not contained in my dictionary of noise patches ...
//		// add the appro noise patch coord to a list of to be made noise patch coords and...
//		// return null
//
//		//		if (!index.isIndexSafe (m_mapDims_blocks))
//		//			return null;
//
//		if (index.y < 0 || index.y >= m_mapDims_blocks.y){
//			//			bug ("index. y out of range: " + index.toString ());
//			return null;
//		}
//
//
//		if (!blocks.noisePatches.ContainsKey (new NoiseCoord (cc / NoisePatch.CHUNKDIMENSION) ))
//		{
//			//			bug ("trying to get a block for which we don't have the noise patch coord");
//			return null;
//		}
//
//
//		//		// new approach
//		//		NoisePatch np = blocks.noisePatches [nco];
//		//		Coord remainder = cc % NoisePatch.CHUNKDIMENSION;
//		//
//		//		Coord noisePatchIndex = noisePatchArrayIndexFrom (cc, offset); //  remainder + offset;
//		//
//		//		bug ("noise patch array index: " + noisePatchIndex.toString () + " for chunk coord: " + cc.toString () + " offset: " + offset.toString () + " world index: " + index.toString());
//		//
//		//		return np.blocks [noisePatchIndex.x, noisePatchIndex.y, noisePatchIndex.z];
//		//		//end new approach
//
//		return blocks [index]; // world co way is more direct
//
//		//		return blocks [index.x, index.y, index.z];  
//	}
//
//	Coord noisePatchArrayIndexFrom(Coord chunkCoord, Coord offset ) {
//		Coord patchRelativeChunkCo = chunkCoord % NoisePatch.CHUNKDIMENSION;
//		patchRelativeChunkCo = (patchRelativeChunkCo + NoisePatch.CHUNKDIMENSION) % NoisePatch.CHUNKDIMENSION; // massage negative chunk dims (-1 becomes 3 e.g.)
//		return (patchRelativeChunkCo * CHUNKLENGTH) + offset;
//	}
//
//	// blocks uses an accessor method backed by a bunch of NoisePatchs Quadruply linked lists? 
//
//	/*
// * Use a dictionary of NoisePatches with Coord keys.
// * 
// */
//	void makeChunksFromOnMainThreadAtCoord(Coord coord)
//	{
//
//		if (!chunkMap.coIsOnMap (coord)) {
//			bug ("got co not on map at coord: " + coord.toString ());
//			//			throw new System.ArgumentException ("looking for a chunk that was not on the map..." + coord.toString());
//			return;
//		}
//
//		Chunk ch = chunkMap.chunkAt (coord);
//		makeChunkOnMainThread (ch);
//
//	}
//
//	void makeChunkOnMainThread(Chunk ch)
//	{
//		if (ch == null )
//		{
//			throw new System.ArgumentException("Ch should not be null at this point", "ch");
//		}
//		if (ch.meshHoldingGameObject == null ) 
//		{
//			bug ("needed to give ch its mesh at this point (make ch on mT)");
//			giveChunkItsMeshObject (ch, ch.chunkCoord);
//		}
//
//		ch.makeMesh ();
//
//		if (!ch.noNeedToRenderFlag) {
//			ch.applyMesh ();
//			activeChunks.Add (ch);
//			ch.isActive = true;
//		}
//		else // why not get rid of the game object now?
//		{
//			chunkMap.destroyChunkAt (ch.chunkCoord);
//		}
//	}
//
//	void makeChunksFromOnMainThread(ChunkCoord start, ChunkCoord range)
//	{
//		bug ("MKING CHS ON MAIN THREAD START");
//		// first give chunks their blocks
//		int i = start.x;
//		for (; i < start.x + range.x; ++i) {
//
//			int j = (int) start.y;
//			for (; j < start.y + range.y; ++j) {
//
//				int k = start.z;
//				for (; k < start.z + range.z; ++k) {
//
//					makeChunksFromOnMainThreadAtCoord (new Coord (i, j, k)); 
//				}
//			}
//		} 
//	}
//
//	//	Chunk makeNewOrGetExistingChunk(Coord _co)
//	//	{
//	//		Chunk ch = chunkMap.chunkAt (_co);
//	//
//	//		if (ch == null){ // if !isActive
//	//			ch = makeNewChunk (_co);
//	//		}
//	//
//	//		return ch;
//	//	}
//
//	Chunk makeNewChunkButNotItsMesh(Coord coord)
//	{
//		Chunk c = new Chunk ();// (Chunk) Instantiate (prefabChunk, pos, transform.rotation);
//
//		c.m_chunkManager = this;
//		c.CHUNKLENGTH = (int) ChunkManager.CHUNKLENGTH;
//
//		c.chunkCoord = coord;
//
//		// give chunk a noise patch
//		// maybe would want the noise patch to conform to an interface instead of exposing the whole noise patch to the chunk
//		// just a thought...
//		NoiseCoord ncoo = noiseCoordContainingChunkCoord (coord);
//
//		if (!blocks.noisePatches.ContainsKey(ncoo))
//		{
//			//			throw new System.ArgumentException ("yo we don't have a noise patch for this chunk?? " + ncoo.toString());
//			bug ("um... we don't have a noise patch for this chunk. " + ncoo.toString ());
//			return null;
//		}
//
//		NoisePatch np = blocks.noisePatches [ncoo];
//
//		if (np == null){
//			throw new System.ArgumentException ("yo. we don't have a noise patch for this chunk??");
//			return null;
//		}
//
//		c.m_noisePatch = np;
//
//		return c;
//	}
//
//	void giveChunkItsMeshObject(Chunk c, Coord coord)
//	{
//		Vector3 pos = new Vector3 (CHUNKLENGTH * coord.x, CHUNKLENGTH * coord.y, CHUNKLENGTH * coord.z);
//
//		//change: the mesh is NOT of type Chunk (just a game object)
//		Transform gObjTrans = (Transform) Instantiate(prefabMeshHolder, pos, transform.rotation);
//		GameObject gObj = gObjTrans.gameObject;
//
//		//c.transform.parent = transform;
//		gObj.transform.parent = transform;
//
//		c.meshHoldingGameObject = gObj; //the chunk gets a reference to the gameObject that it will work with (convenient)
//	}
//
//	//	{
//	//		Vector3 pos = new Vector3 (CHUNKLENGTH * coord.x, CHUNKLENGTH * coord.y, CHUNKLENGTH * coord.z);
//	//
//	//		Chunk c = new Chunk ();// (Chunk) Instantiate (prefabChunk, pos, transform.rotation);
//	//
//	//		//change: the mesh is NOT of type Chunk (just a game object)
//	//		Transform gObjTrans = (Transform) Instantiate(prefabMeshHolder, pos, transform.rotation);
//	//		GameObject gObj = gObjTrans.gameObject;
//	//
//	//		//c.transform.parent = transform;
//	//		gObj.transform.parent = transform;
//	//		c.m_chunkManager = this;
//	//		c.CHUNKLENGTH = (int) ChunkManager.CHUNKLENGTH;
//	//
//	//		c.meshHoldingGameObject = gObj; //the chunk gets a reference to the gameObject that it will work with (convenient)
//	//
//	//		c.chunkCoord = coord;
//	//
//	//		return c;
//	//	}
//
//	//	Chunk makeNewChunk(Coord coord) //OLD WAY
//	//	{
//	//		Vector3 pos = new Vector3 (CHUNKLENGTH * coord.x, CHUNKLENGTH * coord.y, CHUNKLENGTH * coord.z);
//	//
//	//		Chunk c = (Chunk) Instantiate (prefabChunk, pos, transform.rotation);
//	//
//	//		//change: the mesh is NOT of type Chunk (just a game object)
//	//		//		GameObject c = (GameObject) Instantiate(prefabChunk, pos, transform.rotation);
//	//
//	//
//	//		c.transform.parent = transform;
//	//		c.m_chunkManager = this;
//	//		c.CHUNKLENGTH = (int) ChunkManager.CHUNKLENGTH;
//	//		c.chunkCoord = coord;
//	//
//	//		return c;
//	//	}
//
//	public void destroyBlockAt(Coord blockCoord) 
//	{
//		Block destroyMe = blocks.specialGetBlockForTesting (blockCoord);
//		//		Block destroyMe = blocks [blockCoord.x , blockCoord.y, blockCoord.z];
//
//		if (destroyMe.type == BlockType.Air) {
//			bug ("block was air already! " + blockCoord.toString());
//			return;
//		}
//
//		destroyMe.type = BlockType.Air;
//
//		Chunk ch = chunkContainingCoord (blockCoord);
//
//		if (ch == null) {
//			bug ("null chunk, nothing to update");
//			return;
//		}
//
//		updateChunk (ch);
//
//		// also update any adjacent chunks (to the destroyed block)
//		foreach (Chunk adjCh in chunksTouchingBlockCoord(blockCoord) )
//			updateChunk(adjCh);
//	}
//
//	public System.Collections.IEnumerable chunksTouchingBlockCoord(Coord blockWorldCoord)
//	{
//		Coord chunkCoord = chunkCoordContainingBlockCoord (blockWorldCoord);
//		Coord chRelCo = chunkRelativeCoord (blockWorldCoord);
//
//		foreach (Coord dirCo in directionCoordsForRelativeEdgeCoords(chRelCo, blockWorldCoord)) {
//			Chunk adjacentChunk = chunkMap.chunkAt (chunkCoord + dirCo);
//
//			if (adjacentChunk != null)
//				yield return adjacentChunk;
//
//		}
//	}
//
//	public System.Collections.IEnumerable directionCoordsForRelativeEdgeCoords(Coord blkRelativeCoord, Coord worldCoordOfBlockInQuestion)
//	{
//
//		Coord negPosOne = worldCoordOfBlockInQuestion.negNegOnePosPosOne ();
//		if (blkRelativeCoord.x == 0)
//			yield return new Coord (-1, 0, 0) * negPosOne;
//		if (blkRelativeCoord.x == CHUNKLENGTH - 1)
//			yield return new Coord (1, 0, 0) * negPosOne;
//
//		if (blkRelativeCoord.y == 0)
//			yield return new Coord (0, -1, 0) * negPosOne;
//		if (blkRelativeCoord.y == CHUNKLENGTH - 1)
//			yield return new Coord (0, 1, 0) * negPosOne;
//
//		if (blkRelativeCoord.z == 0)
//			yield return new Coord (0, 0, -1) * negPosOne;
//		if (blkRelativeCoord.z  == CHUNKLENGTH - 1)
//			yield return new Coord (0, 0, 1) * negPosOne;
//
//	}
//
//	void bug(string str) {
//		UnityEngine.Debug.Log (str);
//	}
//
//	Coord chunkCoordContainingBlockCoord(Coord co)
//	{
//		//		if (!co.isIndexSafe (m_mapDims_blocks))
//		//			return new Coord (9999999999, 0, 999999999); // dictionary of chunks now. check elsewhere for safety
//		// if a coord member is negative, member - CHUNKLENGTH will give us the chunkCoord we want.
//		// E.G. -2, 3, 4 is inside chunkCoord -1, 0, 0. so (-2 - 16, 3, 4) / 16 = (-1,0,0)
//		// (-2, 3, 4) / 16 = 0,0,0 (!not what we want!)
//
//		Coord chcoAdjustNeg = co.booleanNegative () * -1 * CHUNKLENGTH;
//		return (co + chcoAdjustNeg)  / CHUNKLENGTH;
//	}
//
//	Coord chunkRelativeCoord(Coord worldBlockCoord) {
//		return worldBlockCoord % CHUNKLENGTH;
//	}
//
//
//
//	Chunk chunkContainingCoord(Coord co)
//	{
//		Coord chunkCo = chunkCoordContainingBlockCoord (co);
//		if (!chunkMap.coIsOnMap (chunkCo))
//			return null;
//		//		if (!co.isIndexSafe (m_mapDims_blocks))
//		//			return null;
//
//		return chunkMap.chunkAt(chunkCo);
//	}
//
//	//swiped from 'Noise Handler' (demo.cs)
//	public void OnGUI() {
//		int y = 0;
//		foreach ( string i in System.Enum.GetNames(typeof(NoiseType)) ) {
//			if (GUI.Button(new Rect(0,y,100,20), i) ) {
//				noiseHandler.noise = (NoiseType) Enum.Parse(typeof(NoiseType), i);
//
//				bug ("new noise: " + i);
//				//				buildMap ();
//				noiseHandler.Generate ();
//				//				rebuildMapFrom (new ChunkCoord (0, 2, 0), new ChunkCoord (2, 2, 1));
//
//				ChunkCoord start = ChunkCoord.chunkCoordZero (); // new ChunkCoord (2, 2, 2);
//				ChunkCoord range = new ChunkCoord (8, 4, 4);
//				populateBlocksArray (start, range, false );
//				makeChunksFromOnMainThread (start, range); 
//			}
//			y+=20;
//		}
//
//		Coord playerCoord = new Coord (playerCameraTransform.position);
//		Coord chChoord = chunkCoordContainingBlockCoord (playerCoord);
//		NoiseCoord npatchCo = noiseCoordContainingChunkCoord (chChoord);
//
//		GUI.Box (new Rect (Screen.width - 170, Screen.height - 40, 150, 40), "plyr co: \n" + playerCoord.toString() );
//		GUI.Box (new Rect (Screen.width - 170, Screen.height - 80, 150, 40), "plyr chunk co: \n" + chChoord.toString() );
//		GUI.Box (new Rect (Screen.width - 170, Screen.height - 120, 150, 40), "" + npatchCo.toString() );
//		GUI.Box (new Rect (Screen.width - 170, Screen.height - 160, 150, 40), "actv chs:" + activeChunks.Count + "chldn:" + transform.childCount );
//		GUI.Box (new Rect (Screen.width - 170, Screen.height - 200, 150, 40), "creThsChs Cnt:" + createTheseChunks.Count );
//		GUI.Box (new Rect (Screen.width - 170, Screen.height - 240, 150, 40), "desThsChs Cnt:" + destroyTheseChunks.Count );
//		GUI.Box (new Rect (Screen.width - 170, Screen.height - 280, 150, 40), "verCls Cnt:" + createTheseVeryCloseAndInFrontChunks.Count );
//
//		//		Coord corner = new Coord (Screen.width - 170, 1, Screen.height - 200);
//		//		Coord box = new Coord (10, 1, 10);
//		//		float margin = 2;
//		//		foreach(Chunk chch in activeChunks)
//		//		{
//		//			Coord boxPos = corner - (chch.chunkCoord * box);
//		//			GUI.Box (new Rect(boxPos.z, boxPos.x, box.x, box.z ), ".");
//		//		}
//
//
//		// DRAW A + IN THE MIDDLE
//		float boxSize = 20;
//		GUI.Box (new Rect (Screen.width * .5f - boxSize/2.0f, Screen.height * .5f - boxSize/2.0f, boxSize, boxSize), "+");
//	}
//
//	#region handle block hits
//
//	public void handleBreakBlockAt(RaycastHit hit)
//	{
//		//		Coord blockCoord = new Coord (hit.point);
//
//		// triangle technique...
//		// TODO: if is destroyable...? etc.
//
//
//
//		//		Vector3 avg = chunkRelativePointFromHit (hit);
//		//		Chunk ch = chunkContainingCoord (blockCoord);
//		//		Coord startOfChunkCoord = ch.chunkCoord * CHUNKLENGTH;
//		//		Coord altBlockCoord = startOfChunkCoord + new Coord (avg);
//		//		Coord altBlockCoord = startOfChunkCoord + new Coord (avg);
//
//		Vector3 blockWorldPos = worldPositionOfBlock (hit);
//
//		bug ("world pos of block that we want to break: " + blockWorldPos.ToString ());
//
//		Coord altBlockCoord =  hitOrPlaceBlockCoordFromWorldPos (blockWorldPos);// new Coord (blockWorldPos);
//
//		//		if (blockCoord.equalTo (altBlockCoord)) {
//		//			bug ("block coords from two methods are equal");
//		////			destroyBlockAt (blockCoord );
//		//		} else {
//		//			bug ("block coords not equal: blockCoord: " + blockCoord.toString () + " alt b coord: " + altBlockCoord.toString ());
//		////			destroyBlockAt (altBlockCoord);
//		//		}
//
//		// TODO: somewhere in here, (pos. using the tris, draw an animation or just some lines, around the block)
//
//		m_testBreakMakeBlockWorldPos = altBlockCoord;
//		destroyBlockAt (altBlockCoord);
//		//end of triangle approach
//
//		//		destroyBlockAt (blockCoord );
//	}
//
//	public void handlePlaceBlockAt(RaycastHit hit)
//	{
//		Vector3 blockWorldPos = worldPositionOfBlock (hit, true);
//		Coord placingCoord = hitOrPlaceBlockCoordFromWorldPos (blockWorldPos); //  new Coord (blockWorldPos);
//
//		m_testBreakMakeBlockWorldPos = placingCoord;
//
//		//want
//		//		Block b = blockAt (placingCoord);
//		Block b = blocks.specialGetBlockForTesting (placingCoord);
//
//		//		if (!Coord.Equals(b.coord, placingCoord))
//		//		{
//		//			bug ("cos not equal (only worry if we are withhin abs(16) )");
//		//		}
//
//		bug ("requested block coord: " + placingCoord.toString() );
//		//		bug ("received block coord: " + b.coord);
//
//		if (b == null) {
//			bug ("null block");
//			return;
//		}
//
//		if (b.type != BlockType.Air) {
//			bug ("a solid block is already located here: " + placingCoord);
//			return;
//		}
//
//		b.type = currentHeldBlockType ();
//
//		Chunk ch = chunkContainingCoord (placingCoord);
//		ch.noNeedToRenderFlag = false;
//		updateChunk (ch);
//	}
//
//	BlockType currentHeldBlockType () {
//		return BlockType.Grass;
//	}
//
//	//deal with negative V3 coordinates
//	// still brings the funk...
//	Coord hitOrPlaceBlockCoordFromWorldPos( Vector3 wopos)
//	{
//		Vector3 adjustNegMembers = new Vector3 (0f, 0f, 0f);
//		adjustNegMembers.x = wopos.x < 0 ? -1f : 0f;
//		adjustNegMembers.y = wopos.y < 0 ? -1f : 0f;
//		adjustNegMembers.z = wopos.z < 0 ? -1f : 0f;
//
//		return new Coord (wopos + adjustNegMembers);
//	}
//
//	Vector3 worldPositionOfBlock(RaycastHit hit)
//	{
//		return worldPositionOfBlock (hit, false);
//	}
//
//	Vector3 worldPositionOfBlock(RaycastHit hit, bool placingBlock)
//	{
//		// we should get a Vec3 
//		// that describes the relative position of the triangles to the camera. //relPos
//		// then we can figure out which face we want...by
//		// knowing that if they are facing in a (say) negative direction,
//		// we should 'cheat' the vec3 value slightly negative
//		// if we ever start having wedge shaped blocks,
//		// this won't be an issue, because the vec3 value will be more solidly in the middle?
//
//		// cheat avg:
//
//		MeshCollider meshCollider = hit.collider as MeshCollider;
//		if (meshCollider == null || meshCollider.sharedMesh == null)
//			return new Vector3(9999999999999.0f, 0,0);
//
//		Mesh mesh = meshCollider.sharedMesh;
//		Vector3[] vertices = mesh.vertices;
//		int[] triangles = mesh.triangles;
//		Vector3 p0 = vertices[triangles[hit.triangleIndex * 3 + 0]];
//		Vector3 p1 = vertices[triangles[hit.triangleIndex * 3 + 1]];
//		Vector3 p2 = vertices[triangles[hit.triangleIndex * 3 + 2]];
//
//		//		Vector3 avg = (p0 + p1 + p2) * 1 / 3.0f;
//
//		//		return avg;
//
//
//		//		Transform hitTransform = hit.collider.transform;
//
//		Vector3 worldAvg = hit.point; // TEST**** hitTransform.TransformPoint(avg);
//		Vector3 relPos =  worldAvg - playerCameraTransform.position; // Vector3.Distance (worldAvg, playerCameraTransform.position); //
//
//		// MORE EFFICIENT WOULD BE(?): TO JUST SLIGHTLY EXTEND THE 'SKEWER' REPRESENTED BY REL POS (and use the tip of that skewer)?
//		// AFTER ONE TEST SESSION: THIS WORKED ALMOST AS WELL EXCEPT IN ONE CASE: THE BLOCK THAT WE WERE CLOSEST TO
//		// RIGHT AFTER WE HIT THE GROUND (ON A CORNER BETWEEN SOME CHUNKS)
//		// so for now keep all of this stuff, until we can figure out what happened there (or that this was truly an aberration)
//		//		Vector3 skewerTip =  relPos.normalized * 0.05f;
//		//		return worldAvg + skewerTip;
//
//		// for now assume the world is always all cubes
//		float x_same = (p0.x == p1.x && p1.x == p2.x) ? 1 : 0;
//		float y_same = (p0.y == p1.y && p1.y == p2.y) ? 1 : 0;
//		float z_same = (p0.z == p1.z && p1.z == p2.z) ? 1 : 0;
//
//		Vector3 cheaterV = new Vector3 (x_same, y_same, z_same);
//
//		relPos = vmult (relPos, cheaterV);
//		relPos = relPos.normalized * 0.1f;
//
//		// put the point 'just inside' (by .1) of the block we want...
//		// or if placing a block, put just outside
//		return worldAvg + (relPos * (placingBlock ? -1.0f : 1.0f) ); 
//	}
//
//	Vector3 vmult (Vector3 aa, Vector3 bb) {
//		return Vector3.Scale (aa, bb); // (aa.x * bb.x, aa.y * bb.y, aa.z * bb.z);
//	}
//
//	Vector3 vdiv (Vector3 aa, Vector3 bb) {
//		return new Vector3 (bb.x == 0 ? 0 : aa.x / bb.x, bb.y == 0 ? 0 :  aa.y / bb.y, bb.z == 0 ? 0 :  aa.z / bb.z);
//	}
//
//	#endregion
//
//	Block blockAt(Coord cc) {
//		return blocks [cc.x, cc.y, cc.z];
//	}
//
//	void buildMap()
//	{
//		CoRange some_range = new CoRange (Coord.coordZero (), new Coord (3, 4, 4));
//		buildMapAtRange (some_range);
//	}
//
//	//	void buildMapAtRange(CoRange mapChunkRange) //OLD WAY
//	//	{
//	//		noiseHandler.Generate ();
//	//
//	//		populateBlocksArray (ChunkCoord.chunkCoordZero(), new ChunkCoord(8,4,4));
//	//
//	//
//	//		mapChunkRange = chunkMap.makeCoRangeSafe (mapChunkRange);
//	//
//	//		ChunkCoord start = new ChunkCoord (mapChunkRange.start); //  ChunkCoord.chunkCoordZero(); // ChunkCoord.chunkCoordOne (); // ChunkCoord.chunkCoordZero (); // new ChunkCoord (2, 2, 2);
//	//		ChunkCoord range = new ChunkCoord (mapChunkRange.range);// new ChunkCoord (3, 4, 4); //new ChunkCoord (8, 4, 4); 
//	//
//	//		//must update map before 'manually' adding chunks
//	//		updateCreateTheseChunksList ( mapChunkRange);
//	//		rebuildMapFrom (start, range);
//	//	}
//
//	void buildMapAtRange(CoRange mapChunkRange)
//	{
//		//		noiseHandler.Generate ();
//
//		//		populateBlocksArray (ChunkCoord.chunkCoordZero(), new ChunkCoord(8,4,4));
//
//
//		//		mapChunkRange = chunkMap.makeCoRangeSafe (mapChunkRange);
//
//		ChunkCoord start = new ChunkCoord (mapChunkRange.start); //  ChunkCoord.chunkCoordZero(); // ChunkCoord.chunkCoordOne (); // ChunkCoord.chunkCoordZero (); // new ChunkCoord (2, 2, 2);
//		ChunkCoord range = new ChunkCoord (mapChunkRange.range); // new ChunkCoord (3, 4, 4); //new ChunkCoord (8, 4, 4); 
//
//		//must update map before 'manually' adding chunks
//		updateCreateTheseChunksList ( mapChunkRange);
//		makeChunksFromOnMainThread (start, range);
//	}
//
//
//	void updateChunk(Chunk ch)
//	{
//		//		if (ch.meshHoldingGameObject == null)
//		//		{
//		//			giveChunkItsMeshObject (ch, ch.chunkCoord);
//		//		}
//		//		ch.makeMesh ();
//		//		ch.applyMesh ();
//		bug ("about to update ch at coord: " + ch.chunkCoord.toString ());
//		makeChunkOnMainThread (ch);
//	}
//
//	//	void makeChunkAt(ChunkCoord start)
//	//	{
//	//		makeChunksFrom (start, ChunkCoord.chunkCoordOne ());
//	//	}
//	//
//	//	void makeChunksFrom(Coord start, Coord range)
//	//	{
//	//		makeChunksFrom (new ChunkCoord (start), new ChunkCoord (range));
//	//	}
//	//
//	//	void makeChunksFrom(ChunkCoord start, ChunkCoord range)
//	//	{
//	//		makeChunksFrom (start, range, false);
//	//	}
//	//
//	//	void makeChunksFrom(ChunkCoord start, ChunkCoord range, bool alwaysCreateNewChunks)
//	//	{
//	//		//***want
//	//		if (bakeryOfChunks == null )
//	//		{
//	//			bug ("what bakery null??");
//	//			return;
//	//		}
//	//
//	//		if (bakeryOfChunks.Update () == false) // bakery still baking?
//	//		{
//	////			bug ("don't make chunks, still baking...");
//	//			return;
//	//		}
//	//
//	//		// *** there may be chunks ready from the last 'chunk bake' 
//	//		// *** so add them to the world
//	//
//	//		addJustBakedChunks ();
//	//
//	//
//	//		// *** now collect any more chunks we may need
//	//		List<Chunk> freshlyMadeChunks;
//	//		freshlyMadeChunks = shouldBeBakedChunks (start, range, alwaysCreateNewChunks);
//	//
//	//		if (freshlyMadeChunks.Count > 0) {
//	//			bakeryOfChunks.chunks = freshlyMadeChunks; // NOTE: do we want to protect these chunks from being messed with?
//	//			bakeryOfChunks.Start ();
//	//		}
//	////
//	////		if (false)
//	////		foreach(Chunk c in freshlyMadeChunks) {
//	////			if (c != null) {
//	////				c.makeMesh ();
//	////				c.applyMesh ();
//	////				activeChunks.Add (c);
//	////	//				madeMeshCountTest++;
//	////			}
//	//
//	//	}
//	//
//	void addJustBakedChunks()
//	{
//		//***want
//		List<Chunk> finishedChs = new List<Chunk> ();
//		finishedChs.AddRange (bakeryOfChunks.finishedChunks); // paranoid about the thread safety thing (TODO: learn more about what is safe)
//
//		//DEBUG test
//		if (bakeryOfChunks.finishedChunks.Count > 0)
//		{
//			//			AudioSource auS = playerCameraTransform.parent.GetComponent<AudioSource> ();
//			if (audioSourceCamera != null)
//				audioSourceCamera.Play ();
//			else 
//				bug("no camera Audio source??");
//			bug ("adding this many chunks on main thread: " + finishedChs.Count);
//		} 
//		else {
//			bug ("no chunks were  just baked");
//		}//end test
//
//		int bakeChCountTest = 0;
//		foreach(Chunk fchunk in finishedChs)
//		{
//			bakeChCountTest++;
//			if (fchunk.noNeedToRenderFlag) {
//				fchunk.meshHoldingGameObject.SetActive (false);
//				//				fchunk.transform.gameObject.SetActive (false); // = false;
//				fchunk.clearMesh ();
//			} else {
//				fchunk.applyMesh (); //we couldn't do this in the sep thread
//				fchunk.meshHoldingGameObject.SetActive (true);
//				//fchunk.transform.gameObject.SetActive (true);
//				activeChunks.Add (fchunk);
//			}
//
//
//		}
//		//		bug ("baked this many chunks on alt thread: " + bakeChCountTest);
//	}
//
//	//	List<Chunk > shouldBeBakedChunks(ChunkCoord start, ChunkCoord range, bool alwaysCreateNewChunks)
//	//	{
//	//		// *** NOW collect any more chunks we may need
//	//		List<Chunk> freshlyMadeChunks = new List<Chunk> ();
//	//
//	//		// first give chunks their blocks
//	//		int createdCountTest = 0;
//	//		int i = start.x;
//	//
//	//		for (; i < start.x + range.x; ++i) {
//	//
//	//			int j = (int) start.y;
//	//			for (; j < start.y + range.y; ++j) {
//	//
//	//				int k = start.z;
//	//				for (; k < start.z + range.z; ++k) {
//	//					Coord coord = new Coord (i, j, k);
//	//
//	//					if (alwaysCreateNewChunks || chunkMap.chunkAt(coord) == null)
//	//					{
//	//						Chunk ch = makeNewChunk (coord);
//	//						// add chunk to ch map
//	//						chunkMap.addChunkAt (ch, coord);
//	//						freshlyMadeChunks.Add (ch);
//	//
//	//						createdCountTest++;
//	//					}
//	//				}
//	//			}
//	//		}
//	//		bug ("created this number of chunks: " + createdCountTest);
//	//
//	//		return freshlyMadeChunks;
//	//	}
//
//	#region get player realms and update lists
//
//	Coord playerLocatedAtChunkCoord()
//	{
//		Coord pCoo =  new Coord (playerCameraTransform.position);
//		return chunkCoordContainingBlockCoord (pCoo);
//	}
//
//	CoRange nearbyChunkRange(Coord playerChunkCoord)
//	{
//		//		Coord halfR = new Coord (2, 2, 1);
//		//		Coord halfR = new Coord (1, 1, 1);
//		Coord halfR = new Coord (1, 1, 1);
//		return playerChunkRange (playerChunkCoord, halfR);
//	}
//
//	CoRange nearbyChunkRangeInitial(Coord playerChunkCoord)
//	{
//		//		Coord halfR = new Coord (2, 2, 1);
//		Coord halfR = new Coord (1, 1, 1);
//		//		Coord halfR = new Coord (3, 1, 3);
//		return playerChunkRange (playerChunkCoord, halfR);
//	}
//
//	CoRange getDontDestroyRealm() 
//	{
//		Coord halfR = new Coord (4, 4, 4); // larger than wActiveRealm
//		return playerChunkRange (playerLocatedAtChunkCoord (), halfR);
//	}
//
//	CoRange playerChunkRange(Coord playerChunkCoord, Coord halfRange)
//	{
//		//		int halfLength = 1;
//		//		Coord halfRealmLength = new Coord (halfLength);  
//
//		return new CoRange (playerChunkCoord - halfRange, (halfRange * 2) + 1);
//	}
//
//	CoRange getVeryCloseAndInFrontRange()
//	{
//
//		// 0 z pos, 90 x pos
//		Coord plCoord = playerLocatedAtChunkCoord ();
//		Coord halfR = new Coord (1,0,1);
//		CoRange retRange = playerChunkRange (plCoord, halfR);
//
//
//		//		int eulerZone =(int)((playerCameraTransform.eulerAngles.y + 45.0f) / 90) ;
//		Coord addToStart = new Coord (0);
//		Coord addToRange = new Coord (0);
//
//
//		switch((int)((playerCameraTransform.eulerAngles.y + 45.0f) / 90))
//		{
//			case (0): // z pos
//			addToRange = new Coord (0, 0, 2);
//			break;
//			case (1): // xpos
//			addToRange = new Coord (2, 0, 0);
//			break;
//			case (2): // z neg
//			addToStart = new Coord (0, 0, -2);
//			addToRange = new Coord (0, 0, 2);
//			break;
//			default:
//			addToStart = new Coord (-2, 0, 0);
//			addToRange = new Coord (2, 0, 0);
//			break;
//		}
//		retRange.start += addToStart;
//		retRange.range += addToRange;
//
//		return retRange;
//	}
//
//	IEnumerator updateChunkLists()
//	{
//		//		if (bakeryOfChunks.Update() == false)
//		//		{
//		//			return;
//		//		}
//		while (true) 
//		{
//			//simplistic
//			if (!Coord.Equals (lastPlayerBlockCoord, new Coord (playerCameraTransform.position))) 
//			{
//				lastPlayerBlockCoord = new Coord (playerCameraTransform.position);
//				// very close and in front range
//				m_veryCloseAndInFrontRealm = getVeryCloseAndInFrontRange ();
//				CoRange wantActiveRealm = m_veryCloseAndInFrontRealm;
//
//				//		if (last)
//				Coord playerChunkCoo = playerLocatedAtChunkCoord ();
//				//
//				if (!playerChunkCoo.equalTo (lastPlayerChunkCoord)) {
//
//					lastPlayerChunkCoord = playerChunkCoo;
//
//					wantActiveRealm = nearbyChunkRange (playerChunkCoo);
//					m_wantActiveRealm = wantActiveRealm; // for testing
//
//
//					m_dontDestroyRealm = getDontDestroyRealm ();
//
//					foreach (Chunk chunk in activeChunks) {
//						if (chunk == null)
//							continue;
//
//						//			if (!chunk.chunkCoord.isInsideOfRange(wantActiveRealm))
//						if (!chunk.chunkCoord.isInsideOfRange (m_dontDestroyRealm)) {
//							if (!destroyTheseChunks.Contains (chunk)) {
//								destroyTheseChunks.Add (chunk);
//							}
//
//						}
//					}
//				}
//
//				updateCreateTheseChunksList (wantActiveRealm);
//			}
//			yield return new WaitForSeconds (.3f);
//		}
//
//	}
//
//	//	public IEnumerator destroyFarAwayGameObjects() {
//	//		bug ("destroy chunks");
//	////		ChunkMesh[] chunkMeshes = gameObject.GetComponentsInChildren<ChunkMesh> () as ChunkMesh[];
//	////		for (int i = 0; i < chunkMeshes.Length; ++i)
//	////		foreach (Transform chunkMeshT in gameObject.GetComponentsInChildren<Transform>() as Transform[]) // gets children as all objects have a transform
//	//		foreach (Transform chunkMeshT in gameObject.transform)
//	//		{
//	//			ChunkMesh chunkMesh = chunkMeshT.GetComponent<ChunkMesh>();
//	//
//	//			if (chunkMesh == null || chunkMesh == prefabMeshHolder) {
//	//				bug ("null or the prefab chunk");
//	//				continue;
//	//			}
//	//
//	////			chunkMesh.really (); //test
//	//
//	//			Coord _meshChunkCoord = chunkCoordContainingBlockCoord( new Coord (chunkMesh.transform.position) );
//	//
//	//
//	//			if (!_meshChunkCoord.isInsideOfRange(m_wantActiveRealm)) // will want to change logic later -- "outside of larger active realm"
//	//			{
//	////				if (!destroyTheseChunks.Contains (chunk)) 
//	////				{
//	////					destroyTheseChunks.Add (chunk);
//	////				}
//	//				// for now, just destroy??
//	//				bug ("destroying chunk");
//	//				chunkMap.destroyChunkAt (_meshChunkCoord);
//	////				Destroy (chunkMesh);
//	//				
//	//
//	//			}
//	//			yield return new WaitForSeconds (.01f);
//	//		}
//	//	}
//
//	public void updateCreateTheseChunksList(CoRange _activeCoRange) 
//	{
//		//		if (!chunkMap.coIsOnMap (_activeCoRange.start))
//		//			return; // troubles when we get rid of this???
//
//		Coord start = _activeCoRange.start;
//		Coord range = _activeCoRange.range;
//		int i = start.x;
//		for (; i < start.x + range.x; ++i) 
//		{
//			int j = (int) start.y;
//			for (; j < start.y + range.y; ++j) 
//			{
//				int k = start.z;
//				for (; k < start.z + range.z; ++k) 
//				{
//					Coord chco = new Coord (i, j, k);
//
//					// GOT A NULL REF EXCEPTION HERE AT ONE POINT WHILE SPEEDING AROUND WORLD...(doesn't always happen...)
//					Chunk chh = chunkMap.chunkAtOrNullIfUnready (chco); //change unready to a bool return func. (TODO) //b/c what it was non null in fact...
//					if (chh == null || !chh.isActive) { // or not isActive
//						// chunks don't get destroyed when their meshes do (when, for example, the player moves away from them)
//						// therefore we might have an existing chunk that (hopefully) is just !isActive.
//						// or we may have never have encountered (made) this chunk at all...
//						// in any case, don't make its accompanying mesh just yet (it might be all air, or all solid and surrounded for example)
//
//						if (chh == null) {
//
//							chh = makeNewChunkButNotItsMesh (chco);
//
//							if (chh == null) // careful!
//								continue;
//
//							chh.isActive = false;
//							chunkMap.addChunkAt (chh, chco);
//						} // TODO: separate making chunks and making the chunk meshes. want chunks as soon
//						// as we see that they might need to be created/exist.
//						// want the meshes only after we've been through their blocks
//						// maybe they don't need a mesh if all air, or totally surrounded, etc.
//
//						destroyTheseChunks.Remove (chh);
//
//						if (chco.isInsideOfRange(m_veryCloseAndInFrontRealm)) {
//							createTheseVeryCloseAndInFrontChunks.Add (chh);
//						} else {
//							createTheseChunks.Add (chh);
//						}
//					}
//					else {
//						//						bug ("chunk was not null and/or was ready??? at : " + chco.toString());
//					}
//				}
//			}
//		}
//	}
//
//	#endregion
//
//	#region create and destroy chunk enumerators
//
//	IEnumerator createFurtherAwayChunks()
//	{
//		yield return new WaitForSeconds (1.5f);
//
//		while(true)
//		{
//			if (createTheseChunks.Count == 0) 
//			{
//				float depth = 1.0f;
//				Ray rayMiss;
//				Coord chunkCo;
//				Chunk needToActivateCh;
//				bool foundOneCondition = false;
//
//				bool onMapAndNull = false; // = (needToActivateCh == null) && (chunkMap.coIsOnMap (chunkCo));
//				bool notNullAndNeedToRender = false;
//
//				int attempts = 0;
//
//				rayMiss = frustumChecker.nextRaycastMiss ();
//
//				if (rayMiss.origin != Vector3.zero && rayMiss.direction != Vector3.up) {
//					//want ***
//					do {
//						depth += 1.0f; // (float)frustumChecker.screenIterationCount ();
//
//						//						bug ("depth is: " + depth);
//
//						Vector3 nextChunkPos = rayMiss.GetPoint (CHUNKLENGTH * depth);
//
//						farawayRayTest = rayMiss;
//						farAwayPos = nextChunkPos;
//
//						chunkCo = chunkCoordContainingBlockCoord (new Coord (nextChunkPos));
//						needToActivateCh = chunkMap.chunkAt (chunkCo);
//
//						onMapAndNull = (needToActivateCh == null) && (chunkMap.coIsOnMap (chunkCo));
//						notNullAndNeedToRender = (needToActivateCh != null) && (!needToActivateCh.noNeedToRenderFlag) && !needToActivateCh.isActive;
//						foundOneCondition = onMapAndNull || notNullAndNeedToRender;
//
//						wasNullAndNeededToRenderTest = onMapAndNull;
//						wasInactiveAndNotNullTest = notNullAndNeedToRender;
//
//					} while (!foundOneCondition && attempts++ < 6);
//
//					if (foundOneCondition) {
//
//						//					bug ("found a far away chunk with " + attempts + " attempts.");
//						gotAFarAwayChunkTest = true;
//						// ** want
//						if (onMapAndNull) {
//							needToActivateCh = makeNewChunkButNotItsMesh (chunkCo);
//							needToActivateCh.isActive = false;
//							chunkMap.addChunkAt (needToActivateCh, chunkCo); // danger: this code is a copy from updateCreateList (TODO: unify)...
//						} 
//
//						//				want ***
//						makeChunksFromOnMainThreadAtCoord (chunkCo);
//						//						makeChunksFromOnMainThread (new ChunkCoord (chunkCo), ChunkCoord.chunkCoordOne ());
//					} else {
//						gotAFarAwayChunkTest = false;
//						//					bug ("didn't find any far away chunks with " + attempts + " attempts.");
//					}
//				}
//
//			}
//			yield return new WaitForSeconds (1.5f);
//
//		}
//	}
//
//	void drawDebugRay()
//	{
//		Color col = Color.yellow;
//		if (wasNullAndNeededToRenderTest)
//			col = Color.cyan;
//		else if (wasInactiveAndNotNullTest)
//			col = Color.green;
//
//		//		if (gotAFarAwayChunkTest)
//		UnityEngine.Debug.DrawLine(farawayRayTest.origin, farAwayPos, col);
//	}
//
//
//	IEnumerator createAndDestroyChunksFromLists()
//	{
//		while (true)
//		{
//			if (shouldBeCreatingChunksNow ()) {
//				if (createTheseVeryCloseAndInFrontChunks.Count > 0)
//				{
//
//					Chunk chunk = createTheseVeryCloseAndInFrontChunks [0];
//
//					// TRY THE FOLLOWING: find a chunk that in the current active realm
//					// throw out chunks that aren't in
//					bool foundOne = true;
//					while (!chunk.chunkCoord.isInsideOfRange(m_veryCloseAndInFrontRealm))
//					{
//						createTheseVeryCloseAndInFrontChunks.RemoveAt (0);
//						if (createTheseVeryCloseAndInFrontChunks.Count == 0) {
//							foundOne = false;
//							break;
//						}
//						chunk = createTheseVeryCloseAndInFrontChunks [0];
//
//					}
//
//					if (foundOne)
//					{
//						if (chunk != null) {
//							makeChunksFromOnMainThreadAtCoord (chunk.chunkCoord);
//						}
//						createTheseVeryCloseAndInFrontChunks.RemoveAt (0);
//					}
//				}
//				else if (createTheseChunks.Count > 0) 
//				{
//					Chunk chunk = createTheseChunks [0];
//
//					bool foundOne = true;
//					while(!chunk.chunkCoord.isInsideOfRange(m_wantActiveRealm))
//					{
//						createTheseChunks.RemoveAt (0);
//						if (createTheseChunks.Count == 0) {
//							foundOne = false;
//							break;
//						}
//						chunk = createTheseChunks [0];
//					}
//					if (foundOne) {
//						if (chunk != null) {
//							makeChunksFromOnMainThreadAtCoord (chunk.chunkCoord); 
//						}
//						createTheseChunks.RemoveAt (0);
//					}
//				}
//				else // destroy some chunks 
//				{
//					if (shouldBeDestroyingChunksNow ()) {
//						if (destroyTheseChunks.Count > 0) {
//							Chunk chunk = destroyTheseChunks [0];
//
//							if (chunk != null) {
//								//						if (!chunk.chunkCoord.isInsideOfRange (m_wantActiveRealm)) { // re-check is nec. //  !createTheseChunks.Contains (chunk)) {
//								if (!chunk.chunkCoord.isInsideOfRange (m_dontDestroyRealm)) { // re-check is nec. //  !createTheseChunks.Contains (chunk)) {
//									chunkMap.destroyChunkAt (chunk.chunkCoord);
//									activeChunks.Remove (chunk);
//								} 
//							}
//							destroyTheseChunks.RemoveAt (0);
//						}
//					}
//				}
//
//			}
//			//			bug ("child count: " + transform.childCount + " active Chunks count: " + activeChunks.Count);
//			//			yield return new WaitForSeconds (.05f);
//
//			yield return new WaitForSeconds(.05f);
//		}
//	}
//
//	IEnumerator createChunksFromCreateList()
//	{
//		while (true)
//		{
//			if(shouldBeCreatingChunksNow())
//			{
//				if (createTheseChunks.Count > 0)
//				{
//					Chunk chunk = createTheseChunks [0];
//
//					if (chunk != null) {
//						makeChunksFromOnMainThread (new ChunkCoord (chunk.chunkCoord), ChunkCoord.chunkCoordOne ());
//					}
//					createTheseChunks.RemoveAt (0);
//				}
//			}
//			//			bug ("child count: " + transform.childCount + " active Chunks count: " + activeChunks.Count);
//			yield return new WaitForSeconds (.1f);
//		}
//	}
//
//	IEnumerator destroyChunksFromDestroyList()
//	{
//		yield return new WaitForSeconds (5.0f);
//
//		while (true)
//		{
//			if (shouldBeDestroyingChunksNow())
//			{
//				if (destroyTheseChunks.Count > 0) 
//				{
//					Chunk chunk = destroyTheseChunks[0];
//
//					if (chunk != null)
//					{
//						//						if (!chunk.chunkCoord.isInsideOfRange (m_wantActiveRealm)) { // re-check is nec. //  !createTheseChunks.Contains (chunk)) {
//						if (!chunk.chunkCoord.isInsideOfRange (m_dontDestroyRealm)) { // re-check is nec. //  !createTheseChunks.Contains (chunk)) {
//							chunkMap.destroyChunkAt (chunk.chunkCoord);
//							activeChunks.Remove (chunk);
//						} 
//					}
//					destroyTheseChunks.RemoveAt (0);
//				}
//			}
//			yield return new WaitForSeconds(.1f);
//		}
//	}
//
//	bool shouldBeCreatingChunksNow() {
//		return true;
//	}
//
//	bool shouldBeDestroyingChunksNow() {
//		return true;
//	}
//
//	void drawDebugLinesForBlockAtWorldCoord(Coord woco)
//	{
//		CoRange blockCo = new CoRange (woco, Coord.coordOne ());
//		drawDebugCube (blockCo, true);
//	}
//
//	void drawDebugLinesForChunkRange(CoRange chunkCoRange)
//	{
//		drawDebugCube (chunkCoRange, false);
//	}
//
//	void drawDebugCube(CoRange chunkCoRange, bool drawBlock)
//	{
//		int length = drawBlock ? 1 : (int) CHUNKLENGTH;
//		Coord start = chunkCoRange.start * length;
//		Coord outer = chunkCoRange.outerLimit () * length;
//
//		if (drawBlock)
//		{
//			Vector3 startV = start.toVector3 () - new Vector3 (.5f, .5f, .5f);
//			Vector3 outerV = outer.toVector3 () - new Vector3 (.5f, .5f, .5f);
//
//			//upper box
//			debugLineV (startV, new Vector3 (startV.x, startV.y, outerV.z));
//			debugLineV (startV, new Vector3 (outerV.x , startV.y, startV.z));
//			debugLineV (new Vector3 (outerV.x, startV.y, outerV.z), new Vector3 (startV.x, startV.y, outerV.z));
//			debugLineV (new Vector3 (outerV.x, startV.y, outerV.z), new Vector3 (startV.x , startV.y, startV.z));
//
//			debugLineV (outerV, new Vector3 (startV.x, outerV.y, outerV.z));
//			debugLineV (outerV, new Vector3 (outerV.x , outerV.y, startV.z));
//			debugLineV (new Vector3 (startV.x, outerV.y, startV.z), new Vector3 (startV.x, outerV.y, outerV.z));
//			debugLineV (new Vector3 (startV.x, outerV.y, startV.z), new Vector3 (outerV.x , outerV.y, startV.z));
//
//			return;
//		}
//
//		//upper box
//		debugLine (start, new Coord (start.x, start.y, outer.z));
//		debugLine (start, new Coord (outer.x , start.y, start.z));
//		debugLine (new Coord (outer.x, start.y, outer.z), new Coord (start.x, start.y, outer.z));
//		debugLine (new Coord (outer.x, start.y, outer.z), new Coord (outer.x , start.y, start.z));
//
//		debugLine (outer, new Coord (start.x, outer.y, outer.z));
//		debugLine (outer, new Coord (outer.x , outer.y, start.z));
//		debugLine (new Coord (start.x, outer.y, start.z), new Coord (start.x, outer.y, outer.z));
//		debugLine (new Coord (start.x, outer.y, start.z), new Coord (outer.x , outer.y, start.z));
//	}
//
//	void debugLine(Coord aa, Coord bb)
//	{
//		UnityEngine.Debug.DrawLine (aa.toVector3(), bb.toVector3());
//	}
//
//	void debugLineV(Vector3 aa, Vector3 bb)
//	{
//		UnityEngine.Debug.DrawLine (aa, bb);
//	}
//
//	#endregion
//
//	//	Coord worldDimsInChunkCoords() {
//	//		return m_mapDims_blocks / CHUNKLENGTH;
//	//	}
//
//	int worldHeightInChunkUnits() {
//		return  (int)(m_mapDims_blocks.y / CHUNKLENGTH);
//	}
//
//	void setNoiseHandlerResolution() {
//		int resXZ = (int)(NoisePatch.CHUNKDIMENSION * CHUNKLENGTH);
//		noiseHandler.resolutionX = resXZ;
//		noiseHandler.resolutionZ = resXZ;
//	}
//
//	//test use
//	CoRange corangeFromNoiseCoord(NoiseCoord nco)
//	{
//		Coord start = new Coord(nco.x, 0, nco.z) * (float) NoisePatch.CHUNKDIMENSION;
//		Coord range = new Coord (NoisePatch.CHUNKDIMENSION);
//		return new CoRange (start, range);
//	}
//
//
//
//
//	#region NoisePatches
//	// instead of blocks array, now there's a noise patch 
//	// in the beginning, make noise for and blocks for one noise patch (hopefully the one where we're putting the player
//	// then make noise for the surrounding noise patches
//	// good if we can detect which noise patch the player is closest to
//	// make blocks for that one first? etc.
//
//	void updateSetupPatchesList()
//	{
//		NoiseCoord currentNoiseCo = noiseCoordContainingWorldCoord (new Coord (playerCameraTransform.position));
//
//		if (NoiseCoord.Equal (currentNoiseCo, lastPlayerNoiseCoord))
//			return;
//
//		lastPlayerNoiseCoord = currentNoiseCo;
//
//		foreach(NoiseCoord nco in noiseCoordsSurroundingNoiseCoord(currentNoiseCo))
//		{
//			if (!blocks.noisePatches.ContainsKey (nco)) 
//			{
//				makeNoisePatchIfNotExistsAtNoiseCoord (nco);
//				NoisePatch np = blocks.noisePatches [nco];
//
//				if (!np.generatedBlockAlready) {
//					setupThesePatches.Add (np);
//				}
//			}
//		}
//	}
//
//
//
//	IEnumerator setupPatchesFromPatchesList()
//	{
//		//yield return new WaitForSeconds (0.1f);
//		while (true) 
//		{
//			if (setupThesePatches.Count > 0) 
//			{	
//				NoisePatch np = setupThesePatches [0];
//
//				if (np != null) 
//				{
//					if (!np.generatedNoiseAlready) {
//						//						bug ("about to gen noise at: " + np.coord.x + " y: " + np.coord.z);
//						np.generateNoisePatch ();
//					}
//					if (!np.generatedBlockAlready)
//						np.populateBlocksFromNoise ();
//
//					setupThesePatches.RemoveAt (0);
//				} else {
//					bug ("np was null");
//				}
//			}
//			yield return new WaitForSeconds (0.1f);
//		}
//	}
//
//	void makeNewAndSetupPatchAtNoiseCoord(NoiseCoord ncoord)
//	{
//		bug ("making new noise patch at: " + ncoord.toString());
//		makeNoisePatchIfNotExistsAtNoiseCoord (ncoord); 
//
//		generateNoiseForPatchAtCoord (ncoord, false);
//		populateBlocksForPatchAtCoord (ncoord, false);
//	}
//
//	void makeNewAndSetupPatchAtNoiseCoordOnMainThread(NoiseCoord ncoord)
//	{
//		makeNoisePatchIfNotExistsAtNoiseCoord (ncoord); // TODO: want this here?
//
//		generateNoiseForPatchAtCoord (ncoord, true);
//		populateBlocksForPatchAtCoord (ncoord, true);
//	}
//
//	void makeNewPatchesSurroundingNoiseCoord(NoiseCoord ncoord) 
//	{
//		foreach (NoiseCoord nco in noiseCoordsSurroundingNoiseCoord(ncoord))
//		{
//			makeNoisePatchIfNotExistsAtNoiseCoord (nco);
//		}
//	}
//
//	System.Collections.IEnumerable noiseCoordsSurroundingNoiseCoord(NoiseCoord ncoord)
//	{
//		yield return new NoiseCoord (ncoord.x + 1, ncoord.z);
//		yield return new NoiseCoord (ncoord.x + 1, ncoord.z + 1);
//		yield return new NoiseCoord (ncoord.x - 1, ncoord.z);
//		yield return new NoiseCoord (ncoord.x + 1, ncoord.z - 1);
//		yield return new NoiseCoord (ncoord.x, ncoord.z + 1);
//		yield return new NoiseCoord (ncoord.x - 1, ncoord.z + 1);
//		yield return new NoiseCoord (ncoord.x, ncoord.z - 1);
//		yield return new NoiseCoord (ncoord.x - 1, ncoord.z - 1);
//
//	}
//
//	void makeNoisePatchIfNotExistsAtNoiseCoord(NoiseCoord ncoord) 
//	{
//		if (blocks.noisePatches.ContainsKey (ncoord))
//			return;
//
//		NoisePatch npatch = new NoisePatch (ncoord, (int) CHUNKLENGTH, this);
//		blocks.noisePatches.Add (ncoord, npatch);
//	}
//
//	void generateNoiseForPatchAtCoord(NoiseCoord ncoord, bool mainThread) {
//		if (!blocks.noisePatches.ContainsKey(ncoord))
//		{
//			throw new System.ArgumentException("no noise patch at this coord", "ncoord");
//		}
//
//		NoisePatch np = blocks.noisePatches [ncoord];
//
//		if (mainThread)
//			np.genNoiseOnMainThread();
//		else 
//			np.generateNoisePatch ();
//	}
//
//	void populateBlocksForPatchAtCoord(NoiseCoord ncoord, bool mainThread) {
//		if (!blocks.noisePatches.ContainsKey(ncoord))
//		{
//			throw new System.ArgumentException("no noise patch at this coord", "ncoord");
//		}
//
//		NoisePatch np = blocks.noisePatches [ncoord];
//		//if (mainThread)
//		//	np.pop
//		if (mainThread)
//			np.populateBlocksOnMainThread();
//		else
//			np.populateBlocksFromNoise ();
//	}
//
//	NoiseCoord noiseCoordContainingWorldCoord(Coord coord) {
//		return noiseCoordContainingChunkCoord (chunkCoordContainingBlockCoord (coord));
//	}
//
//	NoiseCoord noiseCoordContainingChunkCoord(Coord chcoord)
//	{
//		Coord adjustChCoord = chcoord.booleanNegative () * -1 * NoisePatch.CHUNKDIMENSION;
//		return new NoiseCoord ((chcoord + adjustChCoord) / NoisePatch.CHUNKDIMENSION);
//	}
//
//
//	// delegate-ish for noise patch
//	public void noisePatchFinishedSetup (NoisePatch npatch) 
//	{
//		bug ("heard from noise patch?");
//		if (!firstNoisePatchDone)
//		{
//
//			firstNoisePatchDone = true;
//			bug ("heard back from first noisepatch");
//			finishStartSetup ();
//			placePlayerAtSpawnPoint ();
//		}
//	}
//
//	#endregion
//
//	void movePlayerToXZofCoordAtSurface (Coord moveToCo)
//	{
//		moveToCo = highestSurfaceBlockCoordAt (moveToCo);
//		moveToCo.y += 22;
//		playerCameraTransform.parent.transform.position = moveToCo.toVector3 ();
//	}
//
//	// Use this for initialization
//	void Start () 
//	{
//		firstNoisePatchDone = false;
//
//		chunkMap = new ChunkMap (new Coord (WORLD_XLENGTH_CHUNKS, WORLD_HEIGHT_CHUNKS, WORLD_ZLENGTH_CHUNKS));
//
//		lastPlayerNoiseCoord = new NoiseCoord (100000000, -100000003);
//		setNoiseHandlerResolution ();
//		noiseHandler.initNoiseMap ();
//		frustumChecker = new FrustumChecker (playerCameraTransform, (int) CHUNKLENGTH);
//
//		audioSourceCamera = playerCameraTransform.parent.GetComponent<AudioSource> ();
//
//		NoiseCoord initialNoiseCoord = new NoiseCoord (0, 0);
//		makeNewAndSetupPatchAtNoiseCoord (initialNoiseCoord);
//		NoisePatch firstNoisePatch = blocks.noisePatches [initialNoiseCoord];
//
//
//		//		wait until its done (like it was on a main thread :)
//		while (!firstNoisePatch.Update()) {
//			bug ("noise patch one still not done");
//		}
//
//		//		foreach(NoiseCoord nc in noiseCoordsSurroundingNoiseCoord(initialNoiseCoord))
//		//		{
//		//			makeNewAndSetupPatchAtNoiseCoord (initialNoiseCoord);
//		//		}
//
//		//		finishStartSetup ();
//
//	}
//
//	private void finishStartSetup() {
//		Coord spawnPAtChunkCoord = chunkCoordContainingBlockCoord (spawnPlayerAtCoord);
//
//		//		CoRange nearbyCoRa = corangeFromNoiseCoord (initalNoiseCoord);
//		//		bug ("noise patch based co range: " + nearbyCoRa.toString ());
//		// want**
//		CoRange nearbyCoRa = nearbyChunkRangeInitial (spawnPAtChunkCoord);
//
//		// world has a fixed height so adjust the spawn nearby range accordingly
//		// * want?
//		nearbyCoRa.start.y = 0;
//		nearbyCoRa.range.y = worldHeightInChunkUnits (); //  worldDimsInChunkCoords().y;
//
//		//		buildMap ();
//		buildMapAtRange (nearbyCoRa);
//
//		//		placePlayerAtSpawnPoint ();
//
//		// *want
//		//		StartCoroutine (createChunksFromCreateList ());
//		//		StartCoroutine (destroyChunksFromDestroyList ());
//
//		StartCoroutine (updateChunkLists ());
//		StartCoroutine (createAndDestroyChunksFromLists ()); // combined above two
//
//		//		StartCoroutine (createFurtherAwayChunks ());
//
//		StartCoroutine (setupPatchesFromPatchesList ()); // LAG CULPRIT (MAKES GAME SHAKEY)
//
//
//	}
//
//	private void placePlayerAtSpawnPoint() {
//		movePlayerToXZofCoordAtSurface (spawnPlayerAtCoord);
//	}
//
//
//	// Update is called once per frame
//	void Update () 
//	{
//		//**want
//		//		updateChunkLists ();
//
//		updateSetupPatchesList ();
//
//		//		jobUpdate ();
//
//		drawDebugLinesForChunkRange (m_wantActiveRealm);
//
//		//		frustumChecker.drawLastRay ();
//		drawDebugRay ();
//
//		drawDebugLinesForBlockAtWorldCoord (m_testBreakMakeBlockWorldPos);
//
//	}
//
//	IEnumerator sillyYieldFunc()
//	{
//		int i = 0;
//		for (; i < 1000000; ++i)
//		{
//			bug ("frame count: " + Time.frameCount + "i was: " + i);
//			yield return i * 2;
//		}
//
//	}
//
//	//	void bakeryStart ()
//	//	{
//	////		myJob = new Job();
//	////		myJob.InData = new Vector3[10];
//	//
//	////		bakeryOfChunks = new BakeryOfChunks ();
//	////
//	////		ChunkCoord start = ChunkCoord.chunkCoordZero(); // ChunkCoord.chunkCoordOne (); // ChunkCoord.chunkCoordZero (); // new ChunkCoord (2, 2, 2);
//	////		ChunkCoord range =  new ChunkCoord (3, 4, 4); //new ChunkCoord (8, 4, 4); 
//	////
//	////
//	////		bakeryOfChunks.chunks = shouldBeBakedChunks (start, range, true); //  new List<Chunk> (); // (want this dummy start? logic acceptable?)
//	////
//	////		bakeryOfChunks.Start ();
//	//
//	////		myJob.Start(); // Don't touch any data in the job class after you called Start until IsDone is true.
//	//	}
//
//	void jobUpdate()
//	{
//		if (myJob != null)
//		{
//			if (myJob.Update())
//			{
//				// Alternative to the OnFinished callback
//				myJob = null;
//			}
//		}
//	}
//
//
//	public Coord highestSurfaceBlockCoordAt(Coord c) 
//	{
//		Block retBlock = null;
//		int yy = m_mapDims_blocks.y - 1;
//		c = c.makeIndexSafe (m_mapDims_blocks);
//		do {
//			retBlock = blocks [c.x, yy--, c.z];
//			if (yy == 0)
//				break;
//		} while (retBlock.type == BlockType.Air);
//
//		return new Coord (c.x, yy, c.z);
//	}
//
//
//}
//
////courtesy of: http://answers.unity3d.com/questions/357033/unity3d-and-c-coroutines-vs-threading.html
//public class ThreadedJob
//{
//	private bool m_IsDone = false;
//	private object m_Handle = new object();
//	private System.Threading.Thread m_Thread = null;
//	public bool IsDone
//	{
//		get
//		{
//			bool tmp;
//			lock (m_Handle)
//			{
//				tmp = m_IsDone;
//			}
//			return tmp;
//		}
//		set
//		{
//			lock (m_Handle)
//			{
//				m_IsDone = value;
//			}
//		}
//	}
//
//	public virtual void Start()
//	{
//		m_Thread = new System.Threading.Thread(Run);
//		m_Thread.Start();
//	}
//	public virtual void Abort()
//	{
//		m_Thread.Abort();
//	}
//
//	protected virtual void ThreadFunction() { }
//
//	protected virtual void OnFinished() { }
//
//	public virtual bool Update()
//	{
//		if (IsDone)
//		{
//			OnFinished();
//			return true;
//		}
//		return false;
//	}
//	private void Run()
//	{
//		ThreadFunction();
//		IsDone = true;
//	}
//}
//
//public class Job : ThreadedJob
//{
//	public Vector3[] InData;  // arbitary job data
//	public Vector3[] OutData; // arbitary job data
//
//	protected override void ThreadFunction()
//	{
//		// Do your threaded task. DON'T use the Unity API here
//		for (int i = 0; i < 100000000; i++)
//		{
//			InData[i % InData.Length] += InData[(i+1) % InData.Length] + Vector3.forward;
//		}
//	}
//	protected override void OnFinished()
//	{
//		// This is executed by the Unity main thread when the job is finished
//		for (int i = 0; i < InData.Length; i++)
//		{
//			UnityEngine.Debug.Log("Results(" + i + "): " + InData[i]);
//		}
//	}
//}
//
//public class BakeryOfChunks : ThreadedJob
//{
//	public List<Chunk> chunks;
//	public List<Chunk> finishedChunks;
//
//	protected override void ThreadFunction()
//	{
//		//		Debug.Log ("starting baking thread");
//
//		//		finishedChunks.Clear ();
//		// bake chunks
//		foreach (Chunk ch in chunks )
//		{
//			ch.makeMesh ();
//		}
//
//		//		Debug.Log ("end of baking thread func");
//	}
//
//	protected override void OnFinished()
//	{
//		//		Debug.Log ("chunks finished at time: " + Time.time);
//
//		finishedChunks = new List<Chunk > ();
//		finishedChunks.AddRange (chunks);
//		chunks.Clear ();
//	}
//}
//
//
//
//public class ChunkMap
//{
//	Dictionary<Coord, Chunk> chunks;
//
//	private Coord m_mapDimensions;
//
//	public ChunkMap(Coord world_dims)
//	{
//
//		m_mapDimensions = world_dims; // world dims is vestigial except for height...
//		chunks = new Dictionary<Coord, Chunk> ();
//	}
//
//	//	public CoRange makeCoRangeSafe(CoRange _cora) {
//	////		throw 
//	//		return _cora.makeIndexSafeCoRange (m_mapDimensions);
//	//	}
//
//	public void addChunkAt(Chunk the_chunk, Coord c)
//	{
//		if (chunks.ContainsKey(c))
//		{
//			destroyChunkAt (c);
//			chunks [c] = the_chunk;	
//			return;
//		}
//
//		//		bug ("adding new chunk at: " + c.toString ());
//		chunks.Add (c, the_chunk);
//
//	}
//
//	//	public void addChunkAt(Chunk the_chunk, Coord c)
//	//	{
//	//		if (!c.isIndexSafe (m_mapDimensions))
//	//			return;
//	//
//	//		destroyChunkAt (c);
//	//
//	//		chunks [c.x, c.y, c.z] = the_chunk;
//	//	}
//
//
//	public void destroyChunkAt(Coord c) {
//
//		if (!chunks.ContainsKey(c))
//			return;
//
//		Chunk ch = chunks[c];
//		if (ch != null) {
//			destroyChunkMeshOfChunk (ch);
//			ch.isActive = false;
//		}
//
//		//TODO DONE: purge (replace with an ACTIVEFLAG)
//		//chunks [c.x, c.y, c.z] = null; 
//
//	}
//
//	//	public void destroyChunkAt(Coord c) {
//	//
//	//		if (!c.isIndexSafe (m_mapDimensions))
//	//			return;
//	//
//	//		Chunk ch = chunks[c.x, c.y, c.z];
//	//		if (ch != null) {
//	//			destroyChunk (ch);
//	//			ch.isActive = false;
//	//		}
//	//
//	//		//TODO DONE: purge (replace with an ACTIVEFLAG)
//	//		//chunks [c.x, c.y, c.z] = null; 
//	//
//	//	}
//
//	void destroyChunkMeshOfChunk(Chunk ch) 
//	{
//		//		foreach (Component compo in ch.GetComponents<SphereCollider>())
//		//			GameObject.Destroy (compo);
//		//		foreach (Component compo in ch.GetComponents< MeshFilter>())
//		//			GameObject.Destroy (compo);
//		//		foreach (Component compo in ch.GetComponents< MeshRenderer>())
//		//			GameObject.Destroy (compo);
//		//		foreach (Component compo in ch.GetComponents<Component>()) //destroy the rest (yeah!)
//		//			GameObject.Destroy (compo);
//
//		GameObject chObj = ch.meshHoldingGameObject; //  ch.transform.gameObject;
//
//		GameObject.Destroy (chObj);
//
//		ch.destroyAndSetGameObjectToNull ();
//	}
//
//	public Chunk chunkAt(int ii, int jj, int kk){
//		return chunkAt (new Coord (ii, kk, jj));
//	}
//
//	public Chunk chunkAt(Coord co) {
//		if (!chunks.ContainsKey(co))
//		{
//			//			bug ("no chunk at this co");
//			return null;
//		}
//
//		//		if (!co.isIndexSafe (m_mapDimensions))
//		//			return null;
//
//		return chunks [co]; //dictionary way
//	}
//
//	public Chunk chunkAtOrNullIfUnready(int ii, int jj, int kk) {
//		return chunkAtOrNullIfUnready (new Coord (ii, kk, jj));
//	}
//
//	public Chunk chunkAtOrNullIfUnready(Coord coo) {
//		Chunk chh = chunkAt (coo);
//
//		if (chh == null) { //TODO: change this logic later...
//			return null;
//		}
//
//		return chh;
//	}
//
//	public bool coIsOnMap(Coord co) {
//		return chunks.ContainsKey (co);
//		//		return co.isIndexSafe (m_mapDimensions);
//	}
//
//	public List<Chunk> nonNullChunksInCoRange(CoRange _corange) {
//		return chunksInCoRange (_corange, true);
//	}
//
//	public List<Chunk> chunksInCoRange(CoRange _corange, bool excludeNullChunks) {
//		List<Chunk> retChunks = new List<Chunk> ();
//		if (!chunks.ContainsKey(_corange.start))
//			return retChunks;
//
//		//		if (!coIsOnMap (_corange.start))
//		//			return retChunks;
//
//		Coord start = _corange.start;
//		Coord range = _corange.range;
//		int i = start.x;
//		for (; i < start.x + range.x; ++i) {
//
//			int j = (int) start.y;
//			for (; j < start.y + range.y; ++j) {
//
//				int k = start.z;
//				for (; k < start.z + range.z; ++k) {
//					Chunk chh = chunkAt (i, j, k);
//					if (!excludeNullChunks || ( chh != null )) {
//						retChunks.Add (chh);
//					}
//				}
//			}
//		}
//		return retChunks;
//	}
//
//	void bug(string str) {
//		UnityEngine.Debug.Log (str);
//	}
//
//}
//
//public class BlockCollection
//{
//	public Dictionary<NoiseCoord, NoisePatch> noisePatches = new Dictionary<NoiseCoord, NoisePatch>();
//	private int BLOCKSPERNOISEPATCH;
//	//	private int BLOCKSPERNOISEPATCHPLUSBUFFERTWO;
//
//	public BlockCollection(int _blocksPerChunkTimesChunksPerPatch) 
//	{
//		BLOCKSPERNOISEPATCH = _blocksPerChunkTimesChunksPerPatch;
//		//		BLOCKSPERNOISEPATCHPLUSBUFFERTWO = _blocksPerChunkTimesChunksPerPatch + 2;  // because we want to give chunks their surrounding blocks
//	}
//
//	public Block this[int xx, int yy, int zz]
//	{
//		get
//		{
//			return this [new Coord (xx, yy, zz)];
//		}
//		set
//		{
//			this [new Coord (xx, yy, zz)] = value;
//		}
//	}
//
//
//	public Block this[Coord woco]
//	{
//		get 
//		{
//			NoiseCoord nco = noiseCoordForWorldCoord (woco);
//			if (!noisePatches.ContainsKey(nco))
//			{
//				//				throw new System.ArgumentException ("noise co not in the dictionary: " + nco.toString ());
//				return null;
//			}
//			NoisePatch np = noisePatches [nco];
//			Coord relCoord = noisePatchRelativeCoordFromWorldCoord (woco); // = woco % BLOCKSPERNOISEPATCH;
//
//			if (!np.coordIsInBlocksArray(relCoord))
//			{
//				bug ("GETTER rel coord out of array bounds");
//				//				return null;
//			}
//			return np.blocks [relCoord.x, relCoord.y, relCoord.z];
//		}
//		set
//		{
//			NoisePatch np = noisePatches [noiseCoordForWorldCoord (woco)];
//			Coord relCoord = noisePatchRelativeCoordFromWorldCoord (woco); //  woco % BLOCKSPERNOISEPATCH;
//			if (!np.coordIsInBlocksArray(relCoord))
//			{
//				bug ("SETTER rel coord out of array bounds");
//				//				throw new System.ArgumentException ("rel coord out of array bounds");
//				return;
//			}
//			np.blocks [relCoord.x, relCoord.y, relCoord.z] = value;
//		}
//	}
//
//	public Block specialGetBlockForTesting(Coord woco) 
//	{
//		NoiseCoord nco = noiseCoordForWorldCoord (woco);
//		if (!noisePatches.ContainsKey(nco))
//		{
//			//			throw new System.ArgumentException ("noise co not in the dictionary: " + nco.toString ());
//			return null;
//		}
//
//		NoisePatch np = noisePatches [noiseCoordForWorldCoord (woco)];
//		Coord relCoord = noisePatchRelativeCoordFromWorldCoord (woco); // = woco % BLOCKSPERNOISEPATCH;
//		bug ("got a rel coord in bcollection: " + relCoord.toString ());
//		if (!np.coordIsInBlocksArray(relCoord))
//		{
//			bug ("GETTER rel coord out of array bounds");
//			//				return null;
//		}
//		return np.blocks [relCoord.x, relCoord.y, relCoord.z];
//	}
//
//	void bug (string str) {
//		UnityEngine.Debug.Log (str);
//	}
//
//	//	private NoiseCoord noiseCoordForWorldCoord(Coord woco){
//	//		return new NoiseCoord (woco / BLOCKSPERNOISEPATCH);
//	//	} // FLAWED?
//
//	private NoiseCoord noiseCoordForWorldCoord(Coord woco){
//		woco = (woco.booleanNegative () * -1 * BLOCKSPERNOISEPATCH) + woco;
//		return new NoiseCoord (woco / BLOCKSPERNOISEPATCH);
//	}
//	//
//
//	private Coord noisePatchRelativeCoordFromWorldCoord(Coord woco) {
//		Coord relCoord = woco % BLOCKSPERNOISEPATCH;
//		//
//		//		//fix:
//		//		Coord flipNeg = relCoord.booleanNegative () * (new Coord (BLOCKSPERNOISEPATCH - 1) - relCoord);
//		//		Coord posPart = relCoord.booleanPositive () * relCoord;
//		//		return flipNeg + posPart;
//		return (relCoord + BLOCKSPERNOISEPATCH) % BLOCKSPERNOISEPATCH;
//	}
//}
//
//public struct NoiseCoord
//{
//	public int x; 
//	public int z;
//
//	public NoiseCoord(int xx, int zz) {
//		x = xx;
//		z = zz;
//	}
//
//	public NoiseCoord(Coord cc) {
//		x = cc.x;
//		z = cc.z;
//	}
//
//	public NoiseCoord (ChunkCoord cc) {
//		this = new NoiseCoord (new Coord (cc));
//	}
//
//	public string toString() {
//		return "Noise Coord :) x: " + x + " z: " + z;
//	}
//
//	public static bool Equal(NoiseCoord aa, NoiseCoord bb) {
//		return (aa.x == bb.x) && (aa.z == bb.z);
//	}
//}
//
//
//// TODO: noise patch can get a patch that is 2 res larger in the x and z dims
//// and make its left right top bottom each start  (say) 1/64th overlapping 
//// with the previous next in x and z.
//// will then be able to hand over the whole array that the chunk needs
//// to build itself.
//// and could even trash its array after its not needed?
//
//public class NoisePatch : ThreadedJob
//{
//	public const int CHUNKDIMENSION = 4;
//	private int CHUNKLENGTH;
//
//	private Coord patchDimensions;
//	public Block[,,] blocks;
//	public NoiseCoord coord;
//
//	public bool generatedNoiseAlready;
//	public bool generatedBlockAlready;
//
//	private ChunkManager m_chunkManager;
//
//	#region implement threaded job funcs
//
//	protected override void ThreadFunction()
//	{
//		// populate arrays..
//		generateNoisePatchSep (); // WHAT THIS WORKS??? (we thought it would crash to have one noise handler doing multiple jobs on dif threads)
//		populateBlocksFromNoiseSepThread ();
//	}
//
//	protected override void OnFinished()
//	{
//		generatedBlockAlready = true;
//		m_chunkManager.noisePatchFinishedSetup (this);
//	}
//
//	#endregion
//
//	public NoisePatch(NoiseCoord _noiseCo, int _chunkLen, ChunkManager _chunkMan)
//	{
//		coord = _noiseCo;
//		CHUNKLENGTH = _chunkLen;
//		int array_dim = _chunkLen * CHUNKDIMENSION; //grab borders of neighbor blocks from the noise as well (therefor extra two elements in array in each dimension)
//
//		blocks = new Block[array_dim, array_dim, array_dim];
//		patchDimensions = new Coord (array_dim);
//		m_chunkManager = _chunkMan;
//	}
//
//	public Block blockAtPatchCoord(Coord _worldCo) {
//		return null;
//	}
//
//	public bool coordIsInBlocksArray(Coord indexCo) {
//		if (!indexCo.isIndexSafe(patchDimensions))
//		{
//			bug ("coord out of array bounds for this noise patch: coord: " + indexCo.toString() + " array bounds: " + patchDimensions.toString ());
//			return false;
//		}
//
//		return indexCo.isIndexSafe (patchDimensions);
//	}
//
//	private Coord patchRelativeBlockCoordForChunkCoord(Coord chunkCo) {
//		chunkCo = chunkCo % CHUNKDIMENSION;
//		return ((chunkCo + CHUNKDIMENSION) % CHUNKDIMENSION) * CHUNKLENGTH; //massage negative coords
//	}
//
//	public Block blockAtChunkCoordOffset(Coord chunkCo, Coord offset) 
//	{
//		Coord index = patchRelativeBlockCoordForChunkCoord(chunkCo) + offset;
//
//		if (!index.isIndexSafe(patchDimensions)) // nothing
//		{
//			return m_chunkManager.blockAtChunkCoordOffset (chunkCo, offset); // looking for a block outside of this patch (will happen sometimes we think)
//		}
//
//		if (!generatedBlockAlready)
//			return null;
//
//		return blocks [index.x, index.y, index.z];
//	}
//
//	void bug(string str) {
//		UnityEngine.Debug.Log (str);
//	}
//
//	void resetAlreadyDoneFlags () {
//		generatedNoiseAlready = false;
//		generatedBlockAlready = false;
//	}
//
//	public void generateNoisePatch()
//	{
//		//		m_chunkManager.noiseHandler.GenerateAtNoiseCoord (coord);
//		//		generatedNoiseAlready = true;
//		this.Start ();
//	}
//
//	public void genNoiseOnMainThread() {
//		generateNoisePatchSep();
//	}
//
//	public void populateBlocksOnMainThread() {
//		generateNoisePatchSep();
//	}
//
//	public void generateNoisePatchSep()
//	{
//		if (generatedNoiseAlready)
//			return;
//
//		m_chunkManager.noiseHandler.GenerateAtNoiseCoord (coord);
//		generatedNoiseAlready = true;
//	}
//
//	public void populateBlocksFromNoise()
//	{
//		//		Coord start = new Coord (0);
//		//		Coord range = patchDimensions;
//		//		populateBlocksFromNoise (start, range);
//		//		this.Start ();
//	}
//
//	public void populateBlocksFromNoiseSepThread()
//	{
//		if (generatedBlockAlready)
//			return;
//
//		Coord start = new Coord (0);
//		Coord range = patchDimensions;
//		populateBlocksFromNoise (start, range);
//	}
//
//	public void populateBlocksFromNoise(Coord start, Coord range)
//	{
//		if (generatedBlockAlready)
//			return;
//		//...
//		int x_start = (int)( start.x);
//		int z_start = (int)( start.z);
//		int y_start = (int)( start.y);
//
//		int x_end = (int)(x_start + range.x);
//		int y_end = (int)(y_start + range.y);
//		int z_end = (int)(z_start + range.z);
//
//		int xx = x_start;
//		for (; xx < x_end; ++xx) 
//		{
//			int zz = z_start;
//			for (; zz < z_end; ++zz) 
//			{
//				float noise_val = m_chunkManager.noiseHandler [xx, zz];
//
//				int yy = y_start; // (int) ccoord.y;
//				for (; yy < y_end; ++ yy) 
//				{
//
//					BlockType btype = BlockType.Air;
//
//					//test
//					//					if (yy < 37)
//					//						btype = BlockType.Grass;
//
//					float wallMaker = 1.0f; //test
//
//					if (xx % 16 == 0)
//						wallMaker = 1.5f;
//					// no caves for now...
//					int noiseAsWorldHeight =(int) ((noise_val * .5 + .5) * patchDimensions.y * .2 * wallMaker);
//
//					if (yy < patchDimensions.y - 4 ) // 4 blocks of air on top
//					{ 
//
//						if (false) {
//							int total = xx + yy + zz;
//							if (total < (CHUNKLENGTH - 5) * 3 && total > 4)
//								btype = BlockType.Grass;
//						} else {
//
//							if (yy == 0) {
//								btype = BlockType.BedRock;
//							} else if (noiseAsWorldHeight > yy) {
//								if (yy < CHUNKLENGTH)
//									btype = BlockType.Grass;
//								else if (yy < CHUNKLENGTH * 2)
//									btype = BlockType.Sand;
//								else if (yy < CHUNKLENGTH * 3)
//									btype = BlockType.Dirt;
//								else
//									btype = BlockType.Stone;
//							}
//						}
//					}
//					//					UnityEngine.Debug.Log (" creating block at x " + xx + " y " + yy + " z " + zz);
//					blocks [xx , yy , zz] = new Block (btype);
//				}
//
//			}
//		}
//
//		generatedBlockAlready = true;
//	}
//
//
//}
//
//
//public struct Coord
//{
//	public int x, y, z;
//
//	public Coord(int xx, int yy, int zz) 
//	{
//		x = xx; y = yy; z = zz;
//	}
//
//	public Coord(int val) 
//	{
//		x = val; y = val; z = val;
//	}
//
//	public Coord(uint xx, uint yy, uint zz) 
//	{
//		x = (int) xx; y =(int) yy; z = (int)zz;
//	}
//
//	public Coord (ChunkCoord cc) {
//		x = (int) cc.x; y = (int) cc.y; z = (int) cc.z;
//	}
//
//	public Coord(Vector3 vv)
//	{
//		//		this = new Coord (vv.x , vv.y, vv.z);
//		this = new Coord (vv.x + .5, vv.y + .5, vv.z + .5);
//	}
//
//	public Coord(long  xx, long  yy, long zz) 
//	{
//		x = (int) xx; y =(int) yy; z = (int)zz;
//	}
//
//	public Coord(double   xx, double  yy, double zz) 
//	{
//		x = (int) xx; y =(int) yy; z = (int)zz;
//	}
//
//	public Coord(float   xx, float  yy, float zz) 
//	{
//		x = (int) xx; y =(int) yy; z = (int)zz;
//	}
//
//	public Coord(int xx, uint yy, int zz) 
//	{
//		x = (int) xx; y =(int) yy; z = (int)zz;
//	}
//
//	public static Coord coordZero() {
//		return new Coord (0, 0, 0);
//	}
//
//	public static Coord coordOne() {
//		return new Coord (1, 1, 1);
//	}
//
//	public static bool greaterThan( Coord aa, Coord bb) {
//		//		Debug.Log ("gr than: x: + " + (aa.x > bb.x) + " y: " + (aa.y > bb.y) + "z: " + (aa.z > bb.z));
//
//		return aa.x > bb.x && aa.y > bb.y && aa.z > bb.z;
//	}
//
//	public static bool greaterThanOrEqual( Coord aa, Coord bb) {
//
//		//		Debug.Log ("gr than: x: + " + (aa.x >= bb.x) + " y: " + (aa.y >= bb.y) + "z: " + (aa.z >= bb.z));
//
//		return aa.x >= bb.x && aa.y >= bb.y && aa.z >= bb.z;
//	}
//
//	public static Coord operator *(Coord aa, Coord bb) {
//		return new Coord (aa.x * bb.x, aa.y * bb.y, aa.z * bb.z);
//	}
//
//	public static Coord operator /(Coord aa, Coord bb) {
//		return new Coord (aa.x / (float) bb.x, aa.y /(float) bb.y, aa.z /(float) bb.z);
//	}
//
//	public static Coord operator +(Coord aa, Coord bb) {
//		return new Coord (aa.x + bb.x, aa.y + bb.y, aa.z + bb.z);
//	}
//
//	public static Coord operator -(Coord aa, Coord bb) {
//		return new Coord (aa.x - bb.x, aa.y - bb.y, aa.z - bb.z);
//	}
//
//	public static Coord operator -(Coord aa, int  bb) {
//		return new Coord (aa.x - bb, aa.y - bb, aa.z - bb );
//	}
//
//	public static Coord operator *(Coord aa, float bb) {
//		return new Coord (aa.x * bb , aa.y * bb , aa.z * bb );
//	}
//
//	public static Coord operator /(Coord aa, float  bb) {
//		return new Coord (aa.x / bb , aa.y / bb , aa.z / bb ); // TODO: what happens when we div by zero???
//	}
//
//	public static Coord operator %(Coord aa, float  bb) {
//		return new Coord (aa.x % bb , aa.y % bb , aa.z % bb );
//	}
//
//	public static Coord operator %(Coord aa, int  bb) {
//		return new Coord (aa.x % bb , aa.y % bb , aa.z % bb );
//	}
//
//	public static Coord operator +(Coord aa, float  bb) {
//		return new Coord (aa.x + bb , aa.y + bb , aa.z + bb );
//	}
//
//	public static Coord operator -(Coord aa, float bb) {
//		return new Coord (aa.x - bb, aa.y - bb , aa.z - bb );
//	}
//
//	public bool equalTo(Coord other) {
//		return this.x == other.x && this.y == other.y && this.z == other.z;
//	}
//
//	public bool isIndexSafe( Coord arraySizes ) {
//		return (Coord.greaterThan (arraySizes, this)) && (Coord.greaterThanOrEqual (this, coordZero ()));
//	}
//
//	public Coord makeIndexSafe( Coord arraySizes) {
//		return this.makeRangeSafe (new CoRange (Coord.coordZero(), arraySizes));
//	}
//
//	public Coord makeRangeSafe(CoRange _coRa) {
//		Coord retCo = Coord.Max (this, _coRa.start);
//		Coord outerMinus = _coRa.outerLimit () - 1;
//		return Coord.Min (this, outerMinus);
//	}
//
//	public bool isInsideOfRange(CoRange coRange)
//	{
//		Coord outerLimit = coRange.outerLimit();
//		return (Coord.greaterThanOrEqual (this, coRange.start)) && (Coord.greaterThan (outerLimit, this));
//
//
//		//		return this.isInsideOfRange (coRange.start, coRange.range);
//	}
//
//	public bool isInsideOfRangeInclusive(CoRange coRange)
//	{
//		Coord outerLimit = coRange.outerLimit();
//		return (Coord.greaterThanOrEqual (this, coRange.start)) && (Coord.greaterThanOrEqual (outerLimit, this));
//
//	}
//
//	public static Coord Min (Coord oneone, Coord twotwo)
//	{
//		return new Coord ((oneone.x < twotwo.x ? oneone.x : twotwo.x), (oneone.y < twotwo.y ? oneone.y : twotwo.y), (oneone.z < twotwo.z ? oneone.z : twotwo.z));
//	}
//
//	public static Coord Max (Coord oneone, Coord twotwo)
//	{
//		return new Coord ((oneone.x > twotwo.x ? oneone.x : twotwo.x), (oneone.y > twotwo.y ? oneone.y : twotwo.y), (oneone.z > twotwo.z ? oneone.z : twotwo.z));
//	}
//
//	public Coord onlyPositive ()
//	{
//		return new Coord ((this.x < 0 ? 0 : this.x), (this.y < 0 ? 0 : this.y ), (this.z < 0 ? 0 : this.z));
//	}
//
//	public Coord onlyNegative()
//	{
//		return new Coord ((this.x > 0 ? 0 : this.x), (this.y > 0 ? 0 : this.y ), (this.z > 0 ? 0 : this.z));
//	}
//
//	public Coord booleanPositive()
//	{
//		return new Coord ((this.x < 0 ? 0 : 1), (this.y < 0 ? 0 : 1 ), (this.z < 0 ? 0 : 1));
//	}
//
//	public Coord booleanNegative()
//	{
//		return new Coord ((this.x > 0 ? 0 : 1), (this.y > 0 ? 0 : 1), (this.z > 0 ? 0 : 1));
//	}
//
//	public Coord negNegOnePosPosOne()
//	{
//		return new Coord ((this.x > 0 ? 1 : -1), (this.y > 0 ? 1 : -1), (this.z > 0 ? 1 : -1));
//	}
//
//	//	public bool isInsideOfRange(Coord start, Coord range)
//	//	{
//	//		CoRange corange = new CoRange (start, range);
//	//		return this.isInsideOfRange (corange);
//	//
//	////
//	////		Coord outerLimit = start + range;
//	////		return (Coord.greaterThanOrEqual (this, start)) && (Coord.greaterThan (outerLimit, this));
//	//	}
//
//	public string toString() 
//	{
//		return "X: " + x + " Y: " + y + " Z: " + z;
//	}
//	public Vector3 toVector3()
//	{
//		return new Vector3 ((float)x, (float)y, (float)z);
//	}
//}
//
//public struct CoRange
//{
//	public Coord start;
//	public Coord range;
//
//	public CoRange( Coord st, Coord ra ) {
//		this.start = st;
//		this.range = ra;
//	}
//
//	public CoRange( Coord st, int ra ) {
//		this.start = st;
//		this.range = new Coord (ra);
//	}
//
//	public Coord outerLimit()
//	{
//		return this.start + this.range;
//	}
//
//	public string toString() {
//		return "Range: start: " + this.start.toString () + " range: " + this.range.toString () + " outer limit: " + this.outerLimit().toString();
//	}
//
//	public CoRange makeRangeSafeCoRange(CoRange mapDims) {
//		CoRange retCo = this;
//		retCo.start = Coord.Max (retCo.start, mapDims.start);
//		retCo.range = Coord.Min (retCo.range, mapDims.range);
//		return retCo;
//	}
//
//	public CoRange makeIndexSafeCoRange(Coord mapDims) {
//		return this.makeRangeSafeCoRange (new CoRange (Coord.coordZero (), mapDims));
//	}
//
//	public static bool Contains(CoRange container, CoRange contained) {
//		bool startIsLess = Coord.greaterThanOrEqual (contained.start, container.start);
//		return Coord.greaterThan (container.outerLimit(), contained.outerLimit());
//	}
//}
//
//public struct ChunkCoord
//{
//	public int x,z;
//	public uint y;
//
//	public ChunkCoord(int xx, uint yy, int zz)
//	{
//		x = xx;
//		z = zz;
//		y = yy;
//	}
//
//	public ChunkCoord (int length) {
//		this = new ChunkCoord (length, (uint)length, length);
//	}
//
//	public ChunkCoord( Coord cc) {
//		x = (int)cc.x;
//		y = (uint)cc.y;
//		z = (int)cc.z;
//	}
//
//	public static ChunkCoord chunkCoordZero()
//	{
//		return new ChunkCoord (0, 0, 0);
//	}
//
//	public static ChunkCoord chunkCoordOne()
//	{
//		return new ChunkCoord (1, 1, 1);
//	}
//
//
//	public static ChunkCoord operator + (ChunkCoord a, ChunkCoord b)
//	{
//		return new ChunkCoord (a.x + b.x, a.y + b.y, a.z + b.z);
//	}
//
//	public static ChunkCoord operator - (ChunkCoord a, ChunkCoord b)
//	{
//		return new ChunkCoord (a.x - b.x, a.y - b.y, a.z - b.z);
//	}
//
//	public static ChunkCoord operator * (ChunkCoord a, int  b)
//	{
//		return new ChunkCoord (a.x * b, (uint)(a.y * b), a.z * b);
//	}
//
//	public static bool greaterThan(ChunkCoord a, ChunkCoord b)
//	{
//		return a.x > b.x && a.y > b.y && a.z > b.z;	
//	}
//
//	public static bool greaterOrEqual(ChunkCoord a, ChunkCoord b)
//	{
//		return a.x >= b.x && a.y >= b.y && a.z >= b.z;	
//	}
//
//	public static ChunkCoord XPosUnitChunk()
//	{
//		return new ChunkCoord (1, 0, 0);
//	}
//
//	public static ChunkCoord YPosUnitChunk()
//	{
//		return new ChunkCoord (0, 1, 0);
//	}
//
//	public static ChunkCoord ZPosUnitChunk()
//	{
//		return new ChunkCoord (0, 0, 1);
//	}
//
//}
//
//public struct ChunkRange
//{
//	ChunkCoord start;
//	ChunkCoord range;
//
//	public ChunkRange(ChunkCoord st, ChunkCoord ra)
//	{
//		start = st; range = ra;
//	}
//
//	public ChunkRange(ChunkCoord st, int length)
//	{
//		start = st; 
//		range = new ChunkCoord (length);
//	}
//}
//
//public struct ChunkDirection
//{
//	public Coord dir;
//	// TO DO: make useful (add constructor that uses Direction enum?)
//}	

//private void populateBlocksFromNoise(Coord start, Coord range)
//{
//	if (generatedBlockAlready)
//		return;
//
//	// put saved blocks in first...
////		updateBlocksArrayWithSavableBlocksList ();
//	//...
//	int x_start = (int)( start.x);
//	bool xIsNeg = coord.x < 0;
//	int z_start = (int)( start.z);
//	bool zIsNeg = coord.z < 0;
//	int y_start = (int)( start.y); 
//
//	int x_end = (int)(x_start + range.x);
//	int y_end = (int)(y_start + range.y);
//	int z_end = (int)(z_start + range.z);
//
//	int xx = x_start;
//	for (; xx < x_end ; xx++ ) 
//	{
//		int zz = z_start;
//		for (; zz < z_end; zz++ ) 
//		{
//			float noise_val = m_chunkManager.noiseHandler [xx, zz];
//
//			int yy = y_start; // (int) ccoord.y;
//			for (; yy < y_end; yy++) 
//			{
//
//				if (blocks [xx, yy, zz] == null) // if not we apparently had a saved block...
//				{
//					BlockType btype = BlockType.Air;
//
//					int noiseAsWorldHeight = (int)((noise_val * .5 + .5) * patchDimensions.y * .4); // * (zz/32.0f) * (xx/64.0f));
//
//					if (yy < patchDimensions.y - 4) // 4 blocks of air on top
//					{ 
//						if (yy == 0) 
//						{
//							btype = BlockType.BedRock;
//						} 
//						else if (noiseAsWorldHeight > yy) 
//						{
//							btype = BlockType.Grass;
//
////									if (yy < CHUNKLENGTH)
////										btype = BlockType.Grass;
////									else if (yy < CHUNKLENGTH * 2)
////										btype = BlockType.Sand;
////									else if (yy < CHUNKLENGTH * 3)
////										btype = BlockType.Dirt;
////									else
////										btype = BlockType.Stone;
//						}
//					}
//					//TODO: add a lock(someObject) here?
//					//					UnityEngine.Debug.Log (" creating block at x " + xx + " y " + yy + " z " + zz);
//
//					blocks [xx, yy, zz] = new Block (btype);
//				}
//			}
//		}
//	}
//
//	generatedBlockAlready = true;
//}
//
//
//}

//public void makeMesh()
//{
//	makeMeshAltThread (CHUNKLENGTH, CHUNKHEIGHT);
//	return; 
//////		throw new Exception ("don't want to call make mesh now");
//		bug ("calling make mesh normal no coro");
//		calculatedMeshAlready = false;
//		// (re)create my mesh.
//		random_new_chunk_color_int_test = (int)(UnityEngine.Random.value * 4.0f);
//
//		vertices_list = new List<Vector3> ();
//		triangles_list = new List<int> ();
//		uvcoords_list = new List<Vector2> ();
//
//		int triangles_index = 0;
//
//		noNeedToRenderFlag = true;
//
//		int i = 0;
//		for (; i < CHUNKLENGTH; ++i) 
//		{
//			int j = 0;
//			for (; j < CHUNKHEIGHT; ++j) 
//			{
//				int k = 0;
//				for (; k< CHUNKLENGTH; ++k) 
//				{
//
//					Block b = m_noisePatch.blockAtChunkCoordOffset (chunkCoord, new Coord (i, j, k));
//
//					if (b == null)
//					{
//						//want *****??????
////						bug ("block was null in makeMesh"); //WEIRD THIS CAUSES THE SEP THREAD TO WORK??? (TODO: why)
//						continue;
//					}
//
//					if ((b.type != BlockType.Air))
//						noNeedToRenderFlag = false;
//
//					int dir = (int) Direction.xpos; // zero // TEST
////					int dir = (int) Direction.yneg; // zero
//
//					Block targetBlock;
//
//					for (; dir <= (int) Direction.zneg; ++ dir) 
////					for (; dir < (int) Direction.zpos; ++ dir) 
//					{
//						ChunkIndex ijk = new ChunkIndex (i, j, k);
//
//						// if block is of type air OR
//						// OR if direction and coord "match" and block is not of type air...
//						bool negDir = dir % 2 == 1;
//
//						dvektor dtotalUnitVek = new dvektor (ijk) * new dvektor ((Direction)dir);
//						int totalUnitVek = dtotalUnitVek.total ();
//
//						bool zeroAndNegDir = negDir && totalUnitVek == 0; // at zero at the coord corresponding to direction & negDir
//						bool chunkMaxAndPosDir = !negDir && totalUnitVek == CHUNKLENGTH - 1; // the opposite
//
//						bool reachingBeyondChunkEdge = zeroAndNegDir || chunkMaxAndPosDir;
//						Block blockNextDoor = null;
//
//						// don't bother if we're not going to use...
//						// if non-air and non-edge
//						if (b.type == BlockType.Air || reachingBeyondChunkEdge) 
//						{
//							blockNextDoor = reachingBeyondChunkEdge && b.type != BlockType.Air ? nextBlock ((Direction)dir, ijk, true) : nextBlock ((Direction)dir, ijk);
//						}
//
//
//
//						//debug 
//						if (blockNextDoor == null && reachingBeyondChunkEdge) 
//						{
//							bug ("we were reaching beyond this chunk but got a null block. reaching from chunk index (coord)" + ijk.toString () + "in Dir: " + dir);
//
//						}
//
//
////						if (b.type == BlockType.Air || (blockNextDoor != null && reachingBeyondChunkEdge && blockNextDoor.type == BlockType.Air)) 
//						if ((reachingBeyondChunkEdge) || (b.type == BlockType.Air && blockNextDoor != null)) 
//						{
//
//							// if we're on the edge and not air, we want to know about the block in the next chunk over. if we are an air block,
//							// we want to throw out those blocks...
//							// (we could have just checked for blocks in the next chunk, only if we were an air block, 
//							// but then, we'd be drawing a bit of geom from the next chunk over...)
//
//							targetBlock = reachingBeyondChunkEdge ? b : blockNextDoor ; // if edge matches dir, we want 'this' block
//
//							if (targetBlock != null && targetBlock.type != BlockType.Air) //OK we got a block face that we can use
//							{
//								//ONE LAST CONDITION: ALLOWS US TO DEAL WITH 'REACHING-BEYOND' BLOCKS THAT WERE NULL
//								//WHILE SKIPPING BEYOND BLOCKS THAT WEREN'T AIR
//								if (reachingBeyondChunkEdge && blockNextDoor != null && blockNextDoor.type != BlockType.Air)
//								{
//									continue;
//								}
//								// get the opposite direction to the current one
//								//direction enum: xpos = 0, xneg, ypos, yneg, zpos, zneg = 5
//								int shift = negDir ? -1 : 1; 
//
//								if (reachingBeyondChunkEdge)
//									shift = 0; // want the same direction in this edge case
//
//								Vector3[] verts = new Vector3[]{};
//								int[] tris = new int[]{};
//
//								Vector2[] uvs;
//
//								if (reachingBeyondChunkEdge) //TEST
//									uvs = uvCoordsForTestBlock ((blockNextDoor == null), dir);
//								else 
//									uvs = uvCoordsForBlockType (targetBlock.type, (Direction) (dir + shift) );
//
//								// if on edge make the face for the block at this chunk index, else the one next to it in Direction dir.
//								ChunkIndex nextToIJK = reachingBeyondChunkEdge ? ijk : ChunkIndex.ChunkIndexNextToIndex (ijk,(Direction) dir);
//
//								Direction meshFaceDirection = (Direction)dir + shift;
//
//								int[] posTriangles = new int[] { 0, 2, 3, 0, 1, 2 };  // clockwise when looking from pos towards neg
//								int[] negTriangles = new int[] { 0, 3, 2, 0, 2, 1 }; // the opposite
//
//								tris =(dir + shift) % 2 == 0 ? posTriangles : negTriangles; 
//
//								for (int ii = 0; ii < tris.Length; ++ii) {
//									tris [ii] += triangles_index;
//								}
//								verts = faceMesh (meshFaceDirection, nextToIJK); // dir + shift == the opposite dir. (if xneg, xpos etc.)
//								vertices_list.AddRange (verts);
//								// 6 triangles (index_of_so_far + 0/1/2, 0/2/3 <-- depending on the dir!)
//								triangles_list.AddRange (tris);
//								// 4 uv coords
//								uvcoords_list.AddRange (uvs);
//								triangles_index += 4;
//							}
//						}
//					}
//				}
//			}
//
//		}
//
//		// ** want
//		if (!noNeedToRenderFlag) // not all air (but are we all solid and solidly surrounded?)
//		{
//			noNeedToRenderFlag = (vertices_list.Count == 0);
//
//		}
//
//		calculatedMeshAlready = true;
//
//		// moved to apply mesh
////		mesh.Clear ();
////		mesh.vertices = vertices_list.ToArray ();
////		mesh.uv = uvcoords_list.ToArray ();
////		mesh.triangles = triangles_list.ToArray ();
////
	////
	//////		GetComponent<MeshFilter>().meshCollider = meshc;
//////		meshc.sharedMesh = mesh ;
////
	////		mesh.RecalculateNormals();
////		mesh.RecalculateBounds();
////		mesh.Optimize();
////
	//////		GetComponent<MeshCollider>().sharedMesh = null; // don't seem to need
////		GetComponent<MeshCollider>().sharedMesh = mesh;
//}

//	public void makeMeshAltThread(int CHLEN, int CHHeight)
//	{
//		calculatedMeshAlready = false;
//		// (re)create my mesh.
////		random_new_chunk_color_int_test = (int)(UnityEngine.Random.value * 4.0f);
//
////		vertices_list.Clear(); // new List<Vector3> ();
////		triangles_list.Clear (); // = new List<int> ();
////		uvcoords_list.Clear (); // = new List<Vector2> ();
//
//		int triangles_index = 0;
//		
//#if ONLY_Y_FACES
//		// y Face approach
//		addYFaces (CHLEN, triangles_index); //want
//#else
//
//				
//		// old approach...
//		/* 
//		 * 
//		 
//		noNeedToRenderFlag = true;
//
//		int iterCount = 0;
//
//		int i = 0;
//		for (; i < CHLEN; ++i) 
//		{
//			int j = 0;
//			for (; j < CHHeight; ++j) 
//			{
//				int k = 0;
//				for (; k< CHLEN; ++k) 
//				{
//
//					Block b = m_noisePatch.blockAtChunkCoordOffset (chunkCoord, new Coord (i, j, k));
//
//					if (b == null)
//					{
//						//want *****??????
////						bug ("block was null in makeMesh"); //WEIRD THIS CAUSES THE SEP THREAD TO WORK??? (TODO: why)
//						continue;
//					}
//
//					if ((b.type != BlockType.Air))
//						noNeedToRenderFlag = false;
//
////					int dir = (int) Direction.xpos; // zero 
////					int dir = (int) Direction.zpos; // zero
//					int dir = (int) Direction.zneg; // zero
//
//					Block targetBlock;
//
//					int directionsLookupIndex = 0;
//
//					#if TESTRENDER
//					directionsLookupIndex = m_meshGenPhaseOneDirections.Length; // i.e. skip this
//					#endif
//
//					for (; directionsLookupIndex < m_meshGenPhaseOneDirections.Length ; ++directionsLookupIndex)
////					for (; dir <= (int) Direction.zneg; ++ dir) 
////					for (; dir < (int) Direction.zpos; ++ dir) 
//					{
//
//						dir = m_meshGenPhaseOneDirections [directionsLookupIndex];
//
//						ChunkIndex ijk = new ChunkIndex (i, j, k);
//
//						// if block is of type air OR
//						// OR if direction and coord "match" and block is not of type air...
//						bool negDir = dir % 2 == 1;
//
//						dvektor dtotalUnitVek = new dvektor (ijk) * new dvektor ((Direction)dir);
//						int totalUnitVek = dtotalUnitVek.total ();
//
//						bool zeroAndNegDir = negDir && totalUnitVek == 0; // at zero at the coord corresponding to direction & negDir
//						bool chunkMaxAndPosDir = !negDir && totalUnitVek == CHLEN - 1; // the opposite
//
//						bool reachingBeyondChunkEdge = zeroAndNegDir || chunkMaxAndPosDir;
//						Block blockNextDoor = null;
//
//						// don't bother if we're not going to use...
//						// if non-air and non-edge
//						if (b.type == BlockType.Air || reachingBeyondChunkEdge) 
//						{
//							blockNextDoor = reachingBeyondChunkEdge && b.type != BlockType.Air ? nextBlock ((Direction)dir, ijk, true) : nextBlock ((Direction)dir, ijk);
//						}
//
//
//
//						//debug 
//						if (blockNextDoor == null && reachingBeyondChunkEdge) 
//						{
////							bug ("we were reaching beyond this chunk but got a null block. reaching from chunk index (coord)" + ijk.toString () + "in Dir: " + dir);
//
//						}
//
//
////						if (b.type == BlockType.Air || (blockNextDoor != null && reachingBeyondChunkEdge && blockNextDoor.type == BlockType.Air)) 
//						if ((reachingBeyondChunkEdge) || (b.type == BlockType.Air && blockNextDoor != null)) 
//						{
//
//							// if we're on the edge and not air, we want to know about the block in the next chunk over. if we are an air block,
//							// we want to throw out those blocks...
//							// (we could have just checked for blocks in the next chunk, only if we were an air block, 
//							// but then, we'd be drawing a bit of geom from the next chunk over...)
//
//							targetBlock = reachingBeyondChunkEdge ? b : blockNextDoor ; // if edge matches dir, we want 'this' block
//
//							if (targetBlock != null && targetBlock.type != BlockType.Air) //OK we got a block face that we can use
//							{
//								//ONE LAST CONDITION: ALLOWS US TO DEAL WITH 'REACHING-BEYOND' BLOCKS THAT WERE NULL
//								//WHILE SKIPPING BEYOND BLOCKS THAT WEREN'T AIR
//								if (reachingBeyondChunkEdge && blockNextDoor != null && blockNextDoor.type != BlockType.Air)
//								{
//									continue;
//								}
//								// get the opposite direction to the current one
//								//direction enum: xpos = 0, xneg, ypos, yneg, zpos, zneg = 5
//								int shift = negDir ? -1 : 1; 
//
//								if (reachingBeyondChunkEdge)
//									shift = 0; // want the same direction in this edge case
//
//								Vector3[] verts = new Vector3[]{};
//								int[] tris = new int[]{};
//
//								Vector2[] uvs;
//
//								if (reachingBeyondChunkEdge) //TEST
//									uvs = uvCoordsForTestBlock ((blockNextDoor == null), dir);
//								else 
//									uvs = uvCoordsForBlockType (targetBlock.type, (Direction) (dir + shift) );
//
//								// if on edge make the face for the block at this chunk index, else the one next to it in Direction dir.
//								ChunkIndex nextToIJK = reachingBeyondChunkEdge ? ijk : ChunkIndex.ChunkIndexNextToIndex (ijk,(Direction) dir);
//
//								Direction meshFaceDirection = (Direction)dir + shift;
//
//								int[] posTriangles = new int[] { 0, 2, 3, 0, 1, 2 };  // clockwise when looking from pos towards neg
//								int[] negTriangles = new int[] { 0, 3, 2, 0, 2, 1 }; // the opposite
//
//								tris =(dir + shift) % 2 == 0 ? posTriangles : negTriangles; 
//
//								for (int ii = 0; ii < tris.Length; ++ii) {
//									tris [ii] += triangles_index;
//								}
//								verts = faceMesh (meshFaceDirection, nextToIJK); // dir + shift == the opposite dir. (if xneg, xpos etc.)
//								vertices_list.AddRange (verts);
//								// 6 triangles (index_of_so_far + 0/1/2, 0/2/3 <-- depending on the dir!)
//								triangles_list.AddRange (tris);
//								// 4 uv coords
//								uvcoords_list.AddRange (uvs);
//								triangles_index += 4;
//
//								//CORO!
////								iterCount++;
////								if (iterCount % 10 == 0) {
////									yield return new WaitForSeconds (.1f);
////								}
//
//							}
//						}
//					}
//				}
//			}
//		}
//		
//		*/
//
//		// y Face approach
//		addYFaces (CHLEN, triangles_index); //want
//
//		// ** want
//		if (!noNeedToRenderFlag) // not all air (but are we all solid and solidly surrounded?)
//		{
//			noNeedToRenderFlag = (vertices_list.Count == 0);
//
//		}
//#endif
//
//		calculatedMeshAlready = true;
//	}


// OLD 3D CAVES -- SIDELINED BECAUSE:
// ITS MORE EFFICIENT TO 
// AVOID CYCLINGING THROUGH Y DIMENSION
//	private void populateBlocksFromNoise(Coord start, Coord range)
//	{
//		if (generatedBlockAlready)
//			return;
//
//		// put saved blocks in first...
////		updateBlocksArrayWithSavableBlocksList ();
//		//...
//		int x_start = (int)( start.x);
//		int z_start = (int)( start.z);
//		int y_start = (int)( start.y); 
//
//		int x_end = (int)(x_start + range.x);
//		int y_end = (int)(y_start + range.y);
//		int z_end = (int)(z_start + range.z);
//
//		Block curBlock = null;
//		Block prevYBlock = null;
//
//		int surface_nudge = (int)(patchDimensions.y * .4f); 
//		float rmf3DValue;
//		int noiseAsWorldHeight;
//		int perturbAmount;
//		
//		float heightScaler = 0f;
//		float heightScalerSkewed = 0f;
//		const float CAVEBASETHRESHOLD = .75f;
//		const float CAVETHRESHOLDRANGE = 1f - CAVEBASETHRESHOLD + CAVEBASETHRESHOLD * .3f;
//		
//		const float RMF_TURBULENCE_SCALE = 2.0f;
//		
//#if TERRAIN_TEST
//		
//		Color airColor = Color.black;
//		float coordColor = (float) (((int)Mathf.Abs(coord.z) % 4) / 4.0f);
//		Color solidColor = new Color(coordColor + .5f, 1f - coordColor, coordColor, 1f );
//		terrainSlice = new Color[range.y, range.z];
//#endif
//		
//		BiomeInputs biomeInputs = biomeInputsAtCoord(0,0);
//		
//		//height map 2.0
//		int[,] heightMapStarts = new int[patchDimensions.x, patchDimensions.z];
//		int[,] heightMapEnds = new int[patchDimensions.x, patchDimensions.z];
//		
//#if TERRAIN_TEST
//		if (TestRunner.DontRunDoTerrainTestInstead)
//			x_end = x_start + 1;
//#endif
//		
//		int xx = x_start;
//		for (; xx < x_end ; xx++ ) 
//		{
//			int zz = z_start;
//			for (; zz < z_end; zz++ ) 
//			{
////				float noise_val = 0.5f; // FLAT TEST get2DNoise(xx, zz);
//			
//#if FLAT_TOO
//				float noise_val = .4f; // get2DNoise(xx, zz);
//				biomeInputs = BiomeInputs.Pasture();
//				
//#else
//				float noise_val = get2DNoise(xx, zz);
//				biomeInputs =  BiomeInputs.Pasture(); // FOR TESTING // biomeInputsAtCoord(xx,zz, biomeInputs);
//				int baseElevation =(int)(biomeInputs.baseElevation * patchDimensions.y);
//				int elevationRange = (int)((patchDimensions.y - baseElevation ) * .5f * biomeInputs.hilliness);
//				
//#endif
//				List<int> yHeights = new List<int> ();
//
//				int yy = y_start; // (int) ccoord.y;
//				for (; yy < y_end; yy++) 
//				{
//					if (blocks [xx, yy, zz] == null) // else, we apparently had a saved block...
//					{
//						BlockType btype = BlockType.Air;
//
////						if (yy == 16)
////							btype = BlockType.Grass;
//
//						
//						heightScaler = (float)yy/(float)y_end;
//						float caveIntensity = caveIntensityForHeightUpperLimit(yy, baseElevation + elevationRange);
//						m_chunkManager.m_libnoiseNetHandler.Gain3D = caveIntensity * 1.3f; // TEST // 0.56f * (1.2f - heightScaler * heightScaler);
//
//						
//#if FLAT_TOO
//						rmf3DValue = 0.2f;
//#elif NO_CAVES
//						rmf3DValue = 0.2f;
//#else
//						rmf3DValue = getRMF3DNoise(xx, yy, zz, biomeInputs.caveVerticalFrequency * (1.0f - caveIntensity * 0.05f), noise_val); 
//#endif
////						noiseAsWorldHeight = (int)((noise_val * .5f + .5f) * patchDimensions.y * biomeInputs.hilliness);
//						noiseAsWorldHeight =  (int)(noise_val * elevationRange + elevationRange + baseElevation);
//						perturbAmount = (int) (rmf3DValue * biomeInputs.overhangness * 20);
//
//						if (yy < patchDimensions.y - 4) // 4 blocks of air on top
//						{ 
//							if (yy == 0) {
//								btype = BlockType.BedRock;
////							} else if ( noiseAsWorldHeight + surface_nudge + perturbAmount > yy) { // solid block
//							} else if ( noiseAsWorldHeight > yy) { // solid block
//								
//								heightScalerSkewed =  (1f - caveIntensity); //  heightScaler - .5f;
//
//#if NO_CAVES
//								btype = BlockType.Grass;
//#else
//								if (rmf3DValue < CAVEBASETHRESHOLD + heightScalerSkewed * 3.0 * CAVETHRESHOLDRANGE) { // heightScalerSkewed * heightScalerSkewed * CAVETHRESHOLDRANGE) {
//									btype = BlockType.Grass; 
//								}
//#endif
//								
//								
//							}
//						}
//
//#if TERRAIN_TEST
//						if (TestRunner.DontRunDoTerrainTestInstead)
//						{
//							float rmf3Scaled = 0.5f + (rmf3DValue * 0.5f);
//							if (btype == BlockType.Air)
//								terrainSlice[yy,zz] = airColor;
//							else if (rmf3DValue > .9f) 
//								terrainSlice[yy,zz] = new Color(0f, rmf3Scaled - .5f, 0f, 1);
//							else if (rmf3DValue < 0f) 
//								terrainSlice[yy,zz] = new Color(.7f + rmf3Scaled, 0f, .3f, 1);
//							else 
//								terrainSlice[yy,zz] = new Color(rmf3Scaled, rmf3Scaled, rmf3Scaled, 1);
//					
////							terrainSlice[yy,zz] = btype == BlockType.Air ? airColor : solidColor;					
//						}
//#endif
//						
//						blocks [xx, yy, zz] = new Block (btype);
//
//					}
//
//					curBlock = blocks [xx, yy, zz];
//					
//					// HEIGHT MAP 2.0
//				
//					if (yy == 0 && curBlock.type != BlockType.Air) {
//						heightMapStarts[xx,zz] = yy;
//					} else {
//						prevYBlock = blocks[xx, yy - 1, zz];
//						if (prevYBlock.type != curBlock.type) 
//						{
//							if (curBlock.type == BlockType.Air) { // end of a solid stack of blocks
//								int stackHeight = (yy - 1) + 1 - heightMapStarts[xx,zz];
//								Range1D range_ = new Range1D(heightMapStarts[xx,zz], stackHeight);
//								
//								List<Range1D> currentHeights = heightMap[xx * patchDimensions.x + zz];
//								if (currentHeights == null)
//								{	
////									bug ("got cur heights null. The range range was: " + range_.range);
//									
//									currentHeights = new List<Range1D>();
//								}
//								
//								currentHeights.Add(range_);
//								heightMap[xx * patchDimensions.x + zz] = currentHeights;
//								
//							}
//							else if (prevYBlock.type == BlockType.Air) { // start of a solid stack of blocks
//								heightMapStarts[xx,zz] = yy;
//							} // note: if there are torches or other transparent blocks we will have to accommodate them here/ change approach a little
//						}
//						
//					}
//				
//					// HEIGHT MAP 2.0 END
//
//				} // end for yy
//				if (yHeights.Count == 0)
//					yHeights = null;
//				ySurfaceMap [xx * patchDimensions.x + zz] = yHeights;
//
//			} // end for zz
//
//		} // end for xx
//
////		#if TERRAIN_TEST
////		textureSlice = GetTexture ();
////		#endif
//
//		generatedBlockAlready = true;
//	}


//	public IEnumerator destroyFarAwayGameObjects() {
//		bug ("destroy chunks");
////		ChunkMesh[] chunkMeshes = gameObject.GetComponentsInChildren<ChunkMesh> () as ChunkMesh[];
////		for (int i = 0; i < chunkMeshes.Length; ++i)
////		foreach (Transform chunkMeshT in gameObject.GetComponentsInChildren<Transform>() as Transform[]) // gets children as all objects have a transform
//		foreach (Transform chunkMeshT in gameObject.transform)
//		{
//			ChunkMesh chunkMesh = chunkMeshT.GetComponent<ChunkMesh>();
//
//			if (chunkMesh == null || chunkMesh == prefabMeshHolder) {
//				bug ("null or the prefab chunk");
//				continue;
//			}
//
////			chunkMesh.really (); //test
//
//			Coord _meshChunkCoord = chunkCoordContainingBlockCoord( new Coord (chunkMesh.transform.position) );
//
//
//			if (!_meshChunkCoord.isInsideOfRange(m_wantActiveRealm)) // will want to change logic later -- "outside of larger active realm"
//			{
////				if (!destroyTheseChunks.Contains (chunk)) 
////				{
////					destroyTheseChunks.Add (chunk);
////				}
//				// for now, just destroy??
//				bug ("destroying chunk");
//				chunkMap.destroyChunkAt (_meshChunkCoord);
////				Destroy (chunkMesh);
//				
//
//			}
//			yield return new WaitForSeconds (.01f);
//		}
//	}


//	IEnumerator createFurtherAwayChunks() // not in use.
//	{
//		yield return new WaitForSeconds (1.5f);
//
//		while(true)
//		{
//			if (createTheseChunks.Count == 0) 
//			{
//				float depth = 1.0f;
//				Ray rayMiss;
//				Coord chunkCo;
//				Chunk needToActivateCh;
//				bool foundOneCondition = false;
//
//				bool onMapAndNull = false; // = (needToActivateCh == null) && (chunkMap.coIsOnMap (chunkCo));
//				bool notNullAndNeedToRender = false;
//
//				int attempts = 0;
//
//				rayMiss = frustumChecker.nextRaycastMiss ();
//
//				if (rayMiss.origin != Vector3.zero && rayMiss.direction != Vector3.up) {
//					//want ***
//					do {
//						depth += 1.0f; // (float)frustumChecker.screenIterationCount ();
//
////						bug ("depth is: " + depth);
//
//						Vector3 nextChunkPos = rayMiss.GetPoint (CHUNKLENGTH * depth);
//
//						farawayRayTest = rayMiss;
//						farAwayPos = nextChunkPos;
//
//						chunkCo = chunkCoordContainingBlockCoord (new Coord (nextChunkPos));
//						needToActivateCh = chunkMap.chunkAt (chunkCo);
//
//						onMapAndNull = (needToActivateCh == null) && (chunkMap.coIsOnMap (chunkCo));
//						notNullAndNeedToRender = (needToActivateCh != null) && (!needToActivateCh.noNeedToRenderFlag) && !needToActivateCh.isActive;
//						foundOneCondition = onMapAndNull || notNullAndNeedToRender;
//
//						wasNullAndNeededToRenderTest = onMapAndNull;
//						wasInactiveAndNotNullTest = notNullAndNeedToRender;
//
//					} while (!foundOneCondition && attempts++ < 6);
//
//					if (foundOneCondition) {
//
////					bug ("found a far away chunk with " + attempts + " attempts.");
//						gotAFarAwayChunkTest = true;
//						// ** want
//						if (onMapAndNull) {
//							needToActivateCh = makeNewChunkButNotItsMesh (chunkCo);
//							needToActivateCh.isActive = false;
//							chunkMap.addChunkAt (needToActivateCh, chunkCo); // danger: this code is a copy from updateCreateList (TODO: unify)...
//						} 
//
//						//				want ***
//						makeChunksFromOnMainThreadAtCoord (chunkCo);
////						makeChunksFromOnMainThread (new ChunkCoord (chunkCo), ChunkCoord.chunkCoordOne ());
//					} else {
//						gotAFarAwayChunkTest = false;
////					bug ("didn't find any far away chunks with " + attempts + " attempts.");
//					}
//				}
//
//			}
//			yield return new WaitForSeconds (1.5f);
//			
//		}
//	}

//	
//	private IEnumerator rebuildChunksFromRebuildList()
//	{
//		//TEST
//		yield return null;
//		/*
//		
//		while(true)
//		{
//			Chunk ch = null;
//
//			ch = getAReadyChunkCoordFromList(rebuildChunkChunkCoordList);
//			
//			if (ch != null) {
//				ch.resetCalculatedAlready(); //must we really?
//				makeChunksFromOnSepThreadAtCoord(ch.chunkCoord);
//				rebuildChunkChunkCoordList.Remove(ch.chunkCoord);
//			}
//			
//			yield return new WaitForSeconds(.1f);
//		}
//		*/
//	}

//	IEnumerator createChunksFromCreateList()
//	{
//		while (true)
//		{
//			if(shouldBeCreatingChunksNow())
//			{
//				if (createTheseChunks.Count > 0)
//				{
//					Chunk chunk = createTheseChunks [0];
//
//					if (chunk != null) {
//						makeChunksFromOnMainThread (new ChunkCoord (chunk.chunkCoord), ChunkCoord.chunkCoordOne ());
//					}
//					createTheseChunks.RemoveAt (0);
//				}
//			}
////			bug ("child count: " + transform.childCount + " active Chunks count: " + activeChunks.Count);
//			yield return new WaitForSeconds (.1f);
//		}
//	}
//
//	IEnumerator destroyChunksFromDestroyList()
//	{
//		yield return new WaitForSeconds (5.0f);
//
//		while (true)
//		{
//			if (shouldBeDestroyingChunksNow())
//			{
//				if (destroyTheseChunks.Count > 0) 
//				{
//					Chunk chunk = destroyTheseChunks[0];
//
//					if (chunk != null)
//					{
////						if (!chunk.chunkCoord.isInsideOfRange (m_wantActiveRealm)) { // re-check is nec. //  !createTheseChunks.Contains (chunk)) {
//						if (!chunk.chunkCoord.isInsideOfRange (m_dontDestroyRealm)) { // re-check is nec. //  !createTheseChunks.Contains (chunk)) {
//							chunkMap.destroyChunkAt (chunk.chunkCoord);
//							activeChunks.Remove (chunk);
//						} 
//					}
//					destroyTheseChunks.RemoveAt (0);
//				}
//			}
//			yield return new WaitForSeconds(.1f);
//		}
//	}