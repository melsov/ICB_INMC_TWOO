using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TestRunner : MonoBehaviour {
	
//	private static bool dontRunGame = true;
	private static bool dontRunGame = false;
//	public const bool DontRunDoTerrainTestInstead = true;
	public const bool DontRunDoTerrainTestInstead = false;
//	public const bool RunGameOnlyOneNoisePatch = true;
	public const bool RunGameOnlyOneNoisePatch = false;	
	
	// Use this for initialization
	List<MeshSet> meshSets;
	
	static Color orthoXColor = new Color(1.0f, .3f, .5f, 1f);
	static Color orthoZColor = new Color(.9f, .5f, .1f, 1f);
	static Color diagColor = new Color(.5f, .7f, .8f, 1f);	
	
	void Start () 
	{

		if (dontRunGame)
		{
//			FaceSetTest fst = new FaceSetTest();
			FaceAggregatorTest fat = new FaceAggregatorTest();
			meshSets = fat.getMeshResults();
		}
	}
	
	public static bool DontRunGame()
	{
		return dontRunGame;	
	}
	
	// Update is called once per frame
	void Update () {
		if (dontRunGame)
			drawMeshSets();
	}
	
	void drawMeshSets()
	{
		List<Vector3> verts;
		List<int> indices;
		
		Vector3 aa;
		Vector3 bb;
		Vector3 cc;
		
		foreach(MeshSet mset in meshSets)
		{
			verts = mset.geometrySet.vertices;
			indices = mset.geometrySet.indices;
			
			int i = 0;
			for(;i< indices.Count; i += 3)
			{
				if(i + 2 < indices.Count) 
				{
					aa = verts[indices[i]];
					bb = verts[indices[i+1]];
					cc = verts[indices[i+2]];
					drawLineChooseColor(aa,bb);
					drawLineChooseColor(bb,cc);
					drawLineChooseColor(cc,aa);
				}
			}
			
		}
		
	}
	
	static void drawLineChooseColor(Vector3 aa, Vector3 bb)
	{
		Color col = diagColor;
		if (aa.x == bb.x) // along z
			col = orthoZColor;
		else if (aa.z == bb.z) // along x
			col = orthoXColor;
		
		debugLineV(aa,bb,col);
	}
	
	static void debugLineV(Vector3 aa, Vector3 bb)
	{
		UnityEngine.Debug.DrawLine (aa, bb);
	}
	
	
	static void debugLineV(Vector3 aa, Vector3 bb, Color color)
	{
		UnityEngine.Debug.DrawLine (aa, bb, color);
	}
}
