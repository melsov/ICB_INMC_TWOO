using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
	
	public Coord worldCoord {
		get {
			return CoordUtil.WorldCoordFromNoiseCoord( m_noisePatch.coord);
		}
	}
	
	public WindowMap(NoisePatch _npatch)
	{
		m_noisePatch = _npatch;
	}
	
	#region query
	
	// TODO: add direction ... so that we know which way to look for windows...
	public float ligtValueAtPatchRelativeCoord(Coord relCo, Direction faceDirection)
	{
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
		if (winList == null || winList.Count == 0)
			return null;
		
		int pickIndex = -1;
		int abs_shortest_y_dist = 258;
		
		for(int i = 0; i < winList.Count; ++i)
		{
			Window win = winList[i];
			if (win.spanContainsZ(z)) {
				int abs_y_dist = Mathf.Abs(win.shortestDifferenceWith(y,z));
				if (abs_y_dist < abs_shortest_y_dist)
				{
					abs_shortest_y_dist = abs_y_dist;
					pickIndex = i;
				}
			}
		}
		
		if (pickIndex != -1)
			return winList[pickIndex];
		
		return null;	
	}
	
	#endregion
//	
//	public void discontinuityAt(SimpleRange disRange, int x, int z)
//	{
//		discontinuityAt(disRange, x, z, SurroundingSurfaceValues.MakeNew());
//	}
	
	public void discontinuityAt(SimpleRange disRange, int x, int z, SurroundingSurfaceValues surroundingSurfaceHeights)
	{
		Window tookDiscontinuity = incorporateDiscontinuityAt(disRange, x, z);
		
		if (tookDiscontinuity != null)
		{
			tookDiscontinuity.editLightWithSurroundingSurfaceValues(surroundingSurfaceHeights, z);
			return;
		}
		
		// TODO: edit windows when we edit a range
		// separate changing the the light values from the rest of the mesh making...carefully...
		
		// new window
		Window win = new Window(this, disRange, x, z); //TODO: add surrounding surf to constructor
		addWindowAt(win, x, z);

	}
	
//	public void updateWindowsWithHeightRanges(List<Range1D> heightRanges, int x, int z)
//	{
//		//TODO
//	}
	
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
	
	public void calculateLight()
	{
		calculateLightForwards();
		calculateLightBackwards();
	}
	
	private void calculateLightForwards()
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
		
		foreach(Window win in thisWinListArrayAtEdge[0])
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
	
	private Window incorporateDiscontinuityAt(SimpleRange disRange, int x, int z)
	{
		foreach(Window aWindow in windowWithExtentAdjacentTo(x,z))
		{
			if (aWindow.incorporateDiscontinuityAt(disRange, z))
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
		
//		foreach (Window aWindow in windowWithStartAdjacentTo(x,z))
		
		// a little inefficient! 
		if (windows[x] == null)
			return null;
		
		foreach(Window aWindow in windows[x])
		{
			if (aWindow.incorporateDiscontinuityAt(disRange, z))
			{
				return aWindow;
			}
		}
		
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
	
}
