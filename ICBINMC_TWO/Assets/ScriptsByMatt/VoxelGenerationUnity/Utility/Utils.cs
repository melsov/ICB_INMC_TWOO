using UnityEngine;
using System.Collections;

//public class Utilities 
//{
//	
//}

public static class Utils
{
	
	public static float DecimatLeftShift(float dec, int places) 
	{
		float result = dec * Mathf.Pow(10f, (float) places);
		return result - Mathf.Floor(result);
	}
	
	public static int IntegerAtDecimalPlace(float dec, int place)
	{
		return (int) (10 * Utils.DecimatLeftShift(dec, place));	
	}

}
