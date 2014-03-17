using UnityEngine;
using System.Collections;
//using System.Collections.Generic;

public static class CoordRadarUtil 
{
	/*
	 * param: a Y axis Euler angle 
	 * (in Unity, y euler angle 0 points in the direction of z axis positive 
	 * and pos angles rotate clockwise when looking from a bird's eye view.
	 * We will adopt this standard.) 
	 * param: an integer radius corresponding to number of blocks out 
	 * (i.e. half the length of some square whose perimeter is described in coords
	 * one or two of which are intersected by the y euler angle)
	 * return: an 'appropriate' intersected coord (appropriate because we need some way of choose when angles go through
	 * a corner coord and an adjacent side coord on the perimeter)
	 * 
	 */
	
	private const float SQROOT_TWO = 1.4142135623731F;
	
	public static Coord NudgeCoordXZForYAxisAngleAndRadius(int yAngleDegrees, int radius )
	{
		float x = (CrudeTrig.Sin(yAngleDegrees)); // x uses Sin.
		float z = (CrudeTrig.Cos(yAngleDegrees));
		
		//how close to a 45 degrees "like" (i.e. 45, 135, 225, 315) angle is yAngDegrees?
		int ang45 = yAngleDegrees % 90 - 45;
		ang45 *= ang45 < 0 ? -1 : 1;
		ang45 = 45 - ang45;
		
		//to get the "radius point" on the square
		//stretch radius proportional to lerp 1<-->SQRT2 ang45
		// CONSIDER: more efficient: radius += .4142 * (ang45/45f) ?
		float radStretch = (float) radius * Mathf.Lerp(1f, SQROOT_TWO, (float)(ang45/45.0f));
		
		x *= radStretch; 
		z *= radStretch; 
		
		//Wrangle inaccuracies
		if (x < 0)
		{
			x -= .5f;
			x = x < -(float) radius ? -(float)radius : x;
		} else {
			x += .5f;
			x = x > (float) radius ? (float)radius : x;
		}
		
		if (z < 0)
		{
			z -= .5f;
			z = z < -(float) radius ? -(float)radius : z;
		} else {
			z += .5f;
			z = z > (float) radius ? (float)radius : z;
		}
		
		return new Coord((int)x, 0, (int)z);
	}
}
