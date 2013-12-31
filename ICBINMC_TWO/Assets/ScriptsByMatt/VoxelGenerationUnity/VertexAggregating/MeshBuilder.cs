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
	
	private Chunk m_chunk;
	
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

		return new MeshSet(new GeometrySet(triangles, vertices), uvs);
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
	
	private void editfaceAggregatorsByChangingBlockAtCoord(Coord co, bool add_block, BlockType b)
	{
		// for the x dir (say)
		// for co.x - 1 
		// is there a block at this coord? (and is x - 1 within our chunk?) (if not..don't edit it but we still want to know. whether it was air or not)
		
		Coord x_one = new Coord(1,0,0);
		Coord y_one = new Coord(0,1,0);
		Coord z_one = new Coord(0,0,1);
		
		FaceAggregator faZY = faceAggregatorsZY[co.x];
		FaceAggregator faXY = faceAggregatorsXY[co.z];
		FaceAggregator faXZ = faceAggregatorsXZ[co.y];
		
		// X
		editFaceAggregatorsAtCoordAndAxis(co, Axis.X, new AlignedCoord(co.z, co.y), new Coord(1,0,0), faceAggregatorsZY, add_block, b);
		// Y
		editFaceAggregatorsAtCoordAndAxis(co, Axis.Y, new AlignedCoord(co.x, co.z), new Coord(0,1,0), faceAggregatorsXZ, add_block, b);
		// Z
		editFaceAggregatorsAtCoordAndAxis(co, Axis.Z, new AlignedCoord(co.x, co.y), new Coord(0,0,1), faceAggregatorsXY, add_block, b);
		
//		Block test_b;
//		test_b = this.m_chunk.blockAt(co - x_one);
//		if (test_b.type != BlockType.Air)
//		{
//			if (co.x > 0)
//			{
//				FaceAggregator faXminusOne = this.faceAggregatorsZY[co.x - 1];
//				AlignedCoord alcoZY = new AlignedCoord(co.z, co.y);
//				
//				if (add_block) // take away x_pos face
//				{
//					faXminusOne.removeBlockFaceAtCoord(alcoZY, true, false);
//				} else {
//					faXminusOne.addFaceAtCoordBlockType(co, test_b.type, Direction.xneg); // x neg right (for the way we ar elooking at it)?
//				}
//			}
//		} else { // it is air at x - 1
//			
//			if (add_block)
//			{
//				faXY.addFaceAtCoordBlockType(co, b.type, Direction.xpos); // again xpos dir for neg side of block
//			} // else nothing : air next to air
//		}
		
		// 
		
		
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
				
				starting_tri_index += gset.vertices.Count;
			}
		}
	}
	
	
	private void collectMeshDataWithFaceAggregators(FaceAggregator[] faceAggs, ref int starting_tri_index)
	{
		FaceAggregator fa;
		for (int i = 0; i < faceAggs.Length ; ++i)
		{
			fa = faceAggs[i];

			
			if (fa != null)
			{
				fa.baseTriangleIndex = starting_tri_index;
				
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

public static class b
{
	public static void bug(string str) {
		UnityEngine.Debug.Log(str);	
	}
}
