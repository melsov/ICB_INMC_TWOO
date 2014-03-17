using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public struct SurroundingSurfaceValues {
	public int xneg, zneg, xpos, zpos;
	
	public static SurroundingSurfaceValues MakeNew() {
		SurroundingSurfaceValues ssv = new SurroundingSurfaceValues();
		ssv.xneg = ssv.zneg = ssv.xpos = ssv.zpos = 257;	
		return ssv;
	}
	
	public void setValueForDirection(int val, Direction dir)
	{
		if (dir == Direction.xneg)
		{
			xneg = val;
		}
		else if (dir == Direction.xpos)
		{
			xpos = val;
		}
		else if (dir == Direction.zneg)
		{
			zneg = val;
		}
		else if (dir == Direction.zpos)
		{
			zpos = val;
		}
	}
	
	public int valueForDirection(Direction dir)
	{
		if (dir == Direction.xneg)
		{
			return xneg;
		}
		else if (dir == Direction.xpos)
		{
			return xpos;
		}
		else if (dir == Direction.zneg)
		{
			return zneg;
		}
		else if (dir == Direction.zpos)
		{
			return zpos;
		}
		
		return 258;
	}
	
	public int[] toArray()
	{
		return new int[]{xneg, xpos, zneg, zpos};
	}
	
	public int lowestValue()
	{
		int lowest = 258;
		foreach(int i in this.toArray())
		{
			if (i > 0 && i < lowest)
				lowest = i;
		}
		
		return lowest;
		
//		int lowest = (xneg < xpos && xneg >= 1) ? xneg : xpos;
//		if (lowest < 1)
//			lowest = zpos;
//		lowest = (lowest > zneg && zneg >= 1)  ?  zneg : lowest;
//		lowest = (lowest > zpos && zpos >= 1) ? zpos : lowest; //just don't allow neg values...
//		if (lowest >= 1)
//			return lowest;
//		return 257;
	}
	
	public void debugAssertLowestNonNegFound()
	{
		int lowest = this.lowestValue();
		
		foreach(int i in this.toArray())
		{
			if (i > 0)
			{
				AssertUtil.Assert(i >= lowest, "what? i is: " + i + " lowest : " + lowest);
			}
		}
	}
	
	public string toString()
	{
		return "SSVs xneg: " + xneg + " xpos: " + xpos + " zneg: " + zneg  + " zpos: " + zpos;
	}
}

public class WindowMap  
{

	// notes:
	/*
	 * While going through ranges in noisePatch
	 * Tell me the x and z you're at
	 * bool: whether there was discontinuity
	 * the discontinuity range (simplerange)
	 * surface height (I can look up) from noisePatch
	 * 
	 * I compile lists of windows
	 * each list stored in an array of length noisepatch.patchdims.z
	 * 
	 * // BY The way. make it possible to update the 'tangents' meshdata (i.e. light values) without recalculating entire facesets.
	 * // implies no change in vert count, so no prob!
	 * 
	 * // must be able to query and be queried by neighbor windows...
	 * 
	 * Consider: useful if windows can SOMETIMES use a measurement of which section (x wise) can see the surface.
	 * (handy for a sky light-like case.) also windows should be able to split and join easily.
	 */
	
	private List<Window>[] windows = new List<Window>[NoisePatch.patchDimensions.x];
	private NoisePatch m_noisePatch;
	

//	private LightColumnCalculator m_lightColumnCalculator; // = new LightColumnCalculator();
//	public LightColumnMap lightColumnMap {
//		get {
//			return m_lightColumnCalculator.lightColumnMap;
//		}
//	}
	
	public DiscreteDomainRangeList<LightColumn> getLightColumnsAt(PTwo patchRelPoint)
	{
		return null; // m_lightColumnCalculator.getLightColumnsAt(patchRelPoint);
	}
	
	public Coord worldCoord {
		get {
			return CoordUtil.WorldCoordFromNoiseCoord( m_noisePatch.coord);
		}
	}
	
	public WindowMap(NoisePatch _npatch)
	{
		m_noisePatch = _npatch;
//		m_lightColumnCalculator = new LightColumnCalculator(_npatch); // FOR DBG give npatch
	}
	
	#region query
	
	// TODO: add direction ... so that we know which way to look for windows...
	public float ligtValueAtPatchRelativeCoord(Coord relCo, Direction faceDirection)
	{
		return 0f; // m_lightColumnCalculator.ligtValueAtPatchRelativeCoord(relCo); //, faceDirection);
//		relCo.x = Mathf.Clamp(relCo.x, 0, NoisePatch.patchDimensions.x - 1); //shouldn't need...
		
		AssertUtil.Assert(relCo.x < windows.Length && relCo.x >= 0 , "relco was out of bounds: " + relCo.x);
		List<Window> wins = windows[relCo.x];
		
		if (wins == null || wins.Count == 0)
			return 0f;
		
		Window closestWindow = windowClosestToYZInDirection(wins, relCo.y, relCo.z, faceDirection);
		
		if (closestWindow != null)
		{
			return closestWindow.lightValueForYZPoint(new PTwo(relCo.y, relCo.z));	
		}

		return 0f;
	}
	
	private Window windowClosestToYZInDirection(List<Window> winList, int y, int z, Direction faceDirection)
	{
		// NOTE: wish we could know secondary directions to avoid...
		// particularly whether to avoid up in this case...
		if (winList == null || winList.Count == 0) {
			throw new Exception("didn't think we'd actually get here");
			return null;
		}
		
		int pickIndex = -1;
		int abs_shortest_y_dist = 258;
		Window win = null;
		
		for(int i = 0; i < winList.Count; ++i)
		{
			win = winList[i];
			if (win.spanContainsZ(z)) 
			{
				int abs_y_dist = Mathf.Abs(win.shortestDifferenceWith(y,z));

				if (abs_y_dist < abs_shortest_y_dist)
				{
					abs_shortest_y_dist = abs_y_dist;
					pickIndex = i;
					if (abs_shortest_y_dist == 0)
						break;
				}
			}
		}
		
		if (pickIndex != -1) {
			Window retWin = winList[pickIndex];
			AssertUtil.Assert(retWin != null, "return window is null?");
			return retWin;
		}
		
		return null;	
	}
	
	#endregion
	
	public void clearColumnAt(int x, int z)
	{
		return; //
		if (x < 0 || x >= windows.Length)
			return;
		
		List<Window> wins = windows[x];	
		if (wins == null)
			return;
		
		foreach(Window win in wins)
		{
			win.resetAt(z);	
		}
	}
	
	public void updateWindowsWithNewSurfaceHeight(int newHeight, int xx, int zz)
	{
//		m_lightColumnCalculator.updateWindowsWithHeightRangesAndUpdateLight(newHeight, xx, zz,);
//		m_lightColumnCalculator.updateWindowsWithNewSurfaceHeight(newHeight, xx,zz);
		return;
		// TODO. deal with this the light col way!
		// prune a list of exposed light columns
		
		// get any windows with edge (start or extent) at z
		
		//deal with Z edges.
		foreach(Window win in windowsFlushWithZatX(xx, zz, false))
		{
			// window deal with new height	
			win.editLightWithTerminatingSurfaceHeightAtPatchRelativeZ(newHeight, zz, false);
		}
		
		foreach(Window win in windowsFlushWithZatX(xx, zz, true))
		{
			//window deal with new height	
			win.editLightWithTerminatingSurfaceHeightAtPatchRelativeZ(newHeight, zz, true);
		}
		
		if (xx < windows.Length - 1)
		{
			List<Window> xplusone = xplusone = windows[xx + 1];
			if (xplusone != null)
			{
				foreach(Window win in xplusone)
				{
					win.editLightWithAdjacentNonTerminatingSurfaceHeightAtPatchRelativeZ(newHeight, zz);
				}
			}
		}
		
		if (xx > 0)
		{
			List<Window> xminusOne = windows[xx - 1];
			if (xminusOne != null)
			{
				foreach(Window win in xminusOne)
				{
					win.editLightWithAdjacentNonTerminatingSurfaceHeightAtPatchRelativeZ(newHeight, zz);
				}
			}
		}
	}
	
	#region update with height ranges
	
	/*
	 * Clients use this to:
	 * update the window map to reflect changed
	 * height ranges which were (potentially) already set (but possibly no longer valid) in windows
	 * 
	 */ 
	public void updateWindowsWithHeightRanges(List<Range1D> heightRanges, int x, int z, SurroundingSurfaceValues ssvs)
	{
//		m_lightColumnCalculator.updateWindowsWithHeightRangesAndUpdateLight(heightRanges, x, z, ssvs, -1, false);
		return;
		
		//reset column 
		clearColumnAt(x,z);
		
		// re-add ranges
		for(int j = 1; j < heightRanges.Count; ++j)
		{
			addDiscontinuityWith(heightRanges[j], heightRanges[j - 1], x, z, ssvs, true, true);
		}
		
		if (windows[x] == null)
			return;
		
		//WINDOW MAINTENANCE
		List<Window> wins = windows[x];
		Window win = null;
		
		//TRIM EDGES
		for(int i = 0; i < wins.Count; ++i)
		{
			win = wins[i];
			
			win.assertSpansAreSaneDebug("bfr trim domain"); //DBG
			
			// trim edges of windows if needed
			// at extent
			if (win.trimDomain()) 
			{
				// check whether we should remove the window
				if (win.spanRange <= 0)
				{
					wins.RemoveAt(i);
					--i;
				}
			}
		}
		
		for(int i = 0; i < wins.Count; ++i)
		{

			// split windows at z if needed
			win = wins[i];
			
			if (win.spanContainsZ(z))
			{
				List<Window> posSideWindows = win.splitAtZ(z);
				
				if (posSideWindows != null)
				{
					int count = 0;
					foreach(Window posSideWin in posSideWindows) //MAX COUNT: 2
					{
						posSideWin.assertSpansAreSaneDebug("pos side win "); //DBG
						
						count++;
						wins.Insert(i + count, posSideWin);	
					}
					i += count;
				}
				
				win.assertSpansAreSaneDebug("after done splitting win (wMAP)"); //DBG
			}
		}
		
		
		//DEBUG
		if (NoiseCoord.Equal(this.m_noisePatch.coord, new NoiseCoord(0,0)))
			debugAllWindowHeightColumns();
	}	
	
	#endregion

	#region add height ranges
	
	/*
	 * clients use this to:
	 * add to window map (by providing a 'column' (list of ranges) of solid areas)
	 * will not 'overwrite' extant window height ranges
	 * (for overwriting, use the update version of this func.)
	 */ 
	public void addDiscontinuityToWindowsWithHeightRanges(List<Range1D> heights1D, int xx, int zz, SurroundingSurfaceValues ssvs)
	{
//		m_lightColumnCalculator.updateWindowsWithHeightRanges(heights1D, xx,zz,ssvs, -1, false);
		return;
		
		for(int j = 1; j < heights1D.Count; ++j)
		{
			addDiscontinuityWith(heights1D[j], heights1D[j - 1], xx, zz, ssvs, false, false);
		}			
	}
	
	#endregion
	
	private void addDiscontinuityWith(Range1D aboveRange, Range1D belowRange, int xx, int zz, SurroundingSurfaceValues ssvs, bool wantToEditPossibly, bool doLightUpDate)
	{
		int gap = aboveRange.start - belowRange.extent();
		if (gap > 0)
		{
			addDiscontinuityAt(new SimpleRange(belowRange.extent(), gap), xx, zz, ssvs, wantToEditPossibly, doLightUpDate);
		}
	}
	
	private void addDiscontinuityAt(SimpleRange disRange, int x, int z, SurroundingSurfaceValues surroundingSurfaceHeights, bool allowEditing, bool doLightUpdate)
	{
		//TODO: deal with this the light column way!
		
		Window tookDiscontinuity = incorporateDiscontinuityAt(disRange, x, z, allowEditing);
		
		if (tookDiscontinuity != null)
		{
			tookDiscontinuity.editLightWithSurroundingSurfaceValues(surroundingSurfaceHeights, z);
			return;
		}
		
		// TODO: edit windows when we edit a range
		
		// new window
		Window win = new Window(this, disRange, x, z); //TODO: add surrounding surf to constructor
		addWindowAt(win, x, z);
		
		if (doLightUpdate)
		{
			win.influenceNeighborsIfAboveSurfaceAddLight();	
		}
	}

	
	private List<Window> windowsListAt(int x)
	{
		if (windows[x] == null)
		{
			windows[x] = new List<Window>();
		}
		return windows[x];
	}
	
	private void addWindowAt(Window win, int x, int z)
	{
		List<Window> wins = windowsListAt(x);
		//CONSDIER inserting according to z
		wins.Add(win);
		windows[x] = wins;
	}
	
	#region calculate light
	
	// automata style?
//	private byte lightLevelAt(int x, int y, int z)
//	{
//		List<Window> wins = windows[x];
//		if (wins == null)
//			return 0;
//		
//		foreach(Window win in wins)
//		{
//			if (win.heightRangeAt(z).contains(y))
//			{
//				return win.lightLevelAt(z);
//			}
//		}
//		return 0;
//	}
	
	public void calculateLight()
	{
//		m_lightColumnCalculator.calculateLight();
	}
		
	
	public void calculateLightAdd()
	{
//		m_lightColumnCalculator.calculateLight();
		return;
		// TODO: devise a way to know when windows have lost light.
		// TODO: once we're done with the first one. devise a way for windows to spread their updates to other windows.
		// CONSIDER HOWEVER: pretty hard to get around iterating through all of the lights. No?
		// some system of lists of Influenced and Influencer windows?
		addLightForwards();
		calculateLightBackwards();
	}
	
	private void resetLightsAndHeightsIn(List<Window> wins)
	{
		if (wins == null)
			return;
		foreach(Window win in wins)
			win.resetAllLightsAndHeights();
	}
	
	private void resetAllLightsAndHeights()
	{
		foreach(List<Window> wins in windows)
		{
			if (wins == null)
				continue;
			foreach(Window win in wins)
				win.resetAllLightsAndHeights();
		}
	}
	
	private void addLightForwards()
	{
		int ix = 0;
		for(; ix < windows.Length - 1 ; ++ix)
		{
			if (windows[ix] != null && windows[ix + 1] != null)
			{
				List<Window> wins = windows[ix];
				List<Window> nextWins = windows[ix + 1];
				for(int k = 0; k < nextWins.Count; ++k)
				{
					nextWins[k].updateWithAdjacentWindowsReturnAddedLightRating(wins);	
				}
			}
		}
	}
	
	private void calculateLightBackwards()
	{
		int ix = windows.Length - 1;
		for(; ix > 0 ; --ix)
		{
			if (windows[ix] != null && windows[ix - 1] != null)
			{
				List<Window> wins = windows[ix];
				List<Window> nextWins = windows[ix - 1];
				for(int k = 0; k < nextWins.Count; ++k)
				{
					nextWins[k].updateWithAdjacentWindowsReturnAddedLightRating(wins);	
				}
			}
		}
	}
	
	#endregion
	
	#region ask a window map
	
	public int surfaceHeightAt(int x , int z)
	{
		return m_noisePatch.highestPointAtPatchRelativeCoord(new Coord(x, 0, z));
	}
	
	public List<Window> windowsFlushWithWindowZFace(Window aWindow, bool wantPosZFace)
	{
		return windowsFlushWithWindowZFace(aWindow, wantPosZFace, false);
	}
	
	public List<Window> windowsFlushWithWindowZFace(Window aWindow, bool wantPosZFace, bool wantThisCoord)
	{
		List<Window> result = new List<Window>();
		int index = wantPosZFace ? aWindow.xCoord + 1 : aWindow.xCoord - 1;
		index = wantThisCoord ? aWindow.xCoord : index;
		
		if (index < 0) {
			// go get from next window...	
			return null;
		} else if (index > windows.Length - 1) {
			//go get from other next
			return null;
		}
		List<Window> wins = windows[index];
		if (wins == null)
			return result;
		
		SimpleRange thisWinSpan = aWindow.span;
		if (wantThisCoord) {
			thisWinSpan = new SimpleRange(thisWinSpan.start - 1, thisWinSpan.range + 1); //want edge windows as well in this case
		}
		
		foreach(Window win in wins) {
			if (SimpleRange.RangesIntersect(aWindow.span, win.span))
			{
				if (!wantThisCoord || (wantThisCoord && !win.Equals(aWindow)))
					result.Add(win);
			}
		}
		return result;
		
	}
	
//	public List<Window> windowsAdjacentToWindowEitherSide(Window window)
//	{
//		List<Window> xNegWindows = windowsXNegAdjacentToWindow(window);	
//		List<Window> xPosWindows = windowsXPosAdjacentToWindow(window);
//		xNegWindows.AddRange(xPosWindows);
//		return xNegWindows;
//	}
//	
//	public List<Window> windowsXPosAdjacentToWindow(Window window)
//	{
//		return windowsAdjacentToWindow(window, true);
//	}
//	
//	public List<Window> windowsXNegAdjacentToWindow(Window window)
//	{
//		return windowsAdjacentToWindow(window, false);
//	}
//	
//	private List<Window> windowsAdjacentToWindow(Window window, bool wantXPos)
//	{
//		return null;
//	}
	
//	public List<Window> windowsAdjacentToStartOfWindow(Window window)
//	{
//		return windowsAdjacentToEndWindow(window, false);
//	}
//	
//	public List<Window> windowsAdjacentToExtentOfWindow(Window window)
//	{
//		return windowsAdjacentToEndWindow(window, true);
//	}
//	
//	private List<Window> windowsAdjacentToEndWindow(Window window, bool wantExtent)
//	{
//		return null;
//	}
	
	#endregion
	
	#region talk to other windows
	
	/*
	 * 'introduce' (share data with) windows from neighbor noise patches
	 * 
	 */ 
	
	public void introduceFlushWindowsWithWindowInNeighborDirection(WindowMap other, NeighborDirection ndir)
	{
		return;
		Direction dir = NeighborDirectionUtils.DirecionFourForNeighborDirection(ndir);
		
		Axis axis = DirectionUtil.AxisForDirection(dir);
		if (axis == Axis.X)
		{
			introduceFlushWithWindowMapInNeighborDirectionX(other, ndir);
			return;
		}
		
		introduceFlushWithWindowMapInNeighborDirectionZ(other, ndir);
	}
	
	/*
	 * X AXIS NEIGHBOR INTRODUCTIONS
	 * 
	 */ 
	private void introduceFlushWithWindowMapInNeighborDirectionX(WindowMap other, NeighborDirection ndir)
	{
		return; // !!!!
		
		Direction dir = NeighborDirectionUtils.DirecionFourForNeighborDirection(ndir);
		Direction oppDir = NeighborDirectionUtils.DirecionFourForNeighborDirection(NeighborDirectionUtils.oppositeNeighborDirection(ndir));
		
		//want windows along a z on a given x val (0 or max)
		List<Window>[] thisWinListArrayAtEdge = this.windowsTouchingEdgeOfNoisePatchNeighborInDirection( dir);
		List<Window>[] winListArrayAtAnEdgeOfOther = other.windowsTouchingEdgeOfNoisePatchNeighborInDirection(oppDir);	
		
		if (thisWinListArrayAtEdge.Length == 0 || winListArrayAtAnEdgeOfOther.Length == 0)
		{
			AssertUtil.Assert(false, "??? 0 length arrays");
			return;
		}
		
		AssertUtil.Assert(thisWinListArrayAtEdge.Length == 1 && winListArrayAtAnEdgeOfOther.Length == 1, "Confusing. dealing with x edge right? length was: " + thisWinListArrayAtEdge.Length);
		
		foreach(Window win in thisWinListArrayAtEdge[0]) // when world was flat this was null!
		{
//			win.testSetAllValuesTo(4f);
			//TEST
			win.setMaxLight();
//			win.updateWithAdjacentWindowsReturnAddedLightRating(winListArrayAtAnEdgeOfOther[0]);	
		}
		
		
		foreach(Window othersWin in winListArrayAtAnEdgeOfOther[0])
		{
//			othersWin.testSetAllValuesTo(4f);
			othersWin.setMaxLight(); //TEST
			
//			othersWin.updateWithAdjacentWindowsReturnAddedLightRating(thisWinListArrayAtEdge[0]);
		}
			
	}
	/*
	 * Z introductions
	 * 
	 * */
	private void introduceFlushWithWindowMapInNeighborDirectionZ(WindowMap other, NeighborDirection ndir)
	{

		Direction dir = NeighborDirectionUtils.DirecionFourForNeighborDirection(ndir);
		Direction oppDir = NeighborDirectionUtils.DirecionFourForNeighborDirection(NeighborDirectionUtils.oppositeNeighborDirection(ndir));
		
		//want windows flush with a given z value (0 or max)
		List<Window>[] thisWinListArrayAtEdge = this.windowsTouchingEdgeOfNoisePatchNeighborInDirection(dir);
		List<Window>[] winListArrayAtAnEdgeOfOther = other.windowsTouchingEdgeOfNoisePatchNeighborInDirection(oppDir);	
		
		AssertUtil.Assert(thisWinListArrayAtEdge.Length == winListArrayAtAnEdgeOfOther.Length, "confusing. and not what we expected. lists should be the same length");
		
		bool wantExtentForThis = DirectionUtil.IsPosDirection(dir);
		
//		this.testSetAllMaxLight(); //TEST
//		other.testSetAllMaxLight();
//		
//		return;
		
		for(int i = 0 ; i < thisWinListArrayAtEdge.Length ; ++i)
		{
			List<Window> wins = thisWinListArrayAtEdge[i];
			foreach(Window win in wins)
			{
				//TEST
//				win.testSetMaxLight(null);
				win.updateWithWindowsFlushWithAnEdge(winListArrayAtAnEdgeOfOther[i], wantExtentForThis); 	
			}
		}
		
		for(int i = 0 ; i < winListArrayAtAnEdgeOfOther.Length ; ++i)
		{
			List<Window> wins = winListArrayAtAnEdgeOfOther[i];
			foreach(Window win in wins)
			{
				//TEST
//				win.testSetMaxLight(null);
				win.updateWithWindowsFlushWithAnEdge(thisWinListArrayAtEdge[i], !wantExtentForThis); 
			}
		}
	}
	
	private void testSetAllMaxLight()
	{
		foreach(List<Window> wins in windows)
		{
			if (wins == null)
				continue;
			foreach(Window win in wins)
			{
				win.setMaxLight();	
			}
		}
	}
	
	#endregion
	
	#region update with edge surface heights
	
	public void updateWindowsFlushWithEdgeInNeighborDirection(byte[] surfaceHeightsAtEdge, NeighborDirection ndir)
	{
		// ndir is neighb's relation to us 
		// so want the windows on the opposite edge
		Direction dir = NeighborDirectionUtils.DirecionFourForNeighborDirection(NeighborDirectionUtils.oppositeNeighborDirection(ndir));
		Axis axis = DirectionUtil.AxisForDirection(dir);
		
		if (axis == Axis.X)
		{
			updateWindowsAlongXEdge(surfaceHeightsAtEdge, DirectionUtil.IsPosDirection(dir));
			return;	
		}
		
		updateWindowsAlongZEdge(surfaceHeightsAtEdge, DirectionUtil.IsPosDirection(dir));
	}
	
	private void updateWindowsAlongXEdge(byte[] surfaceHeightsAtEdge, bool wantMaxXWindows)
	{
		for(int i = 0 ; i < NoisePatch.patchDimensions.x ; ++i)
		{
			List<Window> winsFlushWith = windowsFlushWithEdgeAtX(i, wantMaxXWindows);
			foreach(Window win in winsFlushWith)
			{
				if (win.allValuesMaxed)
					break;
				
				win.addLightLevelsWithAdjacentSurfaceHeightSpanOffset(surfaceHeightsAtEdge[i], wantMaxXWindows ? win.spanRange : 0);
			}
		}
	}
	
	private List<Window> windowsFlushWithEdgeAtX(int x_index, bool wantMaxXEdge)
	{
		List<Window> result = new List<Window>();
		List<Window> atXWins = windows[x_index];
		
		if (atXWins == null || atXWins.Count < 1)
			return result;
		
		if (wantMaxXEdge) {
			
			for(int i = atXWins.Count - 1; i >= 0 ; --i)
			{
				Window win = atXWins[i];
				if (win.zExtent == NoisePatch.patchDimensions.z) {
					result.Add(win);	
				} else {
					break;
				}
			}
			
			return result;	
		}

		for(int i = 0 ; i < atXWins.Count ; i++)
		{
			Window win = atXWins[i];
			if (0 == win.spanStart) {
				result.Add(win);	
			} else {
				break;
			}
		}
		return result;
	}
	
	private List<Window> windowsFlushWithZatX(int x_index, int zz, bool wantExtent)
	{
		List<Window> result = new List<Window>();
		List<Window> atXWins = windows[x_index];
		
		if (atXWins == null || atXWins.Count < 1)
			return result;
		
		foreach(Window win in atXWins)
		{
			if (wantExtent)
			{
				if (win.zExtent == zz) {
					result.Add(win);	
				} 
			} else {
				if (win.spanStart - 1 == zz) {
					result.Add(win);
				}
			}
		}
		
		return result;
	}
	
	private void updateWindowsAlongZEdge(byte[] surfaceHeightsAtEdge, bool wantMaxZWindows)
	{
		int index = wantMaxZWindows ? windows.Length - 1 : 0;
		if (windows[index] == null)
			return;
		
		List<Window> winList = windows[index];
		
		for(int i = 0 ; i < winList.Count ; ++i)
		{
			Window win = winList[i];
			for (int j = win.spanStart ; j < win.zExtent ; ++j)
			{
				if(win.allValuesMaxed)
					break;
				
				win.addLightLevelsWithAdjacentSurfaceHeightSpanOffset(surfaceHeightsAtEdge[j], win.spanStart - j);
			}
		}
		
		if (winList != null)
			windows[index] = winList;
	}
	
	#endregion
	
	#region find windows adjacent to
	
	private List<Window>[] windowsTouchingEdgeOfNoisePatchNeighborInDirection(Direction dir)
	{
		Axis axis = DirectionUtil.AxisForDirection(dir);
		
		if (axis == Axis.X)
		{
			return windowsWithEdgeFlushToXEdge(DirectionUtil.IsPosDirection(dir));	
		}
		
		return windowsWithEdgeFlushToZEdge(DirectionUtil.IsPosDirection(dir));
	}
	
	private List<Window>[] windowsWithEdgeFlushToZEdge(bool wantZMaxEdge)
	{
		List<Window>[] result = new List<Window>[64];
		
		for(int i = 0; i < result.Length; ++i)
		{
			result[i] = windowsFlushWithEdgeAtX(i, wantZMaxEdge);	
		}
		return result;
	}
	
	private List<Window>[] windowsWithEdgeFlushToXEdge(bool wantXMaxEdge)
	{
		if (wantXMaxEdge) {
			return new List<Window>[] {windows[windows.Length - 1]};	
		}
		
		return new List<Window>[] {windows[0]};
	}
	
	//do we need a new func. to edit around a new surface height?
	
	//INCORP DISCONTINUITY *UPDATING WINDOWS VERSION*
	private Window incorporateUpdatedDiscontinuityAt(SimpleRange disRange, int x, int z, bool allowEditing)
	{
		return incorporateDiscontinuityAt(disRange, x, z, allowEditing);
		//at this point (we pray)
		//windows could have gaps (one z wide)
		//therefore, we want to add a discont to the MIDDLE of a window first if we can.
		//But, not if there already is a discont in that z column (that is non-overlapping or contig with the column)
		//in other words, we can't have two disRanges at one z
		//COME TO THINK OF IT...this will work anyway with the adding version.? not quite, but it should?
	}
	
	// INCORP DISCONTINUITY *FOR SIMPLE ADDING DISCONT*
	
	// the flow here is a bit messy! (might be helped by an enum Incorporation Status: atExtent, atStart, inMiddle, notIncorporated)
	private Window incorporateDiscontinuityAt(SimpleRange disRange, int x, int z, bool wantToAllowEditing)
	{
		foreach(Window aWindow in windowWithExtentAdjacentTo(x,z))
		{
			if (aWindow.incorporateDiscontinuityAtNoEdit(disRange, z))
			{
				//check to see if there's a start that we can join with
				foreach(Window flushWindow in windowWithStartAdjacentTo(x,z)) //NOTE: not the fastest way to find?
				{
					if (aWindow.incorporateWindowFlushWithExtent(flushWindow))
					{
						List<Window> winList = windowsListAt(x);
						winList.Remove(flushWindow);
						break;
					}
				}
				
				return aWindow;
			}
		}
		
		// DO start separately
		foreach(Window aWindow in windowWithStartAdjacentTo(x,z))
		{
			if(aWindow.incorporateDiscontinuityAtNoEdit(disRange,z))
			{
//				b.bug("took dis con at start");
				return aWindow;
				//TODO: carefully add break if window dims are beyond (?) (we don't expect to find any more because windows are ordered by dimensions in some way...we hope...)
				//TODO FIRST: test that windows are always ordered by some (which) aspect of their trap.spans. (start or extent, or start start height.start as tie breaker?)
			}
		}
		
		// TODO: 1. ensure that we can incorp windows flush with from start where appropriate
		// (e.g. is the inner foreach working??) (windowWithStartAdjTo(x,z)...) (OR: 1.b is it incorp flush with extent that isn't working?? its one of them.)
		// 2. ensure that when make any fixes for 1. we are still fulfilling the requirments 
		// (i.e. following the procedure for) editing windows as opposed to adding to windows
		// CONSIDER: a sep incorpDisAt-SPEEDYFIRSTTIMEAROUND- version. where we ONLY check
		// for flush with extent? (would make things pretty delicate...)

		if (!wantToAllowEditing)
			return null;
		
		if (windows[x] == null)
			return null;
		
		
		// all these iterations, a little inefficient, no? 
		
		//CONSIDER: do we really care about the order in which we try to incorporate discons?
		foreach(Window aWindow in windows[x])
		{
			if (aWindow.incorporateDiscontinuityAtAllowEdit(disRange, z))
			{
//				b.bug("took discon presumably from middle");
				return aWindow;
			}
			aWindow.assertSpansAreSaneDebug(); //DBG
		}
		
//		b.bug("couldn't take discon");
//		foreach (
		return null;
	}
	
	// CONSIDER (WIDE RANGING): a collection class that allows look up in two ways:
	// by index (as usual for lists)
	// by the start of the type it holds.
	// could do the second one quickly if it had a (doubly?) linked list
	// of start value/index values ... (or is that not faster than just going through a list of these things like normal? O(n) both cases?)
	// could do a dictionary collection + list ....
	
	private IEnumerable windowWithStartAdjacentTo(int x, int z)
	{
		if (z < NoisePatch.patchDimensions.z - 1)
		{
			List<Window> wins = windowsListAt(x);
			for (int i = wins.Count - 1; i >= 0; --i)
			{
				Window w = wins[i];
				if (w.zLevelIsAdjacentToStart(z))
				{
					yield return w;		
				}
			}
		}
	}
	
	private IEnumerable windowWithExtentAdjacentTo(int x, int z)
	{
		if (z > 0)
		{
			List<Window> wins = windowsListAt(x);
			for (int i = wins.Count - 1; i >= 0; --i)
			{
				Window w = wins[i];
				if (w.zLevelIsAdjacentToExtent(z))
				{
					yield return w;
				}
			}
		}
	}
	
	#endregion
	
	private Window lastWindowAt(int x)
	{
		List<Window> windowsatx = windows[x];
		if (windowsatx == null || windowsatx.Count == 0)
			return null;
		return windowsatx[windowsatx.Count - 1];
	}
	
	private Window firstWindowAt(int x)
	{
		List<Window> wins = windows[x];
		if (wins == null || wins.Count == 0)
			return null;
		return wins[0];
	}
	
	#region debug
	
	private void debugAllWindowHeightColumns()
	{
		ChunkManager.debugLinesAssistant.clearColumns();
		
		int count = 0;
		int rowCount = 0;
		foreach(List<Window> wins in windows)
		{
			if (wins == null)
				continue;
			foreach(Window win in wins)
			{
				win.addHeightRangesAsColumns(count++);	
			}
			if (rowCount++ > 16)
				break;
		}
	}
	
	#endregion
	
}
