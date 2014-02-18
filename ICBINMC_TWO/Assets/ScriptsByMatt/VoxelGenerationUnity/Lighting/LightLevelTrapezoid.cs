using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightLevelTrapezoid
{
	public Trapezoid trapezoid;
	public TrapLight trapLight;
	
	private byte[] zLightLevels = new byte[NoisePatch.patchDimensions.z];
	private SimpleRange[] zHeightRanges = new SimpleRange[NoisePatch.patchDimensions.z];
	
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
	
	public SimpleRange heightAt(int zIndex) {
		return zHeightRanges[zIndex];	
	}
	
		
	#region add dis ranges
	
	public void addDiscontinuityRangeBeyondEndAtZExtent(SimpleRange newDisRange, int newZExtent)
	{
		this.zHeightRanges[newZExtent] = newDisRange;
		
		this.trapezoid.endRange = SimpleRange.Average(this.trapezoid.endRange, newDisRange);
		this.trapezoid.span.range = newZExtent - this.trapezoid.span.start;
	}
	
	public void addDiscontinuityRangeAtEnd(SimpleRange newDisRange)
	{
		int zIndex = trapezoid.span.extent(); //not minus one since this is new
		this.zHeightRanges[zIndex] = newDisRange;
		
		this.trapezoid.endRange = SimpleRange.Average(this.trapezoid.endRange, newDisRange);
		this.trapezoid.span.range++;
	}
	
	public void addDiscontinuityRangeAtStart(SimpleRange newDisRange)
	{
		int zIndex = trapezoid.span.start - 1;
		this.zHeightRanges[zIndex] = newDisRange;
		
		this.trapezoid.endRange = SimpleRange.Average(this.trapezoid.startRange, newDisRange);
		this.trapezoid.span.range++;
		this.trapezoid.span.start--;
	}
	
	public void updateDiscontinuityAtPatchRelativeZ(SimpleRange disRange, int patchRelZ)
	{
		this.zHeightRanges[patchRelZ] = disRange;	
	}
	
	#endregion
	
	#region update with surface adjacency
	
	private void updateAddedZLightLevels(byte lightValue, int index, bool _forward)
	{	
		if (index < 0 || index >= zLightLevels.Length)
			return;
		
		zLightLevels[index] = (byte)(lightValue + zLightLevels[index]); //, 0, Window.LIGHT_LEVEL_MAX_BYTE );
		zLightLevels[index] = zLightLevels[index] > Window.LIGHT_LEVEL_MAX_BYTE ? Window.LIGHT_LEVEL_MAX_BYTE : zLightLevels[index]; 
		
		lightValue -= Window.UNIT_FALL_OFF_BYTE;
		
		if (lightValue <= 0)
			return;
		
		if (_forward)
			updateAddedZLightLevels(lightValue, ++index, _forward);
		else
			updateAddedZLightLevels(lightValue, --index, _forward);
		
	}
	
	private void updateSubtractedZLightLevels(byte subtractLightAmount, int index, bool _forward, bool firstTime)
	{
		if (index < 0 || index >= zLightLevels.Length)
			return;
		
		if(!firstTime && zLightLevels[index] >= Window.LIGHT_LEVEL_MAX_BYTE)
			return;
		
		zLightLevels[index] = (byte)(zLightLevels[index] - subtractLightAmount); //, 0, Window.LIGHT_LEVEL_MAX_BYTE );
		zLightLevels[index] = zLightLevels[index] < (byte) 0 ? (byte) 0 : zLightLevels[index]; 
		
		subtractLightAmount -= Window.UNIT_FALL_OFF_BYTE;
		
		if (subtractLightAmount <= 0)
			return;	
		
		if (_forward)
			updateSubtractedZLightLevels(subtractLightAmount, ++index, _forward, false);
		else
			updateSubtractedZLightLevels(subtractLightAmount, --index, _forward, false);
	}
	
	private void setAllZero() {
		for(int i = 0 ; i < zLightLevels.Length; ++i)
		{
			zLightLevels[i] = 0;	
		}
	}
	
	private void updateAddedZLightLevels(byte lightValue, int index)
	{
		updateAddedZLightLevels(lightValue, index, true);
		updateAddedZLightLevels(lightValue, index, false);
	}
	
	private void updateSubtractedZLightLevels(byte lightValue, int index)
	{
		updateSubtractedZLightLevels(lightValue, index, true, true);
		updateSubtractedZLightLevels(lightValue, index, false, true);
	}
	
	private static bool SurroundingValuesGreaterThanHeight(SurroundingSurfaceValues ssvs, int height)
	{
		foreach(Direction dir in DirectionUtil.TheDirectionsXZ())
		{
			int surfaceHeight = ssvs.valueForDirection(dir);
			if (surfaceHeight < height)
				return false;
		}
		return true;
	}
	
	//TODO: rationalize and separate out editing heights from adding heights
	// also, make adding heights more efficient?
	// or is it better to always either edit or add.
	// because we really don't know when were going to do which...
	// a good rule? if a window accepts an edit discontinuity..
	// it must erase any height below or above?
	
	private void resetLightLevels()
	{
		for(int i = trapezoid.span.start; i < trapezoid.span.extent() ; ++i)
		{
			if (zLightLevels[i] == Window.LIGHT_LEVEL_MAX_BYTE)
			{
				while(zLightLevels[i] == Window.LIGHT_LEVEL_MAX_BYTE)
				{
					++i;
					if(i >= trapezoid.span.extent())
						break;
				}
				updateAddedZLightLevels(Window.LIGHT_LEVEL_MAX_BYTE, --i);	
			}
		}
	}
	
//	public void removeLightLevelsBySubtractingAdjacentSurface(SurroundingSurfaceValues surroundingSurfaceValues, int patchRelz)
	public void removeLightLevelsBySubtractingAdjacentSurface(int patchRelz)
	{
		// TEST:
		setAllZero();
		return;
		
//		int heightAtZ = zHeightRanges[patchRelz].extent();
//		if (!LightLevelTrapezoid.SurroundingValuesGreaterThanHeight(surroundingSurfaceValues, heightAtZ))
//			return;
		
//		if (zLightLevels[patchRelz] < Window.LIGHT_LEVEL_MAX_BYTE)
//			return; //WANT?
		
		updateSubtractedZLightLevels(Window.LIGHT_LEVEL_MAX_BYTE, patchRelz);
		
//		resetLightLevels();
	}
	
	public void addLightLevelsWithAdjacentSurfaceHeightSpanOffset(int surfaceHeight, int spanOffset) 
	{
		int zIndex = trapezoid.span.start + spanOffset;
//		int zCoverage = this.zHeightRanges[zIndex].extent() - surfaceHeight;
//		zCoverage = zCoverage <= 0 ? 1 : zCoverage;
		
		zIndex = Mathf.Clamp(zIndex, 0, zLightLevels.Length - 1);
		
//		zLightLevels[zIndex] = Window.LIGHT_LEVEL_MAX_BYTE;
		updateAddedZLightLevels(Window.LIGHT_LEVEL_MAX_BYTE, zIndex);
		
		return;
		
		
		// func . only for adding values....
		if (trapLight.allValuesMaxed())
			return; // CONSIDER: WAIT WHAT IF THERE WAS ADJACENCY BEFORE?
		
		// NOTE: very crude!
		int coverage = this.trapezoid.startRange.extent() - surfaceHeight ;
		int coverageEnd = this.trapezoid.endRange.extent() - surfaceHeight ;
		
		float offsetLightValue;
		if (coverage <= 0)
			coverage = 1;

		
			offsetLightValue = lightAddedWithSurfaceSpanOffset(spanOffset);
			trapLight.upperNeg = incrementedLightValue(trapLight.upperNeg, offsetLightValue);
			trapLight.lowerNeg = incrementedLightValue(trapLight.lowerNeg, 
				offsetLightValue * Mathf.Clamp((coverage/(float)trapezoid.startRange.range), 0f, 1f));
		
		coverage = coverageEnd;

		if (coverage <= 0)
			coverage = 1;

		
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
		return Mathf.Clamp( (float) zLightLevels[patchRelYZPoint.t], 0, Window.LIGHT_LEVEL_MAX);
		
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

