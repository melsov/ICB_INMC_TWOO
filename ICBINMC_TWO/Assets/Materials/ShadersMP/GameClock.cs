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
		float shininess = .7f + .5f * Mathf.Sin (Time.time * .2F); //  Mathf.PingPong(Time.time * .1F, 1.0F);
		_chunkRenderer.sharedMaterial.SetFloat("_GameClock", shininess);

		_playerCamera.backgroundColor = Color.Lerp(nightColor, dayColor, shininess);
	}
}
