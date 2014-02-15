using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public struct Trapezoid
{
	public SimpleRange startRange;
	public SimpleRange endRange;
	public SimpleRange span;
	
	public Trapezoid(SimpleRange _startRange, SimpleRange _endRange, SimpleRange _span)
	{
		startRange = _startRange; endRange = _endRange; span = _span;
	}
	
	public Trapezoid(SimpleRange _startRange, int startZ)
	{
		this = new Trapezoid(_startRange, _startRange, new SimpleRange(startZ, 1));
	}
	
	public PTwo upperNegCorner()
	{
		return new PTwo(this.startRange.extent(), this.span.start);	
	}
	
	public PTwo lowerNegCorner()
	{
		return new PTwo(this.startRange.start, this.span.start);	
	}
	
	public PTwo upperPosCorner()
	{
		return new PTwo(this.endRange.extent(), this.span.extent());	
	}
	
	public PTwo lowerPosCorner()
	{
		return new PTwo(this.endRange.start, this.span.extent());	
	}
	
	public PTwo midPoint()
	{
		int hmid = span.midPoint();
		int vmid =(int)( (startRange.midPoint() + endRange.midPoint())/2f + .5f);
		return new PTwo(hmid, vmid);
	}
}

public struct TrapLight
{
	public float upperNeg;
	public float lowerNeg;
	public float upperPos;
	public float lowerPos;
	
	public TrapLight(float un, float ln, float up, float lp) 
	{
		upperNeg = un; lowerNeg = ln; upperPos = up; lowerPos = lp;	
	}
	
	public static TrapLight MediumLightQuad() 
	{
		float level = Window.LIGHT_LEVEL_MAX * .5f;
		return new TrapLight(level,level,level,level);
	}
	
	public float[] negValues()
	{
		return new float[] {lowerNeg, upperNeg};	
	}
	
	public float[] posValues()
	{
		return new float[] {lowerPos, upperPos};	
	}
	
	public bool allValuesMaxed()
	{
		return this.upperNeg >= Window.LIGHT_LEVEL_MAX && 
			this.lowerNeg >= Window.LIGHT_LEVEL_MAX && 
			this.upperPos >= Window.LIGHT_LEVEL_MAX && 
			this.lowerPos >= Window.LIGHT_LEVEL_MAX;	
	}
	
	public float average() {
		return (upperNeg + lowerNeg + upperPos + lowerPos)/4f;	
	}
}

public struct LightPoint
{
	public PTwo point;
	public float lightValue;
	
	public LightPoint(PTwo _point, float _lightValue) {
		point = _point; lightValue = _lightValue;	
	}
}

public class LightLevelTrapezoid
{
	public Trapezoid trapezoid;
	public TrapLight trapLight;
	
	public LightLevelTrapezoid(TrapLight _trapLight, Trapezoid _trap)
	{
		trapLight = _trapLight;
		trapezoid =_trap;
	}
	
	public LightPoint midPoint() {
		return new LightPoint(trapezoid.midPoint(), trapLight.average());	
	}
	
	#region update with surface adjacency
	
	public void updateLightLevelsWithAdjacentSurfaceHeightSpanOffset(int surfaceHeight, int spanOffset)
	{
		if (trapLight.allValuesMaxed())
			return;
		
		int coverage = this.trapezoid.startRange.extent() - surfaceHeight ;
		float offsetLightValue;
		if (coverage > 0)
		{
			
			offsetLightValue = lightAddedWithSurfaceSpanOffset(spanOffset);
			trapLight.upperNeg = incrementedLightValue(trapLight.upperNeg, offsetLightValue);
			trapLight.lowerNeg = incrementedLightValue(trapLight.lowerNeg, 
				offsetLightValue * Mathf.Clamp((coverage/(float)trapezoid.startRange.range), 0f, 1f));
		}
		
		coverage = this.trapezoid.endRange.extent() - surfaceHeight ;

		if (coverage > 0)
		{
			offsetLightValue = lightAddedWithSurfaceSpanOffset(this.trapezoid.span.extent() - spanOffset);
			trapLight.upperPos = incrementedLightValue(trapLight.upperPos, offsetLightValue);
			trapLight.lowerPos = incrementedLightValue(trapLight.lowerPos, 
				offsetLightValue * Mathf.Clamp((coverage/(float)trapezoid.startRange.range), 0f, 1f));
		}
	}
	
	private float incrementedLightValue(float oldValue, float addToOldValue)
	{
		float newValue = oldValue + addToOldValue;
		return Mathf.Clamp(newValue, 0f, Window.LIGHT_LEVEL_MAX + 1f);
	}
	
	private float lightAddedWithSurfaceSpanOffset(int spanOffset)
	{
		return lightAddedWithDistance(spanOffset, Window.LIGHT_LEVEL_MAX);
//		spanOffset = spanOffset < 0 ? 0 : spanOffset;
//		float travel =  1f - (float)spanOffset/Window.LIGHT_TRAVEL_MAX_DISTANCE;
//		travel = Mathf.Clamp(travel, 0f, 1f);
//		return Window.LIGHT_LEVEL_MAX * travel;
	}
	
	#endregion
	
	private float lightAddedWithDistance(int distance, float sourceLightValue)
	{
		distance = distance < 0 ? 0 : distance;
		float travel =  1f - (float)distance/Window.LIGHT_TRAVEL_MAX_DISTANCE;
		travel = Mathf.Clamp(travel, 0f, 1f);
		return Window.sourceLightValue * travel;
	}
	
	#region update with light point
	
	public void updateWithXAdjacentMidPoint(LightPoint midPoint)
	{
		midPoint.val -= Window.UNIT_FALL_OFF;
		midPoint.val = midPoint.val < 0f ? 0f : midPoint.val;
		
		
	}
	
	private float lightValueAtFromPoint(PTwo destinationPoint, LightPoint fromPoint)
	{
		PTwo diff = PTwo.Abs(destinationPoint - fromPoint.point);
		int dist = diff.s + diff.t/2; // very cheapo pythagoras
		return lightAddedWithDistance(dist, fromPoint.lightValue);
	}
	
	#endregion
	
	
}



public class Window : IEquatable<Window>
{
	List<Window> influencedWindows = new List<Window>();
	
	WindowMap m_windowMap;
	
	private int xLevel;
	private LightLevelTrapezoid lightLevelTrapezoid;
	
	public const float LIGHT_LEVEL_MAX = 24f;
	public const float LIGHT_TRAVEL_MAX_DISTANCE = 16;
	public const float UNIT_FALL_OFF = 1f;
	
	public Coord patchRelativeOrigin {
		get {
			return new Coord(xLevel,  
				this.lightLevelTrapezoid.trapezoid.startRange.start, 
				this.lightLevelTrapezoid.trapezoid.span.start);
		}
	}
	
	public PTwo patchRelativeOriginPTwo {
		get {
			return new PTwo(xLevel, this.lightLevelTrapezoid.trapezoid.span.start );	
		}
	}
	
	private int start{
		get {
			return this.lightLevelTrapezoid.trapezoid.span.start;	
		} 
	}
	
	public int zExtent
	{
		get {
			return this.lightLevelTrapezoid.trapezoid.span.extent();
		}
	}
	
	public bool Equals(Window other) 
	{
		return this.patchRelativeOrigin.Equals(other.patchRelativeOrigin);
	}
	
	public Window(WindowMap _windowMap, SimpleRange startDisRange, int startX, int startZ)
	{
		this.m_windowMap = _windowMap;	
		Trapezoid atrapezoid = new Trapezoid(startDisRange, startZ);
		this.lightLevelTrapezoid = new LightLevelTrapezoid(TrapLight.MediumLightQuad(), atrapezoid);
		xLevel = startX;
		
		// TODO: check windowMap for adjacent surface at...
		checkAdjacentToSurfaceAt(0);
	}
	
	#region update light levels
	
	private void checkAdjacentToSurfaceAt(int spanOffset)
	{
		PTwo checkPoint = patchRelativeOriginPTwo;
		checkPoint.s += spanOffset;
		
		foreach(PTwo surrounding in DirectionUtil.SurroundingPTwoCoordsFromPTwo(checkPoint))
		{
			int surfaceHeight = m_windowMap.surfaceHeightAt(surrounding.s, surrounding.t);	
			this.lightLevelTrapezoid.updateLightLevelsWithAdjacentSurfaceHeightSpanOffset(surfaceHeight, spanOffset);
		}
	}
	
	private void updateWithAdjacentWindow(Window adjacentWindow)
	{
		
	}
	
	#endregion
	
	public bool zLevelIsAdjacentToExtent(int z)
	{
		return z == this.zExtent;
	}
	
	public bool zLevelIsAdjacentToStart(int z)
	{
		return z == this.start - 1;
	}
	
	public bool canIncorporateRangeAtZ(SimpleRange disRange, int z)
	{
		if (zLevelIsAdjacentToStart(z))
		{
			return false;	
		}
		
		return false;
	}
	
	public bool incorporateDiscontinuityAt(SimpleRange disRange, int z)
	{
		return true;	
	}
}
