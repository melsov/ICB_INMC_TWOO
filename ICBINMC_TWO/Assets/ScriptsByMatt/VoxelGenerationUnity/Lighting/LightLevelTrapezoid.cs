using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/*
 * This class should be renamed. Something like: LightZSlice
 *
 */

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
		zHeightRanges[_trap.span.start] = _trap.startRange;
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
		try {
			return zHeightRanges[zIndex];
		} catch(IndexOutOfRangeException e) {
			throw new Exception("the array index was out of bounds: " + zIndex);	
		}
	}
	
	public byte lightLevelAt(int z) {
		return zLightLevels[z];	
	}
	
	public SimpleRange rangeAtExtent {
		get {
			return heightAt(this.trapezoid.span.extentMinusOne());	//caused an ind out of range...
		}
	}
	
	public SimpleRange rangeAtStart{
		get {
			return heightAt(this.trapezoid.span.start);	
		}
	}
	
	public bool rangeExistsAtExtent {
		get {
			return this.rangeAtExtent.range > 0;	
		}
	}
	
	public bool rangeExistsAtStart {
		get {
			return this.rangeAtStart.range > 0;	
		}
	}	
		
	public bool heightRangeExistsAtZ(int patchRelZ) {
		return this.zHeightRanges[patchRelZ].range > 0;
	}
	
	public bool isContiguityAtZ(int patchRelZ, bool wantPosDir) {
		int nudge = (wantPosDir ? 1 : - 1);
		int adjacentIndex = patchRelZ + nudge;
		if (adjacentIndex >= zHeightRanges.Length || adjacentIndex < 0)
			return true;
		
		SimpleRange intersection = SimpleRange.IntersectingRange(heightAt(patchRelZ), heightAt(adjacentIndex));
		
		return intersection.range > 0;
	}
		
	#region add dis ranges
	
//	public void addDiscontinuityRangeBeyondEndAtZExtent(SimpleRange newDisRange, int newZExtent)
	public void incorporateStartFlushWithExtentOther(LightLevelTrapezoid other)
	{
		AssertUtil.Assert(this.trapezoid.span.extent() == other.trapezoid.span.start, 
			"adding ranges from other but not getting other.start == this.extent: " +
			"other start: " + other.trapezoid.span.start + "\nthis extent: " + this.trapezoid.span.extent());
		
		SimpleRange otherSpan = other.trapezoid.span;
		int newZExtent = otherSpan.extent();
		
		copyHeightAndLightFromOther(other);
//		this.zHeightRanges[newZExtent] = newDisRange;
		
		this.trapezoid.endRange = other.trapezoid.endRange; // CONSIDER: whether we need trapezoid end height dims at all at this point // SimpleRange.Average(this.trapezoid.endRange, newDisRange);
		this.trapezoid.span.range = newZExtent - this.trapezoid.span.start;
		
		assertSpanDebug("adding ranges from other");
	}
	
	public void addDiscontinuityRangeAtEnd(SimpleRange newDisRange)
	{
		int zIndex = trapezoid.span.extent(); //not minus one since this is new
		this.zHeightRanges[zIndex] = newDisRange;
		
		this.trapezoid.endRange = SimpleRange.Average(this.trapezoid.endRange, newDisRange);
		this.trapezoid.span.range++;
		
		assertSpanDebug();
	}
	
	public void assertSpanDebug() {
		assertSpanDebug("");
	}
	
	public void assertSpanDebug(string message) {
		AssertUtil.Assert(trapezoid.span.start >= 0, message + "uh oh. span is less than zero: " + trapezoid.span.start);
		AssertUtil.Assert(trapezoid.span.extent() <= NoisePatch.patchDimensions.z, message + "uh oh. span went beyond npatch z: " + trapezoid.span.extent());	
		AssertUtil.Assert(trapezoid.span.start <= NoisePatch.patchDimensions.z, message + "uh oh even weirded. span is greater: " + trapezoid.span.start);
		AssertUtil.Assert(trapezoid.span.extent() > 0, message + "uh oh weirder. extent is less? z: " + trapezoid.span.extent());	
	}
	
	public void addDiscontinuityRangeAtStart(SimpleRange newDisRange)
	{
		int zIndex = trapezoid.span.start - 1;
		this.zHeightRanges[zIndex] = newDisRange;
		
		this.trapezoid.endRange = SimpleRange.Average(this.trapezoid.startRange, newDisRange);
		this.trapezoid.span.range++;
		this.trapezoid.span.start--;
		
		assertSpanDebug();
	}
	
	public SimpleRange considerLightValuesFromOther(LightLevelTrapezoid other, bool subtract, SimpleRange withinRange, float influenceFactor)
	{
		int minInfluenceIndex = 258;
		int maxInfluenceIndex = 0;
		
		if (!SimpleRange.RangesIntersect(trapezoid.span, withinRange))
			return new SimpleRange(0,0);
		
		byte influenceFromOther = (byte)(Window.LIGHT_LEVEL_MAX * influenceFactor - Window.UNIT_FALL_OFF_BYTE);
		
		withinRange = SimpleRange.IntersectingRange(withinRange, new SimpleRange(0, NoisePatch.patchDimensions.z)); // safer...
		if (withinRange.isErsatzNull())
			return new SimpleRange(0,0);
		
		for(int i = withinRange.start; i < withinRange.extent(); ++i) 
		{
			byte myCurrentLightLevel = zLightLevels[i];
			byte othersLightLevel = other.zLightLevels[i];

			if (myCurrentLightLevel == Window.LIGHT_LEVEL_MAX_BYTE)
				continue;
			
			byte influenceAmount = 0;

			if (myCurrentLightLevel <= influenceFromOther) {
				minInfluenceIndex = Mathf.Min(minInfluenceIndex, i);
				maxInfluenceIndex = Mathf.Max(maxInfluenceIndex, i + 1);
				if (!subtract)
					influenceAmount = (byte)(other.zLightLevels[i] - Window.UNIT_FALL_OFF_BYTE);
			} else {
				//cheap0
				influenceAmount = (byte)(myCurrentLightLevel + ( influenceFromOther / 2 * (subtract? -1 : 1) ));
			}
			zLightLevels[i] = influenceAmount;

		}
		
		if (maxInfluenceIndex == 0)
			return new SimpleRange(0,0);
		
		return SimpleRange.SimpleRangeWithStartAndExtent(minInfluenceIndex, maxInfluenceIndex);
	}
	
	private void copyHeightAndLightFromOther(LightLevelTrapezoid other)
	{
		for(int i = other.trapezoid.span.start; i < other.trapezoid.span.extent() ; ++i)
		{
			this.zHeightRanges[i] = other.zHeightRanges[i] ;
			this.zLightLevels[i] = other.zLightLevels[i];
		}
	}
	
	public void copyHeightAndLightValuesFromTo(ref LightLevelTrapezoid other, int zStartCopy, int zCopyExtent)
	{
		for(int i = zStartCopy; i < zCopyExtent ; ++i)
		{
			other.zHeightRanges[i] = this.zHeightRanges[i];
			other.zLightLevels[i] = this.zLightLevels[i];
		}
	}
	
	public void nullifyHeightAndLightValuesFromTo(int zStartCopy, int zCopyExtent)
	{
		for(int i = zStartCopy; i < zCopyExtent ; ++i)
		{
			this.zHeightRanges[i] = new SimpleRange(0,0);
			this.zLightLevels[i] = 0;
		}
	}
	
	public void shortenSpanByOneFromExtent() // removeDiscontinuityRangeAtExtent()
	{
//		resetAt(this.trapezoid.span.extentMinusOne());
		this.trapezoid.span.range--;	
		
		assertSpanDebug();
	}
	
	public void shortenSpanByOneFromStart()
	{
//		resetAt(this.trapezoid.span.start);
		
		this.trapezoid.span.start++;
		this.trapezoid.span.range--;
		
		assertSpanDebug();
	}
	
	public void updateDiscontinuityAtPatchRelativeZ(SimpleRange disRange, int patchRelZ)
	{
		this.zHeightRanges[patchRelZ] = disRange;	
	}
	
	public void resetAt(int patchRelZ)
	{
		//is this a useful way to clear?
		this.zHeightRanges[patchRelZ] = new SimpleRange(0,0);
		this.zLightLevels[patchRelZ] = 0;
	}
	
	public void resetAllLightsAndHeights()
	{
		for(int i = 0; i < zHeightRanges.Length; ++i)
		{
			this.zHeightRanges[i] = new SimpleRange(0,0);
			this.zLightLevels[i] = 0;
		}
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
		assertSpanDebug();
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
	
	public void addMingleLightWithAdjacentTrapezoid(LightLevelTrapezoid other)
	{
		for(int i = this.trapezoid.span.start; i < this.trapezoid.span.extent(); ++i)
		{
			SimpleRange otherHeightRange = other.heightAt(i);
			SimpleRange heightRange = this.heightAt(i);
			
			SimpleRange intersection = SimpleRange.IntersectingRange(otherHeightRange, heightRange);
			if (intersection.range > 0)
			{
				if(other.zLightLevels[i] > this.zLightLevels[i]) 
					this.zLightLevels[i] = (byte)Mathf.Max((int)this.zLightLevels[i], (other.zLightLevels[i] - Window.UNIT_FALL_OFF_BYTE));
				else if (other.zLightLevels[i] < this.zLightLevels[i]) 
					other.zLightLevels[i] = (byte)Mathf.Max((int) other.zLightLevels[i], (this.zLightLevels[i] - Window.UNIT_FALL_OFF_BYTE));
			}
		}
	}
	
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
//		return Window.LIGHT_LEVEL_MAX * .5f; //TEST 
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

