#define ALTNOISE
#define ALTNOISE2

using System.Collections;
using LibNoise.Unity;
using LibNoise.Unity.Generator;
using LibNoise.Unity.Operator;
using System.IO;
using System;
using UnityEngine;

using System.Linq;
using System.Diagnostics;

public enum NoiseType {Perlin, Billow, RiggedMultifractal, Voronoi, Checker, Mix, RiggedMChecker, Cylinders, Spheres};


public class NoiseHandler 
{
	private Noise2D m_noiseMap = null;

	#if ALTNOISE
	private Noise2D m_altNoiseMap = null;
	public NoiseType altnoise = NoiseType.Billow;
	#endif

	#if ALTNOISE2
	private Noise2D m_altNoiseMap2 = null;
	public NoiseType altnoise2 = NoiseType.RiggedMultifractal;
	#endif

//	private Texture2D[] m_textures = new Texture2D[3];

	public int resolutionX = 64; 
	public int resolutionZ = 64; 
	public NoiseType noise = NoiseType.Perlin;
	public float zoom = 1f; 
	public float offset = 0f; 

	public void Generate() {	
		// Create the module network
		ModuleBase moduleBase;

		moduleBase = chooseModuleBase (noise);

		// Initialize the noise map
		this.m_noiseMap = new Noise2D(resolutionX, resolutionZ, moduleBase);
		this.m_noiseMap.GeneratePlanar(
			offset + -1 * 1/zoom, 
			offset + offset + 1 * 1/zoom, 
			offset + -1 * 1/zoom,
			offset + 1 * 1/zoom);

		//Generate the textures
//		this.m_textures[0] = this.m_noiseMap.GetTexture(LibNoise.Unity.Gradient.Grayscale);
//		this.m_textures[0].Apply();
//
//		//MMP experiment
//		this.m_noiseMap.GeneratePlanar( 
//			-2.0, // * 0.0, 
//		    -1.0,
//			-1.0 * 0.0,
//			1.0);  // WoRKS! phew! (makes adjacent tiles...)
//
//		// Generate the textures
//		Texture2D adjTex = this.m_noiseMap.GetTexture(LibNoise.Unity.Gradient.Grayscale);
//		adjTex.Apply();
//		File.WriteAllBytes(Application.dataPath + "/../GrayAdjLeftL.png", adjTex.EncodeToPNG() );
		// END MMP experiment


//		this.m_textures[1] = this.m_noiseMap.GetTexture(LibNoise.Unity.Gradient.Terrain);
//		this.m_textures[1].Apply();
//
//		this.m_textures[2] = this.m_noiseMap.GetNormalMap(3.0f);
//		this.m_textures[2].Apply();
//
//		//display on plane
//		renderer.material.mainTexture = m_textures[0];
//
//
//		//write images to disk
//		File.WriteAllBytes(Application.dataPath + "/../Gray.png", m_textures[0].EncodeToPNG() );
//		File.WriteAllBytes(Application.dataPath + "/../Terrain.png", m_textures[1].EncodeToPNG() );
//		File.WriteAllBytes(Application.dataPath + "/../Normal.png", m_textures[2].EncodeToPNG() );

//		Debug.Log("Wrote Textures out to "+Application.dataPath + "/../");


	}

	private ModuleBase chooseModuleBase(NoiseType moduleType) {
		ModuleBase moduleBase;
		switch(moduleType) {
		case NoiseType.Billow:	
			moduleBase = new Billow();
			break;

		case NoiseType.RiggedMultifractal:	
			moduleBase = new RiggedMultifractal();
			break;   

		case NoiseType.Voronoi:	
			moduleBase = new Voronoi();
			break;

		case NoiseType.Checker:
			moduleBase = new Checker ();
			break; 

		case NoiseType.Cylinders:
			moduleBase = new Cylinders ();
			break; 

		case NoiseType.Spheres:
			moduleBase = new Spheres ();
			break; 		            	         	

		case NoiseType.Mix:            	
			Perlin perlin = new Perlin();
			RiggedMultifractal rigged = new RiggedMultifractal();
			moduleBase = new Add(perlin, rigged);
			break;


		case NoiseType.RiggedMChecker:            	
			Checker checker = new Checker ();
			RiggedMultifractal rigged2 = new RiggedMultifractal ();
			moduleBase = new Add(checker, rigged2);
			break;

		default:
			moduleBase = new Perlin();
			break;

		}

		return moduleBase;
	}

	public void initNoiseMap() { //MMP
		// Create the module network
		ModuleBase moduleBase = chooseModuleBase (noise);

		// Initialize the noise map
		this.m_noiseMap = new Noise2D(resolutionX, resolutionZ, moduleBase);

		initAltNoiseMap();
	}

	private void initAltNoiseMap()
	{
		#if ALTNOISE
		ModuleBase alt_base = chooseModuleBase (altnoise);
		this.m_altNoiseMap = new Noise2D (resolutionX, resolutionZ, alt_base);
		#endif
	}

	private void initAltNoiseMap2()
	{
		#if ALTNOISE2
		ModuleBase alt_base2 = chooseModuleBase (altnoise2);
		this.m_altNoiseMap2 = new Noise2D (resolutionX, resolutionZ, alt_base2);
		#endif
	}

	public void GenerateAtNoiseCoord(NoiseCoord _noiseCoord)  ///MMP
	{	
		float left = (float)_noiseCoord.x * 2.0f;
		float right = left + 2.0f;
		float top = (float) _noiseCoord.z * 2.0f;
		float bottom = top + 2.0f;

		this.m_noiseMap.GeneratePlanar (
			left,
			right, 
			top,
			bottom);
	}

	public void GenerateAltAtNoiseCoord(NoiseCoord _noiseCoord) 
	{
		float left = (float)_noiseCoord.x * 2.0f;
		float right = left + 2.0f;
		float top = (float) _noiseCoord.z * 2.0f;
		float bottom = top + 2.0f;

		this.m_altNoiseMap.GeneratePlanar (
			left,
			right, 
			top,
			bottom);
	}

	public float this[int x, int y]
	{
		get {
			return this.m_noiseMap [x, y];
		}
		set {
			this.m_noiseMap [x, y] = value;
		}
	}

	public float getAltNoise(int x, int y)
	{
		#if ALTNOISE
		return this.m_altNoiseMap [x, y];
		#endif
		throw new Exception ("alt noise is not defined and yet we are trying to use it");
		return 1f;
	}

	public void setAltNoise(int x, int y, float val) {
		#if ALTNOISE
		this.m_altNoiseMap [x, y] = val;
		#endif
	}

	public float getAltNoise2(int x, int y)
	{
		#if ALTNOISE
		return this.m_altNoiseMap2 [x, y];
		#endif
		throw new Exception ("alt noise is not defined and yet we are trying to use it");
		return 1f;
	}

	public void setAltNoise2(int x, int y, float val) {
		#if ALTNOISE
		this.m_altNoiseMap2 [x, y] = val;
		#endif
	}

}