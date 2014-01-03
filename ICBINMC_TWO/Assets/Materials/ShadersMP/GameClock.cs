using UnityEngine;
using System.Collections;
using System.Threading;

public class GameClock : MonoBehaviour 
{

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
		float shininess = .5f + .5f * Mathf.Sin (Time.time * .3F);
		_chunkRenderer.sharedMaterial.SetFloat("_GameClock", shininess);

		_playerCamera.backgroundColor = Color.Lerp(nightColor, dayColor, shininess);
	}
}
