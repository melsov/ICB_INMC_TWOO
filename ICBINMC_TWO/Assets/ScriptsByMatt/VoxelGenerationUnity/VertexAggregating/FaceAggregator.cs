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
	private int[,] faceSetTable; 
	private PTwo faceSetTableHalfDims;
	
	private List<FaceSet> faceSets = new List<FaceSet> ();
	
	private const int FACETABLE_LOOKUP_SHIFT = ChunkManager.CHUNKHEIGHT * 88;
	
	private Axis faceNormal;
	
	public int baseTriangleIndex = 0;
	public MeshSet meshSet;
	
	public FaceAggregator(Axis faceNormalAxis) 
	{
		// face type assumed to be xz (for now)
		
		int across_dim =(int) ChunkManager.CHUNKLENGTH;
		int up_dim =(int) ChunkManager.CHUNKHEIGHT;
		
		if (faceNormalAxis == Axis.Y)
		{
			up_dim = (int)ChunkManager.CHUNKLENGTH;
		}
		faceSetTableHalfDims = new PTwo(across_dim, up_dim);
		faceSetTable = new int[faceSetTableHalfDims.s * 2, faceSetTableHalfDims.t ];
		
		faceNormal = faceNormalAxis;
	}
	
	public static FaceAggregator FaceAggregatorForXFaceNormal()
	{
		return new FaceAggregator(Axis.X);
	}
	
	public static FaceAggregator FaceAggregatorForZFaceNormal()
	{
		return new FaceAggregator(Axis.Z);
	}
	
	public static FaceAggregator FaceAggregatorForYFaceNormal()
	{
		return new FaceAggregator(Axis.Y);
	}

	private AlignedCoord alignedCoordFromCoord(Coord co) {
		if (faceNormal == Axis.X)
			return new AlignedCoord(co.z, co.y);
		if (faceNormal == Axis.Y)
			return new AlignedCoord(co.x, co.z);

		return new AlignedCoord(co.x, co.y);
	}
	
	private int acrossIndexFromCoord(Coord co) {
		return alignedCoordFromCoord(co).across;
	}
	
	public void addFaceAtCoordBlockType(FaceInfo faceInfo)
	{
		// TODO: raise exception if this seems to be the wrong dir...
		AlignedCoord alco = alignedCoordFromCoord(faceInfo.coord);
		BlockType type = faceInfo.blockType;
		Direction dir = faceInfo.direction;
		
		if (alco.across != 0)
		{
			if (addCoordToFaceSetAtCoord(alco, alco.acrossMinusOne(), type, dir, faceInfo.lightLevel) )
				return;
		}
		if (alco.up != 0)
		{
			if (addCoordToFaceSetAtCoord(alco, alco.upMinusOne(), type, dir, faceInfo.lightLevel) )
				return;
		}
		
		// ok we need a new face set...
		newFaceSetAtCoord(alco, type, dir, faceInfo.lightLevel, faceInfo.lightDataProvider);
	}
	
	#region add ranges of faces
	
	public void addFaceInfoRange(FaceInfo faceInfo) 
	{
		faceInfo.range.assertNotNull("Ersatz null range for face info in face agg. add fa range");
		
		int acrossI = acrossIndexFromCoord(faceInfo.coord);
		
		if (acrossI > 0)
			addToFaceSetsAt(acrossI - 1, faceInfo);
		else 
			addNewFaceSetsWith(acrossI, faceInfo);
	}
	
	private void addToFaceSetsAt(int nextToRangeIndex, FaceInfo finfo )
	{
		Range1D addRange = finfo.range;
		Direction dir = finfo.direction;
		
		AssertUtil.Assert(nextToRangeIndex < this.faceSetTableHalfDims.s - 1 , "need next to range to be in our table minus one column");
		
		foreach(int faceSetIndex in faceSetsAtAcrossIndexFlushWithRange(nextToRangeIndex, addRange, dir))
		{
			FaceSet nextToFS = faceSets[faceSetIndex];
			if (nextToFS.canIncorporatePartOrAllOfRange(addRange, nextToRangeIndex + 1))
			{
				Range1D usedRange = nextToFS.addRangeAtAcrossReturnAddedRange(addRange, nextToRangeIndex + 1);
				
				AssertUtil.Assert(usedRange.range > 0, "confusing used range was zero or less? (in face agg add fs w range)");
				
				setIndicesOfFaceSetsAtRangeToIndex(usedRange, nextToRangeIndex + 1, dir, faceSetIndex + FaceAggregator.FACETABLE_LOOKUP_SHIFT);
				
				Range1D unusedRangeBelow = addRange.subRangeBelowRange(usedRange);
				if (!unusedRangeBelow.isErsatzNull())
				{
//					addNewFaceSetsWith(nextToRangeIndex + 1, addRange, dir);
					addNewFaceSetsWith(nextToRangeIndex + 1, finfo);
				}
				
				Range1D unusedAbove = addRange.subRangeAboveRange(usedRange);
				if (!unusedAbove.isErsatzNull())
				{
					finfo.range  = unusedAbove;
//					addToFaceSetsAt(nextToRangeIndex, unusedAbove, dir);
					addToFaceSetsAt(nextToRangeIndex, finfo);
				}
				return;
			}
		}
		
		addNewFaceSetsWith(nextToRangeIndex + 1, finfo);
		
	}
	
	private void addNewFaceSetsWith(int acrossI, FaceInfo finfo )
	{
		Range1D addRange = finfo.range;
		Direction dir = finfo.direction;
		AlignedCoord startAlco = new AlignedCoord(acrossI, addRange.start);
		
		int fsIndex = indexOfFaceSetAtCoord(startAlco, dir);
		
		if (!indexRepresentsAnOccupiedCoord(fsIndex))
			newFaceSetAtCoord(startAlco, addRange.blockType, dir, addRange.bottom_light_level, finfo.lightDataProvider);
		
		FaceSet justAddedFS = faceSetAt(startAlco, dir);
		
		Range1D usedRange = justAddedFS.addRangeAtAcrossReturnAddedRange(addRange, acrossI);
		int nowOccupiedFS = unshiftedIndexOfFaceSetAtCoord(startAlco, dir);
		
		AssertUtil.Assert(nowOccupiedFS > -1, "wha negative occupied coord? " + nowOccupiedFS);
		
		setIndicesOfFaceSetsAtRangeToIndex(usedRange, acrossI, dir, nowOccupiedFS );
		
		AssertUtil.Assert(addRange.subRangeBelowRange(usedRange).isErsatzNull(), "we thought there would be no range below. whoops");
		
		Range1D unusedRangeAbove = addRange.subRangeAboveRange(usedRange);
		if (!unusedRangeAbove.isErsatzNull()) 
		{
			if (unusedRangeAbove.range > 0)
			{
				finfo.range = unusedRangeAbove;
				addNewFaceSetsWith(acrossI, finfo);
			}
		}
	}
	
	#endregion
	
	private bool addCoordToFaceSetAtCoord(AlignedCoord addMeCoord, AlignedCoord adjacentCoord, BlockType type, Direction dir, byte lightLevel)
	{
		// if there is a face set here
		int faceSetIndex = indexOfFaceSetAtCoord(adjacentCoord, dir);
		
		if (indexRepresentsAnOccupiedCoord(faceSetIndex))
		{
			FaceSet fset = faceSets[faceSetIndex];
			// if it matches the type we want, add
			if (type == fset.blockType && fset.blockFaceDirection == dir)
			{
				//beyond limit for this face set?
				if (coordIsBeyondFaceSetMaxAllowArea(fset, addMeCoord))
				{
//					b.bug("need a new faceset at coord: " + addMeCoord.toString() + "face set limits: " + fset.getFaceSetLimits().toString() + " direction: " + dir);
					return false;
				}
				
				fset.addCoord(addMeCoord, lightLevel); //, adjacentCoord);
				copyFaceSetIndexFromToCoord(adjacentCoord, addMeCoord, dir);
				return true;
			}

		}
		return false;
	}
	
	private static bool coordIsBeyondFaceSetMaxAllowArea(FaceSet fset, AlignedCoord alco) 
	{
		Quad currentLimits = fset.getFaceSetLimits();
		if (alco.across - currentLimits.origin.s >= FaceSet.MAX_DIMENSIONS.s)
			return true;
		if (alco.up - currentLimits.origin.t >= FaceSet.MAX_DIMENSIONS.t)
			return true;
		if (currentLimits.extentMinusOne().s - alco.across >= FaceSet.MAX_DIMENSIONS.s)
			return true;
		if (currentLimits.extentMinusOne().t - alco.up >= FaceSet.MAX_DIMENSIONS.t)
			return true;
		return false;
	}
	
	private void removeFaceSetAtCoord(AlignedCoord alco, Direction dir) {
		int index = indexOfFaceSetAtCoord(alco, dir);
		
		//NOTE: problems will/should arise if this face set has more than one face!
		// only remove < 1 face faceSet please!
		
		if (index < faceSets.Count)
			faceSets.RemoveAt(index);
		
		//TODO: decrement the faceSetTable for greater than index
		adjustLookupTableForRemovalOfIndex(index);
		
		//set the index in the table to zero	
		setIndexOfFaceSetAtCoord(0, alco, dir);
	}
	
	#region get face sets in up columns
	
	private IEnumerable faceSetsAtAcrossIndexFlushWithRange(int acrossI, Range1D range, Direction dir)
	{
		int currentFSI = -1;
		
		for(int j = range.start; j < range.extent() ; ++j)
		{
			if (j < 0 )
				continue;
			
			if( j > faceSetTableHalfDims.t)
				break;
			
			AlignedCoord curAlco = new AlignedCoord(acrossI, j);
			int fsindex = indexOfFaceSetAtCoord(curAlco, dir);
			
			if (fsindex >= 0 && fsindex != currentFSI)
			{
				currentFSI = fsindex;
				yield return fsindex;
			}
		}
	}
	
	#endregion
	// TODO: really make an index lookup table class
	private void setIndicesOfFaceSetsAtRangeToIndex(Range1D range, int acrossI, Direction dir, int indexToSetTo)
	{
		for (int i = range.start; i < range.extent(); ++i) {
			setIndexOfFaceSetAtCoord( indexToSetTo, new AlignedCoord(acrossI, i), dir);
		}
	}
	
	private void setIndexOfFaceSetAtCoord(int _index, AlignedCoord coord, Direction dir)
	{
		int nudge_lookup = ((int) dir % 2 == 0) ? 0 : 1; // pos dirs are 0, 2 and 4
		
		try {
			faceSetTable[coord.across * 2 + nudge_lookup, coord.up] = _index; // - FaceAggregator.FACETABLE_LOOKUP_SHIFT;
		} 
		catch(IndexOutOfRangeException e)
		{
			throw new Exception("this index was: across " + (coord.across * 2 + nudge_lookup) + " up: " + coord.up + ". face table length was: dim 0: " +faceSetTable.GetLength(0) + " 1 " + faceSetTable.GetLength(1) + " coord was " + coord.toString() + "Direction was: " + dir + " my face axis is: " + faceNormal);
		}
	}
	
	private int unshiftedIndexOfFaceSetAtCoord(AlignedCoord coord, Direction dir)
	{
		return indexOfFaceSetAtCoord(coord, dir) + FaceAggregator.FACETABLE_LOOKUP_SHIFT;
	}
	
	private int indexOfFaceSetAtCoord(AlignedCoord coord, Direction dir)
	{
		int nudge_lookup = ((int) dir % 2 == 0) ? 0 : 1; // pos dirs are 0, 2 and 4
		
		int ret;
		try {
			ret = faceSetTable[coord.across * 2 + nudge_lookup, coord.up] - FaceAggregator.FACETABLE_LOOKUP_SHIFT;
		} 
		catch(IndexOutOfRangeException e)
		{
			throw new Exception("this index was: across " + (coord.across * 2 + nudge_lookup) + " up: " + coord.up + ". face table length was: dim 0: " +faceSetTable.GetLength(0) + " 1 " + faceSetTable.GetLength(1) + " coord was " + coord.toString() + "Direction was: " + dir + " my face axis is: " + faceNormal);
		}
		return ret;
	}
	
	private void adjustLookupTableForRemovalOfIndex(int index) {
		int shifted_index = index + FaceAggregator.FACETABLE_LOOKUP_SHIFT;
		
		if (index < 0) throw new Exception("adjusting lookup table. index < 0??!!");
		
		int j = 0;
		for (int i = 0 ; i < faceSetTable.GetLength(0) ; ++i) {
			for( j = 0; j < faceSetTable.GetLength(1) ; ++j) {
				if (faceSetTable[i,j] > shifted_index) {
					faceSetTable[i,j] = faceSetTable[i,j] - 1;	
				}
			}
		}
	}
	
	private bool indexRepresentsAnOccupiedCoord(int index) {
		return index > -1 && index < faceSets.Count;
	}
	
	private void newFaceSetAtCoord(AlignedCoord coord, BlockType type, Direction dir, byte lightLevel, ILightDataProvider lightDataProvider)
	{
		//TODO: figure out what we should really to in this case.
		// maybe some kind of look up table re: which block type wins?
		
		int currentLookupIndex = indexOfFaceSetAtCoord(coord, dir);
		
		if (indexRepresentsAnOccupiedCoord(currentLookupIndex)) //
		{
			// remove
//			b.bug("represents an occupied coord: cur lookup index: " + currentLookupIndex + " direction: " + dir);
			
			FaceSet currentOccupantFaceSet = faceSets[currentLookupIndex];
			currentOccupantFaceSet.removeFaceAtCoord(coord); // hang on to your hats! (no bugs please!)
		}
		
//		b.bug("adding a new face set at coord: " + coord.toString() );

		int faceSetsCount = faceSets.Count;
		FaceSet fs = new FaceSet(type, dir, coord, lightLevel, lightDataProvider);
		faceSets.Add (fs);
		
		int nudge_lookup = ((int) dir % 2 == 0) ? 0 : 1; // pos dirs are 0, 2 and 4
		faceSetTable[coord.across * 2 + nudge_lookup, coord.up] = faceSetsCount + FaceAggregator.FACETABLE_LOOKUP_SHIFT;
	}
	
	private void copyFaceSetIndexFromToCoord(AlignedCoord fromC, AlignedCoord toC, Direction dirForBothCoords) 
	{
		int indexFrom = indexOfFaceSetAtCoord(fromC, dirForBothCoords);
		
		//WE THINK WE SHOULD ADD THIS HERE....
		int nudge_across = ((int)dirForBothCoords % 2 == 0) ? 0 : 1;
		
		faceSetTable[toC.across * 2 + nudge_across, toC.up] = indexFrom + FaceAggregator.FACETABLE_LOOKUP_SHIFT;
		
		//for some reason it was this way before....
//		faceSetTable[toC.across , toC.up] = indexFrom + FaceAggregator.FACETABLE_LOOKUP_SHIFT;
	}
	
	private FaceSet faceSetAt(AlignedCoord alco, Direction dir) {
		int index = indexOfFaceSetAtCoord(alco, dir);
//		bug ("got the index: " + index);
		
		if (index < 0)
			return null;
		
		try {
			return faceSets[index];
		} catch(ArgumentOutOfRangeException e) {
			throw new Exception("index was out of range: index was: " + index + " aligned coord: " + alco.toString() + "direction: " + dir);
		}
		return null;
	}
	
	private Direction direction(bool positive) {
		Direction result = Direction.xneg;
		if (faceNormal == Axis.Y)
			result = Direction.yneg;
		else if (faceNormal == Axis.Z)
			result = Direction.zneg;
		
		return (Direction)((int)result - (positive? 1 : 0));		
	}
	
	public int totalVertices() {
		int result = 0;
		foreach(FaceSet fs in faceSets) {
			result += fs.vertexCount();	
		}
		
		return result;
	}
	
	public void removeBothPositiveAndNegativeFacesAtCoord(AlignedCoord alco) {
		removeBlockFaceAtCoord(alco, true, true);
	}
	
	public void removePositiveSideFaceAtCoord(AlignedCoord alco) {
		removeBlockFaceAtCoord(alco, true, false);	
	}
	
	public void removeNegativeSideFaceAtCoord(AlignedCoord alco) {
		removeBlockFaceAtCoord(alco, false, true);	
	}
	
	private void removeBlockFaceAtCoord(AlignedCoord alco, bool removePosFace, bool removeNegFace)
	{
		if (removePosFace)
		{
			FaceSet posFaceSet = faceSetAt(alco, direction(true) );
			
			if (posFaceSet != null)
			{
				posFaceSet.removeFaceAtCoord(alco);
				//now empty?
				if (posFaceSet.faceCount == 0) {
					removeFaceSetAtCoord(alco, direction(true) );	
				}
			} else {
				throw new Exception("why? trying to remove a face set that was null already?");	
			}
		}
		
		if (removeNegFace)
		{
			FaceSet negFaceSet = faceSetAt(alco, direction(false));
			
			if (negFaceSet != null)
			{
				negFaceSet.removeFaceAtCoord(alco);
				if (negFaceSet.faceCount == 0) {
					removeFaceSetAtCoord(alco, direction(false));	
				}
			} else {
				throw new Exception("why? trying to remove a face set that was null already?");
			}
		}
	}
	
	public int faceSetCount {
		get {
			return faceSets.Count;	
		}
	}
	
	private MeshSet newMeshSetByRemovingBlockAtCoordDONTCALL(AlignedCoord alco, float verticalHeight)
	{
		
		throw new Exception("don't call this anymore");
		
		int pos_vert_count_before = 0;
		int neg_vert_count_before = 0;
		
		int pos_vert_count_after = 0; 
		int neg_v_count_after = 0; 
		
		int pos_change = 0;
		int neg_change = 0;
		
		MeshSet mset = MeshSet.emptyMeshSet();
		
		FaceSet posFaceSet = faceSetAt(alco, direction(true) );
		
		if (posFaceSet != null)
		{
			pos_vert_count_before = posFaceSet.vertexCount();
			posFaceSet.removeFaceAtCoord(alco);
			pos_vert_count_after = posFaceSet.vertexCount();
		}
		
		FaceSet negFaceSet = faceSetAt(alco, direction(false));
		
		if (negFaceSet != null)
		{
			neg_vert_count_before = negFaceSet.vertexCount();
			negFaceSet.removeFaceAtCoord(alco);
			neg_v_count_after = negFaceSet.vertexCount();
		}
		
		// rebuild the all facesets (instead, for now of trying to splice the verts/i/uv arrays)
		mset = getFaceGeometry(verticalHeight);
		
		pos_change = pos_vert_count_after - pos_vert_count_before;
		neg_change = neg_v_count_after - neg_vert_count_before;
		
		mset.deltaVertexCount = pos_change + neg_change;
		
		return mset;
	}
	
//	public MeshSet newMeshSetByAddingBlockAtCoord(AlignedCoord alco, float verticalHeight)
//	{
//		
////		FaceSet fs = 
//	}
	
	public MeshSet getFaceGeometry(float verticalHeight)
	{
		List<Vector3> resVecs = new List<Vector3>();
		List<int> resTriIndices = new List<int>();
		List<Vector2> resUVs = new List<Vector2>();
//		List<Vector2> resColors = new List<Vector2>();
		List<Color32> resCol32s = new List<Color32>();
		List<Vector4> resV4s = new List<Vector4>();
		
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
			resCol32s.AddRange(mset.color32s);
			resV4s.AddRange(mset.tangents);
		}

		MeshSet ret_mset = new MeshSet( new GeometrySet(resTriIndices, resVecs), resUVs, resCol32s, resV4s);
		this.meshSet = ret_mset;
		return ret_mset;
	}
	
	#region get light level tanges
	
	//TODO: reset  light level tangents within...?
	// INVESTIGATE WHY LIGHT LEVELS DON'T COMPLETELY UPDATE SOMETIMES...
	
//	public List<Vector4> recalculateLightLevelTangents(int normalHeight)
//	{
//		
//	}
//	
//	public List<Vector4> recalculateLightLevelTangents(int normalHeight)
//	{
//		
//	}
	
	public List<Vector4> recalculateLightLevelTangents(int normalHeight)
	{
		List<Vector4> result = new List<Vector4>();
		
		foreach(FaceSet fs in faceSets)
		{
			result.AddRange(fs.recalculateTangents(normalHeight));	
		}
		
//		assertMeshSetElementsNotNull(); //DBG
		
		return result;
	}
	
	#endregion
	
	#region debugging
	
	public void assertMeshSetElementsNotNull() {
		AssertUtil.Assert(meshSet.geometrySet.vertices != null, "whoops verts null");
		AssertUtil.Assert(meshSet.geometrySet.indices != null, "whoops verts null");
	}
	
	public string toString() {
//		bool vertsNull = (meshSet.geometrySet.vertices == null);
//		if (vertsNull)
//			throw new Exception("hold the phone. vertices are null");
		string faceCoords = "FaceSet faceSetLimits: ";
		foreach(FaceSet fs in faceSets) {
			faceCoords += "\n" + fs.getFaceSetLimits().toString();	
		}
		string result = "FaceAgg: Face normal Axis: " + faceNormal;
		result += " face set count: " + faceSets.Count;
		result += faceCoords;
//		result += " vert count: " + meshSet.geometrySet.vertices.Count;
				
		return  result;
	}
	
	public void LogFaceSets() {
		for (int i=0; i< faceSets.Count; ++i) {
			FaceSet fs = faceSets[i];
			if (fs != null) {
//				bug("FACE AGGRTR: got a face set at: " + i);	
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
				b.bug("geom for faceSet: " + i);
				MeshSet mset = fs.CalculateGeometry(4f);
				
				result.Add(mset);
				faceSets[i] = fs; // need this?
				
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
	public Coord[] testCoords;
	public LightDataProvider lightDataProviderFake = new LightDataProvider(new Chunk());
	public FaceAggregator fa;
	
	private static Coord[] excludedCoords = new Coord[]{
		
//		new Coord(0, 0, 0),
//		new Coord(0, 0, 1),
//		new Coord(1, 0, 0),
//		new Coord(0, 0, 5), // this one gets messed up....
//		new Coord(5, 0, 0), // no this one?
//		new Coord(2, 4, 3), 
//		new Coord(2, 4, 6),
//		new Coord(2, 4, 7),
//		new Coord(5, 4, 6),
//		new Coord(12, 4, 6),
//		new Coord(12, 4, 7),
//		new Coord(12, 4, 8),
//		
//		new Coord(8, 0, 0),
//		new Coord(10, 0, 0),
//		new Coord(12, 0, 0),
//		
//		new Coord(14, 4, 15),
//		new Coord(14, 4, 14),
//		new Coord(14, 4, 12),
//		
//		new Coord(15, 4, 15),
//		new Coord(15, 4, 14),
//		new Coord(15, 4, 12),

	};
	
	private static Coord[] removeCoords = new Coord[]{
		
		new Coord(1, 0, 0),
//		new Coord(0, 0, 7),
//		new Coord(1, 0, 2),
//		new Coord(5, 0, 9),
////		
//		new Coord(15, 4, 14),
//		new Coord(15, 4, 15),
//		
//		new Coord(13, 4, 8),
//		new Coord(13, 4, 6),
//		new Coord(13, 4, 7),

	};
	
	public FaceAggregatorTest()
	{
		bug ("BEGIN FACE AGGR TEST");
		setUpTest();
	}
	
	public void rangeModeSetUp(int coordDims1)
	{
		int rangeRange = 4;
		for(int i = 0; i < coordDims1; ++i)
		{
			int varyRangeBy = i % 2 == 0 ? 2 : 1;
			LightDataProvider lightDP = new LightDataProvider(new Chunk());
			
			FaceInfo faceinfo = new FaceInfo(new Coord(i, 4, 0), new Range1D(0, rangeRange - varyRangeBy ), Direction.ypos, lightDP);
			fa.addFaceInfoRange(faceinfo);
		}
	}
	
	public void setUpTest()
	{
		fa = new FaceAggregator(Axis.Y);
		
		int COORD_DIMS1 = 5, COORD_DIMS2 = 1;
		int coord_count = COORD_DIMS1 * COORD_DIMS2;
//		int i = COORD_DIMS * 0; // TODO: fix bug (if it matters which it probably does): if i > 0 we get a trying to add a face where there already was one
		// the face is at aligned coord: 0,0.
		Coord[] coords = new Coord[coord_count];
//		for(; i < coord_count ; ++i)
//		{
//			int z = i % COORD_DIMS;
//			int x = i / COORD_DIMS;
//			coords[i] = new Coord(x, 4, z);
//		}
		
		rangeModeSetUp(COORD_DIMS1);
		return;
		// ******************** //
		
		for(int j = 0; j < COORD_DIMS1 ; ++j) {
			for (int k = 0; k < COORD_DIMS2 ; ++k) {
				coords[j * COORD_DIMS2 + k] = new Coord(j , 4, k);
			}
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
			} //dont remove for now
			
			if (include) {
				FaceInfo faceinfo = new FaceInfo(co, Block.MAX_LIGHT_LEVEL, Direction.ypos , BlockType.Grass, lightDataProviderFake);
				fa.addFaceAtCoordBlockType(faceinfo);	
			}

		}
		
		
		
//		fa.LogFaceSets();
//		bug (fa.faceSetTableToString());
		
//		fa.LogMeshResults();
	}
	
	public List<MeshSet> getMeshResults()
	{
		 // calling this twice produces odd results: mesh loses some quads.
		/*
		List<MeshSet> ret_before = fa.getMeshResults();
		
		int vertsbefore = fa.totalVertices();
		bug ("verts before: " + vertsbefore);
		
		foreach (Coord removeco in removeCoords)
		{
			fa.removeBothPositiveAndNegativeFacesAtCoord(new AlignedCoord(removeco.x, removeco.z) );
		}
		
		b.bug("###NEW RESULTS AFTER REMOVING COORDS###");
		 */
		List<MeshSet> ret = fa.getMeshResults();
		
		bug("verts after: " + fa.totalVertices() );
		return ret;
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