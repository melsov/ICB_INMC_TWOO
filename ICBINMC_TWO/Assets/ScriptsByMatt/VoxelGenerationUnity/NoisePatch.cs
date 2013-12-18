//#define FLAT

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

[Serializable]
public class NoisePatch : ThreadedJob
{
	public const int CHUNKDIMENSION = 4;// new Coord(4,1,4);
	public const int CHUNKHEIGHTDIMENSION = 1;
	public static Coord PATCHDIMENSIONSCHUNKS = new Coord (CHUNKDIMENSION, CHUNKHEIGHTDIMENSION, CHUNKDIMENSION);

	private static int CHUNKLENGTH = (int) ChunkManager.CHUNKLENGTH;
	private int BLOCKSPERPATCHLENGTH;

	private static Coord patchDimensions =new Coord(CHUNKLENGTH * CHUNKDIMENSION, 
					ChunkManager.CHUNKHEIGHT * ChunkManager.WORLD_HEIGHT_CHUNKS,
					CHUNKLENGTH * CHUNKDIMENSION);
//	public Block[,,] blocks;
	public Block[,,] blocks { get; set;}
//	public NoiseCoord coord;
	public NoiseCoord coord {get; set;}
	
	public bool startedBlockSetup;
	public bool generatedNoiseAlready {get; set;}
	public bool generatedBlockAlready {get; set;}

	private ChunkManager m_chunkManager;

	private SavableBlock[,,] m_RSavedBlocks;
	public List<SavableBlock> savedBlocks { get; set;}

	private List<int>[] ySurfaceMap = new List<int>[patchDimensions.x * patchDimensions.z];
	
	private BiomeTypeCorners biomeCorners;
	
	private const int BIOMELOOKUPOFFSET = 100;
	
	private const float BIOMEFREQUENCY = 120f;
	private const float NOISESCALE2D = 1f;
	
	#if TERRAIN_TEST
//	public Color[] textureSlice;
	public Color[,] textureSlice = new Color[patchDimensions.y, patchDimensions.z];
	#endif

	public NoisePatch(NoiseCoord _noiseCo, ChunkManager _chunkMan)
	{
		coord = _noiseCo;
		m_chunkManager = _chunkMan;

		otherConstructorStuff ();
	}

	private void otherConstructorStuff() {
		savedBlocks = new List<SavableBlock> ();

		blocks = new Block[patchDimensions.x, patchDimensions.y, patchDimensions.z];
		m_RSavedBlocks = new SavableBlock[patchDimensions.x, patchDimensions.y, patchDimensions.z];

		BLOCKSPERPATCHLENGTH = patchDimensions.x;
	}

	public void updateChunkManager(ChunkManager _chunkMan) {
		m_chunkManager = _chunkMan;
	}

	public Block blockAtPatchCoord(Coord _worldCo) {
		return null;
	}

	public bool coordIsInBlocksArray(Coord indexCo) {
		if (!indexCo.isIndexSafe(patchDimensions)) //debug purposes
		{
			bug ("coord out of array bounds for this noise patch: coord: " + indexCo.toString() + " array bounds: " + patchDimensions.toString ());
			return false;
		}

		return indexCo.isIndexSafe (patchDimensions);

	}

	#region Serialization
	// Implement this method to serialize data. The method is called  
	// on serialization. 
	protected override void doGetObject(SerializationInfo info, StreamingContext context)
	{
		info.AddValue ("NoiseCoord", coord, typeof(NoiseCoord));
		updateSavableBlockList ();
		info.AddValue ("SavedBlocks", savedBlocks, typeof(List<SavableBlock>));
	}
//
//	protected override void doSerializeConstructor(SerializationInfo info, StreamingContext context)
//	{
//		//useless?
//	}

	public NoisePatch(SerializationInfo info, StreamingContext context)
	{
		otherConstructorStuff ();

		coord = (NoiseCoord) info.GetValue("NoiseCoord", typeof(NoiseCoord));
		savedBlocks = (List<SavableBlock>) info.GetValue("SavedBlocks", typeof(List<SavableBlock>));
		updateSavableAndNormalBlocksArray ();
		generatedBlockAlready = false;
		generatedNoiseAlready = false;
	}

	private void addSavableBlock(Block _b, Coord patchRelCo) {
		m_RSavedBlocks [patchRelCo.x, patchRelCo.y, patchRelCo.z] = new SavableBlock (_b.type, patchRelCo);
	}

	private void updateSavableBlockList() 
	{
		savedBlocks.Clear ();

		int x_end = m_RSavedBlocks.GetLength (0);
		int y_end = m_RSavedBlocks.GetLength (1);
		int z_end = m_RSavedBlocks.GetLength (2);

		int xx = 0;
		for (; xx < x_end; xx++) {
			int zz = 0;
			for (; zz < z_end; zz++) {
				int yy = 0; // (int) ccoord.y;
				for (; yy < y_end; yy++) 
				{
					if (m_RSavedBlocks[xx,yy,zz] != null) {
						savedBlocks.Add (m_RSavedBlocks [xx, yy, zz]);
					}
				}
			}
		}

	}// end func

	private void updateSavableAndNormalBlocksArray() {
		foreach(SavableBlock sb in savedBlocks) {

			//check index??
			m_RSavedBlocks [sb.coord.x, sb.coord.y, sb.coord.z] = sb;

			if (sb.coord.isIndexSafe(patchDimensions))
				blocks [sb.coord.x, sb.coord.y, sb.coord.z] = new Block(sb.type);
		}
	}

	#endregion

	#region implement threaded job funcs

	protected override void ThreadFunction()
	{
		// populate arrays..
//		doGenerateNoisePatch (); // WHAT THIS WORKS??? (we thought it would crash to have one noise handler doing multiple jobs on dif threads)
		doPopulateBlocksFromNoise ();
	}

	protected override void OnFinished()
	{
		generatedBlockAlready = true;
		m_chunkManager.noisePatchFinishedSetup (this);
	}

	#endregion


	public CoRange coRangeChunks() {
		Coord start = new Coord (coord.x, 0, coord.z);
		Coord range = new Coord (CHUNKDIMENSION, CHUNKHEIGHTDIMENSION, CHUNKDIMENSION);
		return new CoRange (start, range);
	}


	private static Coord patchRelativeChunkCoordForChunkCoord(Coord chunkCo) 
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
		chunkCo = chunkCo % PATCHDIMENSIONSCHUNKS; //  CHUNKDIMENSION; (NOTE: SWAPPED OUT CHDIMS FOR NEW TALL CHUNKS) // x becomes -2

		// - 2 (plus chunkDim) becomes 2 (minus boolNeg) becomes 1 
		// then take a mod again because positive chunk dims will have gone out of bounds
		// then multiply by chunklength (blocks per chunk is a dimension) to get the rel block coord of the chunk (i.e. 
		// the coord of its 'lower left bottom' block -- i.e. block at the chunk's 0,0,0 coord).
		return ((chunkCo + PATCHDIMENSIONSCHUNKS - boolNeg ) % PATCHDIMENSIONSCHUNKS); //massage negative coords
	}

	private static Coord patchRelativeBlockCoordForChunkCoord(Coord chunkCo) 
	{
		return patchRelativeChunkCoordForChunkCoord(chunkCo) * CHUNKLENGTH; //massage negative coords
	}

	private static Coord patchRelativeBlockCoordForWorldBlockCoord(Coord woco) 
	{
		// this function parallels the patchRelBlockCoord function above

		// example woco = (-1,0,0)
		Coord boolNeg = woco.booleanNegative ();
		woco = woco + boolNeg; // x at -1 becomes 0
		woco = woco % patchDimensions; //  BLOCKSPERPATCHLENGTH; // 0 -> 0

		// 0 -> (BPPL = 64) 64 -> (minus booleanNeg) 63 (mod again to put pos coords back where they belong)
		return ((woco + patchDimensions - boolNeg)) % patchDimensions; // BLOCKSPERPATCHLENGTH;
	}


//	//really make private.. public for testing // OLD
//	public Coord patchRelativeBlockCoordForWorldBlockCoord(Coord woco) 
//	{
//		// this function parallels the patchRelBlockCoord function above
//
//		// example woco = (-1,0,0)
//		Coord boolNeg = woco.booleanNegative ();
//		woco = woco + boolNeg; // x at -1 becomes 0
//		woco = woco % BLOCKSPERPATCHLENGTH; // 0 -> 0
//
//		// 0 -> (BPPL = 64) 64 -> (minus booleanNeg) 63 (mod again to put pos coords back where they belong)
//		return ((woco + BLOCKSPERPATCHLENGTH - boolNeg)) % BLOCKSPERPATCHLENGTH;
//	}


	public Block blockAtChunkCoordOffset(Coord chunkCo, Coord offset) 
	{
		Coord index = NoisePatch.patchRelativeBlockCoordForChunkCoord (chunkCo) + offset; 

		if (!index.isIndexSafe(patchDimensions)) 
		{
			return m_chunkManager.blockAtChunkCoordOffset (chunkCo, offset); // look elsewhere
		}

		if (!generatedBlockAlready)
			return null;

		return blocks [index.x, index.y, index.z];
	}

	//FIND NEIGHBORS TOUCHING SIDES
	public static List<NoiseCoord> nudgeNoiseCoordsAdjacentToChunkCoord(Coord chunkCo) 
	{
		List<NoiseCoord> noiseCoords = new List<NoiseCoord> ();
		Coord relCo = NoisePatch.patchRelativeChunkCoordForChunkCoord (chunkCo);

		if (relCo.x == PATCHDIMENSIONSCHUNKS.x - 1)
			noiseCoords.Add (new NoiseCoord (1, 0));
		if (relCo.x == 0)
			noiseCoords.Add (new NoiseCoord (-1, 0));
		if (relCo.z == PATCHDIMENSIONSCHUNKS.z - 1)
			noiseCoords.Add (new NoiseCoord (0, 1));
		if (relCo.z == 0)
			noiseCoords.Add (new NoiseCoord (0, -1));

		return noiseCoords;
	}

	
	public Block blockAtWorldBlockCoord(Coord woco)
	{
		Coord relCo = patchRelativeBlockCoordForWorldBlockCoord (woco);
		return blocks[relCo.x, relCo.y, relCo.z];
	}

	public void setBlockAtWorldCoord(Block bb, Coord woco) {
		Coord relCo = patchRelativeBlockCoordForWorldBlockCoord (woco);
		blocks [relCo.x, relCo.y, relCo.z] = bb;

		//save any set block
		addSavableBlock (bb, relCo);

		updateYSurfaceMapWithBlockAndRelCoord (bb, relCo);
	}

	private void debugList(List<int> list) {
		string str = "";
		foreach(int i in list) {
			str = str + ", " + i;
		}
		bug("A list: " + str);
	}
	
	private void updateYSurfaceMapWithBlockAndRelCoord(Block bb, Coord relCo) 
	{
		List<int> heights = ySurfaceMap [relCo.x * patchDimensions.x + relCo.z];

		bool isAiryBlock = bb.type == BlockType.Air; //CONDITION MIGHT NEED TO CHANGE LATER. HENCE 'AIRY'

		if (heights.Count == 1) 
		{
			if (isAiryBlock) {
				if (heights[0] == relCo.y + 1) {
					heights [0] = relCo.y;
				} else {
					heights.Insert (0, relCo.y);
					heights.Insert (0, relCo.y);
				}
			} 
			else //SOLID BLOCK
			{
				if (heights[0] == relCo.y) {
					heights [0] += 1;
				} else {
					if (heights[0] > relCo.y) 
						throw new Exception ("we are surprised. how can we add a solid block to a solid column area?");
					
					heights.Add (relCo.y - 1);
					heights.Add (relCo.y + 1);
				}
			}
			return;
		}

		int i = 0;
		while (i < heights.Count) {
			if (heights [i] > relCo.y)
				break;
			i++;
		}


		if (isAiryBlock) 
		{
			if (i == 0) {
				if (heights [i] == relCo.y + 1 && heights [i + 1] == relCo.y + 1) {
					heights [i] = relCo.y;
					return;
				}
			}

			//CONNECTED TWO AIR COLUMNS?
			if (i > 0) {
				if (heights [i] == relCo.y + 1 && heights [i - 1] == relCo.y - 1) {
					heights.RemoveAt (i - 1);
					heights.RemoveAt (i - 1);
					return;
				}
			}

			if (heights[i] == relCo.y + 1) {
				heights [i] = relCo.y;
				return;
			}
			if (heights[i -1] == relCo.y - 1) {
				heights [i - 1] = relCo.y;
				return;
			}

			//MADE A GAP IN A SOLID COLUMN
			heights.Insert (i - 1, relCo.y);
			heights.Insert (i - 1, relCo.y);
		} 
		else // SOLID BLOCK
		{
			if (i > 1) //SURELY IT IS, UNLESS HEIGHTS IS EMPTY! (WE KNOW IT'S NOT EQ. TO 1 AT THIS PT)
			{ 
				//FILLED A GAP TO MAKE A SOLID COLUMN?
				if (heights[i - 1] == relCo.y && heights[i -2] == relCo.y) {
					heights.RemoveAt (i - 2);
					heights.RemoveAt (i - 2);
					return;
				}

				if (heights[i - 1] == relCo.y) {
					heights [i - 1] += 1;
					return;
				}

				if (heights[i - 2] == relCo.y) {
					heights [i - 2] += 1;
					return;
				}

				//BLOCK WAS INSERTED INTO AN AIR COLUMN
				heights.Insert (i, relCo.y + 1);
				heights.Insert (i, relCo.y - 1);

				// WEIRD CASE: FILLED A GAP TO MAKE A COLUMN THAT NOW SPANS 0 TO WORLD MAX HEIGHT (HEIGHT[0] == MAX HEIGHT??)
			}
		}
	}

	void bug(string str) {
		UnityEngine.Debug.Log (str);
	}

	#region Surface Maps

	public List<int>[] ySurfaceMapForChunk(Chunk chunk) 
	{
		List<List<int>> retList = new List<List<int>> ();
		Coord patchRel = patchRelativeBlockCoordForChunkCoord (chunk.chunkCoord);

		int startIndex = patchRel.x * patchDimensions.x + patchRel.z;
		int i = 0;
		for(; i < CHUNKLENGTH; ++i) 
		{
			int startRange = startIndex + patchDimensions.x * i;
			retList.AddRange ( ySurfaceMap.Skip(startRange).Take(CHUNKLENGTH));
		}

		return retList.ToArray ();
	}

	#endregion

	void resetAlreadyDoneFlags () {
		generatedNoiseAlready = false;
		generatedBlockAlready = false;
	}
	
//	generateNoiseAndPopulateBlocksAsync
	
	public void populateBlocksAsync()
	{
		this.startedBlockSetup = true;
//		m_chunkManager.noiseHandler.GenerateAtNoiseCoord (coord);
//		generatedNoiseAlready = true;
		this.Start ();
	}

	public void genNoiseOnMainThread() {
//		doGenerateNoisePatch();
	}

	public void populateBlocksOnMainThread() { //TODO: very confusing. clarify the use of this...
//		doGenerateNoisePatch();
	}

	public void genNoiseAndPopulateBlocksOnMainThread() {
//		doGenerateNoisePatch ();
		doPopulateBlocksFromNoise ();
	}

	public void doGenerateNoisePatch()
	{
//		if (generatedNoiseAlready)
//			return;
//
//		m_chunkManager.noiseHandler.GenerateAtNoiseCoord (coord);
//
//		m_chunkManager.noiseHandler.GenerateAltAtNoiseCoord (coord);
		
		//don't need. dif noise lib where we just access the values directly (i.e. no more separate noise map)

		generatedNoiseAlready = true;
	}

	public void populateBlocksFromNoise()
	{
//		Coord start = new Coord (0);
//		Coord range = patchDimensions;
//		populateBlocksFromNoise (start, range);
//		this.Start ();
	}

	private void doPopulateBlocksFromNoise()
	{
		if (generatedBlockAlready)
			return;

		Coord start = new Coord (0);
		Coord range = patchDimensions;
		populateBlocksFromNoise (start, range);
	}

	private void populateBlocksFromNoise(Coord start, Coord range)
	{
		if (generatedBlockAlready)
			return;

		// put saved blocks in first...
//		updateBlocksArrayWithSavableBlocksList ();
		//...
		int x_start = (int)( start.x);
		int z_start = (int)( start.z);
		int y_start = (int)( start.y); 

		int x_end = (int)(x_start + range.x);
		int y_end = (int)(y_start + range.y);
		int z_end = (int)(z_start + range.z);

		Block curBlock = null;
		Block prevYBlock = null;

		int surface_nudge = (int)(patchDimensions.y * .4f); 
		float rmf3DValue;
		int noiseAsWorldHeight;
		int perturbAmount;
		
		float heightScaler = 0f;
		float heightScalerSkewed = 0f;
		const float CAVEBASETHRESHOLD = .95f;
		const float CAVETHRESHOLDRANGE = 1f - CAVEBASETHRESHOLD + CAVEBASETHRESHOLD * .3f;
		
		
//		#if TERRAIN_TEST
//		textureSlice = new Color[range.z, range.y];
//		#endif
		
		BiomeInputs biomeInputs = biomeInputsAtCoord(0,0);
		
		int xx = x_start;
		for (; xx < x_end ; xx++ ) 
		{
			int zz = z_start;
			for (; zz < z_end; zz++ ) 
			{
				float noise_val = get2DNoise(xx, zz);
				biomeInputs = biomeInputsAtCoord(xx,zz, biomeInputs);

				List<int> yHeights = new List<int> ();

				int yy = y_start; // (int) ccoord.y;
				for (; yy < y_end; yy++) 
				{
					if (blocks [xx, yy, zz] == null) // else, we apparently had a saved block...
					{
						BlockType btype = BlockType.Air;
#if FLAT
						if (yy == 16)
							btype = BlockType.Grass;
#else
						
						heightScaler = (float)yy/(float)y_end;
						
						m_chunkManager.m_libnoiseNetHandler.Gain3D = 0.26f * (1.2f - heightScaler * heightScaler);
						
						rmf3DValue = getRMF3DNoise(xx, yy, zz, biomeInputs.caveVerticalFrequency); // * (1f - heightScaler * heightScaler)); 

						noiseAsWorldHeight = (int)((noise_val * .5f + .5f) * patchDimensions.y * biomeInputs.hilliness);
						perturbAmount = (int) (rmf3DValue * patchDimensions.x * biomeInputs.overhangness);

						if (yy < patchDimensions.y - 4) // 4 blocks of air on top
						{ 
							if (yy == 0) {
								btype = BlockType.BedRock;
							} else if ( noiseAsWorldHeight + surface_nudge + perturbAmount > yy) { // solid block
								
								heightScalerSkewed = heightScaler - .5f;
//								bool onlyDeepCavesTest = yy > patchDimensions.y * .3f;
//								
//								if (onlyDeepCavesTest)  {
//									btype = BlockType.Grass;
//								}
//								//check for a cave (note: 'cragginess' now affecting cave shape...maybe don't want that)
//								else
								if (rmf3DValue < CAVEBASETHRESHOLD + heightScalerSkewed * heightScalerSkewed * CAVETHRESHOLDRANGE) {
									btype = BlockType.Grass; 
								}
							}
						}
#endif

						blocks [xx, yy, zz] = new Block (btype);

					}

					curBlock = blocks [xx, yy, zz];

					// Y SURFACE MAP
					if (yy > 0) {
						prevYBlock = blocks [xx, yy - 1, zz];
						if (prevYBlock.type != curBlock.type) {
							if (curBlock.type == BlockType.Air) {
								yHeights.Add (yy);
							} 
							else if (prevYBlock.type == BlockType.Air) {
								yHeights.Add (yy - 1);
							} // note: if there are torches or other transparent blocks we will have to accommodate them here/ change approach a little
						}
					}

				} // end for yy
				if (yHeights.Count == 0)
					yHeights = null;
				ySurfaceMap [xx * patchDimensions.x + zz] = yHeights;

			} // end for zz

		} // end for xx

		#if TERRAIN_TEST
		textureSlice = GetTexture ();
		#endif

		generatedBlockAlready = true;
	}

	private static bool caveIsHere(float cave_noise_val,int yy, int surfaceNudge) {
		float yLevel = yy - surfaceNudge;
//		float filter_value =   (float)((yy) /(float) patchDimensions.y); 
		float filter_value =   (float)((yLevel) /(float) surfaceNudge * 2.0f); 
//		filter_value *= filter_value;
		return cave_noise_val > filter_value;
	}

	private float getSimplexNoise(int xx, int yy, int zz) {
		return SimplexNoise.Noise.Generate ((float)((patchDimensions.x * coord.x + xx) / (float)patchDimensions.x), (float)yy / (float)patchDimensions.y, (float)((patchDimensions.z * coord.z + zz)/(float)patchDimensions.z)); 
	}

	private float getRMF3DNoise(int xx, int yy, int zz, float caveVerticalFrequency) {
		return m_chunkManager.m_libnoiseNetHandler.GetRidgedMultiFractalValue ((float)((patchDimensions.x * coord.x + xx) / (float)patchDimensions.x), 
			caveVerticalFrequency * (float)yy / (float)patchDimensions.y, 
			(float)((patchDimensions.z * coord.z + zz)/(float)patchDimensions.z)); 
	}
	
	private float getBiomeType(int xx, int zz) {
		float xco = (float)((patchDimensions.x * coord.x + xx + BIOMELOOKUPOFFSET * BIOMEFREQUENCY) / (float)(patchDimensions.x * patchDimensions.x));
		float zco = (float)( (patchDimensions.z * coord.z + zz + BIOMELOOKUPOFFSET * BIOMEFREQUENCY)/(float)(patchDimensions.z * patchDimensions.z) ); 
		return m_chunkManager.m_libnoiseNetHandler.GetRidgedMultiFractalValue(xco, 0, zco);
	}
	
	private float get2DNoise(int xx, int zz) {
		return m_chunkManager.m_libnoiseNetHandler.Get2DValue((float)((patchDimensions.x * coord.x + xx) * NOISESCALE2D / (float)patchDimensions.x), 	
			(float)((patchDimensions.z * coord.z + zz) * NOISESCALE2D/(float)patchDimensions.z)); 
	}

	private static float scaleNegOneOneFloatsToRange(float value, float _range ) {
		return value * _range;
	}

	private static int noiseCoordSafeValue(int value) {
		if (value < 0)
			return 0;
		if (value >= patchDimensions.x)
			return patchDimensions.x - 1;

		return value;
	}
	
	//BIOME type calculated for each corner of the patch
	private BiomeTypeCorners getBiomeCorners()
	{
		float llval = getBiomeType(0,0);
		float lrval = getBiomeType(0, patchDimensions.z - 1);
		float ulval = getBiomeType(patchDimensions.x - 1, 0);
		float urval = getBiomeType(patchDimensions.x - 1, patchDimensions.z - 1 );
		
		return new BiomeTypeCorners(biomeTypeForValue(llval),
			biomeTypeForValue(lrval),
			biomeTypeForValue(ulval),
			biomeTypeForValue(urval));
	}
	
	private BiomeType biomeTypeForValue(float value) {
		if(value < 0) {
			return BiomeType.Pasture;
		}else {
			return BiomeType.CraggyMountains;
		}
	}
	
	private BiomeInputs biomeInputsAtCoord(int xx, int zz) 
	{
		BiomeInputs dummyInputs = new BiomeInputs();
		dummyInputs.cragginess = -99f;
		return biomeInputsAtCoord(xx, zz, dummyInputs);
	}
	
	private BiomeInputs biomeInputsAtCoord(int xx, int zz, BiomeInputs lastInputs) 
	{
		//can we skip looking up the inputs this time?
		//skip if lastInputs != the dummy and coords are divisible by 8
		//to avoid doing too much calculating
		if (!(lastInputs.cragginess < -98f || (xx % 8 == 0 && zz % 8 == 0 ))) 
			return lastInputs;
		
		float biomeNoiseValue = getBiomeType(xx, zz);
		
		if (biomeNoiseValue < -.05)
			return BiomeInputs.Pasture();
		else if (biomeNoiseValue > .05)
			return BiomeInputs.CraggyMountains();
		
		//interpolate between
		float pWeight = biomeNoiseValue - (-0.05f);
		float cmWeight = 0.05f - biomeNoiseValue;
		
		return BiomeInputs.Mix(BiomeInputs.Pasture() , BiomeInputs.CraggyMountains(), pWeight, cmWeight);
	}

	#region get texture

	public Color[,] GetTexture()
	{
		return this.GetTexture(LibNoise.Unity.Gradient.Grayscale);
	}

	/// <summary>
	/// Creates a texture map for the current content of the noise map.
	/// </summary>
	/// <param name="device">The graphics device to use.</param>
	/// <param name="gradient">The gradient to color the texture map with.</param>
	/// <returns>The created texture map.</returns>
	public Color[,] GetTexture(LibNoise.Unity.Gradient gradient)
	{
		return this.GetTexture(ref gradient, patchDimensions.z, patchDimensions.y);
	}

	public Color[,] GetTexture(ref LibNoise.Unity.Gradient gradient, int texWidth, int texHeight)
	{
//		Texture2D result = new Texture2D(texWidth, texHeight);
//		Color[] data = new Color[texWidth * texHeight];
		Color[,] data = new Color[texHeight ,texWidth];
		int id = 0;

		int texFour = texHeight / 4;

		for (int y = 0; y < texFour * 4 ; y++)
		{
			for (int z = 0; z < texWidth; z++, id++)
			{
//		for (int z = 0; z < texWidth; z++)
//		{
//			for (int y = 0; y < texFour * 4 ; y++, id++)
//			{
				Block b = blocks [0, y, z];

//				float d =  
//					y % 4 == 0 ? 1f : (float)(x / texWidth);  // (float)(y / texHeight); // .75f; // 0.0f;
//				if (!float.IsNaN(this.m_borderValue) && (x == 0 || x == this.m_width - 1 || y == 0 || y == this.m_height - 1))
//				{
//					d = this.m_borderValue;
//				}
//				else
//				{
//					d = this.m_data[x, y];
//				}
				data [y,z] = b.type == BlockType.Air ? Color.black : Color.white;  // new Color (d, d, d, 1f);// gradient[d];
			}
		}

		return data;
		//result.SetData<Color>(data);
		//Debug.Log("Setting pixels");
//		result.SetPixels(data);
//		return result;
	}

	#endregion

	private static int shiftCoordBy(int coordMaxValue, float scale) {
		return (int)(coordMaxValue * scale);
	}

}

