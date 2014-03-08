using UnityEngine;
using System.Collections;

using System.ComponentModel;
using System.Threading;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;

using System.IO;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Runtime.ConstrainedExecution;
using System.Diagnostics;

using System.Runtime.Serialization.Formatters.Binary;

//public struct CavePoint
//{
//	//TODO: purge?
//}

public enum RelationToRange {
	BelowRange, WithinRange, AboveRange
}

public enum OverlapState {
	BeforeStart = 0, FlushWithStart, FlushWithExtent, BeyondExtent, 
	OverlappingOverStart, OverlappingOverEnd, 
	IContainIt, ItContainsMe
}

public static class OverLapUtil
{
	public static bool OverlapExists(OverlapState overlapState) {
		return overlapState >= OverlapState.OverlappingOverStart;	
	}
	
	public static bool OverlapStateIsABeyondState(OverlapState oState) {
		return oState == OverlapState.BeyondExtent || 
				oState == OverlapState.OverlappingOverEnd || 
				oState == OverlapState.FlushWithExtent ||
				oState == OverlapState.ItContainsMe;	
	}
	
//	public static bool 
}

public struct AlignedCoord
{
	public short acrossS, upS;
	
	public int across {
		get {
			return (int) acrossS;
		} set {
			acrossS = (short) value;
		}
	}
	
	public int up {
		get {
			return (int) upS;
		} set {
			upS = (short) value;
		}
	}
	
	public AlignedCoord(int _ac, int _up) {
		across = _ac;
		up = _up;
	}
	
	public AlignedCoord acrossMinusOne() {
		return new AlignedCoord(this.across - 1, this.up);	
	}
	
	public AlignedCoord upMinusOne() {
		return new AlignedCoord(this.across, this.up - 1);
	}
	
	public string toString() {
		return "Aligned coord: across: " + this.across + " up: " + this.up;
	}
	
	public bool isIndexSafe(AlignedCoord arrayDims) {
		return (this.across >= 0 && this.up >= 0 &&	this.across < arrayDims.across && this.up < arrayDims.up);
	}
	
	public static AlignedCoord LesserValues(AlignedCoord _aa, AlignedCoord _bb) {
		return new AlignedCoord( _aa.across < _bb.across ? _aa.across : _bb.across, _aa.up < _bb.up ? _aa.up : _bb.up);	
	}
}

//[Serializable]
//public struct BlockRange
//{
//	public Range1D range_;
//	public BlockType blockType;
//	public byte top_light_level; // todo: get rid of these horrible add-ons
//	public byte bottom_light_level;
//	
//	public BlockRange(int _start, int _range, BlockType btype, byte light_level_, byte bottom_light_level_) 
//	{
//		start = _start; range = _range;	blockType = btype; top_light_level = light_level_; bottom_light_level = bottom_light_level_;
//	}
//	
//	public BlockRange(Range1D rr, BlockType btype, byte light_level_, byte bottom_light_level_) 
//	{
//		this = new BlockRange (rr.start, rr.range, btype, light_level_, bottom_light_level_);
//	}
//	
//	public BlockRange(int _start, int _range, BlockType btype) 
//	{
//		this = new Range1D(_start, _range, btype, 10, 10); 
//	}
//	
//}

[Serializable]
public struct SimpleRange : IEquatable<SimpleRange>, iRange
{
	public short startShort, rangeShort; // TODO: convert to short?
	
	public int start {
		get {
			return (int) startShort;
		} set {
			startShort = (short) value;
		}
	}
	
	public int range {
		get {
			return (int) rangeShort;
		} set {
			rangeShort = (short) value;
		}
	}
	
	#region iRange
	public int startP{
		get {
			return start;
		}
		set {
			start = value;
		}
	}
	
	public int rangeP{
		get{
			return range;
		}
		set {
			range = value;
		}
	}

// 	int extent(); //below	
//	bool contains(iRange other) {} //below
//	bool contains(int i){} //below
	
	public OverlapState overlapStateWith(iRange other)
	{
		return this.overlapWithRange(other);
	}
	
	public bool intersectsWith(iRange other)
	{ 
		return SimpleRange.RangesIntersect(this, other); 
	}
	
	public iRange intersection(iRange other)
	{
		return SimpleRange.IntersectingIRange(this, other);
	}
	
	#endregion
	
	public SimpleRange (int _start, int _range) 
	{
		start = _start; range = _range;
	}
	
	public Range1D convertToRange1D() {
		return new Range1D(this.start, this.range);	
	}
	
	public static SimpleRange SimpleRangeWithStartAndExtent(int start_, int extent_) {
		return new SimpleRange(start_, extent_ - start_);	
	}
	
	public bool Equals(SimpleRange other)
	{
		return this.start == other.start && this.range == other.range;
	}
	
	public bool Equals(iRange other) 
	{
//		return this.Equals((SimpleRange) other);
		return this.startP == other.startP && this.rangeP == other.rangeP;
	}
	
	public int extent() {
		return this.start + this.range;
	}
	
	public int extentMinusOne() {
		return this.extent() - 1;
	}
	
	public static SimpleRange theErsatzNullRange() {
		return new SimpleRange(-999123, -1);
	}
	
	public static bool Equal(SimpleRange aa, SimpleRange bb) {
		return (aa.start == bb.start && aa.range == bb.range);	
	}
	
	public bool isErsatzNull() {
		return SimpleRange.Equal(this, SimpleRange.theErsatzNullRange() );	
	}
	
	public iRange theErsatzNullIRange() {
		return (iRange) SimpleRange.theErsatzNullRange();
	}
	
	public bool contains(int index) {
		return index >= this.start && index < this.extent();	
	}
	
	public RelationToRange relationToRange(int index) {
		if (this.contains(index))
			return RelationToRange.WithinRange;
		if (index < this.start)
			return RelationToRange.BelowRange;
		
		return RelationToRange.AboveRange;
	}
		
	public bool isOneBelowStart(int index) {
		return index == this.start - 1;
	}
	
	public bool isOneAboveRange(int index) {
		return this.extent() == index;	
	}
	
	public string toString() {
		return "SimpleRange->start: " + this.start + " range: " + this.range;	
	}
	
	public static SimpleRange Copy(SimpleRange copyMe) {
		return new SimpleRange(copyMe.start, copyMe.range);	
	}
	// set funcs.
	
	public int midPoint() {
		return (int)(((start + extent() )/2f) + .5f);	
	}
	
	public SimpleRange subRangeAboveRange(SimpleRange excluder) {
		
		return subRangeAbove(excluder.extentMinusOne());
	}
	
	//TODO: fix sub range so it only can return a real sub range:
	// this func. really gives the area above the level.
	public SimpleRange subRangeAbove(int level) {
		if (level < this.start)
			return this;
		
		int newRange = this.extentMinusOne() - level;
		if (newRange <= 0)
			return SimpleRange.theErsatzNullRange();
		
		return new SimpleRange(level + 1, newRange);
	}
	
	public SimpleRange setRangeStartTo(int level) {
		
		int newRange = this.extent() - level - 1;
		if (newRange <= 0)
			return SimpleRange.theErsatzNullRange();
		
		return new SimpleRange(level + 1, newRange);
	}
	
	public SimpleRange subRangeBelow(int level) {
		if (level <= this.start)
			return SimpleRange.theErsatzNullRange();
		
		int min = Mathf.Min(this.extent(), level);
		
		return new SimpleRange(this.start, min - this.start);
	}
	
	public SimpleRange extendRangeByOne() {
		return new SimpleRange(this.start, this.range + 1);
	}	
	
	public SimpleRange subtractOneFromStart() {
		return this.adjustStartBy(-1);
	}
	
	public SimpleRange adjustStartBy(int adjustBy) {
		return new SimpleRange(this.start + adjustBy, this.range - adjustBy);	
	}
	
	public SimpleRange extendRangeToInclude(SimpleRange extender) {
		return new SimpleRange(this.start, extender.extentMinusOne() - this.start);	
	}
	
	//new for SR
	public SimpleRange extendRangeToInclude(int extendAbsPosition) {
		int new_start = Mathf.Min(start, extendAbsPosition);
		int new_extent = Mathf.Max(this.extent(), extendAbsPosition);
		return SimpleRange.SimpleRangeWithStartAndExtent(new_start, new_extent);
	}
	
	public static bool RangesIntersect(iRange raa, iRange rbb)
	{
		return !(SimpleRange.IntersectingIRange(raa, rbb).isErsatzNull() );
	}
	
	public static iRange IntersectingIRange(iRange raa, iRange rbb)
	{
		return (iRange) SimpleRange.IntersectingRange(raa, rbb);
	}
	
	public static SimpleRange IntersectingRange(iRange raa, iRange rbb)
	{
		int interExtent = raa.extent() < rbb.extent() ? raa.extent() : rbb.extent();
		int interStart = raa.startP > rbb.startP ? raa.startP : rbb.startP;
		
		if (interStart >= interExtent)
			return SimpleRange.theErsatzNullRange();
		
		return new SimpleRange(interStart, interExtent - interStart);
	}	
		
	public OverlapState overlapWithRange(iRange other)
	{
		if (this.start == other.extent())
			return OverlapState.FlushWithStart;
		
		if (this.start > other.extent())
			return OverlapState.BeforeStart;
		
		if (this.extent() == other.startP)
			return OverlapState.FlushWithExtent;
		
		if (RangesIntersect(this, other)) 
		{
			if (this.contains(other)) {
				return OverlapState.IContainIt;					
			}
			if (other.contains(this)) {
				return OverlapState.ItContainsMe;	
			}
			if (other.startP < this.startP)
				return OverlapState.OverlappingOverStart;
			
			return OverlapState.OverlappingOverEnd;
		}
		
		return OverlapState.BeyondExtent;
	}

	
	public static bool SimpleRangeCoversRange(SimpleRange covering, SimpleRange covered)
	{
		return covering.start <= covered.start && covering.extent() >= covered.extent();
	}
	
	public static SimpleRange Average(SimpleRange _aa, SimpleRange _bb)
	{
		return new SimpleRange((_aa.start + _bb.start)/2, (_aa.range + _bb.range)/2);	
	}
	
	public bool contains(iRange r) {
		return this.startP <= r.startP && this.extent() >=r.extent();	
	}
	
}


[Serializable]
public struct Range1D
{
	public short startShort, rangeShort; // TODO: convert to byte?
//	public int start, range; // TODO: convert to byte?
	
	public int start {
		get {
			return (int) startShort;
		} set {
			startShort = (short) value;
		}
	}
	
	public int range {
		get {
			return (int) rangeShort;
		} set {
			rangeShort = (short) value;
		}
	}
	
		
	public static Range1D theErsatzNullRange() {
		return new Range1D(-999123, -1);
	}
	
	public BlockType blockType;
	
//	public byte top_light_level; // todo: get rid of these horrible add-ons
//	public byte bottom_light_level;

	//TODO: move the above accounting variables to another struct
	//that owns a range1d——and replace a bunch of code so that it deals with this new struct
	
//	public Range1D(int _start, int _range, BlockType btype, byte light_level_, byte bottom_light_level_) 
//	{
//		start = _start; range = _range;	blockType = btype; top_light_level = light_level_; bottom_light_level = bottom_light_level_;
//	}
	
	public Range1D(int _start, int _range, BlockType btype) 
	{
		start = _start; range = _range;	blockType = btype;
//		this = new Range1D(_start, _range, btype, Block.MAX_LIGHT_LEVEL, Block.MAX_LIGHT_LEVEL); 
	}
	
	public Range1D(int _start, int _range) 
	{
		this = new Range1D(_start, _range, BlockType.Dirt);
	}
	
	public static Range1D RangeWithStartAndExtent(int start, int extent)
	{
		return new Range1D(start, extent - start);	
	}
	
	public static Range1D RangeWithRangeAndExtent(int range, int extent)
	{
		return new Range1D(extent - range, range);
	}
	
	public int extent() {
		return this.start + this.range;
	}
	
	public int extentMinusOne() {
		return this.extent() - 1;
	}

	
	public static bool Equal(Range1D aa, Range1D bb) {
		return (aa.start == bb.start && aa.range == bb.range);	
	}
	
	public bool isErsatzNull() {
		return Range1D.Equal(this, Range1D.theErsatzNullRange() );	
	}
	
	public void assertNotNull(string str) {
		AssertUtil.Assert(!Range1D.Equal(this, Range1D.theErsatzNullRange()), str);
	}
	
	public bool contains(int index) {
		return index >= this.start && index < this.extent();	
	}
	
	public bool containsOrFlushWithEitherEnd(int index) {
		return index >= this.start - 1 && index <= this.extent();
	}
	
	public RelationToRange relationToRange(int index) {
		if (this.contains(index))
			return RelationToRange.WithinRange;
		if (index < this.start)
			return RelationToRange.BelowRange;
		
		return RelationToRange.AboveRange;
	}
		
	public bool isOneBelowStart(int index) {
		return index == this.start - 1;
	}
	
	public bool isOneAboveRange(int index) {
		return this.extent() == index;	
	}
	
	public string toString() {
		return "Range1D->start: " + this.start + " range: " + this.range;	
	}
	
	public static Range1D Copy(Range1D copyMe) {
		return new Range1D(copyMe.start, copyMe.range, copyMe.blockType); //, copyMe.top_light_level, copyMe.bottom_light_level );	
	}
	// set funcs.
	
	public Range1D subRangeAboveRange(Range1D excluder) {
		return subRangeAbove(excluder.extentMinusOne());
	}
	
	public Range1D subRangeAbove(int level) {
		if (level < this.start)
			return this;
		
		int newRange = this.extentMinusOne() - level;
		if (newRange <= 0)
			return Range1D.theErsatzNullRange();
		
		return new Range1D(level + 1, newRange, this.blockType); //, this.top_light_level, this.bottom_light_level);
	}
	
	public Range1D setRangeStartTo(int level) {
		
		int newRange = this.extent() - level - 1;
		if (newRange <= 0)
			return Range1D.theErsatzNullRange();
		
		return new Range1D(level + 1, newRange, this.blockType); //, this.top_light_level, this.bottom_light_level);
	}
	
	public Range1D subRangeBelowRange(Range1D other) 
	{
		return subRangeBelow(other.start);
	}
	
	public Range1D subRangeBelow(int level) {
		if (level <= this.start)
			return Range1D.theErsatzNullRange();
		
		int min = Mathf.Min(this.extent(), level);
		
		return new Range1D(this.start, min - this.start, this.blockType); //, this.top_light_level, this.bottom_light_level);
	}
	
	public Range1D extendRangeByOne() {
		return new Range1D(this.start, this.range + 1, this.blockType); //, this.top_light_level, this.bottom_light_level);
	}	
	
	public Range1D subtractOneFromStart() {
		return this.adjustStartBy(-1);
	}
	
	public Range1D adjustStartBy(int adjustBy) {
		return new Range1D(this.start + adjustBy, this.range - adjustBy, this.blockType); //, this.top_light_level, this.bottom_light_level);	
	}
	
	public Range1D extendRangeToInclude(Range1D extender) {
		return new Range1D(this.start, extender.extent() - this.start, this.blockType); //, this.top_light_level, this.bottom_light_level);	
	}
	
	public static bool RangesIntersect(Range1D raa, Range1D rbb)
	{
		return !(Range1D.IntersectingRange(raa, rbb).isErsatzNull() );
	}
	
	public OverlapState overlapWithRange(Range1D other)
	{
		if (this.start == other.extent())
			return OverlapState.FlushWithStart;
		
		if (this.start > other.extent())
			return OverlapState.BeforeStart;
		
		if (this.extent() == other.start)
			return OverlapState.FlushWithExtent;
		
		if (RangesIntersect(this, other)) 
		{
			if (this.contains(other)) {
				return OverlapState.IContainIt;					
			}
			if (other.contains(this)) {
				return OverlapState.ItContainsMe;	
			}
			if (other.start < this.start)
				return OverlapState.OverlappingOverStart;
			
			return OverlapState.OverlappingOverEnd;
		}
		
		return OverlapState.BeyondExtent;
	}
	
	public static Range1D IntersectingRange(Range1D raa, Range1D rbb)
	{
		int interExtent = raa.extent() < rbb.extent() ? raa.extent() : rbb.extent();
		int interStart = raa.start > rbb.start ? raa.start : rbb.start;
		
		if (interStart >= interExtent)
			return Range1D.theErsatzNullRange();
		
		return new Range1D(interStart, interExtent - interStart, raa.blockType);
//		int extentDif = raa.e
	}
	
	public static Range1D Union(Range1D raa, Range1D rbb)
	{
		int unionExtent = raa.extent() > rbb.extent() ? raa.extent() : rbb.extent();
		int unionStart = raa.start < rbb.start ? raa.start : rbb.start;	
		
		if (unionStart >= unionExtent)
			return Range1D.theErsatzNullRange();
		
		return new Range1D(unionStart, unionExtent - unionStart, raa.blockType);
	}
	
	public bool contains(Range1D r) {
		return this.start <= r.start && this.extent() >=r.extent();	
	}
	
	public static Range1D[] splitRangeIntoTopAndRemainder(Range1D splitMe, BlockType remainderType) {
		Range1D[] result = new Range1D[2];
		Range1D top = new Range1D(splitMe.extentMinusOne(), 1, remainderType); //, splitMe.top_light_level, splitMe.top_light_level);
		splitMe.range--;
		result[0] = top;
		result[1] = splitMe;
		return result;
	}
	
}

[Serializable]
public struct Coord : IEquatable<Coord>
{
	public int x, y, z;

	public Coord(int xx, int yy, int zz) 
	{
		x = xx; y = yy; z = zz;
	}

	public Coord(int val) 
	{
		x = val; y = val; z = val;
	}

	public Coord(uint xx, uint yy, uint zz) 
	{
		x = (int) xx; y =(int) yy; z = (int)zz;
	}
	
	public Coord(NoiseCoord nco) {
		x = nco.x; y = 0; z = nco.z;
	}

	public Coord (ChunkCoord cc) {
		x = (int) cc.x; y = (int) cc.y; z = (int) cc.z;
	}

	public Coord(Vector3 vv)
	{
		//		this = new Coord (vv.x , vv.y, vv.z);
		this = new Coord (vv.x + .5, vv.y + .5, vv.z + .5);
	}

	public Coord(long  xx, long  yy, long zz) 
	{
		x = (int) xx; y =(int) yy; z = (int)zz;
	}

	public Coord(double   xx, double  yy, double zz) 
	{
		x = (int) xx; y =(int) yy; z = (int)zz;
	}

	public Coord(float   xx, float  yy, float zz) 
	{
		x = (int) xx; y =(int) yy; z = (int)zz;
	}

	public Coord(int xx, uint yy, int zz) 
	{
		x = (int) xx; y =(int) yy; z = (int)zz;
	}
	
	//TODO: purge the static equals func.
	public bool Equals(Coord other) {
		return this.x == other.x && this.y == other.y && this.z == other.z;	
	}

	public static Coord coordZero() {
		return new Coord (0, 0, 0);
	}

	public static Coord coordOne() {
		return new Coord (1, 1, 1);
	}

	public static bool greaterThan( Coord aa, Coord bb) {
		return aa.x > bb.x && aa.y > bb.y && aa.z > bb.z;
	}

	public static bool greaterThanOrEqual( Coord aa, Coord bb) {
		return aa.x >= bb.x && aa.y >= bb.y && aa.z >= bb.z;
	}

	public static Coord operator *(Coord aa, Coord bb) {
		return new Coord (aa.x * bb.x, aa.y * bb.y, aa.z * bb.z);
	}

	public static Coord operator /(Coord aa, Coord bb) {
		return new Coord (aa.x / (float) bb.x, aa.y /(float) bb.y, aa.z /(float) bb.z);
	}

	public static Coord operator +(Coord aa, Coord bb) {
		return new Coord (aa.x + bb.x, aa.y + bb.y, aa.z + bb.z);
	}

	public static Coord operator -(Coord aa, Coord bb) {
		return new Coord (aa.x - bb.x, aa.y - bb.y, aa.z - bb.z);
	}

	public static Coord operator -(Coord aa, int  bb) {
		return new Coord (aa.x - bb, aa.y - bb, aa.z - bb );
	}

	public static Coord operator *(Coord aa, float bb) {
		return new Coord (aa.x * bb , aa.y * bb , aa.z * bb );
	}

	public static Coord operator /(Coord aa, float  bb) {
		return new Coord (aa.x / bb , aa.y / bb , aa.z / bb ); // TODO: what happens when we div by zero???
	}

	public static Coord operator %(Coord aa, float  bb) {
		return new Coord (aa.x % bb , aa.y % bb , aa.z % bb );
	}

	public static Coord operator %(Coord aa, int  bb) {
		return new Coord (aa.x % bb , aa.y % bb , aa.z % bb );
	}

	public static Coord operator % (Coord aa, Coord bb) {
		return new Coord (aa.x % bb.x , aa.y % bb.y , aa.z % bb.z );
	}

	public static Coord operator +(Coord aa, float  bb) {
		return new Coord (aa.x + bb , aa.y + bb , aa.z + bb );
	}

	public static Coord operator -(Coord aa, float bb) {
		return new Coord (aa.x - bb, aa.y - bb , aa.z - bb );
	}
	
	public Coord addPTwoToXZ(PTwo point) {
		return new Coord(this.x + point.s, this.y, this.z + point.t);	
	}

	public bool equalTo(Coord other) {
		return this.x == other.x && this.y == other.y && this.z == other.z;
	}
	
	public void assertNotNull(string str) {
		AssertUtil.Assert(!this.equalTo(Coord.TheErsatzNullCoord()), str);	
	}

	public bool isIndexSafe( Coord arraySizes ) {
		return (Coord.greaterThan (arraySizes, this)) && (Coord.greaterThanOrEqual (this, coordZero ()));
	}

	public Coord makeIndexSafe( Coord arraySizes) {
		return this.makeRangeSafe (new CoRange (Coord.coordZero(), arraySizes));
	}

	public Coord makeRangeSafe(CoRange _coRa) {
		Coord retCo = Coord.Max (this, _coRa.start);
		Coord outerMinus = _coRa.outerLimit () - 1;
		return Coord.Min (this, outerMinus);
	}

	public bool isInsideOfRange(CoRange coRange)
	{
		Coord outerLimit = coRange.outerLimit();
		return (Coord.greaterThanOrEqual (this, coRange.start)) && (Coord.greaterThan (outerLimit, this));


		//		return this.isInsideOfRange (coRange.start, coRange.range);
	}

	public bool isInsideOfRangeInclusive(CoRange coRange)
	{
		Coord outerLimit = coRange.outerLimit();
		return (Coord.greaterThanOrEqual (this, coRange.start)) && (Coord.greaterThanOrEqual (outerLimit, this));

	}

	public static Coord Min (Coord oneone, Coord twotwo)
	{
		return new Coord ((oneone.x < twotwo.x ? oneone.x : twotwo.x), (oneone.y < twotwo.y ? oneone.y : twotwo.y), (oneone.z < twotwo.z ? oneone.z : twotwo.z));
	}

	public static Coord Max (Coord oneone, Coord twotwo)
	{
		return new Coord ((oneone.x > twotwo.x ? oneone.x : twotwo.x), (oneone.y > twotwo.y ? oneone.y : twotwo.y), (oneone.z > twotwo.z ? oneone.z : twotwo.z));
	}

	public Coord onlyPositive ()
	{
		return new Coord ((this.x < 0 ? 0 : this.x), (this.y < 0 ? 0 : this.y ), (this.z < 0 ? 0 : this.z));
	}

	public Coord onlyNegative()
	{
		return new Coord ((this.x >= 0 ? 0 : this.x), (this.y >= 0 ? 0 : this.y ), (this.z >= 0 ? 0 : this.z));
	}

	public Coord booleanPositive()
	{
		return new Coord ((this.x < 0 ? 0 : 1), (this.y < 0 ? 0 : 1 ), (this.z < 0 ? 0 : 1));
	}

	public Coord booleanNegative()
	{
		return new Coord ((this.x >= 0 ? 0 : 1), (this.y >= 0 ? 0 : 1), (this.z >= 0 ? 0 : 1));
	}

	public Coord negNegOnePosPosOne()
	{
		return new Coord ((this.x > 0 ? 1 : -1), (this.y > 0 ? 1 : -1), (this.z > 0 ? 1 : -1));
	}
	
	public static Coord FlipXZ(Coord flipMe) 
	{
		return new Coord(flipMe.z, flipMe.y, flipMe.x);	
	}

	//	public bool isInsideOfRange(Coord start, Coord range)
	//	{
	//		CoRange corange = new CoRange (start, range);
	//		return this.isInsideOfRange (corange);
	//
	////
	////		Coord outerLimit = start + range;
	////		return (Coord.greaterThanOrEqual (this, start)) && (Coord.greaterThan (outerLimit, this));
	//	}

	public string toString() 
	{
		return "X: " + x + " Y: " + y + " Z: " + z;
	}
	public Vector3 toVector3()
	{
		return new Vector3 ((float)x, (float)y, (float)z);
	}
	
	public Coord addNudgeForDirection(Direction dir)
	{
		return this + DirectionUtil.NudgeCoordForDirection(dir);
	}

	public static Vector3 GeomCenterOfBLock(Coord cc) {
		Coord negPos = cc.negNegOnePosPosOne ();
		return cc.toVector3 () + negPos.toVector3 () * .5f;
	}

	public static int Volume(Coord cc) {
		return (cc.x * cc.y * cc.z);
	}
	
	public static int SumOfComponents(Coord coco) {
		return coco.x + coco.y + coco.z;	
	}
	
	public Coord xMinusOne() {
		return new Coord(this.x - 1, this.y, this.z);
	}
	
	public Coord yMinusOne() {
		return new Coord(this.x, this.y - 1, this.z);
	}
	
	public Coord zMinusOne() {
		return new Coord(this.x, this.y, this.z - 1);
	}
	
	public static Coord Not(Coord  aa) {
		aa.x = (aa.x != 0)? 0 : 1;
		aa.y = (aa.y != 0)? 0 : 1;
		aa.z = (aa.z != 0)? 0 : 1;
		return aa;
	}
	
	public static Coord TheErsatzNullCoord() {
		return new Coord (-187923, -123123, -23);	
	}
}

public struct CoRange
{
	public Coord start;
	public Coord range;

	public CoRange( Coord st, Coord ra ) {
		this.start = st;
		this.range = ra;
	}

	public CoRange( Coord st, int ra ) {
		this.start = st;
		this.range = new Coord (ra);
	}

	public Coord outerLimit()
	{
		return this.start + this.range;
	}

	public string toString() {
		return "Range: start: " + this.start.toString () + " range: " + this.range.toString () + " outer limit: " + this.outerLimit().toString();
	}

	public CoRange makeRangeSafeCoRange(CoRange mapDims) {
		CoRange retCo = this;
		retCo.start = Coord.Max (retCo.start, mapDims.start);
		retCo.range = Coord.Min (retCo.range, mapDims.range);
		return retCo;
	}

	public CoRange makeIndexSafeCoRange(Coord mapDims) {
		return this.makeRangeSafeCoRange (new CoRange (Coord.coordZero (), mapDims));
	}

	public static bool Contains(CoRange container, CoRange contained) {
		bool startIsLess = Coord.greaterThanOrEqual (contained.start, container.start);
		return Coord.greaterThan (container.outerLimit(), contained.outerLimit());
	}
}

public struct ChunkCoord
{
	public int x,z;
	public uint y;

	public ChunkCoord(int xx, uint yy, int zz)
	{
		x = xx;
		z = zz;
		y = yy;
	}

	public ChunkCoord (int length) {
		this = new ChunkCoord (length, (uint)length, length);
	}

	public ChunkCoord( Coord cc) {
		x = (int)cc.x;
		y = (uint)cc.y;
		z = (int)cc.z;
	}

	public static ChunkCoord chunkCoordZero()
	{
		return new ChunkCoord (0, 0, 0);
	}

	public static ChunkCoord chunkCoordOne()
	{
		return new ChunkCoord (1, 1, 1);
	}


	public static ChunkCoord operator + (ChunkCoord a, ChunkCoord b)
	{
		return new ChunkCoord (a.x + b.x, a.y + b.y, a.z + b.z);
	}

	public static ChunkCoord operator - (ChunkCoord a, ChunkCoord b)
	{
		return new ChunkCoord (a.x - b.x, a.y - b.y, a.z - b.z);
	}

	public static ChunkCoord operator * (ChunkCoord a, int  b)
	{
		return new ChunkCoord (a.x * b, (uint)(a.y * b), a.z * b);
	}

	public static bool greaterThan(ChunkCoord a, ChunkCoord b)
	{
		return a.x > b.x && a.y > b.y && a.z > b.z;	
	}

	public static bool greaterOrEqual(ChunkCoord a, ChunkCoord b)
	{
		return a.x >= b.x && a.y >= b.y && a.z >= b.z;	
	}

	public static ChunkCoord XPosUnitChunk()
	{
		return new ChunkCoord (1, 0, 0);
	}

	public static ChunkCoord YPosUnitChunk()
	{
		return new ChunkCoord (0, 1, 0);
	}

	public static ChunkCoord ZPosUnitChunk()
	{
		return new ChunkCoord (0, 0, 1);
	}

}

public struct ChunkRange
{
	ChunkCoord start;
	ChunkCoord range;

	public ChunkRange(ChunkCoord st, ChunkCoord ra)
	{
		start = st; range = ra;
	}

	public ChunkRange(ChunkCoord st, int length)
	{
		start = st; 
		range = new ChunkCoord (length);
	}
}

[Serializable]
public struct NoiseCoord : IEquatable<NoiseCoord>
{
	public int x; 
	public int z;

	public NoiseCoord(int xx, int zz) {
		x = xx;
		z = zz;
	}
	
	public NoiseCoord(int aa)  {
		this = new NoiseCoord(aa,aa);	
	}
	
	public static NoiseCoord NoiseCoordZer() {
		return new NoiseCoord(0);	
	}

	public NoiseCoord(Coord cc) {
		x = cc.x;
		z = cc.z;
	}

	public NoiseCoord (ChunkCoord cc) {
		this = new NoiseCoord (new Coord (cc));
	}
	
	public static NoiseCoord TheErsatzNullNoiseCoord() {
		return new NoiseCoord(-99999, 9994);
	}
	
	public static NoiseCoord NoiseCoordWithPTwo(PTwo ptwo) {
		return new NoiseCoord(ptwo.s, ptwo.t);	
	}

	public string toString() {
		return "Noise Coord :) x: " + x + " z: " + z;
	}
	
	public bool Equals(NoiseCoord other) {
		return this.x == other.x && this.z == other.z;	
	}

	public static bool Equal(NoiseCoord aa, NoiseCoord bb) {
		return (aa.x == bb.x) && (aa.z == bb.z);
	}
	
	public bool isCoordZero() {
		return NoiseCoord.Equal(this, new NoiseCoord(0,0));	
	}

	public static NoiseCoord operator + (NoiseCoord aa, NoiseCoord bb) {
		return new NoiseCoord(aa.x + bb.x, aa.z + bb.z); 
	}
	
	public static NoiseCoord operator - (NoiseCoord aa, NoiseCoord bb) {
		return new NoiseCoord(aa.x - bb.x, aa.z - bb.z); 
	}
	
	public static NoiseCoord operator * (NoiseCoord aa, NoiseCoord bb) {
		return new NoiseCoord(aa.x * bb.x, aa.z * bb.z); 
	}
	
	public static NoiseCoord operator * (NoiseCoord aa, int bb) {
		return new NoiseCoord(aa.x * bb, aa.z * bb); 
	}
}


public struct dvektor
{
	public int x,y,z;

	public dvektor(int xx, int yy, int zz) 
	{
		x = xx;
		y = yy;
		z = zz;
	}

	public dvektor(float  xx, float yy, float zz) 
	{
		x = (int) xx;
		y = (int)yy;
		z = (int)zz;
	}

	public dvektor (ChunkIndex ci)
	{
		this = new dvektor (ci.x, ci.y, ci.z);
	}

	public static dvektor dvekXPositive ()
	{
		return new dvektor (1, 0, 0);
	}

	public static dvektor dvekYPositive()
	{
		return new dvektor (0, 1, 0);
	}

	public static dvektor dvekZPositive()
	{
		return new  dvektor (0, 0, 1);
	}

	public static dvektor dvekXNegative ()
	{
		return new dvektor (1, 0, 0) * -1;
	}

	public static dvektor dvekYNegative()
	{
		return new dvektor (0, 1, 0) * -1;
	}

	public static dvektor dvekZNegative()
	{
		return new  dvektor (0, 0, 1) * -1;
	}

	public static dvektor OppositeDirection(dvektor d) {
		return d * -1.0f;
	}

	public static dvektor operator* (dvektor a, float b) {
		return new dvektor (a.x * b, a.y * b, a.z * b);
	}

	public static dvektor operator* (dvektor a, dvektor bb) {
		return new dvektor (a.x * bb.x, a.y * bb.y, a.z * bb.z);
	}

	public static dvektor operator+ (dvektor a, int b) {
		return new dvektor (a.x + b, a.y + b, a.z + b);
	}

	public dvektor (Direction d)
	{
		switch (d) 
		{
		case(Direction.xpos):
			this = dvekXPositive ();
			break;
		case(Direction.xneg):
			this = dvekXNegative ();
			break;
		case(Direction.ypos):
			this = dvekYPositive ();
			break;
		case(Direction.yneg):
			this = dvekYNegative ();
			break;
		case(Direction.zpos):
			this = dvekZPositive ();
			break;
		default: // zneg
			this = dvekZNegative ();
			break;
		}
	}

	public static dvektor Inverse( dvektor aa) {
		int total = aa.x + aa.y + aa.z;
		dvektor retv = aa + (-total);
		return retv * -1;
	}

	public static dvektor Abs( dvektor aa) {
		return aa * aa;
	}

	public int total() {
		return x + y + z;
	}

}

//ChunkIndex (TODO: change name, too easy to confuse with ChunkCoord)
//Chunk indices are meant to hold internal block coords within chunks. (i.e. between x,y,z between integers 0 and 15 for 16 length chunks)
public struct ChunkIndex
{
	public int x,y,z;

	public ChunkIndex(uint xx, uint yy, uint zz) {
		x = (int) xx;
		y = (int) yy;
		z = (int) zz;
	}

	public ChunkIndex(int xx, int yy, int zz) {
		x = (int) xx;
		y = (int) yy;
		z = (int) zz;
	}

	public ChunkIndex(dvektor d) {
		x = (int)d.x;
		y = (int)d.y;
		z = (int)d.z;
	}
	
	public ChunkIndex(Coord d) {
		x = (int)d.x;
		y = (int)d.y;
		z = (int)d.z;
	}
	
	

	public static ChunkIndex ChunkIndexNextToIndex(ChunkIndex ci, Direction dir)
	{
		int idir = (int)dir;
		int which_way = (idir % 2) == 1 ? -1 : 1;
		int xx, yy, zz;
		xx = yy = zz = 0;
		if (dir < Direction.ypos)
			xx = which_way;
		else if (dir < Direction.zpos)
			yy = which_way;
		else
			zz = which_way;

		return new ChunkIndex ((int)(ci.x + xx), (int)( ci.y + yy),(int)( ci.z + zz));
	}

	public static ChunkIndex operator *(ChunkIndex aa, ChunkIndex bb) {
		return new ChunkIndex (aa.x * bb.x, aa.y * bb.y, aa.z * bb.z);
	}

	public static ChunkIndex operator *(ChunkIndex aa, float bb) {
		return new ChunkIndex ((int)(aa.x * bb),(int) (aa.y * bb),(int) (aa.z * bb));
	}

	public static ChunkIndex operator +(ChunkIndex aa, ChunkIndex bb) {
		return new ChunkIndex (aa.x + bb.x, aa.y + bb.y, aa.z + bb.z);
	}

	public string toString(){
		return "Chunk Index: x: " + x + " y: " + y + " z: " + z;
	}


}