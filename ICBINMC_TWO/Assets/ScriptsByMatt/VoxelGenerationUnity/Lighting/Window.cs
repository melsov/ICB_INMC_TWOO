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
	
//	private List<SimpleRange> heights = new List<SimpleRange>();
	
	public const float LIGHT_LEVEL_MAX = 24f;
	public const byte LIGHT_LEVEL_MAX_BYTE = (byte) LIGHT_LEVEL_MAX;
	
	public const float LIGHT_TRAVEL_MAX_DISTANCE = 16;
	public const float UNIT_FALL_OFF = 1f;
	public const byte UNIT_FALL_OFF_BYTE = (byte) UNIT_FALL_OFF;
	
	public const int INFLUENCE_RADIUS = LIGHT_LEVEL_MAX_BYTE/UNIT_FALL_OFF_BYTE;
	
	private const int WINDOW_HEIGHTS_MINIMUN_OVERLAP = 1;
	
	public Coord patchRelativeOrigin {
		get {
			return new Coord(xLevel,  
				this.lightLevelTrapezoid.trapezoid.startRange.start, 
				this.lightLevelTrapezoid.trapezoid.span.start);
		}
	}
	
	#region get influended lists
	
	private List<Window> xNegInfluenced {
		get {
			return m_windowMap.windowsFlushWithWindowZFace(this, false);
		}
	}
	
	private List<Window> xPosInfluenced {
		get {
			return m_windowMap.windowsFlushWithWindowZFace(this, true);
		}
	}
	
	private List<Window> underAboveOrEdgeInfluencedPos {
		get {
			return null;	
		}
	}
	
	private List<Window> underAboveOrEdgeInfluencedNeg {
		get {
			return null;	
		}
	}
	
	private List<Window> neighborsForDirection(Direction dir) {
		if (dir == Direction.xneg) {
			return this.xNegInfluenced;	
		} else if (dir == Direction.xpos) {
			return this.xPosInfluenced;
		} else if (dir == Direction.zneg) {
			return this.underAboveOrEdgeInfluencedNeg;
		} else if (dir == Direction.zpos) {
			return this.underAboveOrEdgeInfluencedPos;
		}
		return null;
	}
//	
//	private List<Window> zNegEdgeInfluenced {
//		get {
//			return null;
//		}
//	}
//	
//	private List<Window> zPosEdgeInfluenced {
//		get {
//			return null;
//		}
//	}
	
	#endregion
	
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
	
	public SimpleRange span {
		get {
			return this.lightLevelTrapezoid.trapezoid.span;
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
	
	public SimpleRange startHeightRange
	{
		get {
//			return this.lightLevelTrapezoid.trapezoid.startRange;
			return this.lightLevelTrapezoid.heightAt(lightLevelTrapezoid.trapezoid.span.start);
		}
	}
	
	public SimpleRange endHeightRange
	{
		get {
//			return this.lightLevelTrapezoid.trapezoid.endRange;
			return this.lightLevelTrapezoid.heightAt(lightLevelTrapezoid.trapezoid.span.extentMinusOne());
		}
	}
	
	public SimpleRange heightRangeAt(int patchRelZ) 
	{
		return this.lightLevelTrapezoid.heightAt(patchRelZ);	
	}
	
	public byte lightLevelAt(int patchRelZ)
	{
		return this.lightLevelTrapezoid.lightLevelAt(patchRelZ);	
	}
	
	public bool heightRangeExistsAtZ(int z) {
		return this.lightLevelTrapezoid.heightRangeExistsAtZ(z);	
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
	
	public Window(WindowMap _windowMap, SimpleRange startDisRange, int startX, int startZ, bool noSurfaceCheck)
	{
		this.constructorSetupWindow(_windowMap, startDisRange, startX, startZ);
	}
	
	public Window(WindowMap _windowMap, SimpleRange startDisRange, int startX, int startZ)
	{
//		this.m_windowMap = _windowMap;	
//		Trapezoid atrapezoid = new Trapezoid(startDisRange, startZ);
//		this.lightLevelTrapezoid = new LightLevelTrapezoid(TrapLight.MediumLightQuad(), atrapezoid);
//		
//		heights.Add(startDisRange);
//		
//		xLevel = startX;
		
		this.constructorSetupWindow(_windowMap, startDisRange, startX, startZ);
		// TODO: make a new class that inventories the locations of discontinuities
		// probably 2 lists of height/span offset values
		// plus 2 height values on either end.
		
		checkAdjacentToSurfaceFourDirectionsAt(0);
	}
	
	private void constructorSetupWindow(WindowMap _windowMap, SimpleRange startDisRange, int startX, int startZ)
	{
		this.m_windowMap = _windowMap;	
		Trapezoid atrapezoid = new Trapezoid(startDisRange, startZ);
		this.lightLevelTrapezoid = new LightLevelTrapezoid(TrapLight.MediumLightQuad(), atrapezoid);
		
//		this.heights.Add(startDisRange);
		
		this.xLevel = startX;
	}
	
	#region adjacency with surface
	
	private void checkAdjacentToSurfaceFourDirectionsAt(int spanOffset)
	{
		PTwo checkPoint = patchRelativeOriginPTwo;
		checkPoint.t += spanOffset; 
		
		foreach(PTwo surrounding in DirectionUtil.SurroundingPTwoCoordsFromPTwo(checkPoint))
		{
			int surfaceHeight = m_windowMap.surfaceHeightAt(surrounding.s, surrounding.t);	

			if (windowHeightAtOffsetGreaterThanHeight(spanOffset, surfaceHeight))
				this.addLightLevelsWithAdjacentSurfaceHeightSpanOffset(surfaceHeight, spanOffset);
			// else...
		}
	}
	
	private bool heightIsAboveSurface(int spanOffset)
	{
		PTwo checkPoint = patchRelativeOriginPTwo;
		checkPoint.t += spanOffset; 
		
		foreach(PTwo surrounding in DirectionUtil.SurroundingPTwoCoordsFromPTwo(checkPoint))
		{
			int surfaceHeight = m_windowMap.surfaceHeightAt(surrounding.s, surrounding.t);	

			if (windowHeightAtOffsetGreaterThanHeight(spanOffset, surfaceHeight))
				return true;
		}
		return false;
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
			this.lightLevelTrapezoid.removeLightLevelsBySubtractingAdjacentSurface(patchRelativeZ);
		}
	}
	
	//clear at z
	public void resetAt(int patchRelZ)
	{
		if (!this.spanContainsZ(patchRelZ))
			return;
		
		this.lightLevelTrapezoid.resetAt(patchRelZ);
	}
	
	public void resetAllLightsAndHeights()
	{
		this.lightLevelTrapezoid.resetAllLightsAndHeights();	
	}
	
	public void editLightWithTerminatingSurfaceHeightAtPatchRelativeZ(int surfaceHeight, int patchRelZ, bool terminalIsExtent)
	{
		//Right now the side that the adjacent light comes from doesn't matter
		//so just nudge the z coord over + or - and treat this like a side (non terminating) adjacency
		
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
		byte lightAtBefore = this.lightLevelAt(patchRelZ);
		
		if (surfaceHeight < heightAt)
		{
			this.addLightLevelsWithAdjacentSurfaceHeightSpanOffset(surfaceHeight, patchRelZ - this.spanStart);
		} else {
			this.lightLevelTrapezoid.removeLightLevelsBySubtractingAdjacentSurface(	patchRelZ);
		}
		
		//INFLUENCE...
		int deltaLight = (int)(lightAtBefore - this.lightLevelAt(patchRelZ));
		if (Mathf.Abs(deltaLight) > (int)UNIT_FALL_OFF_BYTE)
		{
			SimpleRange influenceRange = SimpleRange.SimpleRangeWithStartAndExtent( patchRelZ - INFLUENCE_RADIUS, patchRelZ + INFLUENCE_RADIUS);
			influenceRange = SimpleRange.IntersectingRange(influenceRange, new SimpleRange(0, NoisePatch.patchDimensions.z)); //wimp out (need other noisepatch windows too)
			influenceNeighborsInDirectionWithinZRange(Direction.xpos, influenceRange, INFLUENCE_RADIUS, deltaLight > 0);
			influenceNeighborsInDirectionWithinZRange(Direction.xneg, influenceRange, INFLUENCE_RADIUS, deltaLight > 0);
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
	
	private int windowHeightAtOffset(int spanOffSet) 
	{
		if (spanOffSet < 0 || spanOffSet >= this.zExtent )
			return -1;
			
		return this.lightLevelTrapezoid.heightAt(this.lightLevelTrapezoid.trapezoid.span.start + spanOffSet).extent(); //  heights[spanOffSet].extent();
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
	
	public void influenceNeighborsIfAboveSurfaceAddLight()
	{
		for(int i = this.spanStart; i < this.zExtent; ++i)
		{
			if (this.heightIsAboveSurface(i - this.spanStart))
			{
				influenceNeighborsInDirectionWithinZRange(Direction.xneg, new SimpleRange(i - INFLUENCE_RADIUS, i + INFLUENCE_RADIUS), INFLUENCE_RADIUS, false);
				influenceNeighborsInDirectionWithinZRange(Direction.xpos, new SimpleRange(i - INFLUENCE_RADIUS, i + INFLUENCE_RADIUS), INFLUENCE_RADIUS, false);
			}
		}
	}
	
	/*
	 * this somewhat (totally?) does the same thing as updatewithadjacentwindowsreturnaddedLightRating
	 * except its limited to a local area.
	 * within a given z span
	 * get your neighbors in one of four dir xz pos neg
	 * impart influence by subtracting or adding on the neighbors (if within the span)
	 * you're given a counter, which if its pos, you should call this func on each neighbor (who was influenced at all
	 * within a spanOfInfluence that you were keeping track of whil you imparted influence)
	 * if the sOI has range zero, stop
	 * if counter is zero stop
	 */
	private void influenceNeighborsInDirectionWithinZRange(Direction dir, SimpleRange zSpanOfInfluence, int counter, bool wantSubtract)
	{
		if (counter <= 0)
			return;
		
		List<Window> neisInDirection = neighborsForDirection(dir);
		if (neisInDirection == null || neisInDirection.Count == 0)
			return;
		
		SimpleRange nextZSpanOfInfluence = new SimpleRange(257,0);
		Window neib = null;
		for(int i = 0; i < neisInDirection.Count; ++i)
		{
			neib = neisInDirection[i];
			
			// lilTraps work togethr. return nextZSOI
			nextZSpanOfInfluence = neib.lightLevelTrapezoid.considerLightValuesFromOther(this.lightLevelTrapezoid, 
				wantSubtract, zSpanOfInfluence, (float) (counter/(float) INFLUENCE_RADIUS));
			
			if  (nextZSpanOfInfluence.range > 0)  {
				neib.influenceNeighborsInDirectionWithinZRange(dir, nextZSpanOfInfluence, counter - 1, wantSubtract);	
			}
		}
		
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
	
	private bool spanOverLaps(Window other)
	{
		return SimpleRange.RangesIntersect(lightLevelTrapezoid.trapezoid.span, other.lightLevelTrapezoid.trapezoid.span);	
	}
	
	private void updateWithAdjacentWindow(Window adjacentWindow)
	{
//		if (overlapExistsWithWindow(adjacentWindow)) { //TEST
		if (spanOverLaps(adjacentWindow)) {
//			lightLevelTrapezoid.updateWithXAdjacentPoint(adjacentWindow.midPointLight);
//			lightLevelTrapezoid.updateWithXAdjacentPoint(adjacentWindow.lightLevelTrapezoid.midPointNeg);
//			lightLevelTrapezoid.updateWithXAdjacentPoint(adjacentWindow.lightLevelTrapezoid.midPointPos);
			lightLevelTrapezoid.addMingleLightWithAdjacentTrapezoid(adjacentWindow.lightLevelTrapezoid);
		}
	}
	
	#endregion
	
	#region debug
	
	public void addHeightRangesAsColumns(int debugInteger)
	{
		for(int i = 0; i < NoisePatch.patchDimensions.z / 4 ; ++i)
		{
			SimpleRange r = lightLevelTrapezoid.heightAt(i);
//			if (r.range > 0)
//			{
				ChunkManager.debugLinesAssistant.addColumn(r, new PTwo(xCoord, i), debugInteger);	
//			}
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
		SimpleRange compareRange = this.lightLevelTrapezoid.heightAt(z); // this.startRange;
//		if (z > this.spanStart + this.spanRange/2)
//			compareRange = this.endRange;
		
		if (compareRange.contains(y))
			return 0;
		
		if (y > compareRange.start) // y is above entire range
			return compareRange.extent() - y;
		
		return compareRange.start - y;
	}
	
	#region assert debug
	public void assertSpansAreSaneDebug() {
		assertSpansAreSaneDebug("");
	}
	
	public void assertSpansAreSaneDebug(string msg) {
		this.lightLevelTrapezoid.assertSpanDebug(msg);	
	}
	
	public void assertValueGreaterThanHeightAt(int val, int pRelz, string msg) {
		AssertUtil.Assert(val > (int) this.lightLevelTrapezoid.heightAt(pRelz).extent(), msg);	
	}
	
	public void assertValueLessThanStartAt(int val, int pRelz, string msg) {
		AssertUtil.Assert(val < (int) this.lightLevelTrapezoid.heightAt(pRelz).start, msg);	
	}
	
	public void assertHeightRangesGreaterThanZeroInDomain() {
		for(int i = spanStart; i < zExtent ; ++i)
		{
			if (!(lightLevelTrapezoid.heightAt(i).range > 0)) {
				ChunkManager.debugLinesAssistant.addColumn(lightLevelTrapezoid.heightAt(i), new PTwo(xCoord, i + spanStart), 7);
				AssertUtil.Assert(lightLevelTrapezoid.heightAt(i).range > 0 , "range <= 0 at z: " + i + " the height range: " + lightLevelTrapezoid.heightAt(i).toString());	
			}
		}
	}
	
	#endregion
	
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
	
	public bool incorporateDiscontinuityAtNoEdit(SimpleRange disRange, int z)
	{
		return incorporateDiscontinuityAt(disRange, z, false); // always?
	}
	
	public bool incorporateDiscontinuityAtAllowEdit(SimpleRange disRange, int z)
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
	
	private bool addDiscontinuityAt(SimpleRange disRange, bool wantExtent)
	{
		SimpleRange the_range = wantExtent ? this.endHeightRange : this.startHeightRange; // this.lightLevelTrapezoid.trapezoid.endRange : this.lightLevelTrapezoid.trapezoid.startRange;
		
		if (rangesMeetMergeRequirements(the_range, disRange))
		{
			if (wantExtent)
				this.lightLevelTrapezoid.addDiscontinuityRangeAtEnd(disRange);
			else 
				this.lightLevelTrapezoid.addDiscontinuityRangeAtStart(disRange);
			return true;
		}
		
		return false;
	}
	
	private bool editDiscontinuityAt(SimpleRange disRange, int z)
	{
//		if (rangesMeetMergeRequirements(lightLevelTrapezoid.heightAt(z), disRange))
//		{
			//TODO: and there is not currently a disRange at this z
			//NEVER want to overwrite an existing z...
		if (!this.lightLevelTrapezoid.heightRangeExistsAtZ(z))
		{
			this.lightLevelTrapezoid.updateDiscontinuityAtPatchRelativeZ(disRange, z);
			return true;
		} 
//		else 
//		{
//			throw new Exception("we never want to try to edit a disrange when height exists there...");	//NOT TRUE...
//		}
//		}
		
		assertSpansAreSaneDebug();
		
		return false;
	}
	
	public List<Window> splitAtZ(int patchRelZ)
	{
		if (patchRelZ >= NoisePatch.patchDimensions.z - 1 || patchRelZ <= 0)
			return null;
		
		if(patchRelZ >= zExtent - 1 || patchRelZ <= spanStart) {
			return null;
		}
		
		int myNewExtent = patchRelZ;
		int posWindowNewStart = patchRelZ;
		
		bool shouldSplit = false;
		bool shouldSplitOnBothSides = false;
		
		if (!this.lightLevelTrapezoid.heightRangeExistsAtZ(patchRelZ))
		{
			posWindowNewStart++;
			shouldSplit = true;
		} else {
			if (!this.lightLevelTrapezoid.isContiguityAtZ(patchRelZ, false))
			{
				shouldSplit = true;	
			}
			if (!this.lightLevelTrapezoid.isContiguityAtZ(patchRelZ, true))
			{
				if (shouldSplit) {
					shouldSplitOnBothSides = true;	
				} else {
					myNewExtent++;
				}
				shouldSplit = true;
				posWindowNewStart++;
			}
		}
		
		if (shouldSplit)
		{
			b.bug("got split in two");
			List<Window> result = new List<Window>();
			Window posSideWindow = new Window(this.m_windowMap, this.lightLevelTrapezoid.heightAt(posWindowNewStart), 
				this.xLevel, posWindowNewStart, true); // TODO: new constructor for this? for in general?
			
			this.lightLevelTrapezoid.copyHeightAndLightValuesFromTo(ref posSideWindow.lightLevelTrapezoid, posWindowNewStart, this.zExtent);
			
			posSideWindow.lightLevelTrapezoid.trapezoid.span.range = this.zExtent - posWindowNewStart;
			
			this.lightLevelTrapezoid.trapezoid.span.range = myNewExtent - this.spanStart;
			
			result.Add(posSideWindow);
			
			posSideWindow.assertHeightRangesGreaterThanZeroInDomain(); //DBG
			
			if (shouldSplitOnBothSides)
			{
				b.bug("got split in three");
				Window middleWindow = new Window(this.m_windowMap, this.lightLevelTrapezoid.heightAt(posWindowNewStart - 1), 
					this.xLevel, posWindowNewStart - 1, true); 
			
				this.lightLevelTrapezoid.copyHeightAndLightValuesFromTo(ref middleWindow.lightLevelTrapezoid, posWindowNewStart - 1, posWindowNewStart);
				
				middleWindow.lightLevelTrapezoid.trapezoid.span.range = 1;
				result.Add(middleWindow);
				
				middleWindow.assertHeightRangesGreaterThanZeroInDomain(); //DBG
			}
			
			this.assertHeightRangesGreaterThanZeroInDomain(); //DBG
			
			return result;
		}
		
		return null;
	}
	
	public bool trimDomain()
	{
		lightLevelTrapezoid.assertSpanDebug();
		bool didTrim = false;
		
		if (!this.lightLevelTrapezoid.rangeExistsAtExtent)
		{
			this.lightLevelTrapezoid.resetAt(zExtent - 1);
			this.lightLevelTrapezoid.shortenSpanByOneFromExtent();
			didTrim = true;
		}
		
		//important to do this after remove at extent (it moves the start by + one)
		if (!this.lightLevelTrapezoid.rangeExistsAtStart)
		{
			this.lightLevelTrapezoid.resetAt(spanStart);
			this.lightLevelTrapezoid.shortenSpanByOneFromStart();
			didTrim = true;
		}
		
		if (didTrim)
			b.bug("did trim domain");
		
		return didTrim;	
	}

	
	public bool incorporateWindowFlushWithExtent(Window nextWindow)
	{
		if (rangesMeetMergeRequirements(this.endHeightRange, nextWindow.startHeightRange))
		{
//			this.lightLevelTrapezoid.addDiscontinuityRangeBeyondEndAtZExtent(nextWindow.endHeightRange, nextWindow.zExtent);
			this.lightLevelTrapezoid.incorporateStartFlushWithExtentOther(nextWindow.lightLevelTrapezoid);
			return true;
		}
		return false;
	}
	
	private bool rangesMeetMergeRequirements(SimpleRange terminalRange, SimpleRange disRange)
	{
		return (SimpleRange.IntersectingRange(terminalRange, disRange).range >= Window.WINDOW_HEIGHTS_MINIMUN_OVERLAP);
	}
	
	#endregion
}
