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

	public bool noNeedToRenderFlag;
	public bool isActive;
	public bool calculatedMeshAlready=false;

	public GameObject meshHoldingGameObject;

	private int random_new_chunk_color_int_test;

	private static int[] m_meshGenPhaseOneDirections = new int[] {0, 1, 4, 5}; // (Direction enum)

	public Chunk()
	{
		vertices_list = new List<Vector3> ();
		triangles_list = new List<int> ();
		uvcoords_list = new List<Vector2> ();
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
			y1 = y2 = ci.y + halfunit; // worry about winding with triangle indices, not here... (1,2 always up, 0,3 always down)
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
			y1 = y2 = ci.y + halfunit;
			y0 = y3 = ci.y - halfunit;

		}
		Vector3 v0 = new Vector3 (x0, y0, z0);
		Vector3 v1 = new Vector3 (x1, y1, z1);
		Vector3 v2 = new Vector3 (x2, y2, z2);
		Vector3 v3 = new Vector3 (x3, y3, z3);

		return new Vector3[] { v0, v1, v2, v3 };
	}

	Vector2[] uvCoordsForBlockType( BlockType btype, Direction dir)
	{
		float tile_length = 0.25f;
//		int tiles_per_row = (int)(1.0f / tile_length);
//		int cXBasedOnXCoord = (chunkCoord.x + random_new_chunk_color_int_test) % tiles_per_row;
//		int cYBasedOnZCoord = chunkCoord.z % tiles_per_row;

//test...
//		float cX = cXBasedOnXCoord * tile_length;
//		float cY = cYBasedOnZCoord * tile_length;

		//want
		float cX = 0.0f;
		float cY = 0.0f;

		switch (btype) {
		case BlockType.Stone:
			cY = tile_length;
			cX = tile_length;
			break;
		case BlockType.Sand:
			cX = tile_length * 2;
			cY = tile_length; //<-- silly // * 2;
			break;
		case BlockType.Dirt:
			cY = tile_length * 3;
			break;
		case BlockType.BedRock:
			cY = tile_length * 2; //silly value at the moment
			break;
		default: //GRASS
			//testing...make geom more visible
			if (dir == Direction.xpos) 
			{
				cX = tile_length;
			} else if (dir == Direction.xneg) {
				cX = tile_length; cY = tile_length;
			} else if (dir == Direction.yneg) {
				cX = 2 * tile_length; cY = tile_length;
			} else if (dir == Direction.zneg) {
				cX = 3 * tile_length; cY = 3 * tile_length;
			} else if (dir == Direction.zpos) {
				cY = 3 * tile_length;
			}
			//end testing
//			if (dir != Direction.ypos)
//				cY = tile_length;
			break;
		}

		return new Vector2[] { 
			new Vector2 (cX, cY), 
			new Vector2 (cX, cY + tile_length), 
			new Vector2 (cX + tile_length , cY + tile_length), 
			new Vector2 (cX + tile_length, cY) 
		};
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
		// (re)create my mesh.
//		random_new_chunk_color_int_test = (int)(UnityEngine.Random.value * 4.0f);

//		vertices_list.Clear(); // new List<Vector3> ();
//		triangles_list.Clear (); // = new List<int> ();
//		uvcoords_list.Clear (); // = new List<Vector2> ();

		int triangles_index = 0;
		
#if ONLY_Y_FACES
		// y Face approach
		addYFaces (CHLEN, triangles_index); //want
#else

		noNeedToRenderFlag = true;

		int iterCount = 0;

		int i = 0;
		for (; i < CHLEN; ++i) 
		{
			int j = 0;
			for (; j < CHHeight; ++j) 
			{
				int k = 0;
				for (; k< CHLEN; ++k) 
				{

					Block b = m_noisePatch.blockAtChunkCoordOffset (chunkCoord, new Coord (i, j, k));

					if (b == null)
					{
						//want *****??????
//						bug ("block was null in makeMesh"); //WEIRD THIS CAUSES THE SEP THREAD TO WORK??? (TODO: why)
						continue;
					}

					if ((b.type != BlockType.Air))
						noNeedToRenderFlag = false;

//					int dir = (int) Direction.xpos; // zero 
//					int dir = (int) Direction.zpos; // zero
					int dir = (int) Direction.zneg; // zero

					Block targetBlock;

					int directionsLookupIndex = 0;

					#if TESTRENDER
					directionsLookupIndex = m_meshGenPhaseOneDirections.Length; // i.e. skip this
					#endif

					for (; directionsLookupIndex < m_meshGenPhaseOneDirections.Length ; ++directionsLookupIndex)
//					for (; dir <= (int) Direction.zneg; ++ dir) 
//					for (; dir < (int) Direction.zpos; ++ dir) 
					{

						dir = m_meshGenPhaseOneDirections [directionsLookupIndex];

						ChunkIndex ijk = new ChunkIndex (i, j, k);

						// if block is of type air OR
						// OR if direction and coord "match" and block is not of type air...
						bool negDir = dir % 2 == 1;

						dvektor dtotalUnitVek = new dvektor (ijk) * new dvektor ((Direction)dir);
						int totalUnitVek = dtotalUnitVek.total ();

						bool zeroAndNegDir = negDir && totalUnitVek == 0; // at zero at the coord corresponding to direction & negDir
						bool chunkMaxAndPosDir = !negDir && totalUnitVek == CHLEN - 1; // the opposite

						bool reachingBeyondChunkEdge = zeroAndNegDir || chunkMaxAndPosDir;
						Block blockNextDoor = null;

						// don't bother if we're not going to use...
						// if non-air and non-edge
						if (b.type == BlockType.Air || reachingBeyondChunkEdge) 
						{
							blockNextDoor = reachingBeyondChunkEdge && b.type != BlockType.Air ? nextBlock ((Direction)dir, ijk, true) : nextBlock ((Direction)dir, ijk);
						}



						//debug 
						if (blockNextDoor == null && reachingBeyondChunkEdge) 
						{
//							bug ("we were reaching beyond this chunk but got a null block. reaching from chunk index (coord)" + ijk.toString () + "in Dir: " + dir);

						}


//						if (b.type == BlockType.Air || (blockNextDoor != null && reachingBeyondChunkEdge && blockNextDoor.type == BlockType.Air)) 
						if ((reachingBeyondChunkEdge) || (b.type == BlockType.Air && blockNextDoor != null)) 
						{

							// if we're on the edge and not air, we want to know about the block in the next chunk over. if we are an air block,
							// we want to throw out those blocks...
							// (we could have just checked for blocks in the next chunk, only if we were an air block, 
							// but then, we'd be drawing a bit of geom from the next chunk over...)

							targetBlock = reachingBeyondChunkEdge ? b : blockNextDoor ; // if edge matches dir, we want 'this' block

							if (targetBlock != null && targetBlock.type != BlockType.Air) //OK we got a block face that we can use
							{
								//ONE LAST CONDITION: ALLOWS US TO DEAL WITH 'REACHING-BEYOND' BLOCKS THAT WERE NULL
								//WHILE SKIPPING BEYOND BLOCKS THAT WEREN'T AIR
								if (reachingBeyondChunkEdge && blockNextDoor != null && blockNextDoor.type != BlockType.Air)
								{
									continue;
								}
								// get the opposite direction to the current one
								//direction enum: xpos = 0, xneg, ypos, yneg, zpos, zneg = 5
								int shift = negDir ? -1 : 1; 

								if (reachingBeyondChunkEdge)
									shift = 0; // want the same direction in this edge case

								Vector3[] verts = new Vector3[]{};
								int[] tris = new int[]{};

								Vector2[] uvs;

								if (reachingBeyondChunkEdge) //TEST
									uvs = uvCoordsForTestBlock ((blockNextDoor == null), dir);
								else 
									uvs = uvCoordsForBlockType (targetBlock.type, (Direction) (dir + shift) );

								// if on edge make the face for the block at this chunk index, else the one next to it in Direction dir.
								ChunkIndex nextToIJK = reachingBeyondChunkEdge ? ijk : ChunkIndex.ChunkIndexNextToIndex (ijk,(Direction) dir);

								Direction meshFaceDirection = (Direction)dir + shift;

								int[] posTriangles = new int[] { 0, 2, 3, 0, 1, 2 };  // clockwise when looking from pos towards neg
								int[] negTriangles = new int[] { 0, 3, 2, 0, 2, 1 }; // the opposite

								tris =(dir + shift) % 2 == 0 ? posTriangles : negTriangles; 

								for (int ii = 0; ii < tris.Length; ++ii) {
									tris [ii] += triangles_index;
								}
								verts = faceMesh (meshFaceDirection, nextToIJK); // dir + shift == the opposite dir. (if xneg, xpos etc.)
								vertices_list.AddRange (verts);
								// 6 triangles (index_of_so_far + 0/1/2, 0/2/3 <-- depending on the dir!)
								triangles_list.AddRange (tris);
								// 4 uv coords
								uvcoords_list.AddRange (uvs);
								triangles_index += 4;

								//CORO!
//								iterCount++;
//								if (iterCount % 10 == 0) {
//									yield return new WaitForSeconds (.1f);
//								}

							}
						}
					}
				}
			}
		}

		// y Face approach
		addYFaces (CHLEN, triangles_index); //want

		// ** want
		if (!noNeedToRenderFlag) // not all air (but are we all solid and solidly surrounded?)
		{
			noNeedToRenderFlag = (vertices_list.Count == 0);

		}
#endif

		calculatedMeshAlready = true;
	}


	private void addYFaces(int CHLEN, int starting_tri_index)  // starting tri index might have to be an 'out?'
	{
		List<int>[] ySurfaceMap = m_noisePatch.ySurfaceMapForChunk (this);

		List<int> heights;

		int xx = 0;
		for (; xx < CHLEN; ++xx) {
			int zz = 0;
			for (; zz < CHLEN; ++zz) {
				int height_index = 0;
				heights = ySurfaceMap [xx * CHLEN + zz];
				if (heights != null) {
					foreach (int hh in heights) 
					{
						#if TESTRENDER
						if (height_index == heights.Count - 1)
							break;
						#endif
//						bug ("adding some verts?");
						int targetY = hh + 1;
						Direction dir = Direction.ypos;
						if (height_index % 2 == 0)
						{
							targetY = hh - 1;
							dir = Direction.yneg;
						}

						ChunkIndex targetBlockIndex = new ChunkIndex (xx, targetY, zz);

						Block b = m_noisePatch.blockAtChunkCoordOffset (chunkCoord, new Coord (xx, targetY, zz));

						addYFaceAtChunkIndex (targetBlockIndex, b.type, dir, starting_tri_index);

						height_index++;
						starting_tri_index += 4;
					}
				}

			}
		}
	}

	void addYFaceAtChunkIndex(ChunkIndex ci, BlockType bType, Direction dir, int tri_index)
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
		verts = faceMesh (oppositeDir, ci);
		vertices_list.AddRange (verts);
		// 6 triangles (index_of_so_far + 0/1/2, 0/2/3 <-- depending on the dir!)
		triangles_list.AddRange (tris);
		// 4 uv coords
		uvcoords_list.AddRange (uvs);
	}


	public void applyMesh()
	{
		applyMeshToGameObject (meshHoldingGameObject);
		return; 
		// *****

//		Mesh mesh = new Mesh();
//		GetComponent<MeshFilter>().mesh = mesh; // Can we get the mesh back ? (surely we can right? if so, don't add ivars that would duplicate)
//
//		mesh.Clear ();
//		mesh.vertices = vertices_list.ToArray ();
//		mesh.uv = uvcoords_list.ToArray ();
//		mesh.triangles = triangles_list.ToArray ();
//
//		//clear lists 
//
//		//		GetComponent<MeshFilter>().meshCollider = meshc;
//		//		meshc.sharedMesh = mesh ;
//
//		mesh.RecalculateNormals();
//		mesh.RecalculateBounds();
//		mesh.Optimize();
//
//		//		GetComponent<MeshCollider>().sharedMesh = null; // don't seem to need
//		GetComponent<MeshCollider>().sharedMesh = mesh;
	}

	public void clearMesh()
	{
		clearMeshOfGameObject (meshHoldingGameObject); 
		return;
		// ************v

//		Mesh mesh = new Mesh();
//		GetComponent<MeshFilter>().mesh = mesh; // Can we get the mesh back ? (surely we can right? if so, don't add ivars that would duplicate)
//
//		mesh.Clear ();
//		
//		mesh.RecalculateNormals();
//		mesh.RecalculateBounds();
//		mesh.Optimize();
//
//		GetComponent<MeshCollider>().sharedMesh = mesh;
	}

	//public void applyMeshToGameObject(GameObject _gameObj)
	//{
	//	applyMeshToGameObject(_gameObj, false);
	//}

	public void applyMeshToGameObject(GameObject _gameObj)
	{
		Mesh mesh = new Mesh();
		_gameObj.GetComponent<MeshFilter>().mesh = mesh; // Can we get the mesh back ? (surely we can right? if so, don't add ivars that would duplicate)

		mesh.Clear ();
		mesh.vertices = vertices_list.ToArray ();
		mesh.uv = uvcoords_list.ToArray ();
		mesh.triangles = triangles_list.ToArray ();


		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		mesh.Optimize();

		_gameObj.GetComponent<MeshCollider>().sharedMesh = mesh;

		// TODO: clear the lists?

//		vertices_list.Clear ();
//		uvcoords_list.Clear ();
//		triangles_list.Clear ();
	}

	public IEnumerator applyMeshToGameObjectCoro()
	{
		GameObject _gameObj = meshHoldingGameObject;
		Mesh mesh = new Mesh();
		_gameObj.GetComponent<MeshFilter>().mesh = mesh; // Can we get the mesh back ? (surely we can right? if so, don't add ivars that would duplicate)

		mesh.Clear ();
		mesh.vertices = vertices_list.ToArray ();
		mesh.uv = uvcoords_list.ToArray ();
		mesh.triangles = triangles_list.ToArray ();

		yield return null; // new WaitForSeconds (.1f);

		mesh.RecalculateNormals();

		mesh.RecalculateBounds();

		yield return null;// new WaitForSeconds (.1f);

		mesh.Optimize();

//		yield return null;// new WaitForSeconds (.1f);

		_gameObj.GetComponent<MeshCollider>().sharedMesh = mesh;

//		yield return null;// new WaitForSeconds (.1f);
		// TODO: clear the lists?

//		vertices_list.Clear ();
//		uvcoords_list.Clear ();
//		triangles_list.Clear ();

		yield return new WaitForSeconds (.1f);
	}

	public void clearMeshLists() 
	{
		vertices_list.Clear ();
		uvcoords_list.Clear ();
		triangles_list.Clear ();

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


//	void Start () {
//
//	} // unity start or threaded job start????
	
	// Update is called once per frame
	void Update () {


	}
	
	void bug(string str) {
		Debug.Log (str);
	}

}
