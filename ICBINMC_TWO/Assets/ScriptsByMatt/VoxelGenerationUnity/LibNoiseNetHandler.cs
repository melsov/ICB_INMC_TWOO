using UnityEngine;
using System.Collections;

using Graphics.Tools.Noise;
using Graphics.Tools.Noise.Modifier;
using Graphics.Tools.Noise.Primitive;
using Graphics.Tools.Noise.Filter;
using Graphics.Tools.Noise.Tranformer;

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

public class LibNoiseNetHandler : MonoBehaviour //using monobehaviour for test output only
{
	private IModule3D noiseModule;
	private FilterModule filterModule;
	private PrimitiveModule primitiveModule;
	
	private IModule2D noise2DModule;
	
	public float Threshold = 0f;
	
//	public float PerturbXStrength = 1f;
//	public float PerturbVariationStrength = 4f;
	
	
	private float SetOctaveCount = 6f;
	private float SetFrequency = 2f;
	private bool FadeFrequency = false;
	private float SetLacunarity = 2f;
	private float SetGain = 2f;
	private bool FadeGain = true;
	private float SetSpectralExponent = 1.9f;
	private bool FadeSpectralExponent = true;
	
	private int SetPrimitiveQual = 0;
	
	public int TexWidth = 400;
	public int TexHeight = 300;
	
	public float TexHZoom = 1f;
	public float TexWZoom = 1f;
	
	public bool FadeHZoom = false;
	public bool FadeWZoom = false;
	
	private Texture2D testTex;
	
	public float Gain3D {
		get {
			return filterModule.Gain;
		}
		set {
			filterModule.Gain = value;
		}
	}
	
	public float SpectralExponent3D {
		get {
			return filterModule.SpectralExponent;
		}
		set {
			filterModule.SpectralExponent = value;
		}
	}
	
	public float Frequency3D {
		get {
			return filterModule.Frequency;
		}
		set {
			filterModule.Frequency = value;
		}
	}
	
	private void setDefaults() 
	{
		setFilterModuleDefaults();
		setPrimitiveModuleDefaults();
	}
	
	private void setFilterModuleDefaults() {
		SetOctaveCount = SetOctaveCount < 1 ? 6f : SetOctaveCount;
		SetFrequency = SetFrequency < 1 ? 2f : SetFrequency;
		
		filterModule.OctaveCount = SetOctaveCount;
		filterModule.Frequency = SetFrequency;
		filterModule.Lacunarity = SetLacunarity;
		filterModule.Gain = SetGain;
		filterModule.SpectralExponent = SetSpectralExponent;
	}
	
	private void setPrimitiveModuleDefaults() {
		primitiveModule.Quality = (NoiseQuality) Mathf.Clamp(SetPrimitiveQual, 0, 2);
	}
	
	private void refreshTexture() 
	{
		setDefaults();
		
		testTex = GetTestImage();
		renderer.material.mainTexture = testTex;
	}
	
	public LibNoiseNetHandler()
	{
		
		SetUpRidgedMultiFractal3DModule ();
		SetUpNoise2D();
		
		setDefaults();
	}

	private void SetUpRidgedMultiFractal3DModule() 
	{
		// make a 3d ridged multifractal module
		PrimitiveModule pModule = null;
		pModule = new SimplexPerlin();
		
		primitiveModule = pModule;

		pModule.Seed = 123;

		ScaleBias scale = null;

		filterModule =  new RidgedMultiFractal(); //new Pipe(); //
		
		// Used to show the difference with our gradient color (-1 + 1)
		scale = new ScaleBias (filterModule, 1f, 0f); // 0.9f, -1.25f);
		
		float rmfScale = .75f;
		ScalePoint scalePoint = new ScalePoint(filterModule, rmfScale, rmfScale * 1f, rmfScale);
		
		filterModule.Primitive3D = (IModule3D)pModule;

		noiseModule = scalePoint; // scale;
	}
	
	private void SetUpNoise2D()
	{
		//noise character should vary
		PrimitiveModule pModule2D = new SimplexPerlin();
		pModule2D.Seed = primitiveModule.Seed + 1234;
		
		noise2DModule = (IModule2D) pModule2D;
	}

	public float GetRidgedMultiFractalValue(float x, float y, float z) {
		return noiseModule.GetValue (x, y, z);
	}
	
	public float Get2DValue(float x, float z) {
		return noise2DModule.GetValue(x,z);
	}
	
//	public Texture2D GetTestImage() 
//	{
//		int width = TexWidth;
//		int height = TexHeight;
//
//		float value = 0;
//
//		Texture2D tex = new Texture2D (width, height);
//		
//		int i = 0;
//		for (; i < width; ++i) {
//			
//			float iFF = (float)i / (float)(width *TexWZoom);
//			int j = 0;
//			for(; j < height; ++j) {
//				
//				
//				float mapHeight = (float)j / (float)(height); 
//				
//				
////				float fadeValue = (float)(j / (float)height);
////				
////				// 2D// 2D// 2D// 2D// 2D // 2D // 2D
////				
////				if (FadeGain)
////					filterModule.Gain = SetGain * fadeValue;
////				if (FadeFrequency)
////					filterModule.Frequency = SetFrequency * fadeValue;
////				if (FadeSpectralExponent)
////					filterModule.SpectralExponent = SetSpectralExponent * fadeValue;
//				
//				float zz = 0f; // pretend z coord
//				
//				float yy = mapHeight * .59f ; // PerturbVariationStrength;
//				
//				//tweak (perturb?) 
//				float rmfValue = noiseModule.GetValue(iFF, yy, zz);
//				float xx = iFF; // + rmfValue;
//				
//				value = noise2DModule.GetValue (xx, zz);
////				value += rmfValue;
//				value = value * 0.45f + 0.25f;
//
////				Color c = value > Threshold ? new Color (value, value, value, 1f) : Color.red;
////				new Color (value, value, value, 1f)
//				
//				//perturbXStrength -- .2f
//				Color c = value + Mathf.Abs(rmfValue) * .2f > mapHeight ? Color.white : Color.red;
//
//
//		        tex.SetPixel (i, j, c );
//			}
//		}
//
//		tex.Apply();
//		
//		tex.filterMode = FilterMode.Point;
//		tex.anisoLevel = 1;
//
//		return tex;
//	}

	public Texture2D GetTestImage() 
	{
		return null;
		
//		int width = TexWidth;
//		int height = TexHeight;
//
//		float value = 0;
//
//		Texture2D tex = new Texture2D (width, height);
//		int i = 0;
//		for (; i < width; ++i) {
//			float fadeWZoomBy = 1f;
//			if (FadeWZoom)
//				fadeWZoomBy = .5f + .5f * (float)i / (float)(width );
//			float iFF = (float)i / (float)(width *TexWZoom *fadeWZoomBy);
//			int j = 0;
//			for(; j < height; ++j) {
//				
//				float fadeHZoomBy = 1f;
//				if (FadeHZoom) 
//					fadeHZoomBy = .25f + .5f * (float)j / (float)(height);
//				float jFF = (float)j / (float)(height *TexHZoom * fadeHZoomBy); //(jFF * .5f + jFF * jFF * .5f ); //(jFF * .125f + jFF * jFF * .5f + jFF * jFF * jFF* .375f );
//				
//				float fadeValue = (float)(j / (float)height);
//				
//				if (FadeGain)
//					filterModule.Gain = SetGain * fadeValue;
//				if (FadeFrequency)
//					filterModule.Frequency = SetFrequency * fadeValue;
//				if (FadeSpectralExponent)
//					filterModule.SpectralExponent = SetSpectralExponent * fadeValue;
//
//				value = noiseModule.GetValue (iFF, 0f, jFF);
//				
////				value = noise2DModule.GetValue (iFF, jFF);
//				value = value * 0.45f + 0.5f;
//
//				Color c = value > Threshold ? new Color (value, value, value, 1f) : Color.red;
//				
//				
////				if (value > 1)
////					c = new Color (1f, 2f - value, 0f, 1f);
////				else if (value < 0) 
////					c = new Color (1f, .9f + value, .9f + value, 1f);
////					c = Color.black; // new Color (0f, 0f, 0f, 1f);
//////				} else if (value < 0) {
////////					c = new Color (1f, 1f + value, 1f + value, 1f);
//////					c = new Color (0f, 0f, 1f, 1f);
////				} else {
////					c = Color.white; // new Color (value, value, value, 1f);
////				}
//
//		        tex.SetPixel (i, j, c );
//			}
//		}
//
//		tex.Apply();
//		
//		tex.filterMode = FilterMode.Point;
//		tex.anisoLevel = 1;
//
//		return tex;


	}

	void bug(string str) {
		UnityEngine.Debug.Log (str);
	}
	
	public void Start() 
	{
		refreshTexture();
	}
	
	
//	public void OnGUI() 
//	{
//
////		if (GUI.Button (new Rect (Screen.width - 170, Screen.height - 320, 150, 40), "Refresh" ))
////		{
////			refreshTexture ();
////		}
//
//		
//
//	}

}
