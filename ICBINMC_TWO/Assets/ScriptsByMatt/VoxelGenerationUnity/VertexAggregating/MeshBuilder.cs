#define SEP_MESH_SETS
//#define GAME_OBJECT_FACTORY

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MeshBuilder 
{
	// handles arrays of verts, uvs and trianges (MeshSets)
	// handles inserting into arrays.
	
	private FaceAggregator[] faceAggregatorsXZ = new FaceAggregator[Chunk.CHUNKHEIGHT];
	private FaceAggregator[] faceAggregatorsXY = new FaceAggregator[Chunk.CHUNKLENGTH];
	private FaceAggregator[] faceAggregatorsZY = new FaceAggregator[Chunk.CHUNKLENGTH];
	
	private GameObject chunkGameObject;
	
	private Chunk m_chunk;

	private MeshSet meshSetXZ;
	private MeshSet meshSetXY;
	private MeshSet meshSetZY;

	
//	private List<MeshSet> the_meshSets = null;
//	private List<MeshSet> meshSets {
//		get {
////			if (the_meshSets == null) {
//				the_meshSets = new List<MeshSet>() {this.meshSetXZ, this.meshSetXY, this.meshSetZY};	
////			}
//			return the_meshSets;
//		} 
//		set {
//			the_meshSets = value;
//		}
//	}
	
//	public MeshBuilder(GameObject chunkGameObject_) {
//		this.chunkGameObject = chunkGameObject_;
//	} // later
	
	public MeshBuilder(Chunk owner_chunk) {
		this.m_chunk = owner_chunk;
	}

	
	public void addCoordToFaceAggregatorAtIndex(FaceInfo faceInfo)
	{
		FaceAggregator fa = faceAggregatorAt(faceInfo.coord, faceInfo.direction );
		fa.addFaceAtCoordBlockType(faceInfo);
	}
	
	public void addRangeToFaceAggregatorWithFaceInfoRange(FaceInfo faceInfo)
	{
		FaceAggregator fa = faceAggregatorAt(faceInfo.coord, faceInfo.direction );
		fa.addFaceInfoRange(faceInfo);
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
	
	private bool isFaceAggregatorAt(Coord co, Axis axis) 
	{
		// cases for different face aggs.
		if (axis == Axis.X) {
			return faceAggregatorsZY[co.x] != null;
		}
			
		if (axis == Axis.Y) {
			return faceAggregatorsXZ[co.y] != null;
		}
		
		return faceAggregatorsXY[co.z] != null;
	}
	
	private void removeFaceAggregatorAt(Coord co, Axis axis) {
		if (axis == Axis.X) {
			faceAggregatorsZY[co.x] = null;
			return;
		}
			
		if (axis == Axis.Y) {
			faceAggregatorsXZ[co.y] = null;
			return;
		}
		
		faceAggregatorsXY[co.z] = null;
	}
	
	private FaceAggregator faceAggregatorAt(Coord co, Direction dir)
	{
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
		
		mesh.vertices = mset.geometrySet.vertices.ToArray();
		mesh.triangles = mset.geometrySet.indices.ToArray();
		mesh.uv = mset.uvs.ToArray();
		mesh.colors32 = mset.color32s.ToArray();
		
		//TANGENT TEST
		mesh.tangents = mset.tangents.ToArray(); // tangentsArrayOfLength(mesh.vertices.Length);

		return mesh;
	}
	
	//TANGENT TEST
	private Vector4[] tangentsArrayOfLength(int length)
	{
		Vector4[] result = new Vector4[length];
		
		float testLightLevel = 8f * 8f * 8f * 7f + 8f * 8f * 6f + 8f * 5f + 4f;
		float testLightLevel2 = 8f * 8f * 8f * 3f + 8f * 8f * 2f + 8f * 1f + 0f;
		
		for(int i = 0; i < length ; ++i)
		{
			result[i] = new Vector4(testLightLevel,testLightLevel2,testLightLevel,testLightLevel);	
		}
		return result;
	}
	//END TANGENT TEST
	
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
		
		if (_gameObj == null)
		{
			throw new Exception("got _gameObj null in mesh builder");
		}
		
		_gameObj.GetComponent<MeshFilter>().mesh.Clear();
		
		CombineInstance[] combines = combineInstancesFromAxisMeshSets();

		Mesh mesh_full = new Mesh();
		_gameObj.GetComponent<MeshFilter>().mesh = mesh_full; 
		mesh_full.CombineMeshes(combines, true, false);
		
		_gameObj.GetComponent<MeshFilter>().mesh.RecalculateNormals();
		_gameObj.GetComponent<MeshFilter>().mesh.RecalculateBounds();
//		_gameObj.GetComponent<MeshFilter>().mesh.Optimize(); // docs say this is expensive and seems unnec...
		
		_gameObj.GetComponent<MeshCollider>().sharedMesh =  mesh_full; //  MESH_FULL WAY
		// TODO: learn why the above line works (new geometry is included in the collider)
		// and the below line doesn't (collider made of the old geometry)
//		_gameObj.GetComponent<MeshCollider>().sharedMesh =  _gameObj.GetComponent<MeshFilter>().mesh; //mesh_full; //  NO DIF?

	}
	
	#endregion
	
	public void compileGeometryAndKeepMeshSet(ref int starting_tri_index) 
	{
		compileGeometry(ref starting_tri_index);	
	}
	
	
	public MeshSet compileGeometry(ref int starting_tri_index) 
	{
		clearMeshSets();
		int dummy_tri = 0;
		meshSetXY = collectMeshDataWithFaceAggregators(faceAggregatorsXY, ref dummy_tri);
		int dummy2 = 0;
		meshSetXZ = collectMeshDataWithFaceAggregators(faceAggregatorsXZ, ref dummy2);
		int dum3 = 0;
		meshSetZY = collectMeshDataWithFaceAggregators(faceAggregatorsZY, ref dum3);
		
		//fake return
		return meshSetXZ;

	}
	
	private MeshSet compileGeometryDontRecalculate(ref int starting_tri_index)
	{
		int dummy_tri = 0;
		meshSetXY = collectMeshDataWithFaceAggregatorsDontRecalculate(faceAggregatorsXY, ref dummy_tri);
		int dummy2 = 0;
		meshSetXZ = collectMeshDataWithFaceAggregatorsDontRecalculate(faceAggregatorsXZ, ref dummy2);
		int dum3 = 0;
		meshSetZY = collectMeshDataWithFaceAggregatorsDontRecalculate(faceAggregatorsZY, ref dum3);
		
		//fake return
		return meshSetXZ;
	}
	
	#region light editing
	
	public void updateLightLevelArrays()
	{
		updateLightLevelsInMeshSets();
	}
	
	private void updateLightLevelsInMeshSets()
	{
		meshSetXY.tangents = getTangentsFromFaceAggregator(faceAggregatorsXY);
		meshSetXZ.tangents = getTangentsFromFaceAggregator(faceAggregatorsXZ);
		meshSetZY.tangents = getTangentsFromFaceAggregator(faceAggregatorsZY);	
	}
	
	private List<Vector4> getTangentsFromFaceAggregator(FaceAggregator[] faceAggs) 
	{
		List<Vector4> result = new List<Vector4>();
		FaceAggregator fa;
		for (int i = 0; i < faceAggs.Length ; ++i)
		{
			fa = faceAggs[i];
			
			if (fa != null)
			{	
				result.AddRange(fa.recalculateLightLevelTangents(i));
			}
		}
		
		return result;
	}
	
	#endregion
	
	#region block editing
	
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
		// for the x dir (for example)
		// is there a block at this co.x - 1? (and is x - 1 within our chunk?) 
		// (if not..don't edit it but we still want to know. whether it was air or not)
		
		Coord x_one = new Coord(1,0,0);
		Coord y_one = new Coord(0,1,0);
		Coord z_one = new Coord(0,0,1);
		
		// X
		editFaceAggregatorsAtCoordAndAxis(co, Axis.X, new AlignedCoord(co.z, co.y), x_one, add_block, btype);
		// Y
		editFaceAggregatorsAtCoordAndAxis(co, Axis.Y, new AlignedCoord(co.x, co.z), y_one, add_block, btype);
		// Z
		editFaceAggregatorsAtCoordAndAxis(co, Axis.Z, new AlignedCoord(co.x, co.y), z_one, add_block, btype);
	}

	
	private void editFaceAggregatorsAtCoordAndAxis(Coord co, Axis axis, AlignedCoord alco, Coord nudgeCoord, bool add_block, BlockType btype)
	{
		if (add_block) {
			this.addBlockAtCoordAndAxis(co, axis, alco, nudgeCoord, btype);	
		} else {
			this.removeBlockAtCoordAndAxis(co, axis, alco, nudgeCoord, btype);
		}
	}
	
	private void removeBlockAtCoordAndAxis(Coord co, Axis axis, AlignedCoord alco, Coord nudgeCoord, BlockType btype)
	{
		Block test_b;
		test_b = this.m_chunk.blockAt(new ChunkIndex( co - nudgeCoord)); //x_one
		int relevantComponent = Coord.SumOfComponents (co * nudgeCoord);
		int relevantUpperLimit = Coord.SumOfComponents (Chunk.DIMENSIONSINBLOCKS * nudgeCoord);
		
		Direction relevantPosDir = MeshBuilder.posDirectionForAxis(axis);
		
		FaceAggregator faXY = null;
		if (isFaceAggregatorAt(co, axis))
			faXY = faceAggregatorAt(co, relevantPosDir); 
		
		// *Neg direction neighbor block wasn't air?
		// *we need to add a face on its pos side
		if (test_b.type != BlockType.Air)
		{
			if (relevantComponent > 0)
			{
				FaceAggregator faXminusOne = faceAggregatorAt(co - nudgeCoord, relevantPosDir);// aggregatorArray[relevantComponent - 1];
				// TODO: make sure this func is really 'add face if not exists.'
				faXminusOne.addFaceAtCoordBlockType(new FaceInfo(co, Block.MAX_LIGHT_LEVEL, relevantPosDir, test_b.type, this.m_chunk.lightDataProvider ));
				faXminusOne.getFaceGeometry(relevantComponent - 1);
				
				// CONSIDER: TRY REMOVING A FACE AT THIS FACE AGG AS WELL. EVEN THOUGH THERE 'SHOULDN'T' BE ONE.
				// AND SIMILAR FOR ADDING BLOCKS
			}
		} else { // it is air at x (or whichever co) - 1
			
			if (faXY != null) {
				// * neighbor is an air block, so there should be a face to remove at our block in this direction
				faXY.removeNegativeSideFaceAtCoord(alco);
				
				if (faXY.faceSetCount == 0) {
					removeFaceAggregatorAt(co, axis);	
				} else {
					faXY.getFaceGeometry(relevantComponent);
				}
			}
		}
		
		// x plus one
		test_b = this.m_chunk.blockAt(new ChunkIndex( co + nudgeCoord));
		if (test_b.type != BlockType.Air)
		{
			if (relevantComponent < relevantUpperLimit - 1)
			{
				FaceAggregator faXplusone = faceAggregatorAt(co + nudgeCoord, relevantPosDir); // aggregatorArray[relevantComponent + 1];
					
				faXplusone.addFaceAtCoordBlockType(new FaceInfo(co, Block.MAX_LIGHT_LEVEL, relevantPosDir + 1, test_b.type, this.m_chunk.lightDataProvider));
				faXplusone.getFaceGeometry(relevantComponent + 1);
			}
		} else {
			if (faXY != null) {
				// * neighbor was air, so there should be a face to remove
				faXY.removePositiveSideFaceAtCoord(alco);
				
				if (faXY.faceSetCount == 0) {
					removeFaceAggregatorAt(co, axis);	
				} else {
					faXY.getFaceGeometry(relevantComponent);
				}

				faXY.getFaceGeometry(relevantComponent);
			}
		}
	}
	
	private void addBlockAtCoordAndAxis(Coord co, Axis axis, AlignedCoord alco, Coord nudgeCoord, BlockType btype)
	{

		Block neg_neighbor_block = this.m_chunk.blockAt(new ChunkIndex( co - nudgeCoord)); //x_one
		Block pos_neighbor_block = this.m_chunk.blockAt(new ChunkIndex( co + nudgeCoord));
		int relevantComponent = Coord.SumOfComponents (co * nudgeCoord);
		int relevantUpperLimit = Coord.SumOfComponents (Chunk.DIMENSIONSINBLOCKS * nudgeCoord);
		
		Direction relevantPosDir = MeshBuilder.posDirectionForAxis(axis);
		
		FaceAggregator faXY = null; 
		if (Block.BlockTypeIsATranslucentType(neg_neighbor_block.type) || Block.BlockTypeIsATranslucentType(pos_neighbor_block.type))
			faXY = faceAggregatorAt(co, relevantPosDir); // don't allocate if we won't have to...
		
		//NEG NEIGHBOR
		if (!Block.BlockTypeIsATranslucentType(neg_neighbor_block.type)) // (neg_neighbor_block.type != BlockType.Air)
		{
			if (relevantComponent > 0)
			{
				// * neighbor not air, so there should be a face that is now hidden and that we should remove
				if (isFaceAggregatorAt(co - nudgeCoord, axis))
				{
					FaceAggregator faXminusOne = faceAggregatorAt(co - nudgeCoord, relevantPosDir);// aggregatorArray[relevantComponent - 1];
					
					faXminusOne.removePositiveSideFaceAtCoord(alco);
					
					// if faceSetCount now zero, remove faceAgg
					if (faXminusOne.faceSetCount == 0) {
						removeFaceAggregatorAt(co - nudgeCoord, axis);	
					} else {
						// else get Face geom...
						faXminusOne.getFaceGeometry(relevantComponent - 1);
					}
				}
			}
		} else { 
			
			// * neighbor is air, so we need to add a face at our coord
			faXY.addFaceAtCoordBlockType(new FaceInfo(co, Block.MAX_LIGHT_LEVEL, relevantPosDir + 1, btype, this.m_chunk.lightDataProvider));
			faXY.getFaceGeometry(relevantComponent);
		}
		
		// POS NEIGHBOR
//		neighbor_block = this.m_chunk.blockAt(new ChunkIndex( co + nudgeCoord));
		if (!Block.BlockTypeIsATranslucentType(pos_neighbor_block.type)) // (pos_neighbor_block.type != BlockType.Air)
		{
			if (relevantComponent < relevantUpperLimit - 1)
			{
				if (isFaceAggregatorAt(co + nudgeCoord, axis))
				{
					// * neighbor not air, remove occluded face
					FaceAggregator faXplusone = faceAggregatorAt(co + nudgeCoord, relevantPosDir); // aggregatorArray[relevantComponent + 1];
					
					faXplusone.removeNegativeSideFaceAtCoord(alco);
	
					//check if face agg now empty
					if (faXplusone.faceSetCount == 0) {
						
						removeFaceAggregatorAt(co + nudgeCoord, axis);	
					} else {
						// else get Face geom...
						faXplusone.getFaceGeometry(relevantComponent + 1);
					}
				}
			}
		} else {
			
			// * neighbor is air, need to add a face at this coord
			faXY.addFaceAtCoordBlockType(new FaceInfo(co, Block.MAX_LIGHT_LEVEL, relevantPosDir, btype, this.m_chunk.lightDataProvider));
			faXY.getFaceGeometry(relevantComponent);
		}
	}
	
		
	private static Direction posDirectionForAxis(Axis a) {
		if (a == Axis.X)
			return Direction.xpos;
		if (a == Axis.Y)
			return Direction.ypos;
		return Direction.zpos;
	}
	
	private static Axis axisForDirection(Direction dir) {
		// cases for different face aggs.
		if (dir <= Direction.xneg) {
			return Axis.X;
		}
		if (dir <= Direction.yneg) {
			return Axis.Y;
		}
		return Axis.Z;
	}
	
	#endregion
	
	private MeshSet collectMeshDataWithFaceAggregatorsDontRecalculate(FaceAggregator[] faceAggs, ref int starting_tri_index)
	{
		return collectMeshDataWithFaceAggregators(faceAggs, ref starting_tri_index, false);
	}
	
	private MeshSet collectMeshDataWithFaceAggregators(FaceAggregator[] faceAggs, ref int starting_tri_index)
	{
		return collectMeshDataWithFaceAggregators(faceAggs, ref starting_tri_index, true);
	}

	private MeshSet collectMeshDataWithFaceAggregators(FaceAggregator[] faceAggs, ref int starting_tri_index, bool wantToRecalculate)
	{
		// TODO: keep track of the lowest and highest (vertical only) face aggs.
		// then avoid iterating over empty faceAggs.
		
		List<Vector3> temp_vertices = new List<Vector3>();
		List<Vector2> temp_uvs = new List<Vector2>();
		List<int> temp_triangles = new List<int>();
		List<Color32> temp_col32s = new List<Color32>();
		List<Vector4> temp_v4s = new List<Vector4>();
		
		FaceAggregator fa;
		for (int i = 0; i < faceAggs.Length ; ++i)
		{
			fa = faceAggs[i];
			
			
			if (fa != null)
			{
				
				MeshSet mset;
				if (wantToRecalculate)
					mset = fa.getFaceGeometry(i);
				else
					mset = fa.meshSet;
				
				AssertUtil.Assert(mset.geometrySet.vertices != null, "huh? how did this get in here? face ag at index: " 
					+ i + " \n" + fa.toString());
				
				List<int> geomSetIndices = new List<int>(mset.geometrySet.indices); //copy list... //getting arg cannot be null here
				// when adding blocks (perhaps flush with other blocks?) but only sometimes...(only in flat world??)
				GeometrySet gset = new GeometrySet(geomSetIndices, mset.geometrySet.vertices);
				
				temp_vertices.AddRange(gset.vertices);
				int j = 0;
				for(; j < gset.indices.Count; ++j) {
					gset.indices[j] += starting_tri_index;
				}
				
				temp_triangles.AddRange(gset.indices);
				temp_uvs.AddRange(mset.uvs);
				temp_col32s.AddRange(mset.color32s);
				temp_v4s.AddRange(mset.tangents);
				
				
				fa.baseTriangleIndex = starting_tri_index; // for editing mesh (potentially)
				starting_tri_index += gset.vertices.Count;
			}
		}
		return new MeshSet(new GeometrySet(temp_triangles, temp_vertices), temp_uvs, temp_col32s, temp_v4s);
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
	
//	private void clearMeshArrays() {
//		triangles.Clear();
//		uvs.Clear();
//		vertices.Clear();
//		col32s.Clear();
//	}