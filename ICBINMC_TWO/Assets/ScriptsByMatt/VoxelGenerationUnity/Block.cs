using UnityEngine;
using System.Collections;

public struct Sides
{
	public bool xp, xn, yp, yn, zp, zn;

//	public Sides()
//	{
//
//	}

}

public enum Direction
{
	xpos = 0, xneg, ypos, yneg, zpos, zneg
}

public enum BlockType
{
	Grass, Path, TreeTrunk, TreeLeaves, BedRock, Air, Stone, Stucco, Sand, Dirt, ParapetStucco, LightBulb

}


public class Block  {

	public Sides sides;
	public BlockType type;

	public Block()
	{
		type = BlockType.Grass;
	}

	public Block (BlockType t)
	{
		type = t;
	}


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	
	public void activateSideWithDirection(Direction dir)
	{
		switch (dir) {
		case Direction.xpos:
			sides.xp = true;
			break;
		case Direction.xneg:
			sides.xn = true;
			break;
		case Direction.ypos:
			sides.yp  = true;
			break;
		case Direction.yneg:
			sides.yn  = true;
			break;
		case Direction.zpos:
			sides.zp = true;
			break;
		default:
			sides.zn = true;
			break;
		}
	}
}
