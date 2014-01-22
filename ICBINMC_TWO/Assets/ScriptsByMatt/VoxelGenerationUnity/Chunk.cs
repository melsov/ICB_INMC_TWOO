#define FACE_AG
#define FACE_AG_XZ
#define LIGHT_BY_RANGE
//#define MESH_BUILDER_BUILDS_MESH
//#define NO_MESHBUILDER
//#define ONLY_HIGHEST_Y_FACES
//#define NO_XZ
//#define ONLY_Y_FACES
//#define TESTRENDER

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/*
 *  Chunk class builds meshes for block faces
 *  
 *  Rule made up for constructing vertices/winding triangles, etc.:
 *  If a person is facing the xnegative direction (i.e. further away= more negative x) and looking at a face (a square plane of unit length 1)
 *  they would see x pos side cube faces where the vertice names look like this:
 * 
 *  1------2
 *  |      |
 *  |      |
 *  0------3 
 * 
 *  Ditto for a person looking in the y and z neg directions at y and z pos cube faces.
 */
 

// dvektor == "direction vector"
// duplicates functionality of 'Coord' and vis-versa...
using System;


// TODO:
// It would be handy to keep the Lists of verts/uvs/triangles
// and separate out the construction of the lists from the 
// application of the mesh.--actually we already to that don't we!

// like with noisepatch, we could use some flags
// also, it would be great to not do the whole array
// all at once. so turn the 'make mesh' into an iEnumerator. 
// (We can call enumerators within enumerators, right?)

// this way, build up a large number of chunks that are already built,
// but not nec. applied--good.

// OR (ONE TIME!) TRY TO MAKE CHUNKS THREADED JOBS... (why not?)

// TODO: make a class that acts like a coord dictionary
// but really uses a list and a 2D array--like we use in FaceAgg.

public struct FaceInfo
{
	
	public byte lightLevel;
	public Coord coord;
	public Direction direction;
	public BlockType blockType;
	
	public FaceInfo(Coord _coord, byte _lightlev, Direction _dir, BlockType block_type) {
		coord = _coord; lightLevel = _lightlev;	direction = _dir; blockType = block_type;
	}
}

public class Chunk : ThreadedJob
{
	public static int CHUNKLENGTH = (int) ChunkManager.CHUNKLENGTH; //duplicate of chunkManager's chunklength...
	public static int CHUNKHEIGHT = (int)ChunkManager.CHUNKHEIGHT;

	public static Coord DIMENSIONSINBLOCKS = new Coord (CHUNKLENGTH, CHUNKHEIGHT, CHUNKLENGTH);

	public ChunkManager m_chunkManager;
	public Coord chunkCoord;

	public NoisePatch m_noisePatch; // replace chunk manager?


	List<Vector3> vertices_list; // = new List<Vector3> ();
	List<int> triangles_list; // = new List<int> ();
	List <Vector2> uvcoords_list; // = new List<Vector2> ();
//	List<Vector2> colors_list = new List<Vector2>();
	List<Color32> col32s_list = new List<Color32>();

	public bool noNeedToRenderFlag;
	public bool isActive;
	public bool calculatedMeshAlready=false;

	public GameObject meshHoldingGameObject;

	private int random_new_chunk_color_int_test;

//	private static int[] m_meshGenPhaseOneDirections = new int[] {}; // {1, 4, 5}; // {0, 1, 4, 5}; // (Direction enum)
	
#if NO_MESHBUILDER
	private FaceAggregator[] faceAggregators = new FaceAggregator[CHUNKHEIGHT];
#else
	private MeshBuilder meshBuilder;
#endif
	public const float VERTEXSCALE = 1f;
	
	public const int TEXTURE_ATLAS_TILES_PER_DIM = 4;
	
	private Mesh builtMesh;
	
	public Chunk()
	{
		vertices_list = new List<Vector3> ();
		triangles_list = new List<int> ();
		uvcoords_list = new List<Vector2> ();
		
		meshBuilder = new MeshBuilder(this);
	}

	protected override void ThreadFunction()
	{
		if (!calculatedMeshAlready)
			makeMeshAltThread (CHUNKLENGTH, CHUNKHEIGHT);
//			meshHoldingGameObject.GetComponent<MonoBehaviour> ().StartCoroutine (makeMeshCoro ());
//			MonoBehaviour monbeha = meshHoldingGameObject.GetComponent<MonoBehaviour> ();
//			monbeha.StartCoroutine (makeMeshCoro ()); //mesh holding GO is a unity object...
	}
	

	
	protected override void OnFinished()
	{
//		if (calculatedMeshAlready)
//			applyMesh ();
	}
	
	public void editBlockAtCoord(Coord relCo, BlockType btype) 
	{
		MeshSet mset;
		if (btype == BlockType.Air)
		{
			mset = meshBuilder.newMeshSetByRemovingBlockAtCoord(relCo);
		}
		else 
		{
			mset = meshBuilder.newMeshSetByAddingBlockAtCoord(relCo, btype);
		}
		
		//TODO: now that mesh builder actually builds the mesh anyway
		// get rid of some of this spaghetti-ness.
		
		applyMeshToGameObjectWithMeshSet(mset);
		clearMeshLists(); // why not now...
	}

	Block nextBlock(Direction d, ChunkIndex ci)
	{
		return nextBlock (d, ci, false);
	}

	Block nextBlock(Direction d, ChunkIndex ci, bool notOnlyWithinThisChunk)
	{
//		Block retBlock = null;

		Coord offset = new Coord (ci.x, ci.y, ci.z);

		switch (d) 
		{
		case(Direction.xpos):
			if ( notOnlyWithinThisChunk || ci.x < CHUNKLENGTH - 1)
				offset = new Coord (ci.x + 1, ci.y, ci.z);

			break;
		case(Direction.xneg):
			if (notOnlyWithinThisChunk ||ci.x >  0)
				offset = new Coord (ci.x - 1, ci.y, ci.z);

			break;
		case(Direction.ypos): //Y IS ALWAYS WITHIN NOW!!!
			if ( ci.y < CHUNKHEIGHT - 1)
				offset = new Coord (ci.x , ci.y + 1, ci.z);

			break;
		case(Direction.yneg):
			if ( ci.y  >  0) // NO MORE || NOTONLY
				offset = new Coord (ci.x , ci.y - 1, ci.z);

			break;
		case(Direction.zpos):
			if (notOnlyWithinThisChunk || ci.z  < CHUNKLENGTH - 1)
				offset = new Coord (ci.x , ci.y , ci.z + 1);

			break;
		default: // zneg
			if (notOnlyWithinThisChunk || ci.z >  0)
				offset = new Coord (ci.x , ci.y , ci.z - 1);

			break;
		}

		return m_noisePatch.blockAtChunkCoordOffset (chunkCoord, offset);

//		return m_chunkManager.blockAtChunkCoordOffset (chunkCoord, offset);
//		return retBlock;
	}
	
	Vector3[] faceMesh(Direction d, ChunkIndex ci) // Vector3[] verts, int[] triangles)
	{
		return faceMesh(d, ci, 0f);
	}
	
	Vector3[] faceMesh(Direction d, ChunkIndex ci, float extra_height) // Vector3[] verts, int[] triangles)
	{
		float x0, x1, x2, x3, y0, y1, y2, y3, z0, z1, z2, z3;

		int dir = (int)d;

		float halfunit = .5f;

		bool negDir = dir % 2 == 1;

		float shift =  halfunit;
		if (negDir)
			shift = -halfunit;

//		Vector3 f = new Vector3 (0, 0, 0);
//		Vector3[] ff = new Vector3[] { f, f, f, f, }; //fake

		if (d <= Direction.xneg) 
		{
			//return ff;
			x0 = x1 = x2 = x3 = ci.x + shift;
			y1 = y2 = ci.y + halfunit + extra_height; 
			y0 = y3 = ci.y - halfunit;
			z0 = z1 = ci.z - halfunit;
			z2 = z3 = ci.z + halfunit;
		} 
		else if (d <= Direction.yneg) 
		{

			y0 = y1 = y2 = y3 = ci.y + shift;

			x0 = x3 = ci.x + halfunit;
			x1 = x2 = ci.x - halfunit; 
			z0 = z1 = ci.z - halfunit;
			z2 = z3 = ci.z + halfunit;
		} 
		else 
		{
			//return ff;
			z0 = z1 = z2 = z3 = ci.z + shift;
			x0 = x1 = ci.x + halfunit;
			x2 = x3 = ci.x - halfunit;
			y1 = y2 = ci.y + halfunit + extra_height;
			y0 = y3 = ci.y - halfunit;

		}
//		Vector3 v0 = new Vector3 (x0, y0, z0) * VERTEXSCALE;
//		Vector3 v1 = new Vector3 (x1, y1, z1) * VERTEXSCALE;
//		Vector3 v2 = new Vector3 (x2, y2, z2) * VERTEXSCALE;
//		Vector3 v3 = new Vector3 (x3, y3, z3) * VERTEXSCALE;

		return new Vector3[] { 
				new	Vector3 (x0, y0, z0) * VERTEXSCALE,
				new Vector3 (x1, y1, z1) * VERTEXSCALE,
				new Vector3 (x2, y2, z2) * VERTEXSCALE,
				new Vector3 (x3, y3, z3) * VERTEXSCALE
		};
//			v0, v1, v2, v3 };
	}
	
	//face aggregator helper
	private static Vector2 uvOriginForBlockType( BlockType btype, Direction dir)
	{
		float tile_length = 1.0f; ///(float) TEXTURE_ATLAS_TILES_PER_DIM;
		
		//want
		float cX = 0.0f;
		float cY = 0.0f;

		switch (btype) {
		case BlockType.Stone:
			cY = tile_length;
			cX = tile_length;
			break;
		case BlockType.Sand:
			cX = tile_length;
//			cY = tile_length; //<-- silly // * 2;
			break;
		case BlockType.Dirt:
//			cX = tile_length * 3;
			cY = tile_length;
			break;
		case BlockType.BedRock:
			cX = tile_length * 3;
//			cY = tile_length * 2; //silly value at the moment
			break;
		case BlockType.Stucco:
			cX = tile_length * 1;
			cY = tile_length * 3;
			break;
		case BlockType.TreeTrunk:
			if(dir == Direction.yneg || dir == Direction.ypos)
			{
				cX = tile_length * 2;		
			}
			cY = tile_length * 2;
			
			break;
		case BlockType.TreeLeaves:
			cX = tile_length;
			cY = tile_length * 2;
			break;
		default: //GRASS
			//testing...make geom more visible
//			if (dir == Direction.xpos) 
//			{
//				cX = tile_length;
//			} else if (dir == Direction.xneg) {
//				cX = tile_length; cY = tile_length;
//			} else if (dir == Direction.yneg) {
//				cX = 2 * tile_length; cY = tile_length;
//			} else if (dir == Direction.zneg) {
//				cX = 3 * tile_length; cY = 3 * tile_length;
//			} else if (dir == Direction.zpos) {
//				cY = 3 * tile_length;
//			}
			//end testing
			if ( dir != Direction.yneg)
			{
				cY = tile_length; 
			}
			break;
		}

		return new Vector2(cX, cY);
	}
	
	
	private static float uvIndexForUVOrigin(Vector2 or)
	{
		return (float)((or.y * Chunk.TEXTURE_ATLAS_TILES_PER_DIM + or.x)/
			(Chunk.TEXTURE_ATLAS_TILES_PER_DIM * Chunk.TEXTURE_ATLAS_TILES_PER_DIM));
	}
	
	public static float uvIndexForBlockTypeDirection(BlockType type, Direction dir) 
	{
		Vector2 or = uvOriginForBlockType(type, dir);
		return uvIndexForUVOrigin(or);
	}
	
	public static Vector2[] uvCoordsForBlockType( BlockType btype, Direction dir)
	{
		// new way of life: UV ORIGIN IS HELD IN THE FIRST COMPONENT. AS AN INDEX
		float uv_index = uvIndexForBlockTypeDirection(btype, dir);
		Vector2 mono_v = new Vector2(uv_index, 0f);
		return new Vector2[] {
			mono_v, mono_v, mono_v, mono_v 
		};
		
//		//TODO DONE: replace this duplicate code with the texAtlasOrigin func. and then return vector
//		float tile_length = 0.25f;
////		int tiles_per_row = (int)(1.0f / tile_length);
////		int cXBasedOnXCoord = (chunkCoord.x + random_new_chunk_color_int_test) % tiles_per_row;
////		int cYBasedOnZCoord = chunkCoord.z % tiles_per_row;
//
////test...
////		float cX = cXBasedOnXCoord * tile_length;
////		float cY = cYBasedOnZCoord * tile_length;
//
//		//want
//		float cX = 0.0f;
//		float cY = 0.0f;
//
//		switch (btype) {
//		case BlockType.Stone:
//			cY = tile_length;
//			cX = tile_length;
//			break;
//		case BlockType.Sand:
//			cX = tile_length * 2;
//			cY = tile_length; //<-- silly // * 2;
//			break;
//		case BlockType.Dirt:
//			cY = tile_length * 3;
//			break;
//		case BlockType.BedRock:
//			cY = tile_length * 2; //silly value at the moment
//			break;
//		default: //GRASS
//			//testing...make geom more visible
//			if (dir == Direction.xpos) 
//			{
//				cX = tile_length;
//			} else if (dir == Direction.xneg) {
//				cX = tile_length; cY = tile_length;
//			} else if (dir == Direction.yneg) {
//				cX = 2 * tile_length; cY = tile_length;
//			} else if (dir == Direction.zneg) {
//				cX = 3 * tile_length; cY = 3 * tile_length;
//			} else if (dir == Direction.zpos) {
//				cY = 3 * tile_length;
//			}
//			//end testing
////			if (dir != Direction.ypos)
////				cY = tile_length;
//			break;
//		}
//
//		return new Vector2[] { 
//			new Vector2 (cX, cY), 
//			new Vector2 (cX, cY + tile_length), 
//			new Vector2 (cX + tile_length , cY + tile_length), 
//			new Vector2 (cX + tile_length, cY) 
//		};
	}

	//TEST FUNC...
	Vector2[] uvCoordsForTestBlock(bool forSomethingNull, int dir)
	{
		float tile_length = 0.25f;
		int tiles_per_row = (int)(1.0f / tile_length);



		float cX = 3.0f * tile_length;
		float cY = tile_length;

		if (forSomethingNull) {
			int shift = dir <= (int) Direction.xneg ? (int)dir : (int) (dir - 2);
			shift = shift % tiles_per_row;

			cX = tile_length * shift;
			cY = tile_length * 2f;
		}


		return new Vector2[] { 
			new Vector2 (cX, cY), 
			new Vector2 (cX, cY + tile_length), 
			new Vector2 (cX + tile_length , cY + tile_length), 
			new Vector2 (cX + tile_length, cY) 
		};
	} //TEST FUNC.

	public Block blockAt(ChunkIndex ci) {
		Coord offset = new Coord (ci.x, ci.y, ci.z);
		return m_chunkManager.blockAtChunkCoordOffset (chunkCoord, offset);
//		return blocks [ci.x, ci.y, ci.z];
	}

	public Block blockAdjacentToNeighboringChunkCoord( ChunkIndex neighborCI, dvektor dir ) // OR maybe - after all this.
	{
		ChunkIndex adjIndex;
		ChunkIndex unitCI = new ChunkIndex ( dvektor.Abs(dir));
		dvektor inverseDvek = dvektor.Inverse (dvektor.Abs(dir));

		ChunkIndex inverseCI = new ChunkIndex (inverseDvek);

		adjIndex = inverseCI * neighborCI; // zero out direction that we want

		int dir_coord = dir.total () == -1 ? CHUNKLENGTH : 0; // if dir pos, this chunk is pointing to us from negative and vis-versa

		ChunkIndex adjSide = new ChunkIndex (dir_coord, dir_coord, dir_coord) * unitCI;

		adjIndex = adjIndex + adjSide;

		return blockAt(adjIndex);
	}

	public void makeMesh()
	{
		makeMeshAltThread (CHUNKLENGTH, CHUNKHEIGHT);
	}

	public void makeMeshAltThread(int CHLEN, int CHHeight)
	{
		calculatedMeshAlready = false;

		int triangles_index = 0;

		// y Face approach
		addYFaces (CHLEN, triangles_index); //want

		// ** want
		if (!noNeedToRenderFlag) // not all air (but are we all solid and solidly surrounded?)
		{
			noNeedToRenderFlag = (vertices_list.Count == 0);

		}


		calculatedMeshAlready = true;
	}
	
#if NO_MESHBUILDER
	private FaceAggregator faceAggregatorAt(int index) 
	{
		if(	faceAggregators[index] == null) {
			faceAggregators[index] = new FaceAggregator( );
		}
		return faceAggregators[index];
	}
#endif
//	private void setFaceAggrefatorsAt(FaceAggregator fa, int index) {
////		index = (index * 2) + (Chunk.IsPosDir(dir) ? 1 : 0 );
////		faceAggregators[(index * 2) + (Chunk.IsPosDir(dir) ? 1 : 0 )] = fa;
//		faceAggregators[index] = fa;
//	}
	
	private static bool IsPosDir(Direction dir) {
		return (int) dir % 2 == 0;	
	}
	
	private void addRangeToFaceAggregatorAtXZ(List<Range1D> ranges, BlockType type, Direction dir, int x, int z)
	{
		//TODO: make 'addRange func.s in FaceSet and FAgg
		//TODO: make sure that all ranges are of one block type.
		FaceInfo finfo = new FaceInfo(Coord.coordZero() , 3, dir, type);

		foreach(Range1D range in ranges)
		{
			int j = range.start;
			int end = range.extent();
			while(j < end)
			{
				finfo.coord = new Coord(x, j, z); 
				finfo.lightLevel = (byte) Mathf.Lerp(range.bottom_light_level, range.top_light_level, (j - range.start) / range.range);

//				addCoordToFaceAggregorAtIndex(new Coord	(x, j, z), type, dir);
				addCoordToFaceAggregorAtIndex(finfo);
				++j;
			}
		}
	}
	
//	private void addCoordToFaceAggregorAtIndex(Coord co, BlockType type, Direction dir) 
	private void addCoordToFaceAggregorAtIndex(FaceInfo _faceInfo) 
	{
#if NO_MESHBUILDER
		FaceAggregator fa = faceAggregatorAt(co.y);
		fa.addFaceAtCoordBlockType(co, type, dir );
		faceAggregators[co.y] = fa; // put back...
#else
//		meshBuilder.addCoordToFaceAggregatorAtIndex(co, type, dir);
		meshBuilder.addCoordToFaceAggregatorAtIndex(_faceInfo);
#endif

	}
	
	private void resetFaceAggregatorsArray() {
#if NO_MESHBUILDER
		faceAggregators = new FaceAggregator[ CHUNKHEIGHT];	
#else
		meshBuilder.resetFaceAggregators();
#endif
	}
	
	private void addYFaces(int CHLEN, int starting_tri_index)  // starting tri index might have to be an 'out?'
	{
//		List<int>[] ySurfaceMap = m_noisePatch.ySurfaceMapForChunk (this);
		List<Range1D>[] ySurfaceMap = m_noisePatch.heightMapForChunk (this);
		
		List<Range1D> heights;
		
		resetFaceAggregatorsArray(); // TODO: face aggregators still report trying to add new facesets where there already was one...

		int xx = 0;
		for (; xx < CHLEN; ++xx) 
		{
			int zz = 0;
			for (; zz < CHLEN; ++zz) 
			{
				heights = ySurfaceMap [xx * CHLEN + zz];
				
				if (heights == null)
				{
					throw new Exception("in add y faces heights was Null");	
				}
				
				if (heights != null) 
				{
					
#if ONLY_HIGHEST_Y_FACES
					int j = heights.Count - 1;	
					int jend = heights.Count; // 1;
#else
					int j = heights.Count - 1;
					int jend = 0;
#endif
					
					Range1D h_range;

					for (; j >= jend ; --j)
					{
						
						h_range = heights[j];
						#if TESTRENDER
						if (height_index == heights.Count - 1)
							break;
						#endif
						
						//COMBINE GEOM (MORE THOUGHTS)
						// every level has a geometry aggregator
						// might be better to aggregate in the noise patch?
						// then we'd have to edit when we broke blocks?
						
						// one way or another...
						// let's say we figure out how to render non-rectangular shapes
						// could do this:
						// the aggregator master collects sets of adjacent-same-type faces
						// given an xz coord, go back by x - 1 and check
						// if there's a adjacent set there.
						// how do we do the look up?
						// a two-d array of ints [chlen, chlen] in size
						// the int (minus one) == the index in an aggregators list of the proper face set.
						// if there's already a face set at that next door index, add the current coord to it
						// else make a new one there...
						
						ChunkIndex targetBlockIndex;
						Block b;
						// deal with the start heights
						
#if ONLY_HIGHEST_Y_FACES
						bool no_lower_faces = false;
#else
						bool no_lower_faces = h_range.start != 0;
#endif
						if (no_lower_faces) //don't draw below bedrock
						{
							Coord blockCoord = new Coord (xx, h_range.start, zz);
							targetBlockIndex = new ChunkIndex(blockCoord);
							b = m_noisePatch.blockAtChunkCoordOffset (chunkCoord, blockCoord);
							if (b != null) 
							{
#if FACE_AG
//								addCoordToFaceAggregorAtIndex(blockCoord, b.type, Direction.ypos);								
								addCoordToFaceAggregorAtIndex(new FaceInfo(blockCoord, h_range.bottom_light_level, Direction.ypos, b.type));								
#else
								addYFaceAtChunkIndex(targetBlockIndex, b.type, Direction.ypos, starting_tri_index);
								starting_tri_index += 4;
#endif
							}
						}
						
						Coord extentBlockCoord = new Coord(xx, h_range.extentMinusOne() , zz);
						targetBlockIndex = new ChunkIndex (extentBlockCoord) ; //(xx, h_range.extentMinusOne() , zz);	
						b = m_noisePatch.blockAtChunkCoordOffset (chunkCoord, extentBlockCoord);
#if FACE_AG
						if (b != null)
							addCoordToFaceAggregorAtIndex(new FaceInfo(extentBlockCoord, h_range.top_light_level, Direction.yneg, b.type));
//							addCoordToFaceAggregorAtIndex(extentBlockCoord, b.type, Direction.yneg);
#else
						addYFaceAtChunkIndex(targetBlockIndex, b.type, Direction.yneg, starting_tri_index);
						starting_tri_index += 4;
#endif
						
						// DRAWING X AND Z TOO!
						
						// the rules: non x = (0|CHLEN - 1) or z == (0|CHLEN - 1) ranges only draw the x and z faces on their x and z positive sides
						// they take care of these x and z faces even when they are really attached to the column ahead of them (i.e. next column is higher)
						// (remember possible floating columns by the way--or stalagmites next to stalagmites)
						
						// x,z == CHLEN - 1 ranges only draw the ranges below (may have to resort to rote block check by block check for this--since we don't have a handy way of getting an adjacent npatch's heights list...although how hard would it be...that can be a TODO maybe)
						// and also only the x,z pos faces
						
						// x,z == 0 ranges deal with pos faces just like all other ranges do. 
						// they also only draw the ranges below (i.e. below there extents) for their neg faces
						
						// ------ OR . (alt rules) -----
						
						// all ranges always draw all four directions (x/z, pos/neg)
						// except we want to catch it when there's a face that's not overlapped by any other range.
						// struct can have two booleans: backRubbedX--backRubbedZ (easy! ?)
						// chunks ask noisepatchs for ranges at the chunk limits
						// noise patches just deal with it at the noise patch limit...
						
						//XPOS
						List<Range1D> adjRanges;
						if (xx == CHLEN - 1) {
							adjRanges = m_noisePatch.heightsListAtChunkCoordOffset(this.chunkCoord, new Coord(xx + 1, 0, zz));
						} else {
							adjRanges = ySurfaceMap[(xx + 1) * CHLEN + zz];
						}
						
						List<Range1D> exposedRanges = exposedRangesWithinRange(h_range, adjRanges, heights.Count > 1);
						
#if FACE_AG_XZ
						if (b != null)
							addRangeToFaceAggregatorAtXZ(exposedRanges, b.type, Direction.xneg, xx, zz);
						else 
							throw new Exception("got a null block at: xx: " + xx + " zz: " + zz + " ChunkCoord: " + chunkCoord.toString() );
#else
						addMeshDataForExposedRanges(exposedRanges, Direction.xneg, ref starting_tri_index, xx, zz);
#endif
						

						//ZPOS
						if (zz == CHLEN - 1) {
							adjRanges = m_noisePatch.heightsListAtChunkCoordOffset(this.chunkCoord, new Coord(xx, 0, zz + 1));
						} else {
							adjRanges = ySurfaceMap[xx * CHLEN + zz + 1];
						}
						
						exposedRanges = exposedRangesWithinRange(h_range, adjRanges);
#if FACE_AG_XZ
						if (b != null)
							addRangeToFaceAggregatorAtXZ(exposedRanges, b.type, Direction.zneg, xx, zz);
						else 
							throw new Exception("got a null block at: xx: " + xx + " zz: " + zz + " ChunkCoord: " + chunkCoord.toString() );
#else
						addMeshDataForExposedRanges(exposedRanges, Direction.zneg, ref starting_tri_index, xx, zz);
#endif
						
						//XNEG
						if (xx == 0) {
							adjRanges = m_noisePatch.heightsListAtChunkCoordOffset(this.chunkCoord, new Coord(- 1, 0, zz));
						} else {
							adjRanges = ySurfaceMap[(xx - 1) * CHLEN + zz];
						}
						
						//TODO: BUG: blocks can't be added if they are not right on top of other blocks. (index out of range exception)
						// it wasn't always this way...
						// also TODO: FaceAggs should deal with only one face slice (this is in question): so the upward faces of level n and downward of n+1
						
						exposedRanges = exposedRangesWithinRange(h_range, adjRanges);
#if FACE_AG_XZ
						if (b != null)	
							addRangeToFaceAggregatorAtXZ(exposedRanges, b.type, Direction.xpos, xx, zz);
						else 
							throw new Exception("got a null block at: xx: " + xx + " zz: " + zz + " ChunkCoord: " + chunkCoord.toString() );
#else
						addMeshDataForExposedRanges(exposedRanges, Direction.xpos, ref starting_tri_index, xx, zz);
#endif
						
						//ZNEG
						if (zz == 0) {
							adjRanges = m_noisePatch.heightsListAtChunkCoordOffset(this.chunkCoord, new Coord(xx, 0, -1));
						} else {
							adjRanges = ySurfaceMap[xx * CHLEN + zz - 1];
						}
						
						exposedRanges = exposedRangesWithinRange(h_range, adjRanges);
#if FACE_AG_XZ
						if (b != null)
							addRangeToFaceAggregatorAtXZ(exposedRanges, b.type, Direction.zpos, xx, zz);
						else 
							throw new Exception("got a null block at: xx: " + xx + " zz: " + zz + " ChunkCoord: " + chunkCoord.toString() );
#else
						addMeshDataForExposedRanges(exposedRanges, Direction.zpos, ref starting_tri_index, xx, zz);
#endif

						
						// COMBINE GEOM?? (USING YET ANOTHER SET OF LISTS.)
						// have an array of CHLEN lists per each dimension.
						// come across a face column that you could add to an adjacent/same-type/flush face.
						// add it (as part of some custom collection obj??–-minimesh) 
						// each mini-mesh holds the verts, indices and uv coords that it needs
						// also, in the shader itself, just draw the texture based on the uv
						// coord of a pixel at 0?? (somehow give the shader a hint about the offset)
						// then hardcode the tile size...and hard code the width of a block

						// END DRAWING X AND Z TOO
					}
				}

			} // end for zz
		} // end for xx
		
#if FACE_AG
		addAggregatedFaceGeomToMesh(starting_tri_index);	
#endif
		
	}
	
	private void addAggregatedFaceGeomToMesh(int starting_tri_index) 
	{
#if MESH_BUILDER_BUILDS_MESH
		meshBuilder.compileGeometryAndKeepMeshSet(ref starting_tri_index);
		return;	
#endif
		
#if NO_MESHBUILDER
		FaceAggregator fa;
		for (int i = 0; i < faceAggregators.Length ; ++i)
		{
			fa = faceAggregators[i];
			if (fa != null)
			{
				MeshSet mset = fa.getFaceGeometry(i);
				GeometrySet gset = mset.geometrySet;
				
				vertices_list.AddRange(gset.vertices);	
				
				for(int j = 0; j < gset.indices.Count; ++j) {
					gset.indices[j] += starting_tri_index;
				}
				
				triangles_list.AddRange(gset.indices);
				
				starting_tri_index += gset.vertices.Count;
				
				uvcoords_list.AddRange(mset.uvs);
			}
		}
#else
		MeshSet mesh_set = meshBuilder.compileGeometry(ref starting_tri_index);
		
		triangles_list.AddRange(mesh_set.geometrySet.indices);
		uvcoords_list.AddRange(mesh_set.uvs);
		vertices_list.AddRange(mesh_set.geometrySet.vertices);
		col32s_list.AddRange(mesh_set.color32s);
#endif
		
	}
	
	private void addMeshDataForExposedRanges(List<Range1D> exposedRanges, Direction camFacingDir, ref int starting_tri_i, int xx, int zz)
	{
		ChunkIndex targetBlockIndex;
		foreach(Range1D rng in exposedRanges)
		{
			int blockY = rng.start;
										
//			while(blockY < rng.extent()) {
				targetBlockIndex = new ChunkIndex(xx, blockY, zz);
				
				if (blockY == Range1D.theErsatzNullRange().start)
					throw new Exception ("this range was ersatz nullish " + rng.toString());
				
				Block b = m_noisePatch.blockAtChunkCoordOffset (chunkCoord, new Coord (xx, blockY, zz));
//			addYFaceAtChunkIndex(targetBlockIndex, b.type, camFacingDir, starting_tri_i); // old way with the while loop (now each range is one quad)
				addYFaceAtChunkIndex(targetBlockIndex, b.type, camFacingDir, starting_tri_i, rng.range);
				starting_tri_i += 4;
				
//				blockY++;	
//			}
		}
	}
	
	private List<Range1D> exposedRangesWithinRange(Range1D _range, List<Range1D> adjacentRanges)
	{
		return exposedRangesWithinRange(_range, adjacentRanges, false);
	}
	
	private List<Range1D> exposedRangesWithinRange(Range1D _range, List<Range1D> adjacentRanges, bool debug)
	{
		
		// (assume adjacents ranges is in height order)
		// go through adjRanges. 
		// find any non-overlap below the start of the first adj range
		// add this as a new range (the non-overlapped piece below) to a return list of ranges
		
		// set range to now be the range that excludes the overlapped part and everything below (i.e. new start now = adj range's extent
		// i.e. take what's still above this adj range.)
		// but just keep the same if there was no overlap.
		
		// if there was nothing above the extent: break.
		// else repeat for the next
		
		// also, yo. did we find that the start of the adj. was above? make sure we add the remainder
		List<Range1D> nonOverlappingRanges = new List<Range1D>();
		
		if (adjacentRanges == null)
			return nonOverlappingRanges;
		
		Range1D remainderRange = _range;
		
		bool shouldDebug = debug; 
		Range1D adjacentRange = Range1D.theErsatzNullRange();
//		bool adjacentRangeReallyAssigned = false;
		int i = 0;
		
		for (; i < adjacentRanges.Count ; ++i)
		{
			adjacentRange = adjacentRanges[i];
//			adjacentRangeReallyAssigned = true;
			
			// get the section that's entirely below adj range
			Range1D belowAdj = remainderRange.subRangeBelow(adjacentRange.start);
			
			// legit below range?
			if (!Range1D.Equal(belowAdj, Range1D.theErsatzNullRange()) )
			{
//				if (debug)
//					bug ("the range below that we are adding was: " + belowAdj.toString());
				if (belowAdj.start == Range1D.theErsatzNullRange().start ) //paranoid? (not quite?)
					throw new Exception ("range is funky but we are adding it now" + belowAdj.toString() + " adj range was: " + adjacentRange.toString() + " the orig range was " + _range.toString());
				
//				if (shouldDebug) bug ("yes were adding a range: from below: " + belowAdj.toString());
				
				if (_range.contains(belowAdj))
				{
#if LIGHT_BY_RANGE
					belowAdj.bottom_light_level =  adjacentRange.bottom_light_level; //side ranges borrow adj light lvel
					belowAdj.top_light_level = adjacentRange.bottom_light_level;
#endif
					nonOverlappingRanges.Add(belowAdj);
				}
				else if (debug) {
					throw new Exception ("wha? trying to add a range not contained by the original range?? : new range: " + remainderRange.toString() + "orig range: " + _range.toString() );	
				}
					
				
			}
			else if (shouldDebug) {
//				bug ("skipped a range below because it was ersatz null");	
			}
			
			remainderRange = remainderRange.subRangeAboveRange(adjacentRange);
			remainderRange.bottom_light_level = remainderRange.top_light_level = adjacentRange.top_light_level;
			
			// no hope of finding more ranges?
			if (adjacentRange.extentMinusOne() >= remainderRange.extentMinusOne())
				break;
//			if (belowAdj.start >= remainderRange.extentMinusOne())
//				break;
			
			
		}
		
		if (!Range1D.Equal(remainderRange, Range1D.theErsatzNullRange()) )
		{
//			if (debug || shouldDebug)
//				bug ("adding theh last remainder range: " + remainderRange.toString());
			if (remainderRange.start == Range1D.theErsatzNullRange().start )
					throw new Exception ("remainder range is funky but we are adding iti now" + remainderRange.toString());
			
			if (_range.contains(remainderRange))
			{
#if LIGHT_BY_RANGE
//				if (adjacentRangeReallyAssigned) {
					remainderRange.bottom_light_level =  _range.top_light_level; // the last adjacent range.
					remainderRange.top_light_level = _range.top_light_level;
//				}
#endif
				nonOverlappingRanges.Add(remainderRange);
			}
			else if (debug) {
				bug ("wha? trying to add a range not contained by the original range?? : new range: " + remainderRange.toString() + "orig range: " + _range.toString() );	
			}
			
		}
		return nonOverlappingRanges;
	}

	
	void addYFaceAtChunkIndex(ChunkIndex ci, BlockType bType, Direction dir, int tri_index)
	{
		addYFaceAtChunkIndex(ci, bType, dir, tri_index, 1);
	}
	
	void addYFaceAtChunkIndex(ChunkIndex ci, BlockType bType, Direction dir, int tri_index, int height)
	{
		Vector3[] verts = new Vector3[]{};
		int[] tris = new int[]{};

		Vector2[] uvs;

		int shift = (int) dir % 2 == 1 ? -1 : 1;
		Direction oppositeDir = (Direction) ((int) dir + shift);
		uvs = uvCoordsForBlockType (bType, oppositeDir);

		int[] posTriangles = new int[] { 0, 2, 3, 0, 1, 2 };  // clockwise when looking from pos towards neg
		int[] negTriangles = new int[] { 0, 3, 2, 0, 2, 1 }; // the opposite

		tris =(int)(oppositeDir) % 2 == 0 ? posTriangles : negTriangles; 

		for (int ii = 0; ii < tris.Length; ++ii) {
			tris [ii] += tri_index;
		}
		verts = faceMesh (oppositeDir, ci , (float) (height - 1)); //third param = extra hieght. TODO: change this silly implementation
		vertices_list.AddRange (verts);
		// 6 triangles (index_of_so_far + 0/1/2, 0/2/3 <-- depending on the dir!)
		triangles_list.AddRange (tris);
		// 4 uv coords
		uvcoords_list.AddRange (uvs);
	}


	public void applyMesh()
	{
		applyMeshToGameObject ();
	}

	public void clearMesh()
	{
		clearMeshOfGameObject (meshHoldingGameObject); 
		return;
	}
	
	private void applyMeshToGameObject()
	{
//#if MESH_BUILDER_BUILDS_MESH
//		if (builtMesh == null)
//			throw new Exception("null built mesh");
//		Mesh mesh = builtMesh;
//#else
//#endif
		GeometrySet geomset = new GeometrySet(triangles_list, vertices_list);
		MeshSet mset = new MeshSet(geomset, uvcoords_list, col32s_list);
		this.applyMeshToGameObjectWithMeshSet(mset);
	}
	
	public IEnumerator applyMeshToGameObjectCoro()
	{
		GeometrySet geomset = new GeometrySet(triangles_list, vertices_list);
		MeshSet mset = new MeshSet(geomset, uvcoords_list, col32s_list);
		this.applyMeshToGameObjectWithMeshSet(mset);
		yield return null;
	}
	
	private void applyMeshToGameObjectWithMeshSet(MeshSet mset) 
	{
		meshBuilder.addDataToMesh(meshHoldingGameObject);
		return;
#if MESH_BUILDER_BUILDS_MESH
#endif
		GameObject _gameObj = meshHoldingGameObject;
		Mesh mesh = new Mesh();
		_gameObj.GetComponent<MeshFilter>().mesh = mesh; // Can we get the mesh back ? (surely we can right? if so, don't add ivars that would duplicate)

		mesh.Clear ();
	
		mesh.vertices = mset.geometrySet.vertices.ToArray ();
		mesh.uv = mset.uvs.ToArray ();
		mesh.triangles = mset.geometrySet.indices.ToArray ();
		
		mesh.colors32 = mset.color32s.ToArray();

		
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		
		mesh.Optimize();

		_gameObj.GetComponent<MeshCollider>().sharedMesh = mesh;

	}

	public void clearMeshLists() 
	{
		vertices_list.Clear ();
		uvcoords_list.Clear ();
		triangles_list.Clear ();
		col32s_list.Clear();
	}


	public void clearMeshOfGameObject(GameObject _gameObj)
	{
		Mesh mesh = new Mesh();
		_gameObj.GetComponent<MeshFilter>().mesh = mesh; // Can we get the mesh back ? (surely we can right? if so, don't add ivars that would duplicate)

		mesh.Clear ();

		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		mesh.Optimize();

		_gameObj.GetComponent<MeshCollider>().sharedMesh = mesh;
	}

	public void destroyAndSetGameObjectToNull() {

		meshHoldingGameObject = null;
	}
	
	void bug(string str) {
		Debug.Log (str);
	}
	
	

}

	

//	private void addYFaces(int CHLEN, int starting_tri_index)  // starting tri index might have to be an 'out?'
//	{
//		List<int>[] ySurfaceMap = m_noisePatch.ySurfaceMapForChunk (this);
//
//		List<int> heights;
//
//		int xx = 0;
//		for (; xx < CHLEN; ++xx) {
//			int zz = 0;
//			for (; zz < CHLEN; ++zz) {
//				int height_index = 0;
//				heights = ySurfaceMap [xx * CHLEN + zz];
//				if (heights != null) {
//					foreach (int hh in heights) 
//					{
//						#if TESTRENDER
//						if (height_index == heights.Count - 1)
//							break;
//						#endif
////						bug ("adding some verts?");
//						int targetY = hh + 1;
//						Direction dir = Direction.ypos;
//						if (height_index % 2 == 0)
//						{
//							targetY = hh - 1;
//							dir = Direction.yneg;
//						}
//
//						ChunkIndex targetBlockIndex = new ChunkIndex (xx, targetY, zz);
//
//						Block b = m_noisePatch.blockAtChunkCoordOffset (chunkCoord, new Coord (xx, targetY, zz));
//
//						addYFaceAtChunkIndex (targetBlockIndex, b.type, dir, starting_tri_index);
//
//						height_index++;
//						starting_tri_index += 4;
//					}
//				}
//
//			}
//		}
//	}