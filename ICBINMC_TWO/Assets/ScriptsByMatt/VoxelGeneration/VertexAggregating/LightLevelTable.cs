using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class LightLevelTable
{
//	private byte[,] levels = new byte[FaceSet.MAX_DIMENSIONS.s, FaceSet.MAX_DIMENSIONS.t];
//	private Dictionary<PTwo, byte> levelLookup = new Dictionary<PTwo, byte>();
	
	private List<List<byte>> lights = new List<List<byte>>();
	private Quad domain = new Quad(new PTwo(-1,-1), new PTwo(0,0) );

	private Color32 result_cache;
	
	private bool calculatedAlready = false;
	
	public LightLevelTable() {

	}
	
	public byte this[int s, int t]
	{
		set {
			this[new PTwo(s,t)] = value;
		}
	}
	
	public byte this[PTwo ptwo]
	{
		set {
			if (domain.dimensions.s == 0) // first value test hack
			{
				domain = Quad.UnitQuadWithPoint(ptwo);
				lights.Add(new List<byte>());
				lights[0].Add(value);
			} else {
				Range1D s_range = domain.sRange();
				
				int index = ptwo.s - s_range.start;
				while(index < 0) {
					lights.Insert(0, new List<byte>());
					index++;
					calculatedAlready = false;
				}
				while(index >= s_range.range) {
					lights.Add(new List<byte>());
					index--;
					calculatedAlready = false;
				}
				
				int t_index = ptwo.t - domain.origin.t;
				
				while(t_index < 0) {
					lights[index].Insert(0, 0);
					t_index++;
					calculatedAlready = false;
				}
				
				int count = lights[index].Count();
				while(t_index >= count) {
					lights[index].Add(0);	
					t_index--;
					calculatedAlready = false;
				}
				
				int prev_value = 0;
				try {
					prev_value = lights[index][t_index];
				} catch(ArgumentOutOfRangeException e) {
					throw new ArgumentOutOfRangeException("arg was out of range: our \n s_range was: " + s_range.toString() + " ###\n index was: " + index + " BTW: t_index: " + t_index + " ###\n the domain: " + domain.toString());
				}
				lights[index][t_index] = value;
				
				if (prev_value != value) {
					calculatedAlready = false;	
				}
				
				domain = domain.expandedToContainPoint(ptwo);
				
				if (PTwo.GreaterDimension( domain.dimensions) > 4) {
					throw new Exception("extended domain beyond four in light lev table. the quad: " + domain.toString());	
				}
			}
			
//			if (levelLookup.ContainsKey(ptwo))
//			{
//				byte prevValue = levelLookup[ptwo];
//				
//				if (value != prevValue)
//				{
//					calculatedAlready = false;
	//				if (s >= levels.GetLength(0))
	//					upperHalfCalculatedAlready = false; //TODO: update these flags
	//				else
	//					lowerHalfCalculatedAlready = false;
//				}
//			}
//			levelLookup[ptwo] = value;
		}
	}
	
//	public int getLowerHalfBits() 
//	{
//		if (!lowerHalfCalculatedAlready) {
//			lowerHalfLightBits = getLevelForHalfOfTable(true);	
//			lowerHalfCalculatedAlready = true;
//		}
//		return 65535; // lowerHalfLightBits;
//	}
//	
//	//GENERAL TODO: adjust int collections to smaller types where feasible
//	
//	public int getUpperHalfBits()  //output should be within ushort max BTW
//	{
//		if (!upperHalfCalculatedAlready) {
//			upperHalfLightBits = getLevelForHalfOfTable(false);	
//			upperHalfCalculatedAlready = true;
//		}
//		return upperHalfLightBits;
//	}
//	
//	private int getLevelForHalfOfTable(bool wantLowerHalf) 
//	{
//	}
	
	
	// TODO: skip the array 2d just use dictionary when compiling...	
//	private void setupLightLevelArray() 
//	{
//		int end_s = origin.s + FaceSet.MAX_DIMENSIONS.s;
//		int end_t = origin.t + FaceSet.MAX_DIMENSIONS.t;
//		
//		int s = origin.s, t = 0;
//		
//		
//		
//		foreach(KeyValuePair<PTwo, byte> item in levelLookup)
//		{
//			PTwo index = item.Key - origin;
//			b.bug("key is: " + item.Key.toString() );
//			if (FaceSet.MAX_DIMENSIONS.isIndexSafe(index) && PTwo.GreaterThanOrEqual(index, new PTwo(0,0))) //doesn't help??
//				levels[index.s, index.t] = item.Value;
//			else {
//				
//				b.bug("num items in dictionary level lookup: " + levelLookup.Count() );
//				
//				throw new Exception("dictionary key was too far from origin. key was: " + index.toString() + " d origin: " + origin.toString() + "test domain: " + domain.toString() );
//			}
//		}
//	}
	
	public Color32 color32ForLightLevels(int origin_t_offset)
	{
		if (calculatedAlready)
			return result_cache;
		
//		setupLightLevelArray();
		
		byte t0 = getLevelForStripAtSDimension(0, origin_t_offset);
		byte t1 = getLevelForStripAtSDimension(1, origin_t_offset);
		byte t2 = getLevelForStripAtSDimension(2, origin_t_offset);
		byte t3 = getLevelForStripAtSDimension(3, origin_t_offset);
		
		result_cache = new Color32(t0,t1,t2,t3); // new byte[] {t0,t1,t2,t3};
		
		calculatedAlready = true;
		
		return result_cache;
	}
	
	private byte getLevelForStripAtSDimension(int s_dimension, int t_offset) 
	{
		int s = s_dimension;
		
		List<byte> lightLevels;
		if (s_dimension < lights.Count()) {
			lightLevels = lights[s_dimension];
		} else {
			return 0;	
		}
		
		int t_length = lightLevels.Count();
		byte result = 0;
		
		int t = 0;
		for (t = 0 ; t < t_length ; ++t)
		{
//				int index = (s - s_start) * t_length + t;
			byte index = (byte)((t + t_offset) % FaceSet.MAX_DIMENSIONS.t * 2);
			byte lightlev = lightLevels[t];
			byte shift = 0;
			
			if (lightlev > 0) { // light levels between 0 and 3 inclusive
				if (lightlev >= 2) {
					shift |= (byte)(1 << (index + 1));
				}
				if (lightlev % 2 == 1) {
					shift |= (byte)(1 << index);	
				}
			}
			result |= shift;
		}
		
		return result;
	}
	
	
//	private byte getLevelForStripAtSDimension(int s_dimension, int t_offset) 
//	{
//		int s = s_dimension;
//		int t_length = levels.GetLength(1);
//		byte result = 0;
//		
//		int t = 0;
//		for (t = 0 ; t < t_length ; ++t)
//		{
////				int index = (s - s_start) * t_length + t;
//			byte index = (byte)((t + t_offset) % FaceSet.MAX_DIMENSIONS.t * 2);
//			byte lightlev = levels[s,t];
//			byte shift = 0;
//			
//			if (lightlev > 0) { // light levels between 0 and 3 inclusive
//				if (lightlev >= 2) {
//					shift |= (byte)(1 << (index + 1));
//				}
//				if (lightlev % 2 == 1) {
//					shift |= (byte)(1 << index);	
//				}
//			}
//			result |= shift;
//		}
//		
//		return result;
//	}
//	
//	private void populateColor32FromDictionary()
//	{
//		byte[] temp = new byte[4];
//		
//		
//	}
	
	public void setupTestValues() 
	{
//		int t = 0;
//		for (int s = 0; s < levels.GetLength(0) ; ++s) 
//		{
//			for (t = 0 ; t < levels.GetLength(1) ; ++t)
//			{
//				levelLookup[new PTwo(s,t)] = (byte) ((t % 2 == 0) ? 1 : 3);
//			}
//		}
	}
	
}
