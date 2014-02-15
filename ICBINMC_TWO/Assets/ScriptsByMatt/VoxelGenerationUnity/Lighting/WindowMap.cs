using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
	
	public WindowMap(NoisePatch _npatch)
	{
		m_noisePatch = _npatch;
	}
	
	public void discontinuityAt(SimpleRange disRange, int x, int z)
	{
		if (incorporateDiscontinuityAt(disRange, x, z))
		{
			return;
		}
		
		// new window
		Window win = new Window(this, disRange, x, z);
		addWindowAt(win, x, z);
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
	
	#region ask a window map
	
	public int surfaceHeightAt(int x , int z)
	{
		return m_noisePatch.highestPointAtPatchRelativeCoord(new Coord(x, 0, z));
	}
	
	public List<Window> windowsXPosAdjacentToWindow(Window window)
	{
		return windowsAdjacentToWindow(window, true);
	}
	
	public List<Window> windowsXNegAdjacentToWindow(Window window)
	{
		return windowsAdjacentToWindow(window, false);
	}
	
	private List<Window> windowsAdjacentToWindow(Window window, bool wantXPos)
	{
		return null;
	}
	
	public List<Window> windowsAdjacentToStartOfWindow(Window window)
	{
		return windowsAdjacentToEndWindow(window, false);
	}
	
	public List<Window> windowsAdjacentToExtentOfWindow(Window window)
	{
		return windowsAdjacentToEndWindow(window, true);
	}
	
	private List<Window> windowsAdjacentToEndWindow(Window window, bool wantExtent)
	{
		return null;
	}
	
	#endregion
	
	#region find windows adjacent to
	
	private bool incorporateDiscontinuityAt(SimpleRange disRange, int x, int z)
	{
		foreach(Window aWindow in windowWithExtentAdjacentTo(x,z))
		{
			if (aWindow.incorporateDiscontinuityAt(disRange, z))
			{
				// TODO: check here to see if there's a start that we can join with
				return true;
			}
		}
		foreach (Window aWindow in windowWithStartAdjacentTo(x,z))
		{
			if (aWindow.incorporateDiscontinuityAt(disRange, z))
			{
				return true;
			}
		}
		return false;
	}
	
	private IEnumerable windowWithStartAdjacentTo(int x, int z)
	{
		if (z < NoisePatch.patchDimensions.z - 1)
			yield return windowWithExtentAdjacentTo(x,z,false);
	}
	
	private IEnumerable windowWithExtentAdjacentTo(int x, int z)
	{
		if (z > 0)
			yield return windowWithExtentAdjacentTo(x,z,true);
	}
	
	private IEnumerable windowWithExtentAdjacentTo(int x, int z, bool wantExtent)
	{
		List<Window> wins = windowsListAt(x);
		for (int i = wins.Count - 1; i >= 0; --i)
		{
			Window w = wins[i];
			if (wantExtent)
			{
				if (w.zLevelIsAdjacentToExtent(z))
				{
					yield return w;
				}
			} else {  
				if (w.zLevelIsAdjacentToStart(z))
				{
					yield return w;		
				}
			}
		}
//		return null;
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
