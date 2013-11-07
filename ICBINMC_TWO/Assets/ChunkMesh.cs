using UnityEngine;
using System.Collections;

//CLASS EXISTS ONLY TO
//HAVE A TYPE FOR THE GAME OBJECTS
//THAT OWN THE MESHES THAT CHUNK OBJECTS MAKE
//(CHUNK OBJECTS ARE NO LONGER ATTACHED TO THE MESHES THEY
// CREATE)
using System.Diagnostics;

public class ChunkMesh : MonoBehaviour 
{

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void really()
	{
		UnityEngine.Debug.Log ("yes. i'm really a chunk mesh");
	}
}
