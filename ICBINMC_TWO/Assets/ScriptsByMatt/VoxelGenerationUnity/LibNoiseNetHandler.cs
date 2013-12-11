using UnityEngine;
using System.Collections;

using Graphics.Tools.Noise;
using Graphics.Tools.Noise.Modifier;
using Graphics.Tools.Noise.Primitive;
using Graphics.Tools.Noise.Filter;

//NOISE PLAN:
//we want: 
//biomes
//varied terrain (biomes)
//caves
// one issue -- the biome should be decided by some 2d noise (control for a selector)
// yet in addition, we want to keep track of the values that are output by that 2d noise
// inorder to make other decisions (like whether trees are there e.g.) 

// sidelining this issue for now...
// we need a 2d noise for the height map (probably two types of noise with a selector)
// want to test for caves only underground

public class LibNoiseNetHandler  
{
	private IModule3D noiseModule;
	private FilterModule filterModule;
	
	public LibNoiseNetHandler()
	{
		SetUpRidgedMultiFractalModule ();
	}

	private void SetUpRidgedMultiFractalModule() 
	{
		// make a 3d ridged multifractal module
		PrimitiveModule pModule = null;
		pModule = new SimplexPerlin();

//		pModule.Quality = quality;
//		pModule.Seed = seed;

		FilterModule fModule = null;
		ScaleBias scale = null;

		fModule = new RidgedMultiFractal();
		// Used to show the difference with our gradient color (-1 + 1)
		scale = new ScaleBias(fModule, 0.9f, -1.25f);

//		fModule.Frequency = frequency;
//		fModule.Lacunarity = 5.0f; // lacunarity; //default 2.0f
//		fModule.OctaveCount = 6; // 6; // octaveCount; // default 6
//		fModule.Offset = offset;
//		fModule.Offset = offset;
//		fModule.Gain = 2.0f; // gain; // default 2.0;

		fModule.Primitive3D = (IModule3D)pModule;

		noiseModule = scale;
		filterModule = fModule;
	}

	public float GetRidgedMultiFractalValue(float x, float y, float z) {
		return noiseModule.GetValue (2.3f, 0f, 1f);
	}

	public Texture2D SaveTestImage() 
	{
		int width = 400;
		int height = 300;
		int id = 0;


//		IModule3D i3D = (IModule3D)noiseModule;
//		FilterModule fm = (FilterModule)i3D;

//		filterModule.SpectralExponent = .1f;

		float tweakValue = 3.5f;
		float value = 0;
		float scaleCoords = 2.0f;

//		Color[] result = new Color[width * height];
		Texture2D tex = new Texture2D (width, height);
		int i = 0;
		for (; i < width; ++i) {
			float iFF = (float)i / (float)width * scaleCoords;
			int j = 0;
			for(; j < height; ++j, ++id) {

				float jFF = (float)j / (float)height * scaleCoords;
//				result [id] = noiseModule.GetValue (iFF, 0f, jFF) > 0 ? Color.black : Color.white;
				
//				filterModule.Gain = tweakValue - tweakValue * (jFF * .5f + jFF * jFF * .5f );
//				filterModule.SpectralExponent = tweakValue * (jFF * .125f + jFF * jFF * .5f + jFF * jFF * jFF* .375f );
//				filterModule.Frequency = (float)(j / (float)height);

				value = noiseModule.GetValue (iFF, 0f, jFF);
				value = value * 0.3f + 0.5f;

				Color c = new Color (value, value, value, 1f);
				if (value > 1)
					c = new Color (2f - value, 1f, 0f, 1f);
				else if (value < 0) 
					c = new Color (1f, .9f + value, .9f + value, 1f);
//					c = Color.black; // new Color (0f, 0f, 0f, 1f);
////				} else if (value < 0) {
//////					c = new Color (1f, 1f + value, 1f + value, 1f);
////					c = new Color (0f, 0f, 1f, 1f);
//				} else {
//					c = Color.white; // new Color (value, value, value, 1f);
//				}

				
//				tex.SetPixel (i, j, value  > 0.5f ? Color.black : Color.white);
		        tex.SetPixel (i, j, c );
			}
		}


		tex.Apply();
		
		tex.filterMode = FilterMode.Point;
		tex.anisoLevel = 1;

		return tex;




	}

	void bug(string str) {
		UnityEngine.Debug.Log (str);
	}

}
