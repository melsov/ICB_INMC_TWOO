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
	
	public PTwo[] clockwisePoints() {
		return new PTwo[] {this.lowerNegCorner(), this.upperNegCorner(), this.upperPosCorner(), this.lowerPosCorner() };	
	}
	
	public bool contains(PTwo point) {
		if (!span.contains(point.s))
			return false;
		
		if (startRange.contains(point.t))
		{
			if (endRange.contains(point.t))
				return true;
			else {
				// TODO: fill in better math
				int distToStart = point.s - span.start;
				return (distToStart < (span.range - distToStart));
			}
		} else {
			if (endRange.contains(point.t))
			{
				int distToStart = point.s - span.start;
				return (distToStart > (span.range - distToStart));	
			}
		}
		
		return false;
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
	
	public float[] clockWiseValues()
	{
		return new float[] {lowerNeg, upperNeg, upperPos, lowerPos};
	}
	
	public void setValueWithClockwiseIndex(float _value, int index) {
		switch(index) {
		case 0:
			lowerNeg = _value;	
			break;
		case 1:
			upperNeg = _value;
			break;
		case 2:
			upperPos = _value;
			break;
		default:
			lowerPos = _value;
			break;
		}
	}
	
	public bool allValuesMaxed()
	{
		return this.upperNeg >= Window.LIGHT_LEVEL_MAX && 
			this.lowerNeg >= Window.LIGHT_LEVEL_MAX && 
			this.upperPos >= Window.LIGHT_LEVEL_MAX && 
			this.lowerPos >= Window.LIGHT_LEVEL_MAX;	
	}
	
	public void setAllValuesToMax()
	{
		this.upperNeg = Window.LIGHT_LEVEL_MAX;
		this.lowerNeg = Window.LIGHT_LEVEL_MAX;  
		this.upperPos = Window.LIGHT_LEVEL_MAX;  
		this.lowerPos = Window.LIGHT_LEVEL_MAX;
	}
	
	public void setAllValuesTo(float _val)
	{
		this.upperNeg = _val;
		this.lowerNeg = _val;
		this.upperPos = _val;
		this.lowerPos = _val;
	}
	
	public float average() {
		return (upperNeg + lowerNeg + upperPos + lowerPos)/4f;	
	}
	
	public float averageNeg() {
		return (upperNeg + lowerNeg)/2f;
	}
	
	public float averagePos() {
		return (upperPos + lowerPos)/2f;	
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
	
	private byte[] zLightLevels = new byte[NoisePatch.patchDimensions.z];
	
	private List<Coord> surfaceAdjacenyStarts = new List<Coord>();
	
	public LightLevelTrapezoid(TrapLight _trapLight, Trapezoid _trap)
	{
		trapLight = _trapLight;
		trapezoid =_trap;
	}
	
	public float averageLight {
		get {
			return trapLight.average();
		}
	}
	
	public float averageLightNeg {
		get {
			return trapLight.averageNeg();
		}
	}
	
	public float averageLightPos {
		get {
			return trapLight.averagePos();
		}
	}
	
	public LightPoint midPoint {
		get {
			return new LightPoint(trapezoid.midPoint(), trapLight.average());	
		}
	}
	
	public LightPoint midPointNeg {
		get {
			return new LightPoint(trapezoid.midPoint(), trapLight.averageNeg());	
		}
	}
	
	public LightPoint midPointPos {
		get {
			return new LightPoint(trapezoid.midPoint(), trapLight.averagePos());	
		}
	}
	
		
	#region add dis ranges
	
	public void addDiscontinuityRangeBeyondEndAtZExtent(SimpleRange newDisRange, int newZExtent)
	{
		this.trapezoid.endRange = SimpleRange.Average(this.trapezoid.endRange, newDisRange);
		this.trapezoid.span.range = newZExtent - this.trapezoid.span.start;
	}
	
	public void addDiscontinuityRangeAtEnd(SimpleRange newDisRange)
	{
		this.trapezoid.endRange = SimpleRange.Average(this.trapezoid.endRange, newDisRange);
		this.trapezoid.span.range++;
	}
	
	public void addDiscontinuityRangeAtStart(SimpleRange newDisRange)
	{
		this.trapezoid.endRange = SimpleRange.Average(this.trapezoid.startRange, newDisRange);
		this.trapezoid.span.range++;
		this.trapezoid.span.start--;
	}
	
	#endregion
	
	#region update with surface adjacency
	
	public void addLightLevelsWithAdjacentSurfaceHeightSpanOffset(int surfaceHeight, int spanOffset) // Coord surfaceStartPatchRelCo)
	{
//		int surfaceHeight = surfaceStartPatchRelCo.y; 
//		int spanOffset = surfaceStartPatchRelCo.z - trapezoid.span.start;
		
		// func . only for adding values....
		if (trapLight.allValuesMaxed())
			return; // CONSIDER: WAIT WHAT IF THERE WAS ADJACENCY BEFORE?
		
		// NOTE: very crude!
		int coverage = this.trapezoid.startRange.extent() - surfaceHeight ;
		int coverageEnd = this.trapezoid.endRange.extent() - surfaceHeight ;
		
		float offsetLightValue;
		if (coverage <= 0)
		{
			coverage = 1;
		}
		
			offsetLightValue = lightAddedWithSurfaceSpanOffset(spanOffset);
			trapLight.upperNeg = incrementedLightValue(trapLight.upperNeg, offsetLightValue);
			trapLight.lowerNeg = incrementedLightValue(trapLight.lowerNeg, 
				offsetLightValue * Mathf.Clamp((coverage/(float)trapezoid.startRange.range), 0f, 1f));
		
		coverage = coverageEnd;

		if (coverage <= 0)
		{
			coverage = 1;
		}
		
			offsetLightValue = lightAddedWithSurfaceSpanOffset(this.trapezoid.span.extent() - spanOffset);
			trapLight.upperPos = incrementedLightValue(trapLight.upperPos, offsetLightValue);
			trapLight.lowerPos = incrementedLightValue(trapLight.lowerPos, 
				offsetLightValue * Mathf.Clamp((coverage/(float)trapezoid.startRange.range), 0f, 1f));
	}
	
	private float incrementedLightValue(float oldValue, float addToOldValue)
	{
		return Mathf.Clamp(oldValue + addToOldValue, 0f, Window.LIGHT_LEVEL_MAX - 3f);
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
		if(distance <= 0)
			return 1f;
		float travel =  1f - (float)distance/Window.LIGHT_TRAVEL_MAX_DISTANCE;
		travel = Mathf.Clamp(travel, 0f, 1f);
		return sourceLightValue * travel;
	}
	
	#region update with light point
	
	public void updateWithXAdjacentPoint(LightPoint lightPoint)
	{
		lightPoint.lightValue -= Window.UNIT_FALL_OFF;
		lightPoint.lightValue = lightPoint.lightValue < 0f ? 0f : lightPoint.lightValue;
		
		float[] currentValues = trapLight.clockWiseValues();
		PTwo[] currentPoints = trapezoid.clockwisePoints();
		for (int i = 0; i < 4 ; ++i)
		{
			float addedLight = lightValueAtFromPoint(currentPoints[i], lightPoint);
			float newValue = incrementedLightValue(currentValues[i], addedLight);
			trapLight.setValueWithClockwiseIndex(newValue, i);
//			trapLight.setValueWithClockwiseIndex(Window.LIGHT_LEVEL_MAX, i); // TEST
		}
	}
	
	private float lightValueAtFromPoint(PTwo destinationPoint, LightPoint fromPoint)
	{
		PTwo diff = PTwo.Abs(destinationPoint - fromPoint.point);
		int dist = diff.s + diff.t/2; // very cheapo pythagoras
		return lightAddedWithDistance(dist, fromPoint.lightValue);
	}
	
	#endregion
	
	#region query
	
	public float lightValueForPointClosestToPatchRelativeYZ(PTwo patchRelYZPoint)
	{
		int spanOffset = Mathf.Clamp(patchRelYZPoint.t - trapezoid.span.start, 0, trapezoid.span.range);
		int heightOffSetStart = Mathf.Clamp( patchRelYZPoint.s - trapezoid.startRange.start, 0, trapezoid.startRange.range);
		int heightOffSetEnd = Mathf.Clamp( patchRelYZPoint.s - trapezoid.endRange.start, 0, trapezoid.endRange.range);
		
		float startLerp = Mathf.Lerp(trapLight.lowerNeg, trapLight.upperNeg, (heightOffSetStart/(float)trapezoid.startRange.range));
		float endLerp = Mathf.Lerp(trapLight.lowerPos, trapLight.upperPos, (heightOffSetEnd/(float)trapezoid.endRange.range));
		
		
//		return Window.LIGHT_LEVEL_MAX/12f;
		return Mathf.Lerp(startLerp, endLerp, spanOffset/(float) trapezoid.span.range);
		
	}
	
	#endregion
	
}



public class Window : IEquatable<Window>
{
	List<Window> influencedWindows = new List<Window>();
	
	WindowMap m_windowMap;
	
	private int xLevel;
	public int xCoord {
		get {
			return xLevel;
		}
	}
	
	private LightLevelTrapezoid lightLevelTrapezoid;
	
	private List<SimpleRange> heights = new List<SimpleRange>();
	
	public const float LIGHT_LEVEL_MAX = 24f;
	public const float LIGHT_TRAVEL_MAX_DISTANCE = 16;
	public const float UNIT_FALL_OFF = 6f;
	private const int WINDOW_HEIGHTS_MINIMUN_OVERLAP = 1;
	
	public Coord patchRelativeOrigin {
		get {
			return new Coord(xLevel,  
				this.lightLevelTrapezoid.trapezoid.startRange.start, 
				this.lightLevelTrapezoid.trapezoid.span.start);
		}
	}
	
	public float lightValueForYZPoint(PTwo point) 
	{
		return lightLevelTrapezoid.lightValueForPointClosestToPatchRelativeYZ(point);	
	}
	
	public Coord worldRelativeOrigin {
		get {
			return this.patchRelativeOrigin + this.m_windowMap.worldCoord;	
		}
	}
	
	public bool allValuesMaxed {
		get {
			return this.lightLevelTrapezoid.trapLight.allValuesMaxed();	
		}
	}
	
	public PTwo patchRelativeOriginPTwo {
		get {
			return new PTwo(xLevel, this.lightLevelTrapezoid.trapezoid.span.start );	
		}
	}
	
	public int spanStart{
		get {
			return this.lightLevelTrapezoid.trapezoid.span.start;	
		} 
	}
	
	public int spanRange{
		get {
			return this.lightLevelTrapezoid.trapezoid.span.range;
		}
	}
	
	public int zExtent
	{
		get {
			return this.lightLevelTrapezoid.trapezoid.span.extent();
		}
	}
	
	public SimpleRange startRange
	{
		get {
			return this.lightLevelTrapezoid.trapezoid.startRange;
		}
	}
	
	public SimpleRange endRange
	{
		get {
			return this.lightLevelTrapezoid.trapezoid.endRange;
		}
	}
	
	public bool Equals(Window other) 
	{
		return this.patchRelativeOrigin.Equals(other.patchRelativeOrigin);
	}
	
	public LightPoint midPointLight
	{
		get {
			return this.lightLevelTrapezoid.midPoint;
		}
	}
	
	public float[] clockwiseLightValues
	{
		get {
			return this.lightLevelTrapezoid.trapLight.clockWiseValues();		
		}
	}
	
	public void setIndexLightValue(float val, int index)
	{
		this.lightLevelTrapezoid.trapLight.setValueWithClockwiseIndex(val, index);
	}
	
	public TrapLight trapLight {
		get {
			return this.lightLevelTrapezoid.trapLight;		
		}
	}
	
	public PTwo[] clockwisePoints
	{
		get {
			return this.lightLevelTrapezoid.trapezoid.clockwisePoints();	
		}
	}
	
	public Window(WindowMap _windowMap, SimpleRange startDisRange, int startX, int startZ)
	{
		this.m_windowMap = _windowMap;	
		Trapezoid atrapezoid = new Trapezoid(startDisRange, startZ);
		this.lightLevelTrapezoid = new LightLevelTrapezoid(TrapLight.MediumLightQuad(), atrapezoid);
		
		heights.Add(startDisRange);
		
		xLevel = startX;
		
		// TODO: make a new class that inventories the locations of discontinuities
		// probably 2 lists of height/span offset values
		// plus 2 height values on either end.
		
		checkAdjacentToSurfaceFourDirectionsAt(0);
	}
	
	#region adjacency with surface
	
	private void checkAdjacentToSurfaceFourDirectionsAt(int spanOffset)
	{
		PTwo checkPoint = patchRelativeOriginPTwo;
		checkPoint.s += spanOffset;
		
		foreach(PTwo surrounding in DirectionUtil.SurroundingPTwoCoordsFromPTwo(checkPoint))
		{
			int surfaceHeight = m_windowMap.surfaceHeightAt(surrounding.s, surrounding.t);	

			if (windowHeightAtOffsetGreaterThanHeight(spanOffset, surfaceHeight))
				this.addLightLevelsWithAdjacentSurfaceHeightSpanOffset(surfaceHeight, spanOffset);
			// else...
		}
	}
	
	public void addLightWithSurroundingSurfaceValues(SurroundingSurfaceValues surroundingSurfaceValues, int patchRelativeZ)
	{
		int spanOffset = patchRelativeZ - patchRelativeOrigin.z;
		foreach(Direction dir in DirectionUtil.TheDirectionsXZ())
		{
			int surfaceHeight = surroundingSurfaceValues.valueForDirection(dir);
			
			if (windowHeightAtOffsetGreaterThanHeight(spanOffset, surfaceHeight))
				this.addLightLevelsWithAdjacentSurfaceHeightSpanOffset(surfaceHeight, spanOffset);
		}
	}
	
	public void addLightLevelsWithAdjacentSurfaceHeightSpanOffset(int surfaceHeight, int spanOffset)
	{
		this.lightLevelTrapezoid.addLightLevelsWithAdjacentSurfaceHeightSpanOffset(surfaceHeight, spanOffset);	
	}
	
	private bool windowHeightAtOffsetGreaterThanHeight(int spanOffset, int height) {
		return windowHeightAtOffset(spanOffset) > height;	
	}
	
	private int windowHeightAtOffset(int spanOffSet) {
		if (spanOffSet < -1 || spanOffSet >= heights.Count)
			return -1;
			
		return heights[spanOffSet].extent();
	}
	
	#endregion
	
	#region update light levels
	
	public void testSetAllValuesTo(float _value)
	{
		this.lightLevelTrapezoid.trapLight.setAllValuesTo(_value);	
	}
	
	public void setMaxLight()
	{
		this.lightLevelTrapezoid.trapLight.setAllValuesToMax();		
	}
	
	// part of non recursive approach
	public int updateWithAdjacentWindowsReturnAddedLightRating(List<Window> windows)
	{
		float averageLightBefore = this.lightLevelTrapezoid.averageLight;
		
		foreach(Window adjacentWindow in windows)
		{
			updateWithAdjacentWindow(adjacentWindow);	
		}
		
		float averageLightAfter = this.lightLevelTrapezoid.averageLight;
		
		return (int)(averageLightAfter - averageLightBefore);
	}
	
	public void updateWithWindowsFlushWithAnEdge(List<Window> otherWindows, bool flushWithExtent)
	{
		//cheapo
		//indices setup outside-inside (to be averaged :)
		int[] otherIndices = flushWithExtent ? new int[] {2,3} : new int[] {0,1};
		int[] indices = flushWithExtent ? new int[] {0,1} : new int[] {2,3};
		
		float[] otherClockwiseLightVals;
		float[] clockwiseValues = this.clockwiseLightValues;
		
		foreach(Window other in otherWindows)
		{
			otherClockwiseLightVals = other.clockwiseLightValues;
			
			int count = 0;
			foreach(int index in otherIndices)
			{
				this.setIndexLightValue((otherClockwiseLightVals[index] + clockwiseLightValues[count])/2f, index);
				count++;
			}
		}
	}
	
	//TODO: nice if we could return the name of the point or 'no point'
	private bool overlapExistsWithWindow(Window window)
	{
		foreach(PTwo cornerPoint in window.clockwisePoints)
		{
			if (this.lightLevelTrapezoid.trapezoid.contains(cornerPoint))
			{
				return true;
			}
		}
		
		foreach(PTwo cornerPoint in this.clockwisePoints)
		{
			if (window.lightLevelTrapezoid.trapezoid.contains(cornerPoint))
			{
				return true;
			}
		}
		
		return false;
	}
	
	private void updateWithAdjacentWindow(Window adjacentWindow)
	{
		if (overlapExistsWithWindow(adjacentWindow)) { //TEST
//			lightLevelTrapezoid.updateWithXAdjacentPoint(adjacentWindow.midPointLight);
			lightLevelTrapezoid.updateWithXAdjacentPoint(adjacentWindow.lightLevelTrapezoid.midPointNeg);
			lightLevelTrapezoid.updateWithXAdjacentPoint(adjacentWindow.lightLevelTrapezoid.midPointPos);
		}
	}
	
	#endregion
	
	public bool zLevelIsAdjacentToExtent(int z)
	{
		return z == this.zExtent;
	}
	
	public bool zLevelIsAdjacentToStart(int z)
	{
		return z == this.spanStart - 1;
	}
	
	public bool spanContainsZ(int z)
	{
		return this.lightLevelTrapezoid.trapezoid.span.contains(z);	
	}
	
	public int shortestDifferenceWith(int y, int z)
	{
		// slightly cheapo
		SimpleRange compareRange = this.startRange;
		if (z > this.spanStart + this.spanRange/2)
			compareRange = this.endRange;
		
		if (compareRange.contains(y))
			return 0;
		
		if (y > compareRange.start)
			return compareRange.extent() - y;
		
		return compareRange.start - y;
	}

	
	#region adding to windows
	
//	public bool canIncorporateRangeAtZ(SimpleRange disRange, int z)
//	{
//		if (zLevelIsAdjacentToStart(z))
//		{
//			return false;	
//		}
//		
//		return false;
//	}
	
	public bool incorporateDiscontinuityAt(SimpleRange disRange, int z)
	{
		if (this.zLevelIsAdjacentToExtent(z))
		{
			return addDiscontinuityAtExtent(disRange);
		}
		if (this.zLevelIsAdjacentToStart(z))
		{
			return addDiscontinuityAtStart(disRange);	
		}
		return false;
	}
	
	private bool addDiscontinuityAtExtent(SimpleRange disRange)
	{
		return addDiscontinuityAt(disRange, true);
	}
	
	private bool addDiscontinuityAtStart(SimpleRange disRange)
	{
		return addDiscontinuityAt(disRange, false);
	}
	
	private bool addDiscontinuityAt(SimpleRange disRange, bool wantExtent)
	{
		SimpleRange the_range = wantExtent ? this.lightLevelTrapezoid.trapezoid.endRange : this.lightLevelTrapezoid.trapezoid.startRange;
		
//		SimpleRange intersection = SimpleRange.IntersectingRange(the_range, disRange);
//		if (intersection.range >= Window.WINDOW_HEIGHTS_MINIMUN_OVERLAP)
		if (terminalRangeAndDiscontinuityRangeMeetMergeRequirements(the_range, disRange))
		{
			if (wantExtent)
				this.lightLevelTrapezoid.addDiscontinuityRangeAtEnd(disRange);
			else 
				this.lightLevelTrapezoid.addDiscontinuityRangeAtStart(disRange);
			return true;
		}
		
		return false;
	}
	
	public bool incorporateWindowFlushWithExtent(Window nextWindow)
	{
		if (terminalRangeAndDiscontinuityRangeMeetMergeRequirements(this.endRange, nextWindow.startRange))
		{
			this.lightLevelTrapezoid.addDiscontinuityRangeBeyondEndAtZExtent(nextWindow.endRange, nextWindow.zExtent);
			return true;
		}
		return false;
	}
	
	private bool terminalRangeAndDiscontinuityRangeMeetMergeRequirements(SimpleRange terminalRange, SimpleRange disRange)
	{
		return (SimpleRange.IntersectingRange(terminalRange, disRange).range >= Window.WINDOW_HEIGHTS_MINIMUN_OVERLAP);
	}
	
	#endregion
}
