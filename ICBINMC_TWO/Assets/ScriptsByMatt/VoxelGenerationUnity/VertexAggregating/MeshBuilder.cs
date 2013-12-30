using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshBuilder 
{
	// handles arrays of verts, uvs and trianges
	// handles inserting into arrays.
	private List<Vector3> vertices = new List<Vector3>();
	private List<Vector2> uvs = new List<Vector2>();
	private List<int> triangles = new List<int>();
	
	private FaceAggregator[] faceAggregatorsXZ = new FaceAggregator[Chunk.CHUNKHEIGHT];
	private FaceAggregator[] faceAggregatorsXY = new FaceAggregator[Chunk.CHUNKLENGTH];
	private FaceAggregator[] faceAggregatorsZY = new FaceAggregator[Chunk.CHUNKLENGTH];
	
//	private FaceAggregator[][] faceAggregatorCollection;
	
	private GameObject chunkGameObject;
	
//	public MeshBuilder(GameObject chunkGameObject_) {
//		this.chunkGameObject = chunkGameObject_;
//	} // later
	
	public MeshBuilder() {}
	
//	public MeshSet insertBlockAtCoord(Coord co)
//	{
////		return new MeshSet(	
//	}
	
	public void addCoordToFaceAggregatorAtIndex(Coord co, BlockType type, Direction dir)
	{
		FaceAggregator fa = faceAggregatorAt(co, dir );
		fa.addFaceAtCoordBlockType(co, type, dir );
//		faceAggregators[co.y] = fa; // put back...
		setFaceAggregatorsAt(fa, co, dir);	
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
//		faceAggregatorsXZ[index] = fa;
	}
	
	private void clearMeshArrays() {
		triangles.Clear();
		uvs.Clear();
		vertices.Clear();
	}
	
	public MeshSet compileGeometry(ref int starting_tri_index) 
	{
		clearMeshArrays();
		
		foreach(FaceAggregator[] faceAggs in getFaceAggCollection())
		{
			collectMeshDataWithFaceAggregators(faceAggs, ref starting_tri_index);	
		}
//		FaceAggregator fa;
//		for (int i = 0; i < faceAggregatorsXZ.Length ; ++i)
//		{
//			fa = faceAggregatorsXZ[i];
//			if (fa != null)
//			{
//				MeshSet mset = fa.getFaceGeometry(i);
//				GeometrySet gset = mset.geometrySet;
//				
//				vertices.AddRange(gset.vertices);	
//				
//				for(int j = 0; j < gset.indices.Count; ++j) {
//					gset.indices[j] += starting_tri_index;
//				}
//				
//				triangles.AddRange(gset.indices);
//				
//				starting_tri_index += gset.vertices.Count;
//				
//				uvs.AddRange(mset.uvs);
//			}
//		}
		return new MeshSet(new GeometrySet(triangles, vertices), uvs);
	}
	
//	public MeshSet newMeshSetByRemovingBlockAtCoord(Coord co)
//	{
//			
//	}
//	
//	public MeshSet newMeshSetByAddingBlockAtCoord(Coord co, BlockType btype)
//	{
//			
//	}
	
	
	
	private void collectMeshDataWithFaceAggregators(FaceAggregator[] faceAggs, ref int starting_tri_index)
	{
		FaceAggregator fa;
		for (int i = 0; i < faceAggs.Length ; ++i)
		{
			fa = faceAggs[i];
			if (fa != null)
			{
				MeshSet mset = fa.getFaceGeometry(i);
				GeometrySet gset = mset.geometrySet;
				
				vertices.AddRange(gset.vertices);	
				
				for(int j = 0; j < gset.indices.Count; ++j) {
					gset.indices[j] += starting_tri_index;
				}
				
				triangles.AddRange(gset.indices);
				uvs.AddRange(mset.uvs);
				
				starting_tri_index += gset.vertices.Count;
			}
		}
	}
	
	
}
