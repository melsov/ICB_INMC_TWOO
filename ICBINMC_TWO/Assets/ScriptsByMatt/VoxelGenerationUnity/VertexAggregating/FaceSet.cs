using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using System;



//public struct Face
//{
//	// has an xpos and zpos neighbor
////	Face * xPosNeighbor; //, zPosNeighbor; // these are unsafe (not allowed in this safe code world??)
//	
//	CTwo ctwo;
//	
//	//?? an enum (later an int to be looked at bitwise?) that tells which walls are intact
//	//for now just four bools
//	bool xnegWall, xposWall, znegWall, zposWall;
//	
//	public Face (CTwo _ctwo) {
////		xPosNeighbor = zPosNeighbor = null;
//		ctwo = _ctwo;
//		xnegWall = xposWall = znegWall = zposWall = true;
//	}
//}


public class FaceSet  
{

	public BlockType blockType;
	public Direction blockFaceDirection;
	
	private List<Strip>[] stripsArray = new List<Strip>[(int)ChunkManager.CHUNKLENGTH];
	
	private Quad faceSetLimits = Quad.theErsatzNullQuad();
	private List<Quad> quads = new List<Quad>();
	
	private int[,] quadTable = new int[(int) ChunkManager.CHUNKLENGTH, (int) ChunkManager.CHUNKLENGTH];
	
	private bool[,] filledFaces = new bool[(int) ChunkManager.CHUNKLENGTH, (int) ChunkManager.CHUNKLENGTH];
	
	List<VertexTwo>[] vertexColumns = new List<VertexTwo>[(int) ChunkManager.CHUNKLENGTH + 1];
	
	private int iterationSafety = 0;
	
	private const int SPECIAL_QUAD_LOOKUP_NUMBER = (int)(ChunkManager.CHUNKLENGTH * ChunkManager.CHUNKLENGTH * 805);
	
	
	
//	private static int MAX_FACES;
	
//	private int internalTriIndex = 0;
	
	public FaceSet(BlockType _type, Direction _dir, Coord initialCoord)
	{
		blockType = _type;	
		blockFaceDirection = _dir;
		
		addCoord(initialCoord);
		
//		MAX_FACES = (int)(ChunkManager.CHUNKLENGTH * ChunkManager.CHUNKLENGTH); // for now
	}
	
	private int quadIndexAtCoord(PTwo coord) {
		return quadTable[coord.s, coord.t] - FaceSet.SPECIAL_QUAD_LOOKUP_NUMBER;
	}
	
	private int addNewQuadAtCoord(Quad qq, PTwo coord) {
		int curQuadCount = quads.Count + FaceSet.SPECIAL_QUAD_LOOKUP_NUMBER;
		quadTable[coord.s, coord.t] = curQuadCount ;
		quads.Add(qq);
		
		return curQuadCount - FaceSet.SPECIAL_QUAD_LOOKUP_NUMBER;
	}
	
	private Quad quadAtCoord(PTwo co) {
		return quads[quadIndexAtCoord(co)];	
	}
	
	private void copyQuadIndexFromTo(PTwo fromCo, PTwo toCo) {
		quadTable[toCo.s, toCo.t] = quadTable[fromCo.s, fromCo.t];	
	}
	
	public void addCoord(Coord co)
	{
		// ADD/ADJUST STRIPS
		
		// for later...
		filledFaces[co.x, co.z] = true; 
		if (faceSetLimits.isErsatzNull() ) {
			faceSetLimits = new Quad( PTwo.PTwoXZFromCoord(co), new PTwo(1,1) );	
		} else {
			faceSetLimits = faceSetLimits.expandedToContainPoint(new PTwo(co.x, co.z) );
		}
		
		///
		//...	assume we're dealing with xz faces and z neg is 'up' x pos is 'right'
		int stripsIndex = co.x;
		int addAtHeight = co.z;
		
		List<Strip> strips = stripsAtIndex(stripsIndex);
		
		// no strips yet?
		if (strips.Count == 0)
		{
			Strip newStrip = new Strip(new Range1D(addAtHeight, 1));
			strips.Add(newStrip);
			stripsArray[stripsIndex] = strips;
			return;
		}
		
		for(int i = 0; i < strips.Count ; ++i)
		{
			Strip str = strips[i];
			if 	(str.range.isOneAboveRange(addAtHeight) )
			{
				str.range.range++; // = str.range.extendRangeByOne();
				
				// can we combine with a next range?
				if (i < strips.Count - 1) {
					Strip nextStrip = strips[i + 1];
					if (nextStrip.range.isOneBelowStart(addAtHeight))
					{
						str.range = str.range.extendRangeToInclude(nextStrip.range);
						strips.RemoveAt(i+1);
					}
				}
				
				strips[i] = str; //put it back 
				break;
			}
			
			if (str.range.isOneBelowStart(addAtHeight) ) 
			{	
//				str.range = str.range.subtractOneFromStart();
				str.range.start--;
				str.range.range++;
				
				strips[i] = str;
				break;
			}
			
			if (i == strips.Count - 1) //still didn't find anything? make new
			{
				Strip newStrip = new Strip(new Range1D(addAtHeight, 1));
				strips.Add(newStrip);
				break;
			}
		}
		
		stripsArray[stripsIndex] = strips; // put it back!
	}
	
	private List<Quad> quadsForArea(Quad area, ref List<Quad> out_quads) // make a quad (if possible) from a given area
	{
		// THIS CAUSES AN 'OUT OF MEMORY' ERROR!
		
		if (iterationSafety > 1000) {
			return new List<Quad>();	
		}
		
		iterationSafety ++;
		
		if (areaIsFilled(area))
		{
			List<Quad> ret = new List<Quad>();
			ret.Add(area);
			return ret;
		} else if (area.dimensions.area() == 1) {
			return new List<Quad>();	
		}
		
		if (area.dimensions.t > 1)
		{
			if (area.dimensions.s > 1)
			{
				Quad ULQuad = area.upperLeftQuarter();
				out_quads.AddRange(quadsForArea(ULQuad, ref out_quads) );
			}
			
			Quad URQuad = area.upperRightQuarter();
			out_quads.AddRange(quadsForArea(URQuad, ref out_quads) );
		}
		
		if (area.dimensions.s > 1) {
			Quad LLQuad = area.lowerLeftQuarter();
			out_quads.AddRange(quadsForArea(LLQuad, ref out_quads) );
		}
		
		Quad LRQuad = area.lowerRightQuarter();
		out_quads.AddRange(quadsForArea(LRQuad, ref out_quads) );
		
		return out_quads;

	}
	
	private bool areaIsFilled(Quad area) 
	{
		if (area.dimensions.area() == 0)
			return false;
		
		int i = area.origin.s;
		int end = area.extent().s;
		int jend = area.extent().t;
		int jstart = area.origin.t;
		int j;
		for(; i < end ; ++i)
		{
			j = jstart;
			for(; j < jend ; ++j)
			{
				if (!filledFaces[i, j])
					return false;
			}
			
		}
		return true;
	}
	
	private void optimizeStrips()
	{
		// ENHANCED NAIVE METHOD
		// MAKE A LIST OF QUADS FROM THE LISTS OF STRIPS.
		// (ADMITTEDLY: WE GOT NOTHING OUT OF USING QUADS INSTEAD OF STRIPS WITH LENGTH...)
		int horizontalDim = 1;
		List<Strip> currentStrips;
		List<Strip> lastStrips;
		for(; horizontalDim < stripsArray.Length ; ++horizontalDim)
		{
			currentStrips = stripsArray[horizontalDim];
			lastStrips = stripsArray[horizontalDim - 1];
			
			// corner case
			if (lastStrips == null && horizontalDim == stripsArray.Length - 1)
			{
				if (currentStrips != null)
					foreach(Strip lastRowStrip in currentStrips)
					{
						Quad lq = Quad.QuadFromStrip(lastRowStrip, horizontalDim);
						addNewQuadAtCoord(lq, new PTwo(horizontalDim, lastRowStrip.range.start) );
						
					}
			}
			
			if ( lastStrips == null ) //currentStrips == null) // ||
				continue;
			
			
			for(int j = 0 ; j < lastStrips.Count ; ++j) 
			{
				Strip lstp = lastStrips[j];
				
				//QUAD WAY
				if (horizontalDim == 1)
				{
					Quad qq = Quad.QuadFromStrip(lstp, horizontalDim - 1);
					lstp.quadIndex = addNewQuadAtCoord(qq, new PTwo(horizontalDim - 1, lstp.range.start) );
					lastStrips[j] = lstp;
				}
				
				int curCount = currentStrips == null?  0 : currentStrips.Count;
				for(int i = 0; i < curCount ; ++i) 
				{
					Strip stp = currentStrips[i];
					
					
					if (Range1D.Equal(stp.range, lstp.range) ) // truly naive only 2 wide is pos!
					{
						
						// has a quad index?
						if (lstp.quadIndex != -1)
						{
							Quad qua = quads[ lstp.quadIndex];
							qua.dimensions.s++;
							quads[lstp.quadIndex] = qua;
							stp.quadIndex = lstp.quadIndex;
						} else {
							// make a new quad with these two 
							lstp.width ++;
							Quad doubleWidthqq = Quad.QuadFromStrip(lstp, horizontalDim - 1);
							lstp.quadIndex = addNewQuadAtCoord(doubleWidthqq, new PTwo(horizontalDim - 1, lstp.range.start) );
							stp.quadIndex = lstp.quadIndex;
							
						}
//						lstp.width++;
//						currentStrips.RemoveAt(i);
//						i--;
//						lastStrips[j] = lstp;
						
					} 
					else {
						// new quad with stp
						Quad sq = Quad.QuadFromStrip(stp, horizontalDim);
						stp.quadIndex = addNewQuadAtCoord(sq, new PTwo(horizontalDim, stp.range.start) );
					}
					
					
					currentStrips[i] = stp;
				}
				
				if (lstp.quadIndex == -1) // still no match
				{
					Quad sq = Quad.QuadFromStrip(lstp, horizontalDim - 1);
					lstp.quadIndex = addNewQuadAtCoord(sq, new PTwo(horizontalDim - 1, lstp.range.start) );
				}
				
				lastStrips[j] = lstp;
			}
			
			stripsArray[horizontalDim] = currentStrips;
			stripsArray[horizontalDim - 1] = lastStrips;
		}
		
	}
	

	
	public MeshSet CalculateGeometry(float verticalHeight) //need param y height...
	{
		optimizeStrips();
		
		int curTriIndex = 0;
		
		bool faceIsOnPosSide = ((int) this.blockFaceDirection % 2 == 0);
		
		verticalHeight += (faceIsOnPosSide ? -0.5f : 0.5f);
		
		float uvIndex = Chunk.uvIndexForBlockTypeDirection(this.blockType, this.blockFaceDirection);

		Vector2 monoUVValue = new Vector2(uvIndex, 0f); // origin ;
		
		List<Vector2> returnUVS = new List<Vector2>();
		List<int> returnTriIndices = new List<int>();
		List<Vector3> returnVecs = new List<Vector3>();
		
		float half_unit = 0.5f;

		int i = 0;
		for(; i < quads.Count ; ++i)
		{
			Quad quad = quads[i];
			
			int vertsAddedByStrip = 0;
			
			int curULindex = curTriIndex; // + i * 4;
			int curLLindex = curULindex + 1;
			int curURindex = curULindex + 2;
			int curLRindex = curULindex + 3;
				
			Vector3 ulVec = new Vector3((float) quad.origin.s - half_unit, 
				verticalHeight, 
				(float)quad.origin.t - half_unit) * Chunk.VERTEXSCALE;
			returnVecs.Add(ulVec);
			vertsAddedByStrip++;
			returnUVS.Add(monoUVValue);
		
			Vector3 llVec = new Vector3((float) quad.origin.s - half_unit, 
				verticalHeight, 
				(float)quad.extent().t  - half_unit) * Chunk.VERTEXSCALE;
			returnVecs.Add(llVec);
			
			vertsAddedByStrip++;
		
			returnUVS.Add(monoUVValue);

			Vector3 urVec = new Vector3((float) quad.extent().s - half_unit, 
				verticalHeight, 
				(float)quad.origin.t - half_unit) * Chunk.VERTEXSCALE;
			returnVecs.Add(urVec);
			vertsAddedByStrip++;
			returnUVS.Add( monoUVValue); // + Vector2.Scale(uvURTexDim, uvScalingVec) );
			
			Vector3 lrVec = new Vector3((float)quad.extent().s - half_unit, 
				verticalHeight, 
				(float)quad.extent().t - half_unit) * Chunk.VERTEXSCALE;
			returnVecs.Add(lrVec);
			vertsAddedByStrip++;
			returnUVS.Add(monoUVValue); // + Vector2.Scale(uvLRTexDim, uvScalingVec) );
			
			int[] tris;
			
			if (faceIsOnPosSide) {
				tris = new int[] {curULindex, curURindex, curLLindex,   curURindex, curLRindex, curLLindex};
			} else {
				tris = new int[] {curULindex, curLLindex, curURindex, 	curURindex, curLLindex, curLRindex };
			}
			
			returnTriIndices.AddRange(tris);
			
			curTriIndex += vertsAddedByStrip;

		}
		
		//now maybe just calculate the actual v3 array and the indices....
		GeometrySet geomset = new GeometrySet(returnTriIndices, returnVecs );
		
		return new MeshSet(geomset, returnUVS);
		
	}
	
//	private List<Vector3> collectedVectors()
//	{
//		List<Vector3> resVs = new List<Vector3> ();
//				
//		int hDim = 0;
//		for(;hDim < vertexColumns.Count ; ++hDim)
//		{
//			
//		}
//		
//		return resVs;
//	}
	
//	private List<VertexTwo> vertexListAt(int index) {
//		if 	(vertexColumns[index] == null) {
//			vertexColumns [index] = new List<VertexTwo>();
//		}
//		return vertexColumns[index];
//	}
	
	private bool stripFromListWithStartEqualTo(int equalToThis, List<Strip> _strips, ref Strip out_strip) {
		return stripFromList(equalToThis, _strips, ref out_strip, false);
	}
	
	private bool stripFromListWithExtentEqualTo(int equalToThis, List<Strip> _strips, ref Strip out_strip) {
		return stripFromList(equalToThis, _strips, ref out_strip, true);
	}
	
	private bool stripFromList(int equalToThis, List<Strip> _strips, ref Strip out_strip, bool wantExtent) {
		foreach (Strip str in _strips) {
			if ((wantExtent && str.range.extentMinusOne() == equalToThis) || str.range.start == equalToThis)
			{
				out_strip = str;
				return true;
			}
			
		}
		return false; // Strip.theErsatzNullStrip();
	}
	
	private List<Strip> stripsAtIndex(int ix) {
		if (stripsArray[ix] == null) {
			stripsArray[ix] = new List<Strip>();		
		}
		return stripsArray[ix];
	}
	
	public void logStripsArray() 
	{
		int count = 0;
		string info = "";
		foreach(List<Strip> strips in stripsArray)
		{
			
			if (strips != null)
			{
				info += "" + strips.Count;
				
			
				foreach(Strip st in strips) {
	
					info += "strip: " + st.toString ();
				}
				info += "\n";
			}
			++count;
		}	
		bug (info);
	}
	
	public string getQuadsString() {
		string result = "";
		
		foreach(Quad qq in quads) {
			result += " | " + qq.toString();	
		}
		return result;
	}
	
	public void checkForIntersectingStrips()
	{
		//TODO: this func.
		//Also, check if there's total coverage in the strips across the CHLEN
		// and if not where are the gaps?
	}
	
	private void bug(string str) {
		UnityEngine.Debug.Log(str);	
	}
}

public class FaceSetTest
{
	private List<Vector3> showInGuiVecs = new List<Vector3>();
	public FaceSetTest()
	{
		bug ("BEGIN FACE SET TEST");
		FaceSet fs = new FaceSet(BlockType.Grass, Direction.ypos, new Coord(0,2,0));
		
		Coord[] coords = new Coord[256];
		for(int i = 0; i < 256 ; ++i)
		{
			int z = i % 16;
			int x = i / 16;
			coords[i] = new Coord(x, 2, z);
		}
		
		
		
		foreach (Coord co in coords) {
			fs.addCoord(co);	
		}
		
		MeshSet mset = fs.CalculateGeometry(2);
		
//		fs.logStripsArray();
		
		this.logMeshSet(mset);
		
		bug ("END FACE SET TEST");
	}
	
	private void logMeshSet( MeshSet mset) 
	{
		bug("Mesh set verts");
		string vstr = "";
		int jj = 0;
		foreach(Vector3 vert in mset.geometrySet.vertices) {
			if (jj % 4 == 0)
				vstr += "\n";
			vstr += "[" + vert.x + " , " + vert.z + "]";
			jj++;
		}
		bug (vstr);
		
		vstr="";
		jj = 0;
		foreach(Vector3 vert in mset.geometrySet.vertices) {
//			if (jj % 4 == 0)
			vstr += "*";
			if (jj % 16 == 0)
				vstr += "\n";
			jj++;
		}
		bug (vstr);
		
		showInGuiVecs.AddRange(mset.geometrySet.vertices);
		
		int vertCount = mset.geometrySet.vertices.Count;
		
		bug("VertCount: " + vertCount + " verts/indices (INDEX)[x,z]");
		string indexstr = "";
		int count = 0;
		foreach(int ii in mset.geometrySet.indices) {
			if (count % 6 == 0) 
				indexstr += "\n";
//			indexstr += "|" + ii;
			Vector3 vv = mset.geometrySet.vertices[ii];
			indexstr += "("+ii+")[" + vv.x + " , " + vv.z + "]";
			if (ii >= vertCount || ii < 0) {
				indexstr += "\n out of bounds: ? " + ii + "\n";
			}
			++count;
		}
//		bug (indexstr);
	}
	
	private void bug(string str) {
		
		UnityEngine.Debug.Log(str);
	}
	
//	public void OnGUI() 
//	{
//		foreach(Vector3 vv in showInGuiVecs) {
//			guiBlipAt(vv);	
//		}
//	}
//	
//	private void guiBlipAt(Vector3 vv) {
//		GUI.Box (new Rect (vv.x * 10, vv.z * 10 , 10, 10), "*");
//	}
}




//			int relUpperLeftTriIndex = ++internalTriIndex;
//			
////			IndexSet newIset = IndexSet(
//			// is there a strip to the left?
//			if (co.x > 0)
//			{
//				List<Strip> oneLeftStrips = stripsAtIndex(co.x - 1);
//				Strip leftStartFlushStrip = stripFromListWithSameStartEqualTo(co.z, oneLeftStrips);
//				if ( Strip.StripNotNull(leftStartFlushStrip) ) {
//					relUpperLeftTriIndex = leftStartFlushStrip.indexSet.upperRight;
//					--internalTriIndex;
//				}
//			}


// OLD calc geom

//public MeshSet CalculateGeometry(float verticalHeight) //need param y height...
//	{
//		int curTriIndex = 0;
//		int horizontalDim = 0;
//		
//		bool faceIsOnPosSide = ((int) this.blockFaceDirection % 2 == 0);
//		
//		verticalHeight += (faceIsOnPosSide ? -0.5f : 0.5f);
//		
//		Vector2 monoUVValue = Chunk.uvOriginForBlockType(this.blockType, this.blockFaceDirection);
//		Vector2 uvURTexDimTest = new Vector2(0.25f, 0f);
//		Vector2 uvLLTexDimTest = new Vector2(0f, .25f);
//		Vector2 uvLRTexDimTest = new Vector2(0.25f, 0.25f);
//		
//		List<Vector2> returnUVS = new List<Vector2>();
//		
//		List<Strip> strips;
//		List<Vector3> rightVertexColumn;
//		List<Vector3> leftVertexColumn = new List<Vector3>();
//		
//		List<int> returnTriIndices = new List<int>();
//		List<Vector3> returnVecs = new List<Vector3>();
//		
//		float half_unit = 0.5f;
//		
//		
//		for (; horizontalDim < stripsArray.Length; ++horizontalDim)
//		{
//			rightVertexColumn = new List<Vector3>(); 
//			strips = stripsArray[horizontalDim];
//			
//			float horizontalDimLessHalf = horizontalDim - half_unit;
//			
//			int strips_count = 0;
//			if (strips != null)
//				strips_count = strips.Count; //is continue ok???
//			
//			if (strips_count == 0)
//				bug ("strips count 0 (or null) at h dim: " + horizontalDim);
//			else if (strips_count > 1)
//				bug ("strips count > 1 : " + strips_count + " at h dim: " + horizontalDim);
//			
//			int vertsAddedByStrip = 0;
//			
//			for (int i = 0; i < strips_count ; ++i)
//			{
//				Strip str = strips[i];
//				int curULindex = curTriIndex + i * 2;
//				int curLLindex = curULindex + 1;
//				int curURindex = curTriIndex + strips_count * 2 + i * 2;
//				int curLRindex = curURindex + 1;;
////				int curURindex = curTriIndex + 2;
////				int curLRindex = curTriIndex + 3;
//				
//				bool addULVert = true;
//				bool addLLVert = true;
//				
//				
//#if COMBINE_FLUSH_VERTS
//
//				if (horizontalDim > 0 && stripsArray[horizontalDim - 1] != null) 
//				{
//					// if this strip is flush with the start of any strips in the col to the left...
//					Strip flushWithStartOneLeft = Strip.theErsatzNullStrip();
//					bool gotAStrip = stripFromListWithStartEqualTo(str.range.start, stripsArray[horizontalDim - 1], ref flushWithStartOneLeft);
//					if (gotAStrip) // (Strip.StripNotNull(flushWithStartOneLeft))
//					{
//						curULindex = flushWithStartOneLeft.indexSet.upperRight;
//						curLLindex--;
//						curURindex--;
//						curLRindex--;
//						addULVert = false;
//					}
//					
//					Strip flushWithExtentOneLeft = Strip.theErsatzNullStrip();
//					gotAStrip =	stripFromListWithExtentEqualTo(str.range.extentMinusOne() , stripsArray[horizontalDim - 1], ref flushWithExtentOneLeft);
//					if (gotAStrip)
//					{
//						curLLindex = flushWithExtentOneLeft.indexSet.lowerRight;
//						curURindex--;
//						curLRindex--;
//						addLLVert = false;
//					}
//				}
//#endif
//				
//				if (addULVert) {
//					Vector3 ulVec = new Vector3((float) horizontalDimLessHalf, verticalHeight, (float)str.range.start - half_unit);
//					leftVertexColumn.Add(ulVec); //LEFT COLUMN
//					vertsAddedByStrip++;
//					returnUVS.Add(monoUVValue);
//				}
//				
//				if (addLLVert) {
//					Vector3 llVec = new Vector3((float) horizontalDimLessHalf, verticalHeight, (float)str.range.extent() - half_unit);
//					leftVertexColumn.Add(llVec); //LEFT COLUMN!!
//					vertsAddedByStrip++;
//					returnUVS.Add(monoUVValue + uvLLTexDimTest);
//				}
//				
//				Vector3 urVec = new Vector3((float) horizontalDimLessHalf + 1, verticalHeight, (float)str.range.start - half_unit);
//				rightVertexColumn.Add(urVec);
//				vertsAddedByStrip++;
//				returnUVS.Add(monoUVValue + uvURTexDimTest);
//				
//				Vector3 lrVec = new Vector3((float)horizontalDimLessHalf + 1, verticalHeight, (float)str.range.extent() - half_unit);
//				rightVertexColumn.Add(lrVec);
//				vertsAddedByStrip++;
//				returnUVS.Add(monoUVValue + uvLRTexDimTest);
//				
//				//need to index sets.
//				str.indexSet = new IndexSet(curULindex, curURindex, curLLindex, curLRindex);
//				strips[i] = str;
//				
//				int[] tris;
//				
//				if (faceIsOnPosSide) {
//					tris = new int[] {curULindex, curURindex, curLLindex,   curURindex, curLRindex, curLLindex};
//				} else {
//					tris = new int[] {curULindex, curLLindex, curURindex, 	curURindex, curLLindex, curLRindex };
//				}
//				
//				returnTriIndices.AddRange(tris);
//				
//			}
//			
//			curTriIndex += vertsAddedByStrip;
//			
////			vertexColumns[horizontalDim] = leftVertexColumn;
////			vertexColumns[horizontalDim + 1] = rightVertexColumn;
//			
//			returnVecs.AddRange(leftVertexColumn);
//			
//			if (horizontalDim == stripsArray.Length - 1)
//			{
//				returnVecs.AddRange(rightVertexColumn);
//			}
//			else 
//			{
//				leftVertexColumn = rightVertexColumn;
//			}
//			
////			stripsArray[horizontalDim] = strips;
//		}
//		
//		//now maybe just calculate the actual v3 array and the indices....
//		GeometrySet geomset = new GeometrySet(returnTriIndices, returnVecs );
//		
//		return new MeshSet(geomset, returnUVS);
//		
//	}




//public MeshSet CalculateGeometry(float verticalHeight) //need param y height...
//	{

//  WORKS WITH STRIPS
//		optimizeStrips();
//		
//		int curTriIndex = 0;
//		int horizontalDim = 0;
//		
//		bool faceIsOnPosSide = ((int) this.blockFaceDirection % 2 == 0);
//		
//		verticalHeight += (faceIsOnPosSide ? -0.5f : 0.5f);
//		
//		Vector2 monoUVValue = Chunk.uvOriginForBlockType(this.blockType, this.blockFaceDirection);
//		Vector2 uvURTexDimTest = new Vector2(0.25f, 0f);
//		Vector2 uvLLTexDimTest = new Vector2(0f, .25f);
//		Vector2 uvLRTexDimTest = new Vector2(0.25f, 0.25f);
//		
//		List<Vector2> returnUVS = new List<Vector2>();
//		
//		List<Strip> strips;
//		List<Vector3> rightVertexColumn;
//		List<Vector3> leftVertexColumn = new List<Vector3>();
//		
//		List<int> returnTriIndices = new List<int>();
//		List<Vector3> returnVecs = new List<Vector3>();
//		
//		float half_unit = 0.5f;
//		
//		// NEW WAY: SIMPLY COLLECT THE VERTS AND INDICES FROM EACH STRIP IN ORDER
//		
//		for (; horizontalDim < stripsArray.Length; ++horizontalDim)
//		{
//			rightVertexColumn = new List<Vector3>(); 
//			strips = stripsArray[horizontalDim];
//			
//			float horizontalDimLessHalf = horizontalDim - half_unit;
//			
//			int strips_count = 0;
//			if (strips != null)
//				strips_count = strips.Count; //is continue ok???
//			
////			if (strips_count == 0)
////				bug ("strips count 0 (or null) at h dim: " + horizontalDim);
////			else if (strips_count > 1)
////				bug ("strips count > 1 : " + strips_count + " at h dim: " + horizontalDim);
//			
//			int vertsAddedByStrip = 0;
//			
//			for (int i = 0; i < strips_count ; ++i)
//			{
//				Strip str = strips[i];
//				int curULindex = curTriIndex + i * 4;
//				int curLLindex = curULindex + 1;
////				int curURindex = curTriIndex + strips_count * 2 + i * 2;
////				int curLRindex = curURindex + 1;;
//				int curURindex = curULindex + 2;
//				int curLRindex = curULindex + 3;
//				
//				bool addULVert = true;
//				bool addLLVert = true;
//				
//				
//#if COMBINE_FLUSH_VERTS
//
//				if (horizontalDim > 0 && stripsArray[horizontalDim - 1] != null) 
//				{
//					// if this strip is flush with the start of any strips in the col to the left...
//					Strip flushWithStartOneLeft = Strip.theErsatzNullStrip();
//					bool gotAStrip = stripFromListWithStartEqualTo(str.range.start, stripsArray[horizontalDim - 1], ref flushWithStartOneLeft);
//					if (gotAStrip) // (Strip.StripNotNull(flushWithStartOneLeft))
//					{
//						curULindex = flushWithStartOneLeft.indexSet.upperRight;
//						curLLindex--;
//						curURindex--;
//						curLRindex--;
//						addULVert = false;
//					}
//					
//					Strip flushWithExtentOneLeft = Strip.theErsatzNullStrip();
//					gotAStrip =	stripFromListWithExtentEqualTo(str.range.extentMinusOne() , stripsArray[horizontalDim - 1], ref flushWithExtentOneLeft);
//					if (gotAStrip)
//					{
//						curLLindex = flushWithExtentOneLeft.indexSet.lowerRight;
//						curURindex--;
//						curLRindex--;
//						addLLVert = false;
//					}
//				}
//#endif
//				
//				if (addULVert) {
//					Vector3 ulVec = new Vector3((float) horizontalDimLessHalf, verticalHeight, (float)str.range.start - half_unit);
////					leftVertexColumn.Add(ulVec); //LEFT COLUMN
//					returnVecs.Add(ulVec);
//					vertsAddedByStrip++;
//					returnUVS.Add(monoUVValue);
//				}
//				
//				if (addLLVert) {
//					Vector3 llVec = new Vector3((float) horizontalDimLessHalf, verticalHeight, (float)str.range.extent() - half_unit);
////					leftVertexColumn.Add(llVec); //LEFT COLUMN!!
//					returnVecs.Add(llVec);
//					vertsAddedByStrip++;
//					returnUVS.Add(monoUVValue + uvLLTexDimTest);
//				}
//				
//				Vector3 urVec = new Vector3((float) horizontalDimLessHalf + (float) str.width, verticalHeight, (float)str.range.start - half_unit);
////				rightVertexColumn.Add(urVec);
//				returnVecs.Add(urVec);
//				vertsAddedByStrip++;
//				returnUVS.Add(monoUVValue + uvURTexDimTest);
//				
//				Vector3 lrVec = new Vector3((float)horizontalDimLessHalf + (float) str.width , verticalHeight, (float)str.range.extent() - half_unit);
////				rightVertexColumn.Add(lrVec);
//				returnVecs.Add(lrVec);
//				vertsAddedByStrip++;
//				returnUVS.Add(monoUVValue + uvLRTexDimTest);
//				
//				//need to index sets.
//				str.indexSet = new IndexSet(curULindex, curURindex, curLLindex, curLRindex);
//				strips[i] = str;
//				
//				int[] tris;
//				
//				if (faceIsOnPosSide) {
//					tris = new int[] {curULindex, curURindex, curLLindex,   curURindex, curLRindex, curLLindex};
//				} else {
//					tris = new int[] {curULindex, curLLindex, curURindex, 	curURindex, curLLindex, curLRindex };
//				}
//				
//				returnTriIndices.AddRange(tris);
//				
//			}
//			
//			curTriIndex += vertsAddedByStrip;
//			
////			vertexColumns[horizontalDim] = leftVertexColumn;
////			vertexColumns[horizontalDim + 1] = rightVertexColumn;
//			
//			returnVecs.AddRange(leftVertexColumn);
//			
//			if (horizontalDim == stripsArray.Length - 1)
//			{
//				returnVecs.AddRange(rightVertexColumn);
//			}
//			else 
//			{
//				leftVertexColumn = rightVertexColumn;
//			}
//			
////			stripsArray[horizontalDim] = strips;
//		}
//		
//		//now maybe just calculate the actual v3 array and the indices....
//		GeometrySet geomset = new GeometrySet(returnTriIndices, returnVecs );
//		
//		return new MeshSet(geomset, returnUVS);
//		
//	}

//Coord[] other_coords = new Coord[] {
//			new Coord(0, 2, 0),
//			new Coord(0, 2, 1),
//			new Coord(0, 2, 2),
//			new Coord(0, 2, 3),
//			new Coord(0, 2, 4),
//			
//			new Coord(1, 2, 0),
//			new Coord(1, 2, 1),
//			new Coord(1, 2, 2),
//			new Coord(1, 2, 3),
//			new Coord(1, 2, 4),
//			new Coord(1, 2, 5),
//			
//			new Coord(2, 2, 0),
//			new Coord(2, 2, 1),
//			new Coord(2, 2, 2),
//			
//			new Coord(3, 2, 1),
//			new Coord(3, 2, 2),
//			new Coord(3, 2, 3),
////			
//			new Coord(4, 2, 0),
//			new Coord(4, 2, 1),
//			new Coord(4, 2, 2),
//		};