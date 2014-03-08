using UnityEngine;
using System.Collections;

public class HideCursor : MonoBehaviour {
	
	public bool hideCursor = true;
	// Use this for initialization
	void Start () {
		Screen.showCursor = !hideCursor;
	}
	
	void Update() {
		Screen.showCursor = !hideCursor;
	}
	

}
