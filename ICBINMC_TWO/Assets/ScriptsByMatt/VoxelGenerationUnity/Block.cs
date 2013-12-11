using UnityEngine;
using System.Collections;

//serialize namespaces
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Diagnostics;


public enum Direction
{
	xpos = 0, xneg, ypos, yneg, zpos, zneg
}
// MOVED
//public enum BlockType
//{
//	Grass, Path, TreeTrunk, TreeLeaves, BedRock, Air, Stone, Stucco, Sand, Dirt, ParapetStucco, LightBulb
//}

public enum BiomeType
{
	Pasture, CraggyMountains, Ocean
}

[Serializable]
public class Block  
{
	public BlockType type { get; set;}

	//TODO: add BIOME type memvar. should we use store type and biomeType in one int?

	public Block()
	{
		type = BlockType.Grass;
	}

	public Block (BlockType t)
	{
		type = t;
	}
}
