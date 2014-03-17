using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LightColumn : iRange, IEquatable<LightColumn>
{
	private List<LightColumn> m_InfluencedColumns = new List<LightColumn>();
	public byte lightLevel;
	public SimpleRange range;
	public PTwo coord;
	public bool needsUpdate = true;
	
	public List<LightColumn> influencedColumns
	{
		get {
			return m_InfluencedColumns;
		}
	}
	
	public Coord baseCoord
	{
		get {
			return new Coord(coord.s, range.start, coord.t);
		}
	}
	
	public LightColumn Copy()
	{
		return new LightColumn(this.coord, this.lightLevel, this.range);
	}
	
	public LightColumn(PTwo _coord, byte _lightLevel, SimpleRange _range)
	{
		lightLevel = _lightLevel;
		coord = _coord;
		range = _range;
	}
	
	public LightColumn()
	{
		
	}
	
	public void setLightLevelToMax()
	{
		lightLevel = Window.LIGHT_LEVEL_MAX_BYTE;
	}
	
	public void resetLightLevel()
	{
		lightLevel = 0;
	}
	
	public bool takeInfluenceFromColumn(LightColumn other)
	{
		if (!SimpleRange.RangesIntersect(this.range, other.range))
			return false;
		
		if (this.lightLevel < other.lightLevel - Window.UNIT_FALL_OFF_BYTE)
		{
			this.lightLevel = (byte)(other.lightLevel - Window.UNIT_FALL_OFF_BYTE);
			return true;
		}
		return false;
	}
	
	public bool Equals(LightColumn other)
	{
		return SimpleRange.Equal(range, other.range) && PTwo.Equal(coord, other.coord);
	}
	
	#region iRange
	
	public int startP{
		get {
			return this.range.start;
		}set {
			this.range.start = value;	
		}
	}
	
	public int rangeP{
		get{
			return this.range.range;
		} set {
			this.range.range = value;
		}
	}
	
	public int extent() {
		return this.range.extent();
	}
	
	public bool contains(int i) {
		return this.range.contains(i);
	}
	public bool contains(iRange other) {
		return this.range.contains(other);
	}
	public OverlapState overlapStateWith(iRange other) {
		return this.range.overlapStateWith(other);
	}
	public bool intersectsWith(iRange other) {
		return this.range.intersectsWith(other);
	}
	public iRange intersection(iRange other) {
		return this.range.intersection(other);
	}
	public bool isErsatzNull() {
		return this.range.isErsatzNull();
	}
	public bool Equals(iRange other) {
		return this.range.Equals(other);
	}
	
	public iRange theErsatzNullIRange() 
	{
		return null; // (iRange) SimpleRange.theErsatzNullRange();
	}
	
	public string toString()
	{
		return "Light Colm: start: " + startP + " range: " + rangeP + " coord: " + coord.toString() + " lightlev: " + lightLevel;
	}
	
	public bool heightIsBelowExtent(int surfaceHeight)
	{
		return this.extent() > surfaceHeight;
	}
	
	#endregion
	
	#region debug
	
	public Column toColumnDebug()
	{
		Column col = new Column();
		col.range = this.range;
		col.xz = this.coord;
		col.handyInteger = (int) this.startP;
		return col;
	}
	
	#endregion
	
}
