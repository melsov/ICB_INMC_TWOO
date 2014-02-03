#define NO_CAVES
#define LIGHT_HACK
//#define TURN_ON_STRUCTURES
//#define FLAT_TOO
//#define TERRAIN_TEST
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


				
//generated phenomena notes:
/*
 * Generated phenomena objects (structures) 
  * keep a table2d of lists of ranges 
  * and to make them, you would often feed them a seed number
  * they have a point of origin which can be negative (lower xz corner)
  * and the table dimensions adjust to accommodate only the positive side.
  * 
  * noisepatches--
  * when noise patches come into being, they ask the four noise patches next to them xz pos neg
  * for any of their structures.
  * and when they generate from noise, they keep a list of the structures that they want to give to 
  * each of their neighbors.
  * they always ask their neighbors for the structures, (and if those noisepatches exist, great if not see below)
  * and always try to give all of the structures that they have once they've generated them.
*/
//end generated phenomena nots

// TODO: object wish list:
// a Topography object. 
// like a height map except that it 
// holds sets of ranges that represent
// complete coverage of a noise patch over the xz plane

// sort of two types: top of range only: i.e. the ceiling is the sky
// and ceiling and floor.
// or: two types: ceiling and then the other type is floor.
// all nps have at least one floor.

// really face aggregators could do this job pretty well.

// also can have xpos,neg z pos neg walls (analogous to ceiling and floors which are just ypos and neg walls)..

// conveniently, (sets of?) FaceAggregators can really be walls.

// how to find walls?
// just keep track of the adjacency thing.

// so FaceAggregators that take both north and south pos and neg sides of cubes are not convenient for this really.

// TODO: make face aggregators take only one side.

// could there be some kind of likely to be needed next status for these walls?
// like could we know the wall that is flush with a wall? and especially in the case of floors,
// what level is that floor at? (what is its lowest level) and if the player were at that level
// which walls would we need?



[Serializable]
public class NoisePatch : ThreadedJob
{
	public const int CHUNKDIMENSION = 4;// new Coord(4,1,4);
	public const int CHUNKHEIGHTDIMENSION = 1;
	public static Coord PATCHDIMENSIONSCHUNKS = new Coord (CHUNKDIMENSION, CHUNKHEIGHTDIMENSION, CHUNKDIMENSION);

	private static int CHUNKLENGTH = (int) ChunkManager.CHUNKLENGTH;
	private int BLOCKSPERPATCHLENGTH;

	public static Coord patchDimensions =new Coord(CHUNKLENGTH * PATCHDIMENSIONSCHUNKS.x, 
					ChunkManager.CHUNKHEIGHT * ChunkManager.WORLD_HEIGHT_CHUNKS,
					CHUNKLENGTH * PATCHDIMENSIONSCHUNKS.z);

	public Block[,,] blocks { get; set;}

	public NoiseCoord coord {get; set;}
	
//	public bool startedBlockSetup;
//	public bool generatedNoiseAlready {get; set;}
	public bool generatedBlockAlready {get; set;}

	private ChunkManager m_chunkManager;

	private SavableBlock[,,] m_RSavedBlocks;
	public List<SavableBlock> savedBlocks { get; set;}

	private List<int>[] ySurfaceMap = new List<int>[patchDimensions.x * patchDimensions.z];
	private List<Range1D>[] heightMap = new List<Range1D>[patchDimensions.x * patchDimensions.z];
	private List<Range1D>[] savedRangeLists = new List<Range1D>[patchDimensions.x * patchDimensions.z];
//	private List<Range1D>[] structureMap = new List<Range1D>[patchDimensions.x * patchDimensions.z];
	
	private const int BIOMELOOKUPOFFSET = 100;
	
	private const float BIOMEFREQUENCY = 120f;
	private const float NOISESCALE2D = 1f;
	
	private static int CAVE_MAX_LIKELIHOOD_LEVEL = (int)(patchDimensions.y * .2f);
	
	const float CAVEBASETHRESHOLD = .75f;
	const float CAVETHRESHOLDRANGE = 1f - CAVEBASETHRESHOLD + CAVEBASETHRESHOLD * .3f;
	
	const float RMF_TURBULENCE_SCALE = 2.0f;
	
	private static SimpleRange TREE_MAX_LIKELIHOOD_VRANGE = SimpleRange.SimpleRangeWithStartAndExtent((int)(patchDimensions.y * .45), (int) (patchDimensions.y * .75));
	private const float TREE_MAX_LIKELIHOOD_FACTOR = .125f;
	
//#if TERRAIN_TEST
	public Color[,] terrainSlice = new Color[patchDimensions.y, patchDimensions.z];
//#endif
	
	private List<StructureBase> structures = new List<StructureBase>();
	private DataForNeighbors dataForNeighbors = DataForNeighbors.MakeNew();
	
	private List<Coord> patchRelativeChCosToRebuild = new List<Coord>();
	
#if LIGHT_HACK
	private bool thereWasAnOverhang = false;
	private float currentLightLevel = 3.5f;
	private float lastXRowStartLightLevel = 3.5f;
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
		if (!indexCo.isIndexSafe(patchDimensions)) // debugging
		{
			throw new Exception ("coord out of array bounds for this noise patch: coord: " + indexCo.toString() + " array bounds: " + patchDimensions.toString ());
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
		info.AddValue ("SavedRanges", savedRangeLists, typeof(List<Range1D>[]));
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
		
		//TODO: set up range saving... (NOTE: savedRangeList mem var was added but saving mechanics are incomplete...)
//		heightMap = (List<Range1D>[]) info.GetValue ("SavedRanges", typeof(List<Range1D>[]));
		
		updateSavableAndNormalBlocksArray ();
		generatedBlockAlready = false;
		hasStarted = false;
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
	
	#region neighbors have setup also
	
	public List<NoiseCoord> neighborCoordsWhoHaveNotStartedSetup()
	{
		List<NoiseCoord> result = new List<NoiseCoord>(6);
		
		foreach(NeighborDirection ndir in NeighborDirectionUtils.neighborDirections())
		{
			NoiseCoord nco = NeighborDirectionUtils.nudgeCoordFromNeighborDirection(ndir) + this.coord;	
			bool hasbuilt = m_chunkManager.blocks.noisePatchAtNoiseCoordHasBuiltAtleastOnce(nco);
			if (!hasbuilt) {
				result.Add(nco);
			}
		}
		return result;
	}
	
	private bool m_neighborsHaveBuiltAtLeastOnce = false;
	
	public bool neighborsHaveAllBuiltAtLeastOnce {
		get {
			if (m_neighborsHaveBuiltAtLeastOnce)
				return true;
			
			List<NoiseCoord> unbuiltNeighbors = neighborCoordsWhoHaveNotStartedSetup();
			if ( unbuiltNeighbors.Count == 0) {
				m_neighborsHaveBuiltAtLeastOnce = true;	
			}
			return unbuiltNeighbors.Count == 0;
		}
	}
	
	public bool patchesAdjacentToChunkCoordAreReady(Coord chunkCoord) {
		if (!this.IsDone || this.hasStarted)
			return false;
		
		Coord patchRelCo = patchRelativeChunkCoordForChunkCoord(chunkCoord);
		
		if (patchRelCo.x == PATCHDIMENSIONSCHUNKS.x - 1) {
			// XPOS needed
			if (!neighborInDirectionIsReady(NeighborDirection.XPOS))
				return false;
		}
		else if (patchRelCo.x == 0) {
			if (!neighborInDirectionIsReady(NeighborDirection.XNEG))
				return false;
		}
		
		if (patchRelCo.z == PATCHDIMENSIONSCHUNKS.z - 1) {
			// XPOS needed
			if (!neighborInDirectionIsReady(NeighborDirection.ZPOS))
				return false;
		}
		else if (patchRelCo.z == 0) {
			if (!neighborInDirectionIsReady(NeighborDirection.ZNEG))
				return false;
		}
		return true;
	}
	
	public bool neighborInDirectionIsReady(NeighborDirection ndir) {
		NoiseCoord nudgeCo = NeighborDirectionUtils.nudgeCoordFromNeighborDirection(ndir);
		return m_chunkManager.blocks.noisePatchAtNoiseCoordHasBuiltAtleastOnce(this.coord + nudgeCo);
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
		
#if TURN_ON_STRUCTURES
#else	
		getStructuresFromNeighbors();
		dropOffStructuresForNeighbors();
#endif
//		m_chunkManager.noisePatchFinishedSetup (this);
		
//		throw new Exception("NOISE PATCH ON FINISHED WAS CALLED??"); // this func is not called (understandably)
		
//		if (this.patchRelativeChCosToRebuild.Count > 0)
//			this.m_chunkManager.rebuildChunksAtNoiseCoordPatchRelativeChunkCoords(this.coord, this.patchRelativeChCosToRebuild); //test want but maybe not here (go to main thread)		
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

		return blockAtRelativeCoord(index); //  blocks [index.x, index.y, index.z];
	}
	
	public BlockType blockTypeAtChunkCoordOffset(Coord chunkCo, Coord offset) 
	{
		Coord index = NoisePatch.patchRelativeBlockCoordForChunkCoord (chunkCo) + offset; 

		if (!index.isIndexSafe(patchDimensions)) 
		{
			return m_chunkManager.blockTypeAtChunkCoordOffset (chunkCo, offset); // look elsewhere
		}

		if (!generatedBlockAlready)
			return BlockType.TheNullType;

		return blockTypeFromCoord(index);
	}
	
	public List<Range1D> heightsListAtChunkCoordOffset(Coord chunkCo, Coord offset)
	{
		Coord index = NoisePatch.patchRelativeBlockCoordForChunkCoord(chunkCo) + offset;
		
		if (!index.isIndexSafe(patchDimensions))
		{
			return m_chunkManager.heightsListAtChunkCoordOffset ( chunkCo, offset);
		}
		
		if(!generatedBlockAlready)
			return null;
		
		return heightMap[index.x * patchDimensions.x + index.z];
		
	}
	
	public List<Range1D> heightsListAtWorldCoord(Coord woco)
	{
		Coord index = NoisePatch.patchRelativeBlockCoordForWorldBlockCoord(woco);
		
		if (!generatedBlockAlready)
			return null;
		
		return heightMap[index.x * patchDimensions.x + index.z];
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
	
	private Coord patchWorldCoord() {
		return new Coord(this.coord.x, 0, this.coord.z) * patchDimensions;
	}

	private Coord unsafePatchRelativeCoordFrom(Coord woco) 
	{
		return woco - patchWorldCoord();
	}
	
	public int highestPointAtWorldCoord(Coord woco)
	{
		List<Range1D> heights = rangesAtWorldCoord(woco);
		
		if (heights == null)
			return -1234574;
		
		Range1D last = heights[heights.Count - 1];
		return last.extent();
	}
	
	public List<Range1D> rangesAtWorldCoord(Coord woco)
	{
		Coord unsafeRelCo = unsafePatchRelativeCoordFrom(woco);
		if (!unsafeRelCo.isIndexSafe(patchDimensions))
		{
			return this.m_chunkManager.rangesAtWorldCoord(woco);
		}	
		
		return heightMap[unsafeRelCo.x * patchDimensions.x + unsafeRelCo.z];
	}
	
	// TODO: bug. if block not on the ground--i.e. floating--we think that it is air...
	
	public Block blockAtWorldBlockCoord(Coord woco)
	{
		Coord relCo = patchRelativeBlockCoordForWorldBlockCoord (woco);
		
		return blockAtRelativeCoord(relCo);
	}
	
	private Block blockAtRelativeCoord(Coord relCo) 
	{
		return new Block(blockTypeFromCoord(relCo));
		/*
		if (blocks[relCo.x, relCo.y, relCo.z] == null)
			blocks[relCo.x, relCo.y, relCo.z] = new Block(blockTypeFromCoord(relCo));
//			blocks[relCo.x, relCo.y, relCo.z] = new Block(BlockType.Air);
		return blocks[relCo.x, relCo.y, relCo.z];
		*/
	}
	
	private BlockType blockTypeFromCoord(Coord relCo)
	{
		List<Range1D> y_ranges = heightRangesAtCoord(relCo);
		
		// general note: a lot of work if we ever change the struct / data type that holds ranges!
		int count = 0;
		foreach(Range1D range in y_ranges)
		{
			RelationToRange relation = range.relationToRange(relCo.y);
			if (relation == RelationToRange.WithinRange)
			{
				//NOT pretending to check the 'range.blockType' thing (which actually doesn't exist right now)
				return range.blockType; //  BlockType.Grass;
			}
			
			//MUST ENSURE THAT RANGE LISTS ALWAYS GO FROM LOWEST TO HIGHEST
			
			//weird effects
			//bug when trying to destroy a block that 's part of a structure.
			//test turn off
//			if (relation == RelationToRange.AboveRange) 
//			{
//				if (count < y_ranges.Count - 1)
//				{
//					RelationToRange relationNextOne = y_ranges[count + 1].relationToRange(relCo.y);
//					if (relationNextOne == RelationToRange.BelowRange)
//					{
//						return BlockType.Air;
//					}
//				}
//			}
			++count;
		}

		return BlockType.Air;
	}

	public void setBlockAtWorldCoord(Block bb, Coord woco) 
	{
		Coord relCo = patchRelativeBlockCoordForWorldBlockCoord (woco);
		blocks [relCo.x, relCo.y, relCo.z] = bb;

		//save any set block
		addSavableBlock (bb, relCo);

		updateYSurfaceMapWithBlockAndRelCoord (bb, relCo);
		
		//SAVED RANGE LISTS
		List<Range1D> updatedRanges = heightRangesAtCoord(relCo);
		setSavedRangesAtCoord(relCo, updatedRanges);
	}

	private void debugList(List<int> list) {
		string str = "";
		foreach(int i in list) {
			str = str + ", " + i;
		}
		bug("A list: " + str);
	}
	
	private List<Range1D> heightRangesAtCoord(Coord relCo) {
		return heightMap [relCo.x * patchDimensions.x + relCo.z];
	}
	
	private void setSavedRangesAtCoord(Coord relCo, List<Range1D> ranges) {
		savedRangeLists[relCo.x * patchDimensions.x + relCo.z] = ranges;
	}
	
	private void updateYSurfaceMapWithBlockAndRelCoord(Block bb, Coord relCo) 
	{
		List<Range1D> heights = heightRangesAtCoord(relCo);

		bool isAirBlock = bb.type == BlockType.Air; 
		
		int heightsIndex = 0;
		Range1D rOD = Range1D.theErsatzNullRange();
		if (isAirBlock)
		{
			bool checkGotAContainingRange = false;
			
			for (; heightsIndex < heights.Count ; ++heightsIndex)
			{
				
				rOD = heights[heightsIndex];
				if (rOD.contains(relCo.y))
				{
					checkGotAContainingRange = true;
					if (relCo.y == rOD.start) {
						rOD.start ++;	
						rOD.range--; // corner case: range now zero: (check for this later)
						heights[heightsIndex] = rOD;
					} else if (relCo.y == rOD.extentMinusOne() ) {
						rOD.range--;
					} else {
						int newBelowRange = relCo.y - 1 + 1 - rOD.start;
						Range1D newAboveRange = new Range1D(relCo.y + 1, rOD.range - newBelowRange - 1, rOD.blockType);
						rOD.range = newBelowRange;
						heights.Insert(heightsIndex + 1, newAboveRange);
						heights[heightsIndex] = rOD;
					}
					
					if (rOD.range == 0) // no more blocks here
					{
						heights.RemoveAt(heightsIndex);	
					}
					else
					{
						// need to put back???
						heights[heightsIndex] = rOD;
					}
					
					break; 
				}
			}
			
			if (!checkGotAContainingRange)
			{
				throw new Exception("confusing: we didn't find the height range where an air block was being added");
			}
		}
		else
		{
			bool dealtWithBlock = false;
			for (; heightsIndex < heights.Count ; ++heightsIndex)
			{
				rOD = heights[heightsIndex];
				
				if (rOD.isOneAboveRange(relCo.y) )
				{
					//same type??s
					if (rOD.blockType == bb.type)
					{
						rOD.range++;
						
						//is there another range just above relCo y?
						if (heightsIndex < heights.Count - 1)
						{
							Range1D nextRangeAbove = heights[heightsIndex + 1];
							if (nextRangeAbove.isOneBelowStart(relCo.y) && nextRangeAbove.blockType == bb.type)
							{
								//combine above and below
								rOD.range += nextRangeAbove.range;
								heights.RemoveAt(heightsIndex + 1);
							}
						}
						
						heights[heightsIndex] = rOD;
						dealtWithBlock = true;
					}
					
										
					break;
					
				}
				else if (rOD.isOneBelowStart(relCo.y)) 
				{
					if (rOD.blockType == bb.type)
					{
						rOD.start--;
						rOD.range++;
						heights[heightsIndex] = rOD;
						dealtWithBlock = true;
					}
										
					break;
					// we already checked if the relCo y was one above the previous range (if it existed)
				}
				else if (rOD.extentMinusOne() < relCo.y) // more than one block above a height range (already know its not directly above)
				{
					// two cases: 
					//1. there's another range in the list: 
					//		a. relco y is one below that range (we are jumping the gun): continue
					//		b. relco y is greater than or eq. to the next range's start (we are jumping the gun): continue
					//		(else c. relco y is more than one below: see case two) 
					//2. we didn't jump the gun: whether or not there's a range above, 
					// this block has no blocks one above or below
					// add it at h index + 1
					
					if (heightsIndex < heights.Count - 1)
					{
						Range1D nextRangeAbove = heights[heightsIndex + 1];
						if (relCo.y >= nextRangeAbove.start - 1) // cases 1a and 1b
						{
							continue;
						}
					}
					
					//case 2
					Range1D rangeForRelCoY = new Range1D(relCo.y, 1, bb.type);
					heights.Insert(heightsIndex + 1, rangeForRelCoY);
					dealtWithBlock = true;
															
					break;
				}
				
				if (rOD.contains(relCo.y) )
					throw new Exception("confusing: adding a block to an already solid area??");
				
			}
			
			if (!dealtWithBlock) {
				Range1D rangeForRelCoY = new Range1D(relCo.y, 1, bb.type);
				heights.Insert(heightsIndex + 1, rangeForRelCoY);
			}
		}
		heightMap [relCo.x * patchDimensions.x + relCo.z] = heights;
	}
	

	void bug(string str) {
		UnityEngine.Debug.Log (str);
	}

	#region Surface Maps
	
	public List<Range1D>[] heightMapForChunk(Chunk chunk) 
	{
		List<List<Range1D>> retList = new List<List<Range1D>> ();
		Coord patchRel = patchRelativeBlockCoordForChunkCoord (chunk.chunkCoord);

		int startIndex = patchRel.x * patchDimensions.x + patchRel.z;
		int i = 0;
		for(; i < CHUNKLENGTH; ++i) 
		{
			int startRange = startIndex + patchDimensions.x * i;
			retList.AddRange ( heightMap.Skip(startRange).Take(CHUNKLENGTH));
		}
	
		return retList.ToArray ();
	}
	
//	public List<int>[] ySurfaceMapForChunk(Chunk chunk) 
//	{
//		List<List<int>> retList = new List<List<int>> ();
//		Coord patchRel = patchRelativeBlockCoordForChunkCoord (chunk.chunkCoord);
//
//		int startIndex = patchRel.x * patchDimensions.x + patchRel.z;
//		int i = 0;
//		for(; i < CHUNKLENGTH; ++i) 
//		{
//			int startRange = startIndex + patchDimensions.x * i;
//			retList.AddRange ( ySurfaceMap.Skip(startRange).Take(CHUNKLENGTH));
//		}
//
//		return retList.ToArray ();
//	}
	
	

	#endregion

	void resetAlreadyDoneFlags () {
//		generatedNoiseAlready = false;
		generatedBlockAlready = false;
		hasStarted = false;
	}
	
//	generateNoiseAndPopulateBlocksAsync
	
	public void populateBlocksAsync()
	{
		
//		m_chunkManager.noiseHandler.GenerateAtNoiseCoord (coord);
//		generatedNoiseAlready = true;
		this.Start ();
	}

	public void populateBlocksFromNoiseMainThread()
	{
		this.hasStarted = true;
		doPopulateBlocksFromNoise();
		this.IsDone = true;
		this.hasStarted = false;
	}

	private void doPopulateBlocksFromNoise()
	{
		if (generatedBlockAlready)
			return;

		Coord start = new Coord (0);
		Coord range = patchDimensions;
		populateBlocksFromNoise (start, range);
	}
	
	#region populate blocks
	private float valueByShiftingValueLeft(float value, int left_shift)
	{
		int sign = value < 0 ? -1 : 1;
		value = (float) (value * Mathf.Pow(10f, left_shift) );
		return (float)((value - (int) value) * sign); 
	}
	
	private int noiseAsWorldHeight (float noise_val, float elevation_multiplier, float hilliness) 
	{
		int baseElevation =(int)(elevation_multiplier * patchDimensions.y);
		int elevationRange = (int)((patchDimensions.y - baseElevation ) * .5f * hilliness);
		return (int)(noise_val * elevationRange + elevationRange + baseElevation);
	}
	
	private static bool aTreeIsHere(int xx, int zz, int surfaceHeight, float noise_val) {
		
		// TODO turn the test 90 degrees.
		if ((xx == patchDimensions.x - 1) && zz % 8 == 0) return true; //TEST
		return false;//TEST
		//********* END TEST
		
		if (surfaceHeight > patchDimensions.y - 24)
			return false;
		
		if (Utils.IntegerAtDecimalPlace(noise_val, 4) > 5)
			return false;
		
		
		int tree_interval = 12; // BUG: faceSet gets an index out of range when this number is small (smaller than 8?) //when trees overlap?
		
//		tree_interval += NoisePatch.TREE_MAX_LIKELIHOOD_VRANGE.contains(surfaceHeight) ? 0 : (int)(tree_interval * .5f); //crude
//		
		noise_val = noise_val < 0 ? -noise_val : noise_val;
		tree_interval += (int)(6 * Utils.DecimatLeftShift(noise_val, 3));
		
//		tree_interval = Mathf.Max(tree_interval, 8);
		
		int intervalx = (xx % tree_interval);
		int intervalz = (zz % tree_interval);

		if(intervalx == 0 && intervalz == 0)
			return true;
		return false;
	}
	
#if LIGHT_HACK
	private byte nextLightLevel(bool thereWasAnOverhangAtThisXZ)
	{
		currentLightLevel += thereWasAnOverhangAtThisXZ ? -.05f : .05f;
		currentLightLevel = Mathf.Max(currentLightLevel, 0f);
		currentLightLevel = Mathf.Min((float) Block.MAX_LIGHT_LEVEL, currentLightLevel);
		return (byte) currentLightLevel;
	}
	
	private void resetLightLevelCounter(int xx) { 
		NoiseCoord nudge = NeighborDirectionUtils.nudgeCoordFromNeighborDirection(NeighborDirection.ZNEG);
		if (this.m_chunkManager.blocks.noisePatchExistsAtNoiseCoord(this.coord + nudge))
		{
			NoisePatch zNegNeihbor = this.m_chunkManager.blocks.noisePatches[this.coord + nudge];
			this.currentLightLevel = zNegNeihbor.dataForNeighbors.edgeLightLevels.zMax[xx];
			
			if (this.currentLightLevel != 0)
				return;
		}
		
		
		this.currentLightLevel = this.lastXRowStartLightLevel;
	}
	
	private void setXRowStartLightLevelAtZeroZeroHack() {
		this.lastXRowStartLightLevel = (float) Block.MAX_LIGHT_LEVEL / 1.5f ; //fake hack	
	}
	
	private void influenceXRowLightLevelWithNearbyZLevel() {
		this.lastXRowStartLightLevel = (this.lastXRowStartLightLevel + this.currentLightLevel) /2.0f;	
	}
#endif
	
	//NOTE: variable names may not really reflect the functionality embodied herein :)
	private List<Range1D> heightsAndOverhangRangesWith(float noise_val, float overhangness, float caveness, BiomeInputs biomeInputs)
	{
		List<Range1D> result = new List<Range1D>();
		
		int baseElevation =(int)(biomeInputs.baseElevation * patchDimensions.y);
		int elevationRange = (int)((patchDimensions.y - baseElevation ) * .5f * biomeInputs.hilliness);
		noise_val = (noise_val < -1f) ? -1f : noise_val;
		float height_noise = noise_val * .75f + overhangness * .25f; // (noise_val * noise_val * noise_val * .75f + overhangness * overhangness * overhangness * .25f);
		
		int noiseAsWorldHeight = (int)(height_noise * elevationRange + elevationRange + baseElevation);

		Range1D zeroToSurface = new Range1D(0, noiseAsWorldHeight);

		// concavity insertion point (which may simply eat away at the height or may create a concavity)
		//  overhangeness smallness and noise_val largness
		float concavity = overhangness * overhangness - .2f + noise_val; //   (1f - (.5f +  overhangness * .5f)) * 1f;

		
		if (concavity > 0)
		{
			int elevation = noiseAsWorldHeight - baseElevation;
			int concave_range = (int) (elevation * (concavity));

			int concave_start = (int) (baseElevation + (elevation - concave_range) * .5f);
			zeroToSurface = new Range1D(0, concave_start); 
			
			if (concave_start + concave_range < noiseAsWorldHeight)
			{
				int overhang = noiseAsWorldHeight - (concave_start + concave_range);
				Range1D topRange = new Range1D(concave_start + concave_range, overhang);
				
				result.Add(topRange );
				
#if LIGHT_HACK
				thereWasAnOverhang = true;
			} else {
				thereWasAnOverhang = false;	
#endif
			}
			
//			zeroToSurface.top_light_level = nextLightLevel(thereWasAnOverhang); 
			
			
			
			result.Insert(0, zeroToSurface); //add the lower range after
			
			return result;
		}
	
		result.Add(zeroToSurface);
		return result;
	}
	
	private void populateBlocksFromNoise(Coord start, Coord range)
	{
		this.hasStarted = true;
		
		if (generatedBlockAlready)
			return;

		// put saved blocks in first...
//		updateBlocksArrayWithSavableBlocksList ();
		//...
		int x_start = (int)( start.x);
		int z_start = (int)( start.z);

		int x_end = (int)(x_start + range.x);
		int z_end = (int)(z_start + range.z);

//		Block curBlock = null;
//		Block prevYBlock = null;

		int surface_nudge = (int)(patchDimensions.y * .4f); 
		float rmf3DValue = 0.2f;
		int noiseAsWorldHeight;
		int perturbAmount;
		
		float heightScaler = 0f;
		float heightScalerSkewed = 0f;
		
		float caveIntensity;
		
#if TERRAIN_TEST
		
		Color airColor = Color.black;
		float coordColor = (float) (((int)Mathf.Abs(coord.z) % 4) / 4.0f);
		Color solidColor = new Color(coordColor + .5f, 1f - coordColor, coordColor, 1f );
		terrainSlice = new Color[range.y, range.z];
#endif
		
		BiomeInputs biomeInputs = biomeInputsAtCoord(0,0);
		
		//height map 2.0
//		int[,] heightMapStarts = new int[patchDimensions.x, patchDimensions.z];
//		int[,] heightMapEnds = new int[patchDimensions.x, patchDimensions.z];
		
#if TERRAIN_TEST
		if (TestRunner.DontRunDoTerrainTestInstead)
			x_end = x_start + 1;
#endif
		
		List<StructureBase> structurz = new List<StructureBase>();
		List<Range1D> heightRanges;
		float noise_val = 0f;
		float noise_shifted2 = 0f; float noise_shifted3 = 0f;
		Vector2 local_slope = new Vector2(0f, 0f);
		
#if LIGHT_HACK
		if (x_start == 0)
			setXRowStartLightLevelAtZeroZeroHack();
#endif
		
		int xx = x_start;
		for (; xx < x_end ; xx++ ) 
		{
#if LIGHT_HACK
			resetLightLevelCounter(xx);		
#endif
			
			int zz = z_start;
			for (; zz < z_end; zz++ ) 
			{
//				float noise_val = 0.5f; // FLAT TEST get2DNoise(xx, zz);
			
#if FLAT_TOO
				//FOR DEBUGGING
				noise_val = .4f; // get2DNoise(xx, zz);
				biomeInputs = BiomeInputs.Pasture();
#else
				noise_shifted2 = getAlt2DNoise(xx + 123, zz + 123); //  valueByShiftingValueLeft(noise_val, 1);
				noise_val = get2DNoise(xx - (int)(0 * noise_shifted2) , zz -(int)(0 * noise_shifted2));
				
				noise_shifted3 = valueByShiftingValueLeft(noise_shifted2, 1);
				
				biomeInputs =  BiomeInputs.Pasture(); // FOR TESTING // biomeInputsAtCoord(xx,zz, biomeInputs);
//				int baseElevation =(int)(biomeInputs.baseElevation * patchDimensions.y);
//				int elevationRange = (int)((patchDimensions.y - baseElevation ) * .5f * biomeInputs.hilliness);
//				noiseAsWorldHeight =  noiseAsWorldHeight(noise_val, biomeInputs.baseElevation, biomeInputs.hilliness); // (int)(noise_val * elevationRange + elevationRange + baseElevation);
//				y_end = noiseAsWorldHeight + 1;
				
#endif
				
//				heightRanges // TODO: add save blocks to heightMap as ranges. then set the corresponding range to heightranges.
				// NOTE: must deal with overlapping ranges.
				
				
				heightRanges = heightsAndOverhangRangesWith(noise_val, noise_shifted2, noise_shifted3,biomeInputs);  // new List<Range1D>();
				int highestLevel = heightRanges[heightRanges.Count - 1].extent();
				
				//DEBUGGING NPATCH BORDERS
				if (xx == 0 || zz == 0)
				{
					Range1D topRange = heightRanges[heightRanges.Count - 1];
					topRange.blockType = BlockType.Sand;
					heightRanges[heightRanges.Count - 1] = topRange;
				}
				

#if LIGHT_HACK
				byte nextLevel;
				if (heightRanges.Count > 1) // there was probably an overhang
				{
					Range1D secondHighestRange = heightRanges[heightRanges.Count - 2];
					nextLevel = nextLightLevel(true);
					secondHighestRange.top_light_level = nextLevel;
					heightRanges[heightRanges.Count - 2] = secondHighestRange;
					
					Range1D highestRange = heightRanges[heightRanges.Count - 1];
					highestRange.bottom_light_level = secondHighestRange.top_light_level;
					heightRanges[heightRanges.Count - 1] = highestRange;
					
					
					if (zz == 0)
						this.dataForNeighbors.edgeLightLevels.zZero[xx] = nextLevel;
					else if (zz == patchDimensions.z - 1)
						this.dataForNeighbors.edgeLightLevels.zMax[xx] = nextLevel;
					
				} else {
					nextLevel = nextLightLevel(false);	
					
////					if highrange not contained by last highest z
//					if (zz > 1)
//					{
//						List<Range1D> prevZs = heightMap[xx * patchDimensions.x + zz - 1];
//						if (prevZs.Count > 0)
//						{
//							Range1D prevZRange = prevZs[prevZs.Count - 1];
//							Range1D highestR = heightRanges[0];
//							if (!prevZRange.contains(highestR.extentMinusOne()))
//							{
//								highestR.top_light_level = nextLevel;
//								heightRanges[0] = highestR;
//							}
//						}
//					}
				}
				
				
				
				if (zz < 5) //backwardly influence x light level
				{
					influenceXRowLightLevelWithNearbyZLevel();	
				}
#endif				
				//NO-Y METHOD!!
				
				// TODO: (pos.) make height map a class with an accessor that imitates the array that it currently is
				// this class also know its height range and has some way of knowing it coverage in the noisePatch
				// like whether the it spans the noise patch (no drop offs/overhangs) or (if only partial coverage) somehow describes
				// the extent of partial coverage...
				
//				Range1D zero_ToSurface_range = new Range1D(0, noiseAsWorldHeight);
//				heightRanges.Add(zero_ToSurface_range);
				
//#if FLAT_TOO
//#else
				PTwo patchRelCo = new PTwo(xx, zz);
				if (aTreeIsHere(xx, zz, highestLevel, noise_val))
				{
					Tree tree = new Tree(patchRelCo, highestLevel, noise_val * 4.23f);
					structurz.Add(tree);	
					
					//save trees for neighbs where appro.
					addTreeForNeighborPatches(tree);
				}
				// add a structure on the surface maybe
				// fake test...
				if (xx == 12 && zz == 12)
				{
					Plinth pl = new Plinth(patchRelCo, highestLevel, noise_val + 5f); // silliness
					structurz.Add(pl);
//					addPlinthForNeighborPatches(pl, xx, zz, noise_val);
				}
//#endif
				
				
				heightMap[xx * patchDimensions.x + zz] = heightRanges;

				
				// NOTE: still need a  way of adding the saved blocks! (TODO:)
				
//				List<int> yHeights = new List<int> ();

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
//#if NO_CAVES
//#else
//						heightScaler = (float)yy/(float)y_end;
//						caveIntensity = caveIntensityForHeightUpperLimit(yy, baseElevation + elevationRange);
//						m_chunkManager.m_libnoiseNetHandler.Gain3D = caveIntensity * 1.3f; // TEST // 0.56f * (1.2f - heightScaler * heightScaler);
//#endif
//
//						
//#if FLAT_TOO
////						rmf3DValue = 0.2f;
//#elif NO_CAVES
////						rmf3DValue = 0.2f;
//#else
//						rmf3DValue = getRMF3DNoise(xx, yy, zz, biomeInputs.caveVerticalFrequency * (1.0f - caveIntensity * 0.05f), noise_val); 
//						perturbAmount = (int) (rmf3DValue * biomeInputs.overhangness * 20);
//#endif
////						noiseAsWorldHeight = (int)((noise_val * .5f + .5f) * patchDimensions.y * biomeInputs.hilliness);
//						
//						
//
//						if (yy < patchDimensions.y - 4) // 4 blocks of air on top
//						{ 
//							if (yy == 0) {
//								btype = BlockType.BedRock;
////							} else if ( noiseAsWorldHeight + surface_nudge + perturbAmount > yy) { // solid block
//							} else if ( noiseAsWorldHeight > yy) { // solid block
//								
//								
//
//#if NO_CAVES
//								btype = BlockType.Grass;
//#else
//								heightScalerSkewed =  (1f - caveIntensity); //  heightScaler - .5f;
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

			} // end for zz

		} // end for xx
		
		//add stucturs
		addStructuresToHeightMap(structurz); 
		//TURN ON STRUCTURES really means shared structures
#if TURN_ON_STRUCTURES
		
		
		//any structures for me from neighbors?
		getStructuresFromNeighbors(); 
		
		//are there any neighbors who generated already and need structures?
		dropOffStructuresForNeighbors(); 
#endif
		
		
//		#if TERRAIN_TEST
//		textureSlice = GetTexture ();
//		#endif

		generatedBlockAlready = true;
	}
	
	private void takeStructuresAfterIGeneratedBlocksAlready (List<StructureBase> strs)
	{
		if (this.generatedBlockAlready) 
		{
			addStructuresToHeightMap(strs);
			// get chunk man to rebuild any chunks that we're changed
			List<Coord> patchRelCoords = new List<Coord>();
			
			foreach( StructureBase str in strs) {
				patchRelCoords = patchRelativeChunkCoordsThatTouchQuad(str.plot, ref patchRelCoords);
//				foreach(Coord pRelCo in patchRelativeChunkCoordsThatTouchQuad(str.plot, ref patchRelCoords)
			}
			
			this.patchRelativeChCosToRebuild = patchRelCoords;
			
			this.m_chunkManager.rebuildChunksAtNoiseCoordPatchRelativeChunkCoords(this.coord, patchRelCoords); //test want but maybe not here (go to main thread)
		} 
		// else this noisepatch will take what it needs later so don't bother.
	}
	
	private List<Coord> patchRelativeChunkCoordsThatTouchQuad(Quad quad, ref List<Coord> alreadyAddedCoords)
	{
		int xStart = quad.origin.s / (int) ChunkManager.CHUNKLENGTH;
		int zStart = quad.origin.t / (int) ChunkManager.CHUNKLENGTH;
		
		int xEnd = quad.extent().s / (int) ChunkManager.CHUNKLENGTH;
		int zEnd = quad.extent().t / (int) ChunkManager.CHUNKLENGTH;
		
		List<Coord> result = alreadyAddedCoords; // new List<Coord>();
		
		for (int xx = xStart; xx <= xEnd; ++xx)
		{
			for (int zz = zStart; zz <= zEnd; ++zz)
			{
				bool already = false;
				Coord newCo = new Coord(xx, 0, zz);
				foreach(Coord alreadyCo in alreadyAddedCoords)
				{
					if (alreadyCo.equalTo(newCo) )
					{
						already = true;
						break;
					}
				}
				if (!already)
					result.Add(newCo);
			}
		}
		return result;
	}
	
	private void dropOffStructureForNeighborAt(NeighborDirection ndir)
	{
#if TURN_ON_STRUCTURES
#else
		return;
#endif
		NoisePatch neighborPatch = null;
		// X POS NEIGHBOR
		NoiseCoord nco = NeighborDirectionUtils.nudgeCoordFromNeighborDirection(ndir) + this.coord; // new NoiseCoord(this.coord.x + 1, this.coord.z);
		if (this.m_chunkManager.blocks.noisePatchExistsAtNoiseCoord(nco)) 
		{
			neighborPatch = this.m_chunkManager.blocks.noisePatches[nco];
			if (neighborPatch.generatedBlockAlready) {
				List<StructureBase> strs = this.giveStructuresFor(ndir);
				if (strs != null)
					neighborPatch.takeStructuresAfterIGeneratedBlocksAlready(strs);
			}
		}
	}
	
	//TODO: noisepatches only ready when their neighbors have setup (their structures) (i.e. given them there structures...)
	
	private void dropOffStructuresForNeighbors() // if the neighbor was build before we were
	{
		foreach(NeighborDirection ndir in NeighborDirectionUtils.neighborDirections()) {
			dropOffStructureForNeighborAt(ndir);	
		}
	}
	
	private void getStructuresFromNeighbors()
	{
#if TURN_ON_STRUCTURES
#else
		return;
#endif
		NoisePatch neighborPatch = null;
		List<StructureBase> strs = null;
		NoiseCoord nco = new NoiseCoord(this.coord.x - 1, this.coord.z);
		
		foreach(NeighborDirection ndir in NeighborDirectionUtils.neighborDirections()) {
			addStructuresFromNeighborInDirection(ndir);
		}
	}
	
	private void addStructuresFromNeighborInDirection(NeighborDirection directionToNeighbor)
	{
		NoiseCoord nco = this.coord + NeighborDirectionUtils.nudgeCoordFromNeighborDirection(directionToNeighbor);
		if (this.m_chunkManager.blocks.noisePatchExistsAtNoiseCoord(nco)) 
		{
			NoisePatch neighborPatch = this.m_chunkManager.blocks.noisePatches[nco];
			if (neighborPatch.IsDone)
			{
				List<StructureBase> strs = neighborPatch.giveStructuresFor(NeighborDirectionUtils.oppositeNeighborDirection(directionToNeighbor) );
				if (strs != null) {
					addStructuresToHeightMap(strs);
					strs.Clear();
				}
			}
		}
	}

	
	private List<StructureBase> giveStructuresFor(NeighborDirection nDir)
	{
		return dataForNeighbors.structures.getStructureListForDirectionOnce(nDir);
	}
	
	private void addTreeForNeighborPatches(Tree tree)
	{
		foreach(NeighborDirection neighborDir in NeighborDirectionUtils.neighborDirections())
		{
			Tree treeForNeighbor = tree.sectionOfTreeContainedByNoisePatchNeighborInDirection(neighborDir);
			if (treeForNeighbor != null)
			{
				dataForNeighbors.structures.addToListForDirection(neighborDir, treeForNeighbor);
			}
		}
	}
	
	private void addPlinthForNeighborPatches(Plinth pl, int xx, int zz, float noise_val) 
	{
		//populate structure for neighbors... 
		//i.e. if the structure's area extends beyond this patch
		// add a structure for the next patch (only deal with z and x pos dirs).
		bool zTruncated = pl.tDimensionWasTruncated();
		bool xTruncated = pl.sDimensionWasTruncated();
		
		if (zTruncated)
		{
			int zPosNeighborRelativeZCoord = zz - patchDimensions.z;
			PTwo zPosRelCo = new PTwo(xx, zPosNeighborRelativeZCoord);
			Plinth plzpos = new Plinth(zPosRelCo, pl.y_origin, noise_val + 5f);
			dataForNeighbors.structures.forZPos.Add(plzpos);
		}
		if (xTruncated)
		{
			int xPosNeighborRelativeZCoord = xx - patchDimensions.x;
			PTwo xPosRelCo = new PTwo(xPosNeighborRelativeZCoord, zz);
			Plinth plxpos = new Plinth(xPosRelCo, pl.y_origin, noise_val + 5f);
			dataForNeighbors.structures.forXPos.Add(plxpos);
		}
		if (xTruncated && zTruncated)
		{
			int xPosNeighborRelativeZCoord = xx - patchDimensions.x;
			int zPosNeighborRelativeZCoord = zz - patchDimensions.z;
			PTwo xzPosRelCo = new PTwo(xPosNeighborRelativeZCoord, zPosNeighborRelativeZCoord);
			Plinth plxzpos = new Plinth(xzPosRelCo, pl.y_origin, noise_val + 5f);
			dataForNeighbors.structures.forXZPos.Add(plxzpos);
		}
	}
	
	private void addStructuresToHeightMap(List<StructureBase> structurz) 
	{
		if (structurz == null || structurz.Count == 0)
			return;
		
		Range1D highest_at;
		StructureBase structure;
		
		PTwo origin;
		PTwo dims;
		
		List<Range1D> range_l;
		
		int i = 0;
		int j = 0;
		int k = 0;
		
		int u = 0;
		
		PTwo offset;
		PTwo lookup;
		
		Range1D str_range;
		
		for(; i < structurz.Count; ++i)
		{
			structure = structurz[i];
			origin	= structure.getOrigin();
			dims = structure.getDimensions();
			
			for(j = 0; j < dims.s; ++j)
			{
				for (k = 0; k < dims.t; ++k)
				{
					offset =  new PTwo(j, k);
					lookup = origin + offset;
					
					List<Range1D> str_ranges = structure[offset];
					range_l = heightMap[ lookup.s * patchDimensions.x + lookup.t];

					Range1D above_struck;
					
					for (u = 0; u < str_ranges.Count; ++u) 
					{
						highest_at = range_l[range_l.Count - 1];
						
						str_range = str_ranges[u];
						str_range.start +=  structure.y_origin ; 
						str_ranges[u] = str_range;
						
						// if structure range happens to contain the highest_at range:
						if (str_range.contains(highest_at))
						{
							// and then what about ranges below that? TODO:
//							range_l.RemoveAt(range_l.Count - 1);
//							range_l.Add(str_range); //NO. the terrain -- or previous structures win this one. (commenting out)
							continue;
						}
						
						above_struck = highest_at.subRangeAbove(str_range.extentMinusOne());

						//adjust height map (truncated by structures)
						Range1D below_structure = highest_at.subRangeBelow(str_range.start);
						
						if (!below_structure.isErsatzNull() )
						{
							//add it as the new 'highest at' (now a misnomer potentially)
							if (below_structure.range > 0) {
								range_l[range_l.Count - 1] = below_structure;
							} else
								throw new Exception("below struct range was not ersatz null but was range < 1 ?? the range: " + below_structure.toString());
						} else {
							range_l.RemoveAt(range_l.Count - 1); //happens when the have the same start...	
						}
						
						if (!str_range.isErsatzNull() && str_range.range > 0)
							range_l.Add(str_range); 
						else
							throw new Exception("a structure range was ersatz null or range < 1 ?? the range: " + str_range.toString());

						highest_at = above_struck;
						
						if (u == str_ranges.Count - 1) // last one
						{
							if (!above_struck.isErsatzNull() && above_struck.range > 0) {
								range_l.Add(above_struck);	
							} else if (above_struck.range == 0)
								throw new Exception("above struck range range less than 1: " + above_struck.toString());
						}
						
					}
					
					heightMap[ lookup.s * patchDimensions.x + lookup.t] = range_l;
				}
			}
		}
	}

	private static bool caveIsHere(float cave_noise_val,int yy, int surfaceNudge) {
		float yLevel = yy - surfaceNudge;
//		float filter_value =   (float)((yy) /(float) patchDimensions.y); 
		float filter_value =   (float)((yLevel) /(float) surfaceNudge * 2.0f); 
//		filter_value *= filter_value;
		return cave_noise_val > filter_value;
	}
	
	private static float caveIntensityForHeight(int yy) { // scale between 0 and 1
		// caves are largest and most likely to exist at patchDim.y * .25 <--we just decided.	
		int ydif = yy - CAVE_MAX_LIKELIHOOD_LEVEL;
		ydif = ydif < 0 ? ydif * -1 : ydif;
		int yrange = patchDimensions.y - CAVE_MAX_LIKELIHOOD_LEVEL;
		
		ydif = yrange - ydif;
		
//		return (float)( (ydif) /(float)(yrange));
		return (float)( (ydif * ydif) /(float)(yrange * yrange));
	}
	
	private static float caveIntensityForHeightUpperLimit(int yy, int upperLimitLevel) { // scale between 0 and 1
		// caves are largest and most likely to exist at patchDim.y * .25 <--we just decided.	
		yy = Mathf.Min(yy, upperLimitLevel);
		int ydif = yy - CAVE_MAX_LIKELIHOOD_LEVEL;
//		bool ydif_below = ydif < (int)( -CAVE_MAX_LIKELIHOOD_LEVEL * 0f);
		ydif = ydif < 0 ? ydif * -1 : ydif;
		int yrange = upperLimitLevel - CAVE_MAX_LIKELIHOOD_LEVEL;
		
//		ydif = yrange - ydif;
		
//		if (ydif_below)
//			return (float)(1f - (ydif * ydif *ydif) /(float)(yrange * yrange * yrange));
		return (float)(1f - (ydif * ydif) /(float)(yrange * yrange));
	}

	private float getSimplexNoise(int xx, int yy, int zz) {
		return SimplexNoise.Noise.Generate ((float)((patchDimensions.x * coord.x + xx) / (float)patchDimensions.x), (float)yy / (float)patchDimensions.y, (float)((patchDimensions.z * coord.z + zz)/(float)patchDimensions.z)); 
	}

	private float getRMF3DNoise(int xx, int yy, int zz, float caveVerticalFrequency, float turbulence) {
		return m_chunkManager.m_libnoiseNetHandler.GetRidgedMultiFractalValue (
			(float)((patchDimensions.x * coord.x + xx) / (float)patchDimensions.x) + turbulence, 
			caveVerticalFrequency * (float)yy / (float)patchDimensions.y + turbulence, 
			(float)((patchDimensions.z * coord.z + zz)/(float)patchDimensions.z) + turbulence); 
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
	
	private float getAlt2DNoise(int xx, int zz) {
		return m_chunkManager.m_libnoiseNetHandler.GetAlt2DValue((float)((patchDimensions.x * coord.x + xx) * NOISESCALE2D / (float)patchDimensions.x), 	
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
		
		biomeNoiseValue += .5f;
		
		return BiomeInputs.Lerp(BiomeInputs.Pasture(), BiomeInputs.CraggyMountains(), biomeNoiseValue);
	}
	
	#endregion


}

public struct DataForNeighbors
{
	public StructuresForNeighbors structures;
	public EdgeLightLevels edgeLightLevels;
	
	public DataForNeighbors(StructuresForNeighbors _structuresForNeighbors, EdgeLightLevels ell) {
		this.structures = _structuresForNeighbors;	
		this.edgeLightLevels = ell;
	}
	
	public static DataForNeighbors MakeNew() {
		return new DataForNeighbors(StructuresForNeighbors.MakeNew(), EdgeLightLevels.MakeNew());	
	}
}

public struct EdgeLightLevels
{
	public byte[] zZero;
	public byte[] zMax;
	
	public static EdgeLightLevels MakeNew() {
		EdgeLightLevels edgeLL = new EdgeLightLevels();
		edgeLL.zZero = new byte[NoisePatch.patchDimensions.x];
		edgeLL.zMax = new byte[NoisePatch.patchDimensions.x];
		return edgeLL;
	}
}

public struct NeighborBooleans
{
	public bool xpos, zpos, xzpos, xneg, zneg, xzneg;
	
	public bool booleanForDirection(NeighborDirection ndir) {
		if (ndir == NeighborDirection.XPOS)
			return xpos;
		if (ndir == NeighborDirection.ZPOS)
			return zpos;
		if (ndir == NeighborDirection.XZPOS)
			return xzpos;
		if (ndir == NeighborDirection.XNEG)
			return xneg;
		if (ndir == NeighborDirection.ZNEG)
			return zneg;
		return xzneg;
	}
	
	public void setBooleanForDirection(NeighborDirection ndir, bool _boo) {
		if (ndir == NeighborDirection.XPOS)
			 xpos = _boo;
		else if (ndir == NeighborDirection.ZPOS)
			zpos = _boo;
		else if (ndir == NeighborDirection.XZPOS)
			xzpos = _boo;
		else if (ndir == NeighborDirection.XNEG)
			xneg = _boo;
		else if (ndir == NeighborDirection.ZNEG)
			zneg = _boo;
		else 
			xzneg = _boo;
	}
}

public struct StructuresForNeighbors
{
//	List<StructureBase> forXNeg;
//	List<StructureBase> forZNeg;
	public List<StructureBase> forXPos;
	public List<StructureBase> forZPos;
	public List<StructureBase> forXZPos;
	
	public List<StructureBase> forXNeg;
	public List<StructureBase> forZNeg;
	public List<StructureBase> forXZNeg;
	
	public NeighborBooleans givenAlready;
	
	public static StructuresForNeighbors MakeNew() 
	{
		StructuresForNeighbors sfn = new StructuresForNeighbors();

		sfn.forXPos = new List<StructureBase>();
		sfn.forZPos = new List<StructureBase>();
		sfn.forXZPos = new List<StructureBase>();
		
		sfn.forXNeg = new List<StructureBase>();
		sfn.forZNeg = new List<StructureBase>();
		sfn.forXZNeg = new List<StructureBase>();
		
		sfn.givenAlready = new NeighborBooleans();
		
		return sfn;
	}
	
	public void addToListForDirection(NeighborDirection ndir, StructureBase structure) {
		List<StructureBase> structureList = structureListForDirection(ndir);
		structureList.Add(structure);
	}
	
	public List<StructureBase> getStructureListForDirectionOnce(NeighborDirection ndir) {
		
		if (givenAlready.booleanForDirection(ndir))
		{
			return null;
		} else {
			givenAlready.setBooleanForDirection(ndir, true);	
		}
		
		if (ndir == NeighborDirection.XPOS)
			return forXPos;
		if (ndir == NeighborDirection.ZPOS)
			return forZPos;
		if (ndir == NeighborDirection.XZPOS)
			return forXZPos;
		if (ndir == NeighborDirection.XNEG)
			return forXNeg;
		if (ndir == NeighborDirection.ZNEG)
			return forZNeg;
//		if (ndir == NeighborDirection.XZNEG)
		return forXZNeg;
	}
	
	public List<StructureBase> structureListForDirection(NeighborDirection ndir) {
		
		if (ndir == NeighborDirection.XPOS)
			return forXPos;
		if (ndir == NeighborDirection.ZPOS)
			return forZPos;
		if (ndir == NeighborDirection.XZPOS)
			return forXZPos;
		if (ndir == NeighborDirection.XNEG)
			return forXNeg;
		if (ndir == NeighborDirection.ZNEG)
			return forZNeg;
//		if (ndir == NeighborDirection.XZNEG)
		return forXZNeg;
	}
}

public enum NeighborDirection {
	XPOS, ZPOS, XZPOS,
	XNEG, ZNEG, XZNEG
};

public static class NoisePatchUtils
{
	public static IEnumerable chunkCoordsWithinNoiseCoord(NoiseCoord nco) 
	{
		Coord originCoord = chunkCoordOfNoiseCoord(nco);
		
		for (int x = 0; x < NoisePatch.PATCHDIMENSIONSCHUNKS.x; ++x)
		{
			for (int z = 0; z < NoisePatch.PATCHDIMENSIONSCHUNKS.z; ++z)
			{
				yield return originCoord + new Coord(x , 0, z);
			}
		}
	}
	
	public static Coord chunkCoordOfNoiseCoord(NoiseCoord nco) {
		return new Coord(nco.x * NoisePatch.PATCHDIMENSIONSCHUNKS.x, 0, nco.z * NoisePatch.PATCHDIMENSIONSCHUNKS.z);	
	}
}

public static class NeighborDirectionUtils
{
		
	public static NoiseCoord nudgeCoordFromNeighborDirection(NeighborDirection neighborDir) 
	{
		if (neighborDir	== NeighborDirection.XNEG)
			return new NoiseCoord(-1, 0);
		if (neighborDir	== NeighborDirection.XPOS)
			return new NoiseCoord(1, 0);
		
		if (neighborDir	== NeighborDirection.ZNEG)
			return new NoiseCoord(0, -1);
		if (neighborDir	== NeighborDirection.ZPOS)
			return new NoiseCoord(0, 1);
		
		if (neighborDir	== NeighborDirection.XZNEG)
			return new NoiseCoord(-1, -1);
		
		//XZPOS
		return new NoiseCoord(1,1); 
	}
	
	public static IEnumerable neighborDirections() {
		yield return NeighborDirection.XPOS;
		yield return NeighborDirection.ZPOS;
		yield return NeighborDirection.XZPOS;
		yield return NeighborDirection.XNEG;
		yield return NeighborDirection.ZNEG;
		yield return NeighborDirection.XZNEG;
	}
	
	public static PTwo neighborPatchRelativeCoordForNeighborInDirection(NeighborDirection neighborDir, PTwo currentPatchRelCoord) {
		NoiseCoord nudgeCo = nudgeCoordFromNeighborDirection( neighborDir);
		PTwo nudge = new PTwo(nudgeCo.x, nudgeCo.z) * PTwo.PTwoXZFromCoord(NoisePatch.patchDimensions) * -1;
		
		return currentPatchRelCoord + nudge;
	}
	
	public static NeighborDirection oppositeNeighborDirection(NeighborDirection neighborDir) 
	{
		return (NeighborDirection) (((int)neighborDir + 3 ) % 6);
	}
}



//	#region get texture
//
//	public Color[,] GetTexture()
//	{
//		return this.GetTexture(LibNoise.Unity.Gradient.Grayscale);
//	}
//
//	/// <summary>
//	/// Creates a texture map for the current content of the noise map.
//	/// </summary>
//	/// <param name="device">The graphics device to use.</param>
//	/// <param name="gradient">The gradient to color the texture map with.</param>
//	/// <returns>The created texture map.</returns>
//	public Color[,] GetTexture(LibNoise.Unity.Gradient gradient)
//	{
//		return this.GetTexture(ref gradient, patchDimensions.z, patchDimensions.y);
//	}
//
//	public Color[,] GetTexture(ref LibNoise.Unity.Gradient gradient, int texWidth, int texHeight)
//	{
////		Texture2D result = new Texture2D(texWidth, texHeight);
////		Color[] data = new Color[texWidth * texHeight];
//		Color[,] data = new Color[texHeight ,texWidth];
//		int id = 0;
//
//		int texFour = texHeight / 4;
//
//		for (int y = 0; y < texFour * 4 ; y++)
//		{
//			for (int z = 0; z < texWidth; z++, id++)
//			{
////		for (int z = 0; z < texWidth; z++)
////		{
////			for (int y = 0; y < texFour * 4 ; y++, id++)
////			{
//				Block b = blocks [0, y, z];
//
////				float d =  
////					y % 4 == 0 ? 1f : (float)(x / texWidth);  // (float)(y / texHeight); // .75f; // 0.0f;
////				if (!float.IsNaN(this.m_borderValue) && (x == 0 || x == this.m_width - 1 || y == 0 || y == this.m_height - 1))
////				{
////					d = this.m_borderValue;
////				}
////				else
////				{
////					d = this.m_data[x, y];
////				}
//				data [y,z] = b.type == BlockType.Air ? Color.black : Color.white;  // new Color (d, d, d, 1f);// gradient[d];
//			}
//		}
//
//		return data;
//		//result.SetData<Color>(data);
//		//Debug.Log("Setting pixels");
////		result.SetPixels(data);
////		return result;
//	}
//
//	#endregion
//
//	private static int shiftCoordBy(int coordMaxValue, float scale) {
//		return (int)(coordMaxValue * scale);
//	}