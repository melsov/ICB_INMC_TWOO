using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshBuilder 
{
	// handles arrays of verts, uvs and trianges
	// handles inserting into arrays.
	private List<Vector3> vertices;
	private List<Vector2> uvs;
	private List<int> triangles;
	
	private FaceAggregator[] faceAggregatorsXZ = new FaceAggregator[Chunk.CHUNKHEIGHT];
	private FaceAggregator[] faceAggregatorsXY = new FaceAggregator[Chunk.CHUNKLENGTH];
	private FaceAggregator[] faceAggregatorsZY = new FaceAggregator[Chunk.CHUNKLENGTH];
	
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
		setFaceAggrefatorsAt(fa, co, dir);	
	}
	
	private FaceAggregator faceAggregatorAt(Coord co, Direction dir)
	{
		// TODO: add switch case for different face aggs.
		
		if (faceAggregatorsXZ[co.y] == null)
		{
			faceAggregatorsXZ[co.y] = new FaceAggregator();	
		}
		
		return faceAggregatorsXZ[co.y];
	}
	
	private void setFaceAggrefatorsAt(FaceAggregator fa, Coord co, Direction dir)
	{
		int index = co.y; // TODO: which coord based on dir
		
		//switch...
		faceAggregatorsXZ[index] = fa;
	}
	
	
}
