using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class WindowMapTest
{
	WindowMap wmap;
	NoisePatch npatch;
	ChunkManager chman = new ChunkManager();
	
	public WindowMapTest()
	{
		npatch = new NoisePatch(new NoiseCoord(0,0), chman);
//		wmap = new WindowMap(npatch);		
		
		npatch.Start();
		
		while(!npatch.Update())
		{
		}
		
		
	}
}