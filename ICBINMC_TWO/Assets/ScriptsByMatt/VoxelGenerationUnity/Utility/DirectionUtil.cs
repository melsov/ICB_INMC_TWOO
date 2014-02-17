using UnityEngine;
using System.Collections;

public static class DirectionUtil 
{
	public static Coord CoordForPTwoAndNormalDirectionWithFaceAggregatorRules(PTwo ptwo, Direction dir)
	{
		Axis axis = AxisForDirection(dir);
		if (axis == Axis.X)
			return new Coord(0, ptwo.t, ptwo.s); // Y is up , z is across (s)
		if (axis == Axis.Y)
			return new Coord(ptwo.s, 0, ptwo.t);
		
		return new Coord(ptwo.s, ptwo.t, 0);
	}
	
	public static Coord CoordForPTwoNormalOffsetNormalDirectionWithFaceAggregatorRules(PTwo ptwo, int normalOffset, Direction dir)
	{
		Axis axis = AxisForDirection(dir);
		if (axis == Axis.X)
			return new Coord(normalOffset, ptwo.t, ptwo.s); // Y is up , z is across (s)
		if (axis == Axis.Y)
			return new Coord(ptwo.s, normalOffset, ptwo.t);
		
		return new Coord(ptwo.s, ptwo.t, normalOffset);
	}
	
	public static Direction XIfZZifXKeepSign(Direction dir) {
		Axis axis = AxisForDirection(dir);
		Direction result = Direction.zpos;
		
		if (axis == Axis.Z)
			result = Direction.xpos;
		
		if (!IsPosDirection(dir)) {
			return OppositeDirection(result);	
		}
		return result;
	}
	
	public static Direction OppositeDirection(Direction dir) {
		return (Direction) ((int) dir + ((int) dir % 2 == 0 ? 1 : -1));
	}
	
	public static Axis AxisForDirection(Direction dir)
	{
		if (dir <= Direction.xneg)
			return Axis.X;
		if (dir <= Direction.yneg)
			return Axis.Y;
		return Axis.Z;
	}
	
	public static Coord NudgeCoordForDirection(Direction dir)
	{
		Coord result = new Coord(0,0,1);
		if (dir <= Direction.xneg)
			result = new Coord(1,0,0);
		else if (dir <= Direction.yneg)
			result = new Coord(0,1,0);
		
		if (!IsPosDirection(dir))
			result *= -1;
		
		return result;
	}
	
	public static Direction[] TheDirections()
	{
		return new Direction[] {Direction.xpos, Direction.xneg, Direction.ypos, Direction.yneg, Direction.zpos, Direction.zneg};
	}
	
	public static Direction[] TheDirectionsXZ()
	{
		return new Direction[] {Direction.xpos, Direction.xneg, Direction.zpos, Direction.zneg};
	}
	
	public static bool IsPosDirection(Direction dir)
	{
		return (int) dir % 2 == 0;	
	}
	
	public static PTwo[] SurroundingPTwoCoordsFromPTwo(PTwo coord)
	{
		PTwo[] surrounding = SurroundingNudgeCoordsFour();
		for (int i = 0; i < 4; ++i)
			surrounding[i] += coord;
		
		return surrounding;
	}
	
	public static PTwo[] SurroundingNudgeCoordsFour()
	{
		return new PTwo[] {
			new PTwo(-1, 0),
			new PTwo(1, 0),
			new PTwo(0, -1),
			new PTwo(0, 1)
		};
	}
}
