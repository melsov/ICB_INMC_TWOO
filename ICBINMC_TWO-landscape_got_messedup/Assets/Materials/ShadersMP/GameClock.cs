using UnityEngine;
using System.Collections;
using System.Threading;

public class GameClock : MonoBehaviour 
{
	public bool ClockIsOn = false;
	
	public Transform _UrChunkTransform;
	private Renderer _chunkRenderer;
	public Camera _playerCamera;
	public Color dayColor;
	public Color nightColor;

	// Use this for initialization
	void Start () {
		_chunkRenderer = _UrChunkTransform.renderer;	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!ClockIsOn) {
			_chunkRenderer.sharedMaterial.SetFloat("_GameClock", 1f);
			_playerCamera.backgroundColor = Color.Lerp(nightColor, dayColor, 1f);
			return;
		}
		
		float shininess = .5f + .5f * Mathf.Sin (Time.time * .3F);
		_chunkRenderer.sharedMaterial.SetFloat("_GameClock", shininess);

		_playerCamera.backgroundColor = Color.Lerp(nightColor, dayColor, shininess);
	}
}
