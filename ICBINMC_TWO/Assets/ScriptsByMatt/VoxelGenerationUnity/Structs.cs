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

public enum Axis { X, Y, Z }

public struct MeshSet
{
	public GeometrySet geometrySet;
	public List<Vector2> uvs;
	public List<Vector2> colors;
	public List<Color32> color32s;
	
	public int deltaVertexCount;
	
	public MeshSet(GeometrySet gs, List<Vector2> _uvs, List<Vector2> _colors, List<Color32> _color32s) {
		this.geometrySet = gs; this.uvs = _uvs;	
		deltaVertexCount = 0;
		color32s = _color32s;
		colors = _colors;
	}
	
	public MeshSet(GeometrySet gs, List<Vector2> _uvs, List<Vector2> _colors) {
		this = new MeshSet(gs, _uvs, _colors, new List<Color32>() );
	}
	
	public MeshSet(GeometrySet gs, List<Vector2> _uvs, List<Color32> _color32s) {
		this = new MeshSet(gs, _uvs, new List<Vector2>() , _color32s);
	}
	
	public MeshSet(GeometrySet gs, List<Vector2> _uvs) {
		this = new MeshSet(gs, _uvs, new List<Vector2>() );
	}
	
	public static MeshSet emptyMeshSet() {
		List<Vector3> vs = new List<Vector3>();
		List<int> tris = new List<int>();
		List<Vector2> uvs = new List<Vector2>();
		return new MeshSet(new GeometrySet(tris, vs), uvs);
	}
	
	public void clearAll() {
		if (this.geometrySet.vertices == null)
			return;
		
		this.geometrySet.vertices.Clear();
		this.geometrySet.indices.Clear();
		this.uvs.Clear();
		this.colors.Clear();
		this.color32s.Clear();
	}
}
//
//public struct CoRange2D
//{
//	public PTwo origin;
//	public PTwo range;
//	
//	public CoRange2D(PTwo _origin, PTwo _range) {
//		origin = _origin; range = _range;	
//	}
//	
//	public PTwo extent() {
//			
//	}
//}

public struct PTwo
{
	public int s, t;
	
	public PTwo(int ss, int tt) {
		s = ss; t = tt;	
	}
	
	public PTwo(int ss, float tt) {
		this = new PTwo(ss, (int) tt);	
	}
	
	public PTwo(float ss, int tt) {
		this = new PTwo((int) ss, tt);
	}
	
	public PTwo(float sss, float ttt) {
		this = new PTwo((int) sss, (int) ttt);	
	}
	
	public static PTwo PTwoOne() {
		return new PTwo(1,1);	
	}
	
	public static PTwo PTwoZero() {
		return new PTwo(1,1);	
	}
	
	
	public PTwo plusOne() {
		return new PTwo(this.s + 1, this.t + 1);	
	}
	
	public static PTwo newPTwoWidthHeight (int width, int height) {
		if (width < 0 || height < 0)
			UnityEngine.Debug.LogError("a negative width or height when you apparently meant otherwise?");
		return new PTwo(width, height);
	}
	
	public static PTwo PTwoXZFromCoord(Coord co) {
		return new PTwo(co.x, co.z);	
	}
	
	public static Coord CoordFromPTwoWithY(PTwo point, int height) {
		return new Coord(point.s, height, point.t);	
	}
	
	public static PTwo PTwoFromAlignedCoord(AlignedCoord alco) {
		return new PTwo(alco.across, alco.up);	
	}
	
	public int area() {
		return s * t;	
	}
	
	public string toString() {
		return " PTwo: s: " + s + " t: " + t;	
	}
	
	public int width() { return s; }
	
	public void setWidth(int ww) { s = ww; }
	
	public int height() { return t; }
	
	public void setHeight (int height) { t = height; }
	
	public static PTwo operator+  (PTwo ff, PTwo gg) {
		return new PTwo(ff.s + gg.s, ff.t + gg.t);	
	}
	
	public static PTwo operator-  (PTwo ff, PTwo gg) {
		return new PTwo(ff.s - gg.s, ff.t - gg.t);	
	}
	
	public static PTwo operator*  (PTwo ff, PTwo gg) {
		return new PTwo(ff.s * gg.s, ff.t * gg.t);	
	}
	
	public static PTwo operator*  (PTwo ff, int gg) {
		return new PTwo(ff.s * gg, ff.t * gg);	
	}
	
	
	public static PTwo operator*  (PTwo ff, float gg) {
		return new PTwo((int)(ff.s * gg), (int) (ff.t * gg));	
	}
	
	public static PTwo operator%  (PTwo ff, PTwo gg) {
		return new PTwo(ff.s % gg.s, ff.t % gg.t);	
	}
	
	public static PTwo Min(PTwo aaa, PTwo bbb) {
		return new PTwo( (aaa.s < bbb.s) ? aaa.s : bbb.s , (aaa.t < bbb.t ) ? aaa.t : bbb.t);	
	}
	
	public static PTwo Max(PTwo aaa, PTwo bbb) {
		return new PTwo( (aaa.s > bbb.s) ? aaa.s : bbb.s , (aaa.t > bbb.t ) ? aaa.t : bbb.t);	
	}
	
	public static bool Equal(PTwo aaa, PTwo bbb) {
		return aaa.s == bbb.s && aaa.t == bbb.t;	
	}
	
	public static bool GreaterThan(PTwo aaa, PTwo bbb) {
		return aaa.s > bbb.s && aaa.t > bbb.t;	
	}
	
	public static bool GreaterThanOrEqual(PTwo aaa, PTwo bbb) {
		return aaa.s >= bbb.s && aaa.t >= bbb.t;	
	}
	
	public static PTwo Abs(PTwo aa) {
		return new PTwo(Mathf.Abs(aa.s), Mathf.Abs(aa.t) );
	}
	
	public static int GreaterDimension(PTwo _a) {
		if (_a.s > _a.t) 
			return _a.s;
		return _a.t;
	}
	
	public static int LesserDimension(PTwo _a) {
		if (_a.s < _a.t) 
			return _a.s;
		return _a.t;
	}
	
	public bool isIndexSafe(PTwo indexPoint) {
		// NOTE. this index safe works backwards resp. to coord's isIndexSafe (yikes!)
		return PTwo.GreaterThan(this, indexPoint) && PTwo.GreaterThanOrEqual(indexPoint, PTwo.PTwoZero());
//		return (indexPoint.s < this.s && indexPoint.t < this.t);	
	}
	
	public bool hasANegativeElement() {
		return this.s < 0 || this.t < 0;	
	}
	
}

public struct VertexTwo
{
	public PTwo coord;
//	int tri_index;
	
	public VertexTwo(PTwo _ctwo) {  //, int _tri_index) {
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

public struct LightCorners
{
	public byte oo;
	public byte mo;
	public byte om;
	public byte mm;
	
	public LightCorners(byte _oo, byte _mo, byte _om, byte _mm)
	{
		oo = _oo; mo = _mo; om = _om; mm = _mm;
	}
	
	public LightCorners(byte _all_values)
	{
		this = new LightCorners(_all_values,_all_values,_all_values,_all_values);
	}
	
	public LightCorners(byte _top, byte _bottom)
	{
		this = new LightCorners(_top,_top,_bottom,_bottom);
	}
	
	public static LightCorners LightCornersFromRangeTopBottom( Range1D _range) 
	{
		return new LightCorners(_range.top_light_level, _range.bottom_light_level);
	}
	
	public static LightCorners CombineHorizontally(LightCorners left, LightCorners right) {
		return new LightCorners(left.oo, right.mo, left.om, right.mm);	
	}
	
	public void setRightCornersWithRange(Range1D right_side_range)
	{
		this.mo = right_side_range.top_light_level;
		this.mm = right_side_range.bottom_light_level;
	}
	
	public void setLeftCornersWithRange(Range1D right_side_range)
	{
		this.oo = right_side_range.top_light_level;
		this.om = right_side_range.bottom_light_level;
	}
	
	public static LightCorners MaxLightLightCorners() {
		return new LightCorners(Block.MAX_LIGHT_LEVEL);	
	}
}

public struct LitQuad
{
	public Quad quad;
	public LightCorners lightCorners;
	
	public LitQuad(Quad _quad, LightCorners _lightCorners) {
		quad = _quad; lightCorners = _lightCorners;
	}
	
	public LitQuad(Quad _quad) {
		this = new LitQuad(_quad, LightCorners.MaxLightLightCorners() );
	}	
	
	public static LitQuad LitQuadFromStrip(Strip strip, int horizontalLocation) {
		return new LitQuad(Quad.QuadFromStrip(strip, horizontalLocation), LightCorners.LightCornersFromRangeTopBottom(strip.range));
	}
}

public struct Quad
{
	public PTwo origin;
	public PTwo dimensions;
	
	public Quad(PTwo origin_, PTwo dims) {
		origin = origin_; dimensions = dims;
	}
	
	public static Quad QuadZero() {
		return new Quad(PTwo.PTwoZero(), PTwo.PTwoZero());
	}
	
	public static Quad UnitQuadWithPoint(PTwo point) {
		return new Quad(point, new PTwo(1,1) );	
	}
	
	public static Quad UnitQuadWithAlignedCoord(AlignedCoord alco) {
		return new Quad(PTwo.PTwoFromAlignedCoord(alco), new PTwo(1,1));	
	}
	
	public static Quad QuadWithOriginAndExtent(PTwo origin, PTwo extent) {
		return new Quad( origin, extent - origin);	
	}
	
	public string toString() {
		return "Quad: origin: " + origin.toString() + " and dims: " + dimensions.toString();	
	}
	
	public PTwo extent() {
		return origin + dimensions;	
	}
	
	public PTwo extentMinusOne() {
		return origin + dimensions - new PTwo(1,1);	
	}
	
	public static bool Equal(Quad _a, Quad _b) {
		return PTwo.Equal(_a.origin, _b.origin) && PTwo.Equal(_a.dimensions, _b.dimensions);	
	}
	
	public static Quad theErsatzNullQuad() {
		return new Quad(new PTwo (-1, -777), new PTwo (-12399, -88) );	
	}
	
	public bool isErsatzNull() {
		return this.dimensions.s == -12399;	
	}
	
	public bool contains(PTwo point) {
		return this.sRange().contains(point.s) && this.tRange().contains(point.t);	
	}
	
	public Quad expandedToContainPoint(PTwo point) {
		return Quad.QuadWithOriginAndExtent(PTwo.Min(this.origin, point) , PTwo.Max(this.extent(), point.plusOne()) );
	}
	
	public Quad limitExtent(PTwo extentLimit) {
		return Quad.QuadWithOriginAndExtent(this.origin, PTwo.Min(this.extent(), extentLimit));	
	}
	
	public Quad upperLeftQuarter() {
		return 	new Quad(this.origin, this.dimensions * .5f);
	}
	
	public Quad upperRightQuarter() {
		PTwo shift = this.dimensions * .5f;
		return new Quad(new PTwo (this.origin.s + shift.s , this.origin.t), new PTwo(this.dimensions.s - shift.s , shift.t ));
	}
	
	public Quad lowerLeftQuarter() {
		PTwo shift = this.dimensions * .5f;
		return new Quad(new PTwo (this.origin.s , this.origin.t + shift.t), new PTwo(shift.s , this.dimensions.t - shift.t ));	
	}
	
	public Quad lowerRightQuarter() {
		PTwo shift = this.dimensions * .5f;
		return new Quad( this.origin + shift, this.dimensions - shift);	
	}
	
	public static Quad QuadFromStrip(Strip strip, int hLocation) {
		return new Quad( new PTwo(hLocation, strip.range.start) , new PTwo(strip.width, strip.range.range) );	
	}
	
	//TODO: test this func.
	public static Quad Intersection(Quad ii, Quad kk) {
		Range1D ii_s = new Range1D(ii.origin.s, ii.dimensions.s);
		Range1D ii_t = new Range1D(ii.origin.t, ii.dimensions.t);
		Range1D kk_s = new Range1D(kk.origin.s, kk.dimensions.s);
		Range1D kk_t = new Range1D(kk.origin.t, kk.dimensions.t);
		
		ii_s = Range1D.IntersectingRange(ii_s, kk_s);
		
		kk_t = Range1D.IntersectingRange(ii_t, kk_t);
		
		if (ii_s.isErsatzNull() || kk_t.isErsatzNull() )
			return Quad.theErsatzNullQuad();
		
		return new Quad(new PTwo(ii_s.start, kk_t.start), new PTwo(ii_s.range, kk_t.range) );
	}
	
	public Range1D sRange() {
		return new Range1D(this.origin.s, this.dimensions.s);	
	}
	
	public Range1D tRange() {
		return new Range1D(this.origin.t, this.dimensions.t);	
	}
	
//	public static Quad Union(Quad one, Quad two)
//	{
////		PTwo newO = 	
//	}
}

public struct Strip
{
	public Range1D range;
	public IndexSet indexSet;
	public int width;
	
	public int quadIndex;
	
	private static int QUAD_INDEX_DEFAULT = -1;
	
	public Strip(Range1D rr, IndexSet iset, int ww) {
		range = rr; indexSet = iset; width = ww; quadIndex = QUAD_INDEX_DEFAULT;
	}
	
	public Strip(Range1D rr, IndexSet iset) {
		range = rr; indexSet = iset; width = 1; quadIndex = QUAD_INDEX_DEFAULT;
	}
	
	public Strip(Range1D rr) {
		range = rr; indexSet = IndexSet.theErsatzNullIndexSet(); width = 1;	quadIndex = QUAD_INDEX_DEFAULT;
	}
	
	public void resetQuadIndex() {
		quadIndex = QUAD_INDEX_DEFAULT;
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
	public float hilliness; // 0 to 1f
	public float cragginess;
	public float overhangness;
	public float caveVerticalFrequency;
	public float baseElevation;
	
	public static BiomeInputs Pasture()
	{
		BiomeInputs inputs = new BiomeInputs();
		//pasture
		inputs.hilliness = .32f;
		inputs.cragginess = 2.2f;
		inputs.overhangness = .1f;	
		inputs.caveVerticalFrequency = 3.5f;
		inputs.baseElevation = .35f;
		
		return inputs;
	}
	
	public static BiomeInputs CraggyMountains()
	{
		BiomeInputs inputs = new BiomeInputs();
		
		//cragginess
		inputs.hilliness = .75f;
		inputs.cragginess = 3.2f;
		inputs.overhangness = 8.5f;
		inputs.caveVerticalFrequency = 9.4f;
		inputs.baseElevation = .6f;
		
		return inputs;
	}
	
	public static BiomeInputs Lerp(BiomeInputs abi, BiomeInputs bbi, float t) {
		t = t > 1 ? 1 : t;
		t = t < 0 ? 0 : t;
		
		return BiomeInputs.Mix(abi, bbi, 1f - t, t);
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
		abi.baseElevation *= k;
		return abi;
	}
	
	public static BiomeInputs Add(BiomeInputs abi, BiomeInputs bbi) {
		abi.hilliness += bbi.hilliness;
		abi.cragginess += bbi.cragginess;
		abi.overhangness += bbi.overhangness;
		abi.caveVerticalFrequency += bbi.caveVerticalFrequency;
		abi.baseElevation += bbi.baseElevation;
		return abi;
	}
	

}
