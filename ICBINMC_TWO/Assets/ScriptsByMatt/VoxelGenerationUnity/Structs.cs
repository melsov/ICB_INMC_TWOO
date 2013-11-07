using UnityEngine;
using System.Collections;




public struct NeighborChunks
{
	public Chunk xpos, xneg, ypos, yneg, zpos, zneg;

	public NeighborChunks(Chunk xp, Chunk xn, Chunk yp, Chunk yn, Chunk zp , Chunk zn)
	{
		xpos = xp;
		xneg = xn;
		ypos = yp;
		yneg = yn;
		zpos = zp;
		zneg = zn;
	}
}



public class Structs 
{


}
