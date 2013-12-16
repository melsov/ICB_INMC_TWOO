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



public struct BiomeTypeCorners
{
	public BiomeType lowerLeft;
	public BiomeType lowerRight;
	public BiomeType upperLeft;
	public BiomeType upperRight;
	
	public BiomeTypeCorners(BiomeType ll, BiomeType lr, BiomeType ul, BiomeType ur) 
	{
		lowerLeft = ll;
		lowerRight = lr; 
		upperLeft = ul;
		upperRight = ur;
	}
}

public struct BiomeInputs
{
	public float hilliness;
	public float cragginess;
	public float overhangness;
	public float caveVerticalFrequency;
	
	public static BiomeInputs Pasture()
	{
		BiomeInputs inputs = new BiomeInputs();
		//pasture
		inputs.hilliness = .2f;
		inputs.cragginess = 3.2f;
		inputs.overhangness = .03f;	
		inputs.caveVerticalFrequency = 1.5f;
		
		return inputs;
	}
	
	public static BiomeInputs CraggyMountains()
	{
		BiomeInputs inputs = new BiomeInputs();
		
		//cragginess
		inputs.hilliness = .4f;
		inputs.cragginess = 3.2f;
		inputs.overhangness = .5f;
		inputs.caveVerticalFrequency = 1.4f;
		
		return inputs;
	}
	
	public static BiomeInputs Mix(BiomeInputs aBI, BiomeInputs bBi, float aWeight, float bWeight) {
		BiomeInputs inputs = new BiomeInputs();

		aBI = BiomeInputs.Mult(aBI, aWeight);
		bBi = BiomeInputs.Mult(bBi, bWeight);
		
		return BiomeInputs.Add(aBI, bBi);
	}
	
	public static BiomeInputs Mult(BiomeInputs abi, float k) {
		abi.hilliness *= k;	
		abi.cragginess *= k;
		abi.overhangness *= k;
		abi.caveVerticalFrequency *= k;
		return abi;
	}
	
	public static BiomeInputs Add(BiomeInputs abi, BiomeInputs bbi) {
		abi.hilliness += bbi.hilliness;
		abi.cragginess += bbi.cragginess;
		abi.overhangness += bbi.overhangness;
		abi.caveVerticalFrequency += bbi.caveVerticalFrequency;
		return abi;
	}
	
}
