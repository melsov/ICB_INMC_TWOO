using UnityEngine;
using System.Collections;

//serialize namespaces
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization; // ?
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

public class BlockHitter : MonoBehaviour {

	public ChunkManager blockDelegate;



	private float punchStartTime; 
	private Coord hitBlockCoord;
	private Coord alreadyHandledBlockCoord;



	private float blockBreakTimeSeconds;
	
	//TODO: block cursor should really show whenever midscreen is aimed at a block
	// block cursor should also actually be a plain that goes on the face in question (b/c they might want to add not subtract)
	// the current cube --could appear over blocks that are being destroyed
	// it would have a version of the normal block shader that could animate somehow.
	// maybe we could tweak the texture offset so that it moves around, change color? change scale, etc..

	//High score table
//	public List<Thing> highScores = new List<Thing>();
//	public Thing[,,] highScores = new Thing [16,256,16];
	
	public void handleLeftButtonUp()
	{
		blockDelegate.handleBreakBlockAborted();
	}

	public void handleLeftButtonHit(RaycastHit hit)
	{
		Coord worldHitCoord = new Coord (hit.point);

		if (!hitBlockCoord.equalTo(worldHitCoord) )
		{
			hitBlockCoord = worldHitCoord;
			punchStartTime = Time.fixedTime;
			blockBreakTimeSeconds = getBlockBreakTimeSeconds(hitBlockCoord);
			
			//break block in progress tell the block delegate
			blockDelegate.handleBreakBlockInProgress(hit);
			
			return;
		}

		if (Time.fixedTime - punchStartTime > blockBreakTimeSeconds)
		{
			if (hitBlockCoord.equalTo (alreadyHandledBlockCoord)) {
				
				return;
			}

//			bug ("sending hit to ch manager block is: " + hitBlockCoord.toString() );
//			blockDelegate.handleBreakBlockAt(hitBlockCoord);
			blockDelegate.handleBreakBlockAt(hit );

			alreadyHandledBlockCoord = hitBlockCoord;
		}
	}

	public void handleRightButtonHit(RaycastHit hit)
	{
		blockDelegate.handlePlaceBlockAt (hit);
	}

	private float getBlockBreakTimeSeconds(Coord b_coord)
	{
		return 0.7f; // for now
	}

	void bug(string str) {
		UnityEngine.Debug.Log(str);
	}


	// Use this for initialization
	void Start () {

//		PlayerPrefs.DeleteAll ();
		//Get the data
//		var data = PlayerPrefs.GetString("HighScores");
//		//If not blank then load it
//		if(!string.IsNullOrEmpty(data))
//		{
//			//Binary formatter for loading back
//			var b = new BinaryFormatter();
//			//Create a memory stream with the data
//			var m = new MemoryStream(Convert.FromBase64String(data));
//			//Load back the scores
////			highScores = (List<Thing>)b.Deserialize(m);
//			highScores = (Thing[,,])b.Deserialize(m);
//
////			Thing t_one = highScores [0,0,0];
////			foreach(Thing t_one in highScores)
//			for (int i = 0; i < highScores.GetLength(1); ++i)
//				for (int j = 0; j < highScores.GetLength(0); ++j) {
//					Thing t_one = highScores [j, i, 0];
//					if (t_one != null)
//						bug ("got high scores: " + t_one.name + t_one.randoo);
//				}
//		} else {
//			bug ("no High scores");
//		}
//
//
//
//		highScores = new Thing[16, 256, 16];
//		Thing tthing = new Thing { randoo = (Direction) 1, name = new Namey(765, "YoMyName") };
////		highScores.Add (tthing);
////		highScores [0, 0, 0] = tthing;
//
//		for(int i = 0; i < 256; ++i) {
//			for (int k = 0; k < 16; ++k) { 
//				for (int j = 0; j < 16; ++j) {
//					highScores [j, i, k] = tthing;
//				}
//			}
//		}
//
//		SaveScores ();
//
//		UnityEngine.Debug.Break ();
	}

	// Update is called once per frame
	void Update () {

	}



//	void SaveScores()
//	{
//		//Get a binary formatter
//		var b = new BinaryFormatter();
//		//Create an in memory stream
//		var m = new MemoryStream();
//		//Save the scores
//		b.Serialize(m, highScores);
//		//Add it to player prefs
//		PlayerPrefs.SetString("HighScores", 
//			Convert.ToBase64String(
//				m.GetBuffer()
//			)
//		);
//	}

}

//[Serializable]
//public struct Namey {
//	int hihi;
//	string yo;
//
//	public Namey(int h, string y) {
//		hihi = h;
//		yo = y;
//	}
//
//	public string toString() {
//		return "Namey struct: " + yo + hihi;
//	}
//}
//
//[Serializable]
////[XmlType(TypeName = "Thing")]
//public class Thing
//{
//	public Direction randoo { get; set;} 
//	public Namey name { get; set;}
//
//	public Thing() {
//
//	}
//}
