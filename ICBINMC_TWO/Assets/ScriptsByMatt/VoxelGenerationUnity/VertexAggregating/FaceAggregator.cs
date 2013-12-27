using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using System.IO;
using System;
//using System.Security.Cryptography.X509Certificates;
//using System.Security.Cryptography;
//using System.Runtime.ConstrainedExecution;
using System.Diagnostics;

//using System.Runtime.Serialization.Formatters.Binary;
//using System.Runtime.Serialization;


//using System.ComponentModel;
using System.Threading;
using System.Collections.Specialized;
//using System.Runtime.InteropServices;

//there's a FaceAggregator object.
//it knows its vertices index.
// and its vertices count (and hence its indices index and count)
// 
// Also, chunks have a MeshConstructor object
// holds onto the vertices, ind, uvs lists.
// can insert sets of verticies into the verts.
// and then it will also increment the indices 
// on the above side of the insertion point accordingly.

public class FaceAggregator 
{
	private int[,] faceSetTable = new int[(int) ChunkManager.CHUNKLENGTH * 2,(int) ChunkManager.CHUNKLENGTH]; // would have to change for xy or zy type aggregator
	
	private List<FaceSet> faceSets = new List<FaceSet> ();
	
	private const int FACETABLE_LOOKUP_SHIFT = ChunkManager.CHUNKHEIGHT * 88;
	
	public FaceAggregator() 
	{
		// face type assumed to be xz (for now)
		
	}

	
	public void addFaceAtCoordBlockType(Coord coord, BlockType type, Direction dir)
	{
		if (coord.x != 0)
		{
			if (addCoordToFaceSetAtCoord(coord, coord.xMinusOne(), type, dir) )
				return;
		}
		if (coord.z != 0)
		{
			if (addCoordToFaceSetAtCoord(coord, coord.zMinusOne(), type, dir) )
				return;
		}
		
		// ok make a new face set...
		newFaceSetAtCoord(coord, type, dir);
	}
	
	private bool addCoordToFaceSetAtCoord(Coord addMeCoord, Coord adjacentCoord, BlockType type, Direction dir)
	{
		// if there is a face set here
		int faceSetIndex = indexOfFaceSetAtCoord(adjacentCoord, dir);
		
		
		if (faceSetIndex > -1)
		{
			FaceSet fset = faceSets[faceSetIndex];
			// if it matches the type we want, add
			if (type == fset.blockType && fset.blockFaceDirection == dir)
			{

				fset.addCoord(addMeCoord); //, adjacentCoord);
				copyFaceSetIndexFromToCoord(adjacentCoord, addMeCoord, dir);
				return true;
			}

		}
		return false;
	}

	private int indexOfFaceSetAtCoord(Coord coord, Direction dir)
	{
		int nudge_lookup = ((int) dir % 2 == 0) ? 0 : 1; // pos dirs are 0, 2 and 4
		
		// assume we want the xz plane for now
		return faceSetTable[coord.x * 2 + nudge_lookup, coord.z] - FaceAggregator.FACETABLE_LOOKUP_SHIFT;
	}
	
	private void newFaceSetAtCoord(Coord coord, BlockType type, Direction dir)
	{
		if (indexOfFaceSetAtCoord(coord, dir) > -1) //
//			bug ("trying to add a new face set where there already was one? " + coord.toString());
			throw new Exception("trying to add a new face set where there already was one? coord is: " + coord.toString() );
		
		int faceSetsCount = faceSets.Count;
		FaceSet fs = new FaceSet(type, dir, coord);
		faceSets.Add (fs);
		
		
		int nudge_lookup = ((int) dir % 2 == 0) ? 0 : 1; // pos dirs are 0, 2 and 4
		faceSetTable[coord.x * 2 + nudge_lookup, coord.z] = faceSetsCount + FaceAggregator.FACETABLE_LOOKUP_SHIFT;
	}
	
	private void copyFaceSetIndexFromToCoord(Coord fromC, Coord toC, Direction dirForBothCoords) {
		int indexFrom = indexOfFaceSetAtCoord(fromC, dirForBothCoords);
		faceSetTable[toC.x, toC.z] = indexFrom + FaceAggregator.FACETABLE_LOOKUP_SHIFT;
	}
	
	public MeshSet getFaceGeometry(float verticalHeight)
	{
		List<Vector3> resVecs = new List<Vector3>();
		List<int> resTriIndices = new List<int>();
		List<Vector2> resUVs = new List<Vector2>();
		GeometrySet geomset; // = new GeometrySet();
		MeshSet mset;
		
		int rel_tri_index = 0;
		
		foreach(FaceSet fset in faceSets)
		{
			mset = fset.CalculateGeometry(verticalHeight);
			geomset = mset.geometrySet;
			
			resVecs.AddRange(geomset.vertices);
			
			for(int i = 0; i < geomset.indices.Count ; ++i) {
				geomset.indices[i] += rel_tri_index;	
			}
			resTriIndices.AddRange(geomset.indices);
			
			rel_tri_index += geomset.vertices.Count;
			
			resUVs.AddRange(mset.uvs);
			
			
		}

		return new MeshSet( new GeometrySet(resTriIndices, resVecs), resUVs);
	}
	
	#region debugging
	
	public void LogFaceSets() {
		for (int i=0; i< faceSets.Count; ++i) {
			FaceSet fs = faceSets[i];
			if (fs != null) {
				bug("FACE AGGRTR: got a face set at: " + i);	
//				fs.logStripsArray();
//				bug (" quads: " + fs.getQuadsString());
			} else {
				bug("face set was null");
			}
		}
	}
	
	public void LogMeshResults()
	{
		string info = "";
		for (int i=0; i< faceSets.Count; ++i) 
		{
			FaceSet fs = faceSets[i];
			if (fs != null) 
			{
				bug("FACE AGGRTR: got a face set at: " + i);	
				MeshSet ms = fs.CalculateGeometry(4f);
				
				int count = 0;
				foreach (Vector3 vv in ms.geometrySet.vertices) 
				{
					info += "[" + vv.x + "," + vv.z + "]" ;	
					if (count % 4 == 3)
						info += "\n";
					++count;
				}
				
				bug (info);
				
			} else {
				bug("face set was null");
			}
		}
		
	}
	
	public List<MeshSet> getMeshResults()
	{
		List<MeshSet> result = new List<MeshSet>();
		for (int i=0; i< faceSets.Count; ++i) 
		{
			FaceSet fs = faceSets[i];
			if (fs != null) 
			{
				result.Add(fs.CalculateGeometry(4f));
				
			} else {
				bug("face set was null");
			}
		}
		return result;
	}
	
	public string faceSetTableToString() {
		string result = "";
		for (int i = 0; i < faceSetTable.GetLength(0) ; ++i) {
			for (int j = 0; j < faceSetTable.GetLength(1) ; ++j) 
			{
				int index = faceSetTable[i,j] ;
				string indexStr = "&&";
				if (index != 0) {
					index = index - FaceAggregator.FACETABLE_LOOKUP_SHIFT;
					if (index > 9)
						indexStr = "" + index;
					else
						indexStr = " " + index;
				}					
				indexStr += " |";
				result += indexStr;	
			}
			result += "\n";
		}
		return result;
	}
		
	
	private void bug(string ss) {
		UnityEngine.Debug.Log(ss);
	}
	
		private static Coord[] bugTheseCoords = new Coord[]{
		new Coord(0, 2, 0), 
		new Coord(2, 2, 3), 
		new Coord(0,2,1)
	};
	
	private static bool CoordMatchesADebugCoord(Coord co) {
		foreach (Coord dbco in FaceAggregator.bugTheseCoords) {
			if (dbco.x == co.x && dbco.z == co.z)
				return true;
		}
		return false;
	}
	
	private void zeroZeroBug(Coord co, string str) {
		if (!(co.x == 0 && co.z == 0) ) {
			return;
		}
		
		bug (str + " : the coord: " + co.toString());
	}
	
	private void coordOfInterestBug(Coord co, string str) {
		if (!FaceAggregator.CoordMatchesADebugCoord(co) ) {
			return;
		}
		
		bug (str + " : the coord: " + co.toString());
	}
	#endregion
		
}



public class FaceAggregatorTest
{
	public Coord[] testCoords = new Coord[256];
	public FaceAggregator fa;
	
	private static Coord[] excludedCoords = new Coord[]{
		
		new Coord(0, 0, 0),
		new Coord(0, 0, 1),
		new Coord(1, 0, 0),
		new Coord(0, 0, 5),
		new Coord(5, 0, 0),
		new Coord(2, 4, 3), 
		new Coord(2, 4, 6),
		new Coord(2, 4, 7),
		new Coord(5, 4, 6),
		new Coord(12, 4, 6),
		new Coord(12, 4, 7),
		new Coord(12, 4, 8),

	};
	
	public FaceAggregatorTest()
	{
		bug ("BEGIN FACE AGGR TEST");
		setUpTest();
	}
	
	public void setUpTest()
	{
		fa = new FaceAggregator();
		
		Coord[] coords = new Coord[256];
		for(int i = 0; i < 256 ; ++i)
		{
			int z = i % 16;
			int x = i / 16;
			coords[i] = new Coord(x, 4, z);
		}
		
		testCoords = coords;
		
		
		foreach (Coord co in coords) 
		{
			Coord[] excluded = FaceAggregatorTest.excludedCoords;
			
			bool include = true;
			foreach (Coord exclu in excluded) {
				if (exclu.x == co.x && exclu.z == co.z) {
					include = false;
					break;
				}
			}
			
			if (include)
				fa.addFaceAtCoordBlockType(co, BlockType.Grass, Direction.ypos);	
		}
		
		fa.LogFaceSets();
//		bug (fa.faceSetTableToString());
		
//		fa.LogMeshResults();
	}
	
	public List<MeshSet> getMeshResults()
	{
		return fa.getMeshResults();
	}
	
	private void bug(string strr) {
		UnityEngine.Debug.Log(strr);
	}
}



//Coord[] alt_coords = new Coord[] {
//			new Coord(0, 2, 0),
//			new Coord(0, 2, 1),
//			new Coord(0, 2, 2),
//			new Coord(0, 2, 3),
//			new Coord(0, 2, 4),
//			
//			new Coord(1, 2, 0),
//			new Coord(1, 2, 1),
//			new Coord(1, 2, 2),
//
//			new Coord(4, 2, 0),
//			new Coord(4, 2, 1),
//			
//			new Coord(4, 2, 6),
//			new Coord(5, 2, 6),
//			new Coord(6, 2, 6),
//			new Coord(7, 2, 6),
//		};