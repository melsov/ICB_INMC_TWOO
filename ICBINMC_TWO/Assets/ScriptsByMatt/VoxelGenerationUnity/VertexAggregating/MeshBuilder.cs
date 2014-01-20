#define SEP_MESH_SETS
//#define GAME_OBJECT_FACTORY

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MeshBuilder 
{
	// handles arrays of verts, uvs and trianges
	// handles inserting into arrays.
	private List<Vector3> vertices = new List<Vector3>();
	private List<Vector2> uvs = new List<Vector2>();
	private List<int> triangles = new List<int>();
//	private List<Vector2> colors = new List<Vector2>();
	private List<Color32> col32s = new List<Color32>();
	
	private FaceAggregator[] faceAggregatorsXZ = new FaceAggregator[Chunk.CHUNKHEIGHT];
	private FaceAggregator[] faceAggregatorsXY = new FaceAggregator[Chunk.CHUNKLENGTH];
	private FaceAggregator[] faceAggregatorsZY = new FaceAggregator[Chunk.CHUNKLENGTH];
	
//	private FaceAggregator[][] faceAggregatorCollection;
	
	private GameObject chunkGameObject;
	
	private Chunk m_chunk;
	
#if SEP_MESH_SETS
	private MeshSet meshSetXZ;
	private MeshSet meshSetXY;
	private MeshSet meshSetZY;
#endif
	
//	public MeshBuilder(GameObject chunkGameObject_) {
//		this.chunkGameObject = chunkGameObject_;
//	} // later
	
	public MeshBuilder(Chunk owner_chunk) {
		this.m_chunk = owner_chunk;
	}
	
//	public MeshSet insertBlockAtCoord(Coord co)
//	{
////		return new MeshSet(	
//	}
	
//	public void addCoordToFaceAggregatorAtIndex(Coord co, BlockType type, Direction dir)
	public void addCoordToFaceAggregatorAtIndex(FaceInfo faceInfo)
	{
//		FaceAggregator fa = faceAggregatorAt(co, dir );
		FaceAggregator fa = faceAggregatorAt(faceInfo.coord, faceInfo.direction  );
//		fa.addFaceAtCoordBlockType(co, type, dir );
		fa.addFaceAtCoordBlockType(faceInfo); //.coord, faceInfo.blockType, faceInfo.direction );
//		faceAggregators[co.y] = fa; // put back...
//		setFaceAggregatorsAt(fa, co, dir);	
		setFaceAggregatorsAt(fa, faceInfo.coord, faceInfo.direction );
	}
	
	public void resetFaceAggregators() 
	{
		faceAggregatorsXZ = new FaceAggregator[Chunk.CHUNKHEIGHT];
		faceAggregatorsXY = new FaceAggregator[Chunk.CHUNKLENGTH];
		faceAggregatorsZY = new FaceAggregator[Chunk.CHUNKLENGTH];
	}
	
	private FaceAggregator[][] getFaceAggCollection()
	{
		return new FaceAggregator[][] {faceAggregatorsXZ, faceAggregatorsXY, faceAggregatorsZY };	
	}
//	
//	private static Axis axisFromDirection(Direction dir) {
//		if (dir < Direction.yneg)	
//			return Axis.X;
//		if (dir < Direction.zneg)
//			return Axis.Y;
//		
//		return Axis.Z;
//	}
	
	private FaceAggregator faceAggregatorAt(Coord co, Direction dir)
	{
		// cases for different face aggs.
		if (dir <= Direction.xneg)	
		{
			if (faceAggregatorsZY[co.x] == null)
				faceAggregatorsZY[co.x] = FaceAggregator.FaceAggregatorForXFaceNormal();
			return faceAggregatorsZY[co.x];
		}
			
		if (dir <= Direction.yneg)
		{
			if (faceAggregatorsXZ[co.y] == null)
				faceAggregatorsXZ[co.y] = FaceAggregator.FaceAggregatorForYFaceNormal();
			return faceAggregatorsXZ[co.y];
		}
			
		if (faceAggregatorsXY[co.z] == null)
				faceAggregatorsXY[co.z] = FaceAggregator.FaceAggregatorForZFaceNormal();
		return faceAggregatorsXY[co.z];
		
	}
	
//	private static int indexForDirection(Coord co, Direction dir) {
//		
//		if (dir <= Direction.xneg)	
//			return co.x;
//		if (dir <= Direction.yneg)
//			return co.y;
//		
//		return co.z;
//	
//	}
	
	private void setFaceAggregatorsAt(FaceAggregator fa, Coord co, Direction dir)
	{
		// cases for different face aggs.
		if (dir <= Direction.xneg)	
		{
			faceAggregatorsZY[co.x] = fa;
			return;
		}
			
		if (dir <= Direction.yneg)
		{
			faceAggregatorsXZ[co.y] = fa;
			return;
		}
		
		faceAggregatorsXY[co.z] = fa;
	}
	
	private void clearMeshArrays() {
		triangles.Clear();
		uvs.Clear();
		vertices.Clear();
		col32s.Clear();
	}
	
	private void clearMeshSets() {
		meshSetXY.clearAll();
		meshSetXZ.clearAll();
		meshSetZY.clearAll();
	}
	
	#region game object factory
//	private Mesh meshWithMeshSet(GameObject _gameObj, MeshSet mset)
	private Mesh meshWithMeshSet(MeshSet mset)
	{
		Mesh mesh = new Mesh();

//		GameObject otherGameOb = (GameObject) Transform.Instantiate(_gameObj, _gameObj.transform.position, _gameObj.transform.rotation);
//		Transform trans = (Transform) Transform.Instantiate(_gameObj.transform, _gameObj.transform.position, _gameObj.transform.rotation);
//		GameObject otherGameOb = trans.gameObject;
//		otherGameOb.transform.parent = _gameObj.transform;
		
//		otherGameOb.GetComponent<MeshFilter>().mesh = mesh; 

//		mesh.Clear ();
		
		if (mset.geometrySet.vertices.Count < 1) {
			throw new System.Exception("empty vertices!");
		}
		
		mesh.vertices = mset.geometrySet.vertices.ToArray();
		mesh.triangles = mset.geometrySet.indices.ToArray();
		mesh.uv = mset.uvs.ToArray();
		mesh.colors32 = mset.color32s.ToArray();
		
//		mesh.RecalculateNormals();
//		mesh.RecalculateBounds();
//		mesh.Optimize();
		

//		CombineInstance combine = new CombineInstance();
//		combine.mesh = mesh;
//		combine.transform = _gameObj.transform.localToWorldMatrix;	
//		
//		return combine;
		return mesh;
	}
	
	private CombineInstance combineInstanceFromMeshSet(MeshSet mset) {
		Mesh mesh = meshWithMeshSet(mset);
		CombineInstance combine = new CombineInstance();
		combine.mesh = mesh;
		return combine;
	}
	
	private CombineInstance[] combineInstancesFromAxisMeshSets() 
	{
		CombineInstance combineXZ = combineInstanceFromMeshSet(this.meshSetXZ);
		CombineInstance combineXY = combineInstanceFromMeshSet(this.meshSetXY);
		CombineInstance combineZY = combineInstanceFromMeshSet(this.meshSetZY);
		
		return new CombineInstance[3] {combineXY, combineZY, combineXZ};
	}
	
	public void addDataToMesh(GameObject _gameObj) 
	{
		
		_gameObj.GetComponent<MeshFilter>().mesh.Clear();
		
#if SEP_MESH_SETS
		
		CombineInstance[] combines = combineInstancesFromAxisMeshSets();
#else
		
		MeshSet msetfull = meshSetFromMemberLists();
		Mesh mesh_full = meshWithMeshSet(msetfull);
		CombineInstance combine = new CombineInstance();
		combine.mesh = mesh_full;
//		combine.transform = _gameObj.transform.localToWorldMatrix; // don't need since CombineMeshes (param #3 == false == ignore matrix)
		CombineInstance[] combines = new CombineInstance[] {combine};
#endif	
		
		_gameObj.GetComponent<MeshFilter>().mesh.CombineMeshes(combines, true, false);
		
		//DEBUG
		int mesh_tri_count = _gameObj.GetComponent<MeshFilter>().mesh.triangles.Length;
		int[] subMeshTris = _gameObj.GetComponent<MeshFilter>().mesh.GetTriangles(0);
		int sub_tri_count = subMeshTris.Length;
		b.bug(" mesh triangles count: " + mesh_tri_count + " sub mesh tri count: " + sub_tri_count);
		
		_gameObj.GetComponent<MeshFilter>().mesh.RecalculateNormals();
		_gameObj.GetComponent<MeshFilter>().mesh.RecalculateBounds();
//		_gameObj.GetComponent<MeshFilter>().mesh.Optimize();
		_gameObj.GetComponent<MeshCollider>().sharedMesh =  _gameObj.GetComponent<MeshFilter>().mesh; //mesh_full; //  NO DIF?
		
		
	}
	

	
	#endregion
	public void compileGeometryAndKeepMeshSet(ref int starting_tri_index) 
	{
//	TODO: GET RID OF LIST THAT DUPLICATE MESHSET MEM VAR
		compileGeometry(ref starting_tri_index);	
	}
	
	
	public MeshSet compileGeometry(ref int starting_tri_index) 
	{
#if SEP_MESH_SETS
		clearMeshSets();
		int dummy_tri = 0;
		meshSetXY = collectMeshDataWithFaceAggregators(faceAggregatorsXY, ref dummy_tri);
		int dummy2 = 0;
		meshSetXZ = collectMeshDataWithFaceAggregators(faceAggregatorsXZ, ref dummy2);
		int dum3 = 0;
		meshSetZY = collectMeshDataWithFaceAggregators(faceAggregatorsZY, ref dum3);
		
		//fake return
		return meshSetXZ;
#endif
		
		clearMeshArrays();
		
		foreach(FaceAggregator[] faceAggs in getFaceAggCollection())
		{
			MeshSet mset = collectMeshDataWithFaceAggregators(faceAggs, ref starting_tri_index);	
			addMeshSetToMemberLists(mset);
		}

		return this.meshSetFromMemberLists();
	}
	
	private void addMeshSetToMemberLists(MeshSet mset) 
	{
		GeometrySet gset = mset.geometrySet;
		
		vertices.AddRange(gset.vertices);	
		triangles.AddRange(gset.indices);
		uvs.AddRange(mset.uvs);
		col32s.AddRange(mset.color32s);	
	}
	
	private MeshSet meshSetFromMemberLists() {
		return new MeshSet(new GeometrySet(triangles, vertices), uvs, col32s);
	}
	
	private MeshSet compileGeometryDontRecalculate(ref int starting_tri_index)
	{
		clearMeshArrays();
		
		foreach(FaceAggregator[] faceAggs in getFaceAggCollection())
		{
			
			collectMeshDataWithFaceAggregatorsDontRecalculate(faceAggs, ref starting_tri_index);	
		}

		return new MeshSet(new GeometrySet(triangles, vertices), uvs);
	}
	
	public MeshSet newMeshSetByRemovingBlockAtCoord(Coord co)
	{
		editfaceAggregatorsByChangingBlockAtCoord(co, false, BlockType.Air);
		int starting_tri_i = 0;
		return compileGeometryDontRecalculate(ref starting_tri_i);
	}
//	
	public MeshSet newMeshSetByAddingBlockAtCoord(Coord co, BlockType btype)
	{
		editfaceAggregatorsByChangingBlockAtCoord(co, true, btype);		
		int starting_tri_i = 0;
		return compileGeometryDontRecalculate(ref starting_tri_i);
	}
	
	private void editfaceAggregatorsByChangingBlockAtCoord(Coord co, bool add_block, BlockType btype)
	{
		// for the x dir (say)
		// for co.x - 1 
		// is there a block at this coord? (and is x - 1 within our chunk?) (if not..don't edit it but we still want to know. whether it was air or not)
		
		Coord x_one = new Coord(1,0,0);
		Coord y_one = new Coord(0,1,0);
		Coord z_one = new Coord(0,0,1);
		
		// X
		editFaceAggregatorsAtCoordAndAxis(co, Axis.X, new AlignedCoord(co.z, co.y), x_one, faceAggregatorsZY, add_block, btype);
		// Y
		editFaceAggregatorsAtCoordAndAxis(co, Axis.Y, new AlignedCoord(co.x, co.z), y_one, faceAggregatorsXZ, add_block, btype);
		// Z
		editFaceAggregatorsAtCoordAndAxis(co, Axis.Z, new AlignedCoord(co.x, co.y), z_one, faceAggregatorsXY, add_block, btype);
	}
	
	private Direction posDirectionForAxis(Axis a) {
		if (a == Axis.X)
			return Direction.xpos;
		if (a == Axis.Y)
			return Direction.ypos;
		return Direction.zpos;
	}
	
	private void editFaceAggregatorsAtCoordAndAxis(Coord co, Axis axis, AlignedCoord alco, Coord nudgeCoord, FaceAggregator[] aggregatorArray, bool add_block, BlockType btype)
	{
		// OH DEAR: THIS WAS VERY NOT WORKING AND CAUSED TWO DIFFERENT EXCEPTIONS ON A FIRST RUN!
		// one exception was inside face set, in the dreaded 'optimize strips funcs. (index out of range or something)
		// another was...I forget at this point.
		Block test_b;
		test_b = this.m_chunk.blockAt(new ChunkIndex( co - nudgeCoord)); //x_one
		int relevantComponent = Coord.SumOfComponents (co * nudgeCoord);
		int relevantUpperLimit = Coord.SumOfComponents (Chunk.DIMENSIONSINBLOCKS * nudgeCoord);
		
		Direction relevantPosDir = this.posDirectionForAxis(axis);
		
		if (test_b.type != BlockType.Air)
		{
			if (relevantComponent > 0)
			{
				FaceAggregator faXminusOne = aggregatorArray[relevantComponent - 1];
								
				if (add_block) // take away x_pos face
				{
					faXminusOne.removeBlockFaceAtCoord(alco, true, false);
					faXminusOne.getFaceGeometry(relevantComponent - 1);
				} else {
					faXminusOne.addFaceAtCoordBlockType(co, test_b.type, relevantPosDir + 1); // x neg right (for the way we ar elooking at it)?
					faXminusOne.getFaceGeometry(relevantComponent - 1);
				}
			}
		} else { // it is air at x - 1
			
			if (add_block)
			{
				FaceAggregator faXY = aggregatorArray[relevantComponent];
				faXY.addFaceAtCoordBlockType(co, btype, relevantPosDir); // again xpos dir for neg side of block
				faXY.getFaceGeometry(relevantComponent);
			} // else nothing : air next to air
		}
		
		// x plus one
		test_b = this.m_chunk.blockAt(new ChunkIndex( co + nudgeCoord));
		if (test_b.type != BlockType.Air)
		{
			if (relevantComponent < relevantUpperLimit - 1)
			{
				FaceAggregator faXplusone = aggregatorArray[relevantComponent + 1];
				
				if (add_block) 
				{
					faXplusone.removeBlockFaceAtCoord(alco, false, true); // remove lower
					faXplusone.getFaceGeometry(relevantComponent + 1);
				} else {
					faXplusone.addFaceAtCoordBlockType(co, test_b.type, relevantPosDir);
					faXplusone.getFaceGeometry(relevantComponent + 1);
				}
				
			}
		} else {
			
			if (add_block)
			{
				FaceAggregator fXY = aggregatorArray[relevantComponent];
				fXY.addFaceAtCoordBlockType(co, btype, relevantPosDir + 1);
				fXY.getFaceGeometry(relevantComponent);
			}
		}
	}
	
	private void collectMeshDataWithFaceAggregatorsDontRecalculate(FaceAggregator[] faceAggs, ref int starting_tri_index)
	{
		FaceAggregator fa;
		for (int i = 0; i < faceAggs.Length ; ++i)
		{
			fa = faceAggs[i];

			
			if (fa != null)
			{
				fa.baseTriangleIndex = starting_tri_index;
				
				MeshSet mset = fa.meshSet;
				GeometrySet gset = mset.geometrySet;
				
				vertices.AddRange(gset.vertices);	
				
				for(int j = 0; j < gset.indices.Count; ++j) {
					gset.indices[j] += starting_tri_index;
				}
				
				triangles.AddRange(gset.indices);
				uvs.AddRange(mset.uvs);
				col32s.AddRange(mset.color32s);
				
				starting_tri_index += gset.vertices.Count;
			}
		}
	}
	

	private MeshSet collectMeshDataWithFaceAggregators(FaceAggregator[] faceAggs, ref int starting_tri_index)
	{
		// TODO: keep track of the lowest and highest (vertically speaking) face aggs.
		// then avoid iterating over empty faceAggs.
		
		List<Vector3> temp_vertices = new List<Vector3>();
		List<Vector2> temp_uvs = new List<Vector2>();
		List<int> temp_triangles = new List<int>();
		List<Color32> temp_col32s = new List<Color32>();		
		
		FaceAggregator fa;
		for (int i = 0; i < faceAggs.Length ; ++i)
		{
			fa = faceAggs[i];
			
			if (fa != null)
			{
				fa.baseTriangleIndex = starting_tri_index; // for editing mesh (potentially)
				
				MeshSet mset = fa.getFaceGeometry(i);
				GeometrySet gset = mset.geometrySet;
				
				temp_vertices.AddRange(gset.vertices);	
				
				for(int j = 0; j < gset.indices.Count; ++j) {
					gset.indices[j] += starting_tri_index;
				}
				
				temp_triangles.AddRange(gset.indices);
				temp_uvs.AddRange(mset.uvs);
				temp_col32s.AddRange(mset.color32s);
				
				starting_tri_index += gset.vertices.Count;
			}
		}
		return new MeshSet(new GeometrySet(temp_triangles, temp_vertices), temp_uvs, temp_col32s);
	}
	
	
}

public static class b
{
	public static void bug(string str) {
		UnityEngine.Debug.Log(str);	
	}
}


//	public void addDataToMeshOO(GameObject _gameObj) 
//	{
//		Mesh mesh = new Mesh();
//#if GAME_OBJECT_FACTORY
//		GameObject otherGameOb = (GameObject) Transform.Instantiate(_gameObj, _gameObj.transform.position, _gameObj.transform.rotation);
//		otherGameOb.GetComponent<MeshFilter>().mesh = mesh; 
//#else 
//		_gameObj.GetComponent<MeshFilter>().mesh = mesh; 
//#endif
//		mesh.Clear ();
//		
//		//CONSIDER: returning to copy/Instantiate method and copy the tangents from the old object? (Clear doesn't erase them by default).
//		// also consider, seeing whether you need to instantiate a new GO to get a new mesh with which to make a combineinstance.
//		// and if you don't just make a new mesh without doing the _gO.GComponent<>().mesh = mesh thing
//		
//		MeshSet mset = meshSetFromMemberLists();
//		
//		if (mset.geometrySet.vertices.Count < 1) {
//			throw new System.Exception("empty vertices!");
//		}
//		
//		mesh.vertices = mset.geometrySet.vertices.ToArray();
//		mesh.triangles = mset.geometrySet.indices.ToArray();
//		mesh.uv = mset.uvs.ToArray();
//		mesh.colors32 = mset.color32s.ToArray();
//		
//		mesh.RecalculateNormals();
////		mesh.RecalculateBounds(); //assigning triangles will automatically recalc bounds
//		
//		mesh.Optimize();
//		
//#if GAME_OBJECT_FACTORY
//		
//		//TEST NO COMBINE
////		CombineInstance combine = combineInstanceWithGameObjectMeshSet(_gameObj, mset); //  new CombineInstance();
////		CombineInstance combine = new CombineInstance();
////		combine.mesh = mesh;
////		combine.transform = _gameObj.transform.localToWorldMatrix;
////		CombineInstance[] combines = new CombineInstance[] {combine};
//		
//		//clear the orig object.
////		_gameObj.transform.GetComponent<MeshFilter>().mesh.Clear();
//		
//		//combine
//
////		_gameObj.GetComponent<MeshFilter>().mesh.CombineMeshes(combines,true,false);
//		
//		//is this good. finish up with mesh?
////		Mesh masterMesh = new Mesh();
//		
////		_gameObj.GetComponent<MeshFilter>().mesh = masterMesh;
////		_gameObj.GetComponent<MeshFilter>().mesh.RecalculateNormals();
////		_gameObj.GetComponent<MeshFilter>().mesh.RecalculateBounds();
////		_gameObj.GetComponent<MeshFilter>().mesh.Optimize();
////		_gameObj.GetComponent<MeshCollider>().sharedMesh = _gameObj.GetComponent<MeshFilter>().mesh;
//		
//#else
//#endif
//		_gameObj.GetComponent<MeshCollider>().sharedMesh = mesh;
//
//	}