using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;



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
	public const byte LIGHT_LEVEL_MAX_BYTE = (byte) LIGHT_LEVEL_MAX;
	
	public const float LIGHT_TRAVEL_MAX_DISTANCE = 16;
	public const float UNIT_FALL_OFF = 2f;
	public const byte UNIT_FALL_OFF_BYTE = (byte) UNIT_FALL_OFF;
	
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
	
	public void editLightWithSurroundingSurfaceValues(SurroundingSurfaceValues surroundingSurfaceValues, int patchRelativeZ)
	{
//		int patchRelativeZ = patchRelativeYZ.t;
		int spanOffset = patchRelativeZ - patchRelativeOrigin.z;
		bool canSeeSurface = false;
		foreach(Direction dir in DirectionUtil.TheDirectionsXZ())
		{
			int surfaceHeight = surroundingSurfaceValues.valueForDirection(dir);
			
			if (windowHeightAtOffsetGreaterThanHeight(spanOffset, surfaceHeight))
			{
				canSeeSurface = true;
				this.addLightLevelsWithAdjacentSurfaceHeightSpanOffset(surfaceHeight, spanOffset);
			}
		}
		
		if (!canSeeSurface) {
			this.lightLevelTrapezoid.removeLightLevelsBySubtractingAdjacentSurface( patchRelativeZ);	
		}
	}
	
	public void editLightWithTerminatingSurfaceHeightAtPatchRelativeZ(int surfaceHeight, int patchRelZ, bool terminalIsExtent)
	{
		if(terminalIsExtent)
			patchRelZ--;
		else
			patchRelZ++;
		
		AssertUtil.Assert(patchRelZ >= 0 && patchRelZ < NoisePatch.patchDimensions.z, "confusing and not want we wanted. z beyond bounds");
		
		editLightWithAdjacentNonTerminatingSurfaceHeightAtPatchRelativeZ(surfaceHeight, patchRelZ);
	}
	
	public void editLightWithAdjacentNonTerminatingSurfaceHeightAtPatchRelativeZ(int surfaceHeight, int patchRelZ)
	{
		if (!this.spanContainsZ(patchRelZ))
			return;
		
		int heightAt = this.lightLevelTrapezoid.heightAt(patchRelZ).extent();
		if (surfaceHeight < heightAt)
		{
			this.addLightLevelsWithAdjacentSurfaceHeightSpanOffset(surfaceHeight, patchRelZ - this.spanStart);
		} else {
			this.lightLevelTrapezoid.removeLightLevelsBySubtractingAdjacentSurface(	patchRelZ);
		}
	}
	
	// should this be public???
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
		return incorporateDiscontinuityAt(disRange, z, true); // always?
	}
	
//	public bool incorporateDiscontinuityAtAllowEditing(SimpleRange disRange, int z)
//	{
//		return incorporateDiscontinuityAt(disRange, z, true);
//	}
	
	private bool incorporateDiscontinuityAt(SimpleRange disRange, int z, bool wantEditing)
	{
		if (this.zLevelIsAdjacentToExtent(z))
		{
			return addDiscontinuityAtExtent(disRange);
		}
		if (this.zLevelIsAdjacentToStart(z))
		{
			return addDiscontinuityAtStart(disRange);	
		}
		if (wantEditing && this.spanContainsZ(z))
		{
			return editDiscontinuityAt(disRange,z);		
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
	
	private bool editDiscontinuityAt(SimpleRange disRange, int z)
	{
		if (rangeAndDiscontinuityRangeMeetMergeRequirements(lightLevelTrapezoid.heightAt(z), disRange))
		{
			this.lightLevelTrapezoid.updateDiscontinuityAtPatchRelativeZ(disRange, z);
			return true;
		}
		return false;
	}
	
	private bool addDiscontinuityAt(SimpleRange disRange, bool wantExtent)
	{
		SimpleRange the_range = wantExtent ? this.lightLevelTrapezoid.trapezoid.endRange : this.lightLevelTrapezoid.trapezoid.startRange;
		
//		SimpleRange intersection = SimpleRange.IntersectingRange(the_range, disRange);
//		if (intersection.range >= Window.WINDOW_HEIGHTS_MINIMUN_OVERLAP)
		if (rangeAndDiscontinuityRangeMeetMergeRequirements(the_range, disRange))
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
		if (rangeAndDiscontinuityRangeMeetMergeRequirements(this.endRange, nextWindow.startRange))
		{
			this.lightLevelTrapezoid.addDiscontinuityRangeBeyondEndAtZExtent(nextWindow.endRange, nextWindow.zExtent);
			return true;
		}
		return false;
	}
	
	private bool rangeAndDiscontinuityRangeMeetMergeRequirements(SimpleRange terminalRange, SimpleRange disRange)
	{
		return (SimpleRange.IntersectingRange(terminalRange, disRange).range >= Window.WINDOW_HEIGHTS_MINIMUN_OVERLAP);
	}
	
	#endregion
}
