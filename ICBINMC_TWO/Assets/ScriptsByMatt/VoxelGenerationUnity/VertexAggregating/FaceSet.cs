using UnityEngine;
using System.Collections;

using System.Collections.Generic;

public struct CTwo
{
	int r, s;
	
	public CTwo(int rr, int ss) {
		r = rr; s = ss;	
	}
	
	public static CTwo CTwoXZFromCoord(Coord co) {
		return new CTwo(co.x, co.z);	
	}
	
	public string toString() {
		return " CTwo: r: " + r + " s: " + s;	
	}
}

public struct VertexTwo
{
	public CTwo coord;
//	int tri_index;
	
	public VertexTwo(CTwo _ctwo) {  //, int _tri_index) {
		coord = _ctwo;// tri_index = _tri_index;	
	}
	
	public string toString() {
		return " VertexTwo: coord: " + coord.toString(); // + " tri index: " + tri_index;
	}
}

public struct Face
{
	// has an xpos and zpos neighbor
//	Face * xPosNeighbor; //, zPosNeighbor; // these are unsafe (not allowed in this safe code world??)
	
	CTwo ctwo;
	
	//?? an enum (later an int to be looked at bitwise?) that tells which walls are intact
	//for now just four bools
	bool xnegWall, xposWall, znegWall, zposWall;
	
	public Face (CTwo _ctwo) {
//		xPosNeighbor = zPosNeighbor = null;
		ctwo = _ctwo;
		xnegWall = xposWall = znegWall = zposWall = true;
	}
}

public struct IndexSet
{
	public int upperLeft, upperRight, lowerLeft, lowerRight;
	
	public IndexSet(int ul, int ur, int ll, int lr) 
	{
		upperLeft = ul; upperRight = ur; lowerLeft = ll; lowerRight = lr;	
	}
	
	public static IndexSet theErsatzNullIndexSet() {
		return new IndexSet(-444, -123, -987554, -33);	
	}
}

public struct Strip
{
	public Range1D range;
	public IndexSet indexSet;
	
	public Strip(Range1D rr, IndexSet iset) {
		range = rr; indexSet = iset;
	}
	
	public Strip(Range1D rr) {
		range = rr; indexSet = IndexSet.theErsatzNullIndexSet();	
	}
	
	public static Strip theErsatzNullStrip() {
		return new Strip(Range1D.theErsatzNullRange(), IndexSet.theErsatzNullIndexSet() );	
	}
	
	public static bool StripNotNull(Strip ss) {
		return !Range1D.Equal(ss.range, Range1D.theErsatzNullRange() );	
	}
	
}

public struct GeometrySet
{
	public List<int> indices;
	public List<Vector3> vertices;
	
//	public GeometrySet() {
//		indices = new List<int>();
//		vertices = new List<Vector3>();
//	}
	
	public GeometrySet(List<int> ind, List<Vector3> vecs) {
		indices = ind; vertices = vecs;
	}
}

public class FaceSet  
{

	public BlockType blockType;
	
	private List<Strip>[] stripsArray = new List<Strip>[(int)ChunkManager.CHUNKLENGTH];
	
	List<VertexTwo>[] vertexColumns = new List<VertexTwo>[(int) ChunkManager.CHUNKLENGTH + 1];
	
//	private int internalTriIndex = 0;
	
	public FaceSet(BlockType _type)
	{
		blockType = _type;	
	}
	
	public void addCoord(Coord co)
	{
		//...	assume we're dealing with xz faces and z neg is 'up' xpos is 'right'
		int stripsIndex = co.x;
		int addAtHeight = co.z;
		
		List<Strip> strips  = stripsAtIndex(stripsIndex);
		// JUST ADD STRIPS (OR ADD/ADJUST STRIPS)
		// MAKE INDICES AND VERTICES LATER
		
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
				str.range = str.range.extendRangeByOne();
				
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
				str.range = str.range.subtractOneFromStart();
				
				strips[i] = str;
				break;
			}
			
			if (i == strips.Count - 1) //still didn't find anything? make new
			{
				Strip newStrip = new Strip(new Range1D(addAtHeight, 1));
				strips.Add(newStrip);
			}
		}
		
		stripsArray[stripsIndex] = strips; // put it back!
	}
	
	public GeometrySet CalculateGeometry(float verticalHeight) //need param y height...
	{
		int curTriIndex = 0;
		int horizontalDim = 0;
		
		List<Strip> strips;
		List<Vector3> rightVertexColumn;
		List<Vector3> leftVertexColumn = new List<Vector3>();
		
		List<int> returnTriIndices = new List<int>();
		List<Vector3> returnVecs = new List<Vector3>();
		
		for (; horizontalDim < stripsArray.Length; ++horizontalDim)
		{
			rightVertexColumn = new List<Vector3>(); 
			strips = stripsArray[horizontalDim];
			
			for (int i = 0; i < strips.Count ; ++i)
			{
				Strip str = strips[i];
				int curULindex = curTriIndex;
				int curLLindex = curTriIndex + 1;
				int curURindex = curTriIndex + 2;
				int curLRindex = curTriIndex + 3;
				
				bool addULVert = true;
				bool addLLVert = true;
				
				if (horizontalDim > 0) 
				{
					// if this strip is flush with the start of any strips in the col to the left...
					Strip flushWithStartOneLeft = stripFromListWithStartEqualTo(str.range.start, stripsArray[horizontalDim - 1]);
					if (Strip.StripNotNull(flushWithStartOneLeft))
					{
						curULindex = flushWithStartOneLeft.indexSet.upperRight;
						curURindex--;
						curLRindex--;
						addULVert = false;
					}
					
					Strip flushWithExtentOneLeft = stripFromListWithExtentEqualTo(str.range.extentMinusOne() , stripsArray[horizontalDim - 1]);
					if (Strip.StripNotNull(flushWithExtentOneLeft))
					{
						curLLindex = flushWithExtentOneLeft.indexSet.lowerRight;
						curURindex--;
						curLRindex--;
						addLLVert = false;
					}
				}
				
				if (addULVert) {
					Vector3 ulVec = new Vector3((float) horizontalDim, verticalHeight, (float)str.range.start);
//					VertexTwo ulVert = new VertexTwo(new CTwo(horizontalDim, str.range.start));//, curULindex);
					leftVertexColumn.Add(ulVec); //LEFT COLUMN
					curTriIndex++;
				}
				
				if (addLLVert) {
					Vector3 llVec = new Vector3((float) horizontalDim, verticalHeight, (float)str.range.extent());
//					VertexTwo llVert = new VertexTwo(new CTwo(horizontalDim, str.range.extent()));//, curULindex);
					leftVertexColumn.Add(llVec); //LEFT COLUMN!!
					curTriIndex++;
				}
				
				Vector3 urVec = new Vector3((float) horizontalDim + 1, verticalHeight, (float)str.range.start);
//				VertexTwo urVert = new VertexTwo(new CTwo(horizontalDim + 1, str.range.start));
				rightVertexColumn.Add(urVec);
				curTriIndex++;
				
				Vector3 lrVec = new Vector3((float)horizontalDim + 1, verticalHeight, (float)str.range.extent());
//				VertexTwo lrVert = new VertexTwo(new CTwo(horizontalDim + 1, str.range.extent()));
				rightVertexColumn.Add(lrVec);
				curTriIndex++;
				
				//maybe won't need index sets.
//				str.indexSet = new IndexSet(curULindex, curURindex, curLLindex, curLRindex);
//				strips[i] = str;
				
				returnTriIndices.Add(curULindex);
				returnTriIndices.Add(curURindex);
				returnTriIndices.Add(curLLindex);
				returnTriIndices.Add(curLRindex);
			}
			
//			vertexColumns[horizontalDim] = leftVertexColumn;
//			vertexColumns[horizontalDim + 1] = rightVertexColumn;
			
			returnVecs.AddRange(leftVertexColumn);
			
			if (horizontalDim == stripsArray.Length - 1)
			{
				returnVecs.AddRange(rightVertexColumn);
			}
			else 
			{
				leftVertexColumn = rightVertexColumn;
			}
			
//			stripsArray[horizontalDim] = strips;
		}
		
		//now maybe just calculate the actual v3 array and the indices....
		GeometrySet geomset = new GeometrySet();
		geomset.vertices = returnVecs;
		geomset.indices = returnTriIndices;
		return geomset;
		
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
	
	private Strip stripFromListWithStartEqualTo(int equalToThis, List<Strip> _strips) {
		foreach (Strip str in _strips) {
			if (str.range.start == equalToThis)
				return str;
		}
		return Strip.theErsatzNullStrip();
	}
	
	private Strip stripFromListWithExtentEqualTo(int equalToThis, List<Strip> _strips) {
		foreach (Strip str in _strips) {
			if (str.range.extentMinusOne() == equalToThis)
				return str;
		}
		return Strip.theErsatzNullStrip();
	}
	
	private List<Strip> stripsAtIndex(int ix) {
		if (stripsArray[ix] == null) {
			stripsArray[ix] = new List<Strip>();		
		}
		return stripsArray[ix];
	}
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