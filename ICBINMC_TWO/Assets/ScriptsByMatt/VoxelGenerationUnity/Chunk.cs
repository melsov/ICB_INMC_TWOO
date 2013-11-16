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
// sometimes duplicates functionality of 'Coord' and vis-versa
// 'oh well'
using System;


public struct dvektor
{
	public int x,y,z;

	public dvektor(int xx, int yy, int zz) 
	{
		x = xx;
		y = yy;
		z = zz;
	}

	public dvektor(float  xx, float yy, float zz) 
	{
		x = (int) xx;
		y = (int)yy;
		z = (int)zz;
	}

	public dvektor (ChunkIndex ci)
	{
		this = new dvektor (ci.x, ci.y, ci.z);
	}

	public static dvektor dvekXPositive ()
	{
		return new dvektor (1, 0, 0);
	}

	public static dvektor dvekYPositive()
	{
		return new dvektor (0, 1, 0);
	}

	public static dvektor dvekZPositive()
	{
		return new  dvektor (0, 0, 1);
	}

	public static dvektor dvekXNegative ()
	{
		return new dvektor (1, 0, 0) * -1;
	}

	public static dvektor dvekYNegative()
	{
		return new dvektor (0, 1, 0) * -1;
	}

	public static dvektor dvekZNegative()
	{
		return new  dvektor (0, 0, 1) * -1;
	}

	public static dvektor OppositeDirection(dvektor d) {
		return d * -1.0f;
	}

	public static dvektor operator* (dvektor a, float b) {
		return new dvektor (a.x * b, a.y * b, a.z * b);
	}

	public static dvektor operator* (dvektor a, dvektor bb) {
		return new dvektor (a.x * bb.x, a.y * bb.y, a.z * bb.z);
	}

	public static dvektor operator+ (dvektor a, int b) {
		return new dvektor (a.x + b, a.y + b, a.z + b);
	}

	public dvektor (Direction d)
	{
		switch (d) 
		{
		case(Direction.xpos):
			this = dvekXPositive ();
			break;
		case(Direction.xneg):
			this = dvekXNegative ();
			break;
		case(Direction.ypos):
			this = dvekYPositive ();
			break;
		case(Direction.yneg):
			this = dvekYNegative ();
			break;
		case(Direction.zpos):
			this = dvekZPositive ();
			break;
		default: // zneg
			this = dvekZNegative ();
			break;
		}
	}

	public static dvektor Inverse( dvektor aa) {
		int total = aa.x + aa.y + aa.z;
		dvektor retv = aa + (-total);
		return retv * -1;
	}

	public static dvektor Abs( dvektor aa) {
		return aa * aa;
	}

	public int total() {
		return x + y + z;
	}

}

//ChunkIndex (TODO: change name, too easy to confuse with ChunkCoord)
//Chunk indices are meant to hold internal block coords within chunks. (i.e. between x,y,z between integers 0 and 15 for 16 length chunks)
public struct ChunkIndex
{
	public int x,y,z;

	public ChunkIndex(uint xx, uint yy, uint zz) {
		x = (int) xx;
		y = (int) yy;
		z = (int) zz;
	}

	public ChunkIndex(int xx, int yy, int zz) {
		x = (int) xx;
		y = (int) yy;
		z = (int) zz;
	}

	public ChunkIndex(dvektor d) {
		x = (int)d.x;
		y = (int)d.y;
		z = (int)d.z;
	}

	public static ChunkIndex ChunkIndexNextToIndex(ChunkIndex ci, Direction dir)
	{
		int idir = (int)dir;
		int which_way = (idir % 2) == 1 ? -1 : 1;
		int xx, yy, zz;
		xx = yy = zz = 0;
		if (dir < Direction.ypos)
			xx = which_way;
		else if (dir < Direction.zpos)
			yy = which_way;
		else
			zz = which_way;

		return new ChunkIndex ((int)(ci.x + xx), (int)( ci.y + yy),(int)( ci.z + zz));
	}

	public static ChunkIndex operator *(ChunkIndex aa, ChunkIndex bb) {
		return new ChunkIndex (aa.x * bb.x, aa.y * bb.y, aa.z * bb.z);
	}

	public static ChunkIndex operator *(ChunkIndex aa, float bb) {
		return new ChunkIndex ((int)(aa.x * bb),(int) (aa.y * bb),(int) (aa.z * bb));
	}

	public static ChunkIndex operator +(ChunkIndex aa, ChunkIndex bb) {
		return new ChunkIndex (aa.x + bb.x, aa.y + bb.y, aa.z + bb.z);
	}

	public string toString(){
		return "Chunk Index: x: " + x + " y: " + y + " z: " + z;
	}


}

public class Chunk // : MonoBehaviour 
{
	public int CHUNKLENGTH; //duplicate of chunkManager's chunklength...
	public int CHUNKHEIGHT;

	public ChunkManager m_chunkManager;
	public Coord chunkCoord;

	public NoisePatch m_noisePatch; // replace chunk manager?


	List<Vector3> vertices_list; // = new List<Vector3> ();
	List<int> triangles_list; // = new List<int> ();
	List <Vector2> uvcoords_list; // = new List<Vector2> ();

	public bool noNeedToRenderFlag;
	public bool isActive;

	public GameObject meshHoldingGameObject;

	private int random_new_chunk_color_int_test;

	public Chunk()
	{

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
//				retBlock = m_chunkManager.blockAtChunkCoordOffset (chunkCoord, offset);
//				retBlock = blocks [ci.x + 1, ci.y, ci.z];
			break;
		case(Direction.xneg):
			if (notOnlyWithinThisChunk ||ci.x >  0)
				offset = new Coord (ci.x - 1, ci.y, ci.z);
//				retBlock = m_chunkManager.blockAtChunkCoordOffset (chunkCoord, offset);
//				retBlock = blocks [ci.x - 1, ci.y, ci.z];
			break;
		case(Direction.ypos):
			if (notOnlyWithinThisChunk || ci.y < CHUNKHEIGHT - 1)
				offset = new Coord (ci.x , ci.y + 1, ci.z);
//				retBlock = m_chunkManager.blockAtChunkCoordOffset (chunkCoord, offset);
//				retBlock = blocks [ci.x, ci.y + 1, ci.z];
			break;
		case(Direction.yneg):
			if (notOnlyWithinThisChunk || ci.y  >  0)
				offset = new Coord (ci.x , ci.y - 1, ci.z);
//				retBlock = m_chunkManager.blockAtChunkCoordOffset (chunkCoord, offset);
//				retBlock = blocks [ci.x, ci.y - 1, ci.z];
			break;
		case(Direction.zpos):
			if (notOnlyWithinThisChunk || ci.z  < CHUNKLENGTH - 1)
				offset = new Coord (ci.x , ci.y , ci.z + 1);
//				retBlock = m_chunkManager.blockAtChunkCoordOffset (chunkCoord, offset);
//				retBlock = blocks [ci.x, ci.y, ci.z + 1];
			break;
		default: // zneg
			if (notOnlyWithinThisChunk || ci.z >  0)
				offset = new Coord (ci.x , ci.y , ci.z - 1);
//				retBlock = m_chunkManager.blockAtChunkCoordOffset (chunkCoord, offset);
//				retBlock = blocks [ci.x , ci.y, ci.z - 1];
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
		int tiles_per_row = (int)(1.0f / tile_length);
		int cXBasedOnXCoord = (chunkCoord.x + random_new_chunk_color_int_test) % tiles_per_row;
		int cYBasedOnZCoord = chunkCoord.z % tiles_per_row;

//test...
//		float cX = cXBasedOnXCoord * tile_length;
//		float cY = cYBasedOnZCoord * tile_length;

		float cX = 0.0f;
		float cY = 0.0f;

		switch (btype) {
		case BlockType.Stone:
			cY = tile_length;
			cX = tile_length;
			break;
		case BlockType.Sand:
			cY = tile_length * 2;
			break;
		case BlockType.Dirt:
			cY = tile_length * 3;
			break;
		case BlockType.BedRock:
			cY = tile_length * 2; //silly value at the moment
			break;
		default: //GRASS
			if (dir != Direction.ypos)
				cY = tile_length;
			break;
		}

		return new Vector2[] { 
			new Vector2 (cX, cY), 
			new Vector2 (cX, cY + tile_length), 
			new Vector2 (cX + tile_length , cY + tile_length), 
			new Vector2 (cX + tile_length, cY) 
		};
	}

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
		// (re)create my mesh.
		random_new_chunk_color_int_test = (int)(UnityEngine.Random.value * 4.0f);

		vertices_list = new List<Vector3> ();
		triangles_list = new List<int> ();
		uvcoords_list = new List<Vector2> ();

		int triangles_index = 0;

		noNeedToRenderFlag = true;

		int i = 0;
		for (; i < CHUNKLENGTH; ++i) 
		{
			int j = 0;
			for (; j < CHUNKHEIGHT; ++j) 
			{
				int k = 0;
				for (; k< CHUNKLENGTH; ++k) 
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

					int dir = (int) Direction.xpos; // zero // TEST
//					int dir = (int) Direction.yneg; // zero

					Block targetBlock;

					for (; dir <= (int) Direction.zneg; ++ dir) 
//					for (; dir < (int) Direction.zpos; ++ dir) 
					{
						ChunkIndex ijk = new ChunkIndex (i, j, k);

						// if block is of type air OR
						// OR if direction and coord "match" and block is not of type air...
						bool negDir = dir % 2 == 1;

						dvektor dtotalUnitVek = new dvektor (ijk) * new dvektor ((Direction)dir);
						int totalUnitVek = dtotalUnitVek.total ();

						bool zeroAndNegDir = negDir && totalUnitVek == 0; // at zero at the coord corresponding to direction & negDir
						bool chunkMaxAndPosDir = !negDir && totalUnitVek == CHUNKLENGTH - 1; // the opposite

						bool reachingBeyondChunkEdge = zeroAndNegDir || chunkMaxAndPosDir;
						Block blockNextDoor = null;

						// don't bother if we're not going to use...
						// if non-air and non-edge
						if (b.type == BlockType.Air || reachingBeyondChunkEdge) 
						{
							blockNextDoor = reachingBeyondChunkEdge && b.type != BlockType.Air ? nextBlock ((Direction)dir, ijk, true) : nextBlock ((Direction)dir, ijk);
						}



						//debug 
						if (blockNextDoor != null && reachingBeyondChunkEdge && blockNextDoor.type == BlockType.Air)
						{
							if (blockNextDoor == null) {
								bug ("we were reaching beyond this chunk but got a null block. reaching from chunk index (coord)" + ijk.toString () + "in Dir: " + dir);
							}
						}


						if (b.type == BlockType.Air || (blockNextDoor != null && reachingBeyondChunkEdge && blockNextDoor.type == BlockType.Air)) 
						{

							// if we're on the edge and not air, we want to know about the block in the next chunk over. if we are an air block,
							// we want to throw out those blocks...
							// (we could have just checked for blocks in the next chunk, only if we were an air block, 
							// but then, we'd be drawing a bit of geom from the next chunk over...)

							targetBlock = reachingBeyondChunkEdge ? b : blockNextDoor ; // if edge matches dir, we want 'this' block

							if (targetBlock != null && targetBlock.type != BlockType.Air) //OK we got a block face that we can use
							{
								// get the opposite direction to the current one
								//direction enum: xpos = 0, xneg, ypos, yneg, zpos, zneg = 5
								int shift = negDir ? -1 : 1; 

								if (reachingBeyondChunkEdge)
									shift = 0; // want the same direction in this edge case

								Vector3[] verts = new Vector3[]{};
								int[] tris = new int[]{};

								Vector2[] uvs = uvCoordsForBlockType (targetBlock.type, (Direction) (dir + shift) );

								// if on edge make the face for the block at this chunk index, else the one next to it in Direction dir.
								ChunkIndex nextToIJK = reachingBeyondChunkEdge ? ijk : ChunkIndex.ChunkIndexNextToIndex (ijk,(Direction) dir);

								Direction meshFaceDirection = (Direction)dir + shift;

//								int[] posTriangles = new int[] { 0, 2, 3, 1, 2, 0 };  // clockwise when looking from pos towards neg
//								int[] negTriangles = new int[] { 0, 3, 2, 1, 0, 2 }; // the opposite

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
							}
						}
					}
				}
			}
		}

		// ** want
		if (!noNeedToRenderFlag) // not all air (but are we all solid and solidly surrounded?)
		{
			noNeedToRenderFlag = (vertices_list.Count == 0);

		}

		// moved to apply mesh
//		mesh.Clear ();
//		mesh.vertices = vertices_list.ToArray ();
//		mesh.uv = uvcoords_list.ToArray ();
//		mesh.triangles = triangles_list.ToArray ();
//
//
////		GetComponent<MeshFilter>().meshCollider = meshc;
////		meshc.sharedMesh = mesh ;
//
//		mesh.RecalculateNormals();
//		mesh.RecalculateBounds();
//		mesh.Optimize();
//
////		GetComponent<MeshCollider>().sharedMesh = null; // don't seem to need
//		GetComponent<MeshCollider>().sharedMesh = mesh;
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


	void Start () {

	}
	
	// Update is called once per frame
	void Update () {


	}
	
	void bug(string str) {
		Debug.Log (str);
	}

}
