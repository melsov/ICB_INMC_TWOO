using UnityEngine;
using System.Collections;

public class BlockCollectionTest : MonoBehaviour 
{
	BlockCollection blocks;
	
	Quad testNoiseCoordArea = new Quad(PTwo.PTwoZero(), new PTwo(4));
	
	// Use this for initialization
	void Start () 
	{
		blocks = new BlockCollection();
		
		foreach(NoiseCoord nco in CoordUtil.NoiseCoordsWithingQuad(testNoiseCoordArea))
		{
//			NoisePatch npatch = new NoisePatch(nco, )
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
