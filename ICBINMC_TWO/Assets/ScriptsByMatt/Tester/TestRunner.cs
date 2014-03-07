using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class TestRunner : MonoBehaviour 
{
	public static CoRange WorldLimits = new CoRange(new Coord (-1, 0, -1), new Coord(2, 1, 2));
	
//	private static bool dontRunGame = true;
	private static bool dontRunGame = false;
	
//	public const bool DontRunDoTerrainTestInstead = true;
	public const bool DontRunDoTerrainTestInstead = false;
	
//	public const bool RunGameOnlyOneNoisePatch = true;
	public const bool RunGameOnlyOneNoisePatch = false;	
	
//	public const bool RunGameOnlyNoisPatchesWithinWorldLimits = true;
	public const bool RunGameOnlyNoisPatchesWithinWorldLimits = false;
	
	
	
	private bool doRunTest = true;
	
	// Use this for initialization
	List<MeshSet> meshSets;
	
	static Color orthoXColor = new Color(.1f, .3f, .5f, 1f);
	static Color orthoZColor = new Color(.4f, .5f, .1f, 1f);
	static Color diagColor = new Color(.5f, .2f, .8f, 1f);	
	
	void Start () 
	{
		if (dontRunGame)
		{
//			FaceSetTest fst = new FaceSetTest();
			FaceAggregatorTest fat = new FaceAggregatorTest();
			meshSets = fat.getMeshResults();
		}
	}
	
	public static bool NoiseCoordWithinTestLimits(NoiseCoord nco)
	{
		if (TestRunner.RunGameOnlyNoisPatchesWithinWorldLimits)
		{
			int xstart, zstart, xend, zend;
			xstart = TestRunner.WorldLimits.start.x;
			xend = TestRunner.WorldLimits.outerLimit().x;
			zstart = TestRunner.WorldLimits.start.z;
			zend = TestRunner.WorldLimits.outerLimit().z;
			
			return xstart < nco.x && xend > nco.x && zstart < nco.z && zend > nco.z;
		}
		return true;
	}
	
	public static bool DontRunGame()
	{
		return dontRunGame;	
	}
	
	// Update is called once per frame
	void Update () {
		if (doRunTest && dontRunGame)
			drawMeshSets();
	}
	
	void drawMeshSets()
	{
		List<Vector3> verts;
		List<int> indices;
		
		Vector3 aa;
		Vector3 bb;
		Vector3 cc;
		
		Color add_color;
		int meshSetCount = 0;
		float color_cycle = 0f;
		float color_cycle2 = .5f;
		
		if (meshSets == null)
		{
			UnityEngine.Object[] objects = FindObjectsOfType (typeof(GameObject));
			foreach (GameObject go in objects) {
				go.SendMessage ("OnPauseGame", SendMessageOptions.DontRequireReceiver);
			}
			doRunTest = false;
			throw new Exception("mesh sets was null");
			
			
		}
		
		foreach(MeshSet mset in meshSets)
		{
			verts = mset.geometrySet.vertices;
			indices = mset.geometrySet.indices;
			
			float cos_col1 = (float) Mathf.Cos (color_cycle * 3.14159f);
			float cos_col2 = (float) Mathf.Cos (color_cycle2 * 3.14159f * .75f);
			add_color = new Color( (cos_col1 + cos_col2) * .15f , cos_col1 * .5f , cos_col2 , 1f);
			int i = 0;
			for(;i< indices.Count; i += 3)
			{
				if(i + 2 < indices.Count) 
				{
					aa = verts[indices[i]];
					bb = verts[indices[i+1]];
					cc = verts[indices[i+2]];
					drawLineChooseColor(aa,bb, add_color);
					drawLineChooseColor(bb,cc, add_color);
					drawLineChooseColor(cc,aa, add_color);
				}
			}
			meshSetCount += 1;
			color_cycle = (float)((int)(meshSetCount * 4) % 7)/7f;
			color_cycle2 = (float)((int)(meshSetCount * 6) % 11)/11f;
		}
		
	}
	
	static void drawLineChooseColor(Vector3 aa, Vector3 bb, Color addColor)
	{
		Color col = diagColor;
		if (aa.x == bb.x) // along z
			col = orthoZColor;
		else if (aa.z == bb.z) // along x
			col = orthoXColor;
		
		col += addColor;
		debugLineV(aa,bb,col);
	}
	
	static void debugLineV(Vector3 aa, Vector3 bb)
	{
		UnityEngine.Debug.DrawLine (aa, bb);
	}
	
	public static void bug(string str) {
		if (dontRunGame)
			UnityEngine.Debug.Log(str);	
	}
	
	
	static void debugLineV(Vector3 aa, Vector3 bb, Color color)
	{
		UnityEngine.Debug.DrawLine (aa, bb, color);
	}
}
