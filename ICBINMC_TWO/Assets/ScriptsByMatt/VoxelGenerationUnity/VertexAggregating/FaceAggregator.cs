using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class FaceAggregator 
{
	private int[,] faceSetTable = new int[(int) ChunkManager.CHUNKLENGTH,(int) ChunkManager.CHUNKLENGTH]; // would have to change for xy or zy type aggregator
	
	private List<FaceSet> faceSets = new List<FaceSet> ();
	
	public FaceAggregator() 
	{
		// face type assumed to be xz (for now)
	}
	
	public void addFaceAtCoordBlockType(Coord coord, BlockType type)
	{
		if (coord.x != 0)
		{
			if (addCoordToFaceSetAtCoord(coord, coord.xMinusOne(), type) )
				return;
		}
		if (coord.z != 0)
		{
			if (addCoordToFaceSetAtCoord(coord, coord.zMinusOne(), type) )
				return;
		}
		
		// ok make a new face set...
		newFaceSetAtCoord(coord, type);
	}
	
	private bool addCoordToFaceSetAtCoord(Coord addMeCoord, Coord adjacentCoord, BlockType type)
	{
		// if there is a face set here
		int faceSetIndex = indexOfFaceSetAtCoord(adjacentCoord);
		if (faceSetIndex > -1)
		{
			FaceSet fset = faceSets[faceSetIndex];
			// if it matches the type we want, add
			if (type == fset.blockType)
			{
				fset.addCoord(addMeCoord); //, adjacentCoord);
				return true;
			}
		}
		
		return false;
	}
	
	private int indexOfFaceSetAtCoord(Coord coord)
	{
		// assume we want the xz plane for now
		return faceSetTable[coord.x, coord.z] - 1;
	}
	
	private void newFaceSetAtCoord(Coord coord, BlockType type)
	{
		int faceSetsCount = faceSets.Count;
		FaceSet fs = new FaceSet(type);
		faceSets.Add (fs);
		
		faceSetTable[coord.x, coord.z] = faceSetsCount;
	}
	
	public GeometrySet getFaceGeometry(float verticalHeight)
	{
		List<Vector3> resVecs = new List<Vector3>();
		List<int> resTriIndices = new List<int>();
		GeometrySet geomset; // = new GeometrySet();
		
		
		foreach(FaceSet fset in faceSets)
		{
			geomset = fset.CalculateGeometry(verticalHeight);
			resVecs.AddRange(geomset.vertices);
			resTriIndices.AddRange(geomset.indices);
		}
		
		return new GeometrySet(resTriIndices, resVecs);
	}
	
		
}
