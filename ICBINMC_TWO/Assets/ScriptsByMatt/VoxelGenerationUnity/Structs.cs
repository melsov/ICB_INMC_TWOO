using UnityEngine;
using System.Collections;

using System.Collections.Generic;


public struct NeighborChunks
{
	public Chunk xpos, xneg, ypos, yneg, zpos, zneg;

	public NeighborChunks(Chunk xp, Chunk xn, Chunk yp, Chunk yn, Chunk zp , Chunk zn)
	{
		xpos = xp;
		xneg = xn;
		ypos = yp;
		yneg = yn;
		zpos = zp;
		zneg = zn;
	}
}


public struct MeshSet
{
	public GeometrySet geometrySet;
	public List<Vector2> uvs;
	
	public MeshSet(GeometrySet gs, List<Vector2> _uvs) {
		this.geometrySet = gs; this.uvs = _uvs;	
	}
}

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


public struct IndexSet
{
	public int upperLeft, upperRight, lowerLeft, lowerRight;
	
	public IndexSet(int ul, int ur, int ll, int lr) 
	{
		upperLeft = ul; upperRight = ur; lowerLeft = ll; lowerRight = lr;	
	}
	
	public static IndexSet theErsatzNullIndexSet() {
		return new IndexSet(-444, -321, -987554, -33);	
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
	
	public string toString() {
		return "strip with range: " + range.toString();	
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


public struct BiomeTypeCorners
{
	public BiomeType lowerLeft;
	public BiomeType lowerRight;
	public BiomeType upperLeft;
	public BiomeType upperRight;
	
	public BiomeTypeCorners(BiomeType ll, BiomeType lr, BiomeType ul, BiomeType ur) 
	{
		lowerLeft = ll;
		lowerRight = lr; 
		upperLeft = ul;
		upperRight = ur;
	}
}

public struct BiomeInputs
{
	public float hilliness;
	public float cragginess;
	public float overhangness;
	public float caveVerticalFrequency;
	
	public static BiomeInputs Pasture()
	{
		BiomeInputs inputs = new BiomeInputs();
		//pasture
		inputs.hilliness = .2f;
		inputs.cragginess = 3.2f;
		inputs.overhangness = .03f;	
		inputs.caveVerticalFrequency = 1.5f;
		
		return inputs;
	}
	
	public static BiomeInputs CraggyMountains()
	{
		BiomeInputs inputs = new BiomeInputs();
		
		//cragginess
		inputs.hilliness = .4f;
		inputs.cragginess = 3.2f;
		inputs.overhangness = .5f;
		inputs.caveVerticalFrequency = 1.4f;
		
		return inputs;
	}
	
	public static BiomeInputs Mix(BiomeInputs aBI, BiomeInputs bBi, float aWeight, float bWeight) {
		BiomeInputs inputs = new BiomeInputs();

		aBI = BiomeInputs.Mult(aBI, aWeight);
		bBi = BiomeInputs.Mult(bBi, bWeight);
		
		return BiomeInputs.Add(aBI, bBi);
	}
	
	public static BiomeInputs Mult(BiomeInputs abi, float k) {
		abi.hilliness *= k;	
		abi.cragginess *= k;
		abi.overhangness *= k;
		abi.caveVerticalFrequency *= k;
		return abi;
	}
	
	public static BiomeInputs Add(BiomeInputs abi, BiomeInputs bbi) {
		abi.hilliness += bbi.hilliness;
		abi.cragginess += bbi.cragginess;
		abi.overhangness += bbi.overhangness;
		abi.caveVerticalFrequency += bbi.caveVerticalFrequency;
		return abi;
	}
	

}
