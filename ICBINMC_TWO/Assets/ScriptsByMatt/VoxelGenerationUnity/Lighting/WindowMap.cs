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
	
	public void discontinuityAt(SimpleRange disRange, int x, int z)
	{
		discontinuityAt(disRange, x, z, SurroundingSurfaceValues.MakeNew());
	}
	
	public void discontinuityAt(SimpleRange disRange, int x, int z, SurroundingSurfaceValues surroundingSurfaceHeights)
	{
		Window tookDiscontinuity = incorporateDiscontinuityAt(disRange, x, z);
		
		if (tookDiscontinuity != null)
		{
			tookDiscontinuity.addLightWithSurroundingSurfaceValues(surroundingSurfaceHeights, z);
			return;
		}
		
		// new window
		Window win = new Window(this, disRange, x, z);
		addWindowAt(win, x, z);
		ChunkManager.debugLinesAssistant.addWindowToDraw(win);
	}
	
	public void updateWindowsWithHeightRanges(List<Range1D> heightRanges, int x, int z)
	{
		//TODO
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
	
	#region find windows adjacent to
	
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
		
		foreach (Window aWindow in windowWithStartAdjacentTo(x,z))
		{
			if (aWindow.incorporateDiscontinuityAt(disRange, z))
			{
				return aWindow;
			}
		}
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
