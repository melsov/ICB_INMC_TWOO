﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class CrudeTrig  
{
	private static float[] SinZeroTo360 = new float[] {
		0f, 0.01745241f, 0.0348995f, 0.05233596f, 0.06975647f, 0.08715574f, 0.1045285f, 0.1218693f, 0.1391731f, 0.1564345f
, 0.1736482f, 0.190809f, 0.2079117f, 0.224951f, 0.2419219f, 0.258819f, 0.2756374f, 0.2923717f, 0.309017f, 0.3255681f
, 0.3420201f, 0.3583679f, 0.3746066f, 0.3907311f, 0.4067366f, 0.4226183f, 0.4383712f, 0.4539905f, 0.4694716f, 0.4848096f
, 0.5f, 0.5150381f, 0.5299193f, 0.5446391f, 0.5591929f, 0.5735765f, 0.5877852f, 0.601815f, 0.6156615f, 0.6293204f
, 0.6427876f, 0.656059f, 0.6691306f, 0.6819984f, 0.6946584f, 0.7071068f, 0.7193398f, 0.7313537f, 0.7431449f, 0.7547095f
, 0.7660444f, 0.7771459f, 0.7880107f, 0.7986355f, 0.809017f, 0.8191521f, 0.8290375f, 0.8386706f, 0.8480481f, 0.8571673f
, 0.8660254f, 0.8746197f, 0.8829476f, 0.8910065f, 0.8987941f, 0.9063078f, 0.9135455f, 0.9205048f, 0.9271839f, 0.9335804f
, 0.9396926f, 0.9455186f, 0.9510565f, 0.9563047f, 0.9612617f, 0.9659258f, 0.9702957f, 0.9743701f, 0.9781476f, 0.9816272f
, 0.9848077f, 0.9876884f, 0.9902681f, 0.9925461f, 0.9945219f, 0.9961947f, 0.9975641f, 0.9986295f, 0.9993908f, 0.9998477f
, 1f, 0.9998477f, 0.9993908f, 0.9986295f, 0.9975641f, 0.9961947f, 0.9945219f, 0.9925461f, 0.9902681f, 0.9876884f
, 0.9848077f, 0.9816272f, 0.9781476f, 0.9743701f, 0.9702957f, 0.9659258f, 0.9612617f, 0.9563047f, 0.9510565f, 0.9455186f
, 0.9396926f, 0.9335805f, 0.9271839f, 0.9205049f, 0.9135455f, 0.9063078f, 0.8987941f, 0.8910066f, 0.8829476f, 0.8746197f
, 0.8660254f, 0.8571673f, 0.848048f, 0.8386706f, 0.8290376f, 0.819152f, 0.809017f, 0.7986355f, 0.7880108f, 0.777146f
, 0.7660444f, 0.7547096f, 0.7431448f, 0.7313537f, 0.7193399f, 0.7071068f, 0.6946585f, 0.6819983f, 0.6691306f, 0.656059f
, 0.6427876f, 0.6293205f, 0.6156614f, 0.6018151f, 0.5877852f, 0.5735765f, 0.559193f, 0.5446391f, 0.5299193f, 0.515038f
, 0.5000001f, 0.4848095f, 0.4694716f, 0.4539906f, 0.4383711f, 0.4226183f, 0.4067366f, 0.3907312f, 0.3746067f, 0.3583679f
, 0.3420202f, 0.3255681f, 0.309017f, 0.2923718f, 0.2756374f, 0.2588191f, 0.2419219f, 0.2249511f, 0.2079116f, 0.190809f
, 0.1736483f, 0.1564344f, 0.1391732f, 0.1218693f, 0.1045285f, 0.08715588f, 0.06975647f, 0.05233605f, 0.03489945f, 0.01745246f
, -8.742278E-08f, -0.01745239f, -0.03489939f, -0.05233599f, -0.0697564f, -0.08715581f, -0.1045284f, -0.1218692f, -0.1391731f, -0.1564344f
, -0.1736482f, -0.190809f, -0.2079118f, -0.224951f, -0.2419218f, -0.2588191f, -0.2756373f, -0.2923718f, -0.309017f, -0.3255681f
, -0.3420202f, -0.3583679f, -0.3746066f, -0.3907311f, -0.4067365f, -0.4226183f, -0.4383711f, -0.4539905f, -0.4694715f, -0.4848097f
, -0.5f, -0.515038f, -0.5299193f, -0.544639f, -0.559193f, -0.5735764f, -0.5877851f, -0.601815f, -0.6156614f, -0.6293204f
, -0.6427876f, -0.6560591f, -0.6691306f, -0.6819983f, -0.6946584f, -0.7071067f, -0.7193398f, -0.7313537f, -0.7431448f, -0.7547096f
, -0.7660446f, -0.777146f, -0.7880107f, -0.7986354f, -0.8090168f, -0.8191521f, -0.8290376f, -0.8386706f, -0.848048f, -0.8571672f
, -0.8660254f, -0.8746197f, -0.8829476f, -0.8910065f, -0.8987941f, -0.9063078f, -0.9135454f, -0.9205048f, -0.9271838f, -0.9335805f
, -0.9396927f, -0.9455186f, -0.9510565f, -0.9563047f, -0.9612617f, -0.9659259f, -0.9702957f, -0.9743701f, -0.9781476f, -0.9816272f
, -0.9848078f, -0.9876883f, -0.9902681f, -0.9925461f, -0.9945219f, -0.9961947f, -0.997564f, -0.9986295f, -0.9993908f, -0.9998477f
, -1f, -0.9998477f, -0.9993908f, -0.9986295f, -0.997564f, -0.9961947f, -0.9945219f, -0.9925462f, -0.9902681f, -0.9876883f
, -0.9848077f, -0.9816272f, -0.9781476f, -0.97437f, -0.9702957f, -0.9659258f, -0.9612617f, -0.9563048f, -0.9510565f, -0.9455186f
, -0.9396926f, -0.9335805f, -0.9271839f, -0.9205048f, -0.9135454f, -0.9063078f, -0.8987941f, -0.8910066f, -0.8829476f, -0.8746197f
, -0.8660254f, -0.8571674f, -0.848048f, -0.8386705f, -0.8290376f, -0.8191521f, -0.8090171f, -0.7986354f, -0.7880107f, -0.777146f
, -0.7660445f, -0.7547097f, -0.7431448f, -0.7313536f, -0.7193398f, -0.7071069f, -0.6946585f, -0.6819983f, -0.6691306f, -0.6560591f
, -0.6427878f, -0.6293206f, -0.6156614f, -0.601815f, -0.5877853f, -0.5735766f, -0.5591931f, -0.5446389f, -0.5299193f, -0.5150381f
, -0.5000002f, -0.4848095f, -0.4694715f, -0.4539905f, -0.4383712f, -0.4226184f, -0.4067365f, -0.3907311f, -0.3746066f, -0.3583681f
, -0.3420204f, -0.325568f, -0.3090169f, -0.2923717f, -0.2756375f, -0.2588193f, -0.2419218f, -0.224951f, -0.2079118f, -0.1908092f
, -0.1736484f, -0.1564344f, -0.1391731f, -0.1218694f, -0.1045287f, -0.08715603f, -0.06975638f, -0.05233596f, -0.0348996f, -0.01745261f
	};
	
	private static float[] CosZeroTo360 = new float[] {
		1f, 0.9998477f, 0.9993908f, 0.9986295f, 0.9975641f, 0.9961947f, 0.9945219f, 0.9925461f, 0.9902681f, 0.9876884f
, 0.9848077f, 0.9816272f, 0.9781476f, 0.9743701f, 0.9702957f, 0.9659258f, 0.9612617f, 0.9563048f, 0.9510565f, 0.9455186f
, 0.9396926f, 0.9335804f, 0.9271839f, 0.9205049f, 0.9135454f, 0.9063078f, 0.8987941f, 0.8910065f, 0.8829476f, 0.8746197f
, 0.8660254f, 0.8571673f, 0.8480481f, 0.8386706f, 0.8290376f, 0.8191521f, 0.809017f, 0.7986355f, 0.7880108f, 0.7771459f
, 0.7660444f, 0.7547096f, 0.7431448f, 0.7313537f, 0.7193398f, 0.7071068f, 0.6946584f, 0.6819984f, 0.6691306f, 0.656059f
, 0.6427876f, 0.6293204f, 0.6156615f, 0.601815f, 0.5877853f, 0.5735765f, 0.5591929f, 0.5446391f, 0.5299193f, 0.5150381f
, 0.5f, 0.4848096f, 0.4694716f, 0.4539905f, 0.4383712f, 0.4226182f, 0.4067366f, 0.3907312f, 0.3746066f, 0.358368f
, 0.3420202f, 0.3255681f, 0.309017f, 0.2923718f, 0.2756374f, 0.2588191f, 0.2419219f, 0.224951f, 0.2079117f, 0.1908091f
, 0.1736482f, 0.1564345f, 0.1391731f, 0.1218693f, 0.1045284f, 0.0871558f, 0.06975651f, 0.05233597f, 0.0348995f, 0.01745238f
, -4.371139E-08f, -0.01745235f, -0.03489946f, -0.05233594f, -0.06975648f, -0.08715577f, -0.1045285f, -0.1218693f, -0.1391731f, -0.1564344f
, -0.1736482f, -0.190809f, -0.2079116f, -0.224951f, -0.2419219f, -0.258819f, -0.2756374f, -0.2923717f, -0.3090169f, -0.3255681f
, -0.3420201f, -0.3583679f, -0.3746066f, -0.3907312f, -0.4067366f, -0.4226183f, -0.4383711f, -0.4539904f, -0.4694716f, -0.4848095f
, -0.5000001f, -0.515038f, -0.5299193f, -0.5446391f, -0.5591928f, -0.5735765f, -0.5877852f, -0.6018151f, -0.6156614f, -0.6293203f
, -0.6427876f, -0.656059f, -0.6691307f, -0.6819983f, -0.6946583f, -0.7071068f, -0.7193397f, -0.7313538f, -0.7431448f, -0.7547097f
, -0.7660444f, -0.7771459f, -0.7880108f, -0.7986355f, -0.8090171f, -0.8191521f, -0.8290375f, -0.8386706f, -0.848048f, -0.8571673f
, -0.8660254f, -0.8746198f, -0.8829476f, -0.8910065f, -0.8987941f, -0.9063078f, -0.9135455f, -0.9205049f, -0.9271838f, -0.9335805f
, -0.9396926f, -0.9455186f, -0.9510565f, -0.9563047f, -0.9612617f, -0.9659258f, -0.9702957f, -0.9743701f, -0.9781476f, -0.9816272f
, -0.9848077f, -0.9876884f, -0.9902681f, -0.9925461f, -0.9945219f, -0.9961947f, -0.9975641f, -0.9986295f, -0.9993908f, -0.9998477f
, -1f, -0.9998477f, -0.9993908f, -0.9986295f, -0.9975641f, -0.9961947f, -0.9945219f, -0.9925461f, -0.9902681f, -0.9876884f
, -0.9848077f, -0.9816272f, -0.9781476f, -0.9743701f, -0.9702957f, -0.9659258f, -0.9612617f, -0.9563047f, -0.9510565f, -0.9455186f
, -0.9396926f, -0.9335805f, -0.9271838f, -0.9205049f, -0.9135455f, -0.9063078f, -0.8987941f, -0.8910065f, -0.8829476f, -0.8746197f
, -0.8660254f, -0.8571674f, -0.8480481f, -0.8386706f, -0.8290375f, -0.8191521f, -0.8090171f, -0.7986355f, -0.7880108f, -0.7771459f
, -0.7660445f, -0.7547095f, -0.7431448f, -0.7313538f, -0.7193398f, -0.7071068f, -0.6946583f, -0.6819984f, -0.6691307f, -0.656059f
, -0.6427875f, -0.6293203f, -0.6156615f, -0.6018152f, -0.5877854f, -0.5735763f, -0.5591929f, -0.5446391f, -0.5299194f, -0.5150383f
, -0.4999999f, -0.4848096f, -0.4694716f, -0.4539907f, -0.438371f, -0.4226182f, -0.4067366f, -0.3907312f, -0.3746068f, -0.3583678f
, -0.3420201f, -0.3255682f, -0.3090171f, -0.2923719f, -0.2756372f, -0.258819f, -0.2419219f, -0.2249512f, -0.2079119f, -0.1908088f
, -0.1736481f, -0.1564345f, -0.1391733f, -0.1218696f, -0.1045283f, -0.08715571f, -0.06975655f, -0.05233612f, -0.03489976f, -0.0174523f
, 1.192488E-08f, 0.01745232f, 0.03489931f, 0.05233615f, 0.06975657f, 0.08715574f, 0.1045284f, 0.1218691f, 0.1391733f, 0.1564345f
, 0.1736481f, 0.1908089f, 0.2079115f, 0.2249512f, 0.2419219f, 0.258819f, 0.2756372f, 0.2923715f, 0.3090171f, 0.3255682f
, 0.3420201f, 0.3583678f, 0.3746064f, 0.3907312f, 0.4067367f, 0.4226182f, 0.438371f, 0.4539903f, 0.4694717f, 0.4848096f
, 0.4999999f, 0.5150379f, 0.5299194f, 0.5446391f, 0.5591929f, 0.5735763f, 0.5877851f, 0.6018152f, 0.6156615f, 0.6293204f
, 0.6427875f, 0.6560588f, 0.6691307f, 0.6819984f, 0.6946583f, 0.7071066f, 0.7193396f, 0.7313538f, 0.7431449f, 0.7547095f
, 0.7660443f, 0.7771458f, 0.7880108f, 0.7986355f, 0.8090169f, 0.8191519f, 0.8290374f, 0.8386706f, 0.8480481f, 0.8571672f
, 0.8660253f, 0.8746198f, 0.8829476f, 0.8910065f, 0.898794f, 0.9063077f, 0.9135455f, 0.9205049f, 0.9271839f, 0.9335804f
, 0.9396926f, 0.9455186f, 0.9510565f, 0.9563047f, 0.9612616f, 0.9659258f, 0.9702958f, 0.9743701f, 0.9781476f, 0.9816272f
, 0.9848077f, 0.9876884f, 0.9902681f, 0.9925461f, 0.9945219f, 0.9961947f, 0.9975641f, 0.9986295f, 0.9993908f, 0.9998477f	
	};
	
	private static int[] radiusOneSquareAnglesDegrees = new int[] {
		0, 45, 90, 135, 180, 225, 270, 315
	};
	
	private static int[] radiusTwoSquareAnglesDegrees = new int[] {
		0, 27, 45, 63, 90, 117, 135, 153, 180, 207, 225, 243, 270, 297, 315, 333
	};
	
	private static int[] radiusThreeSquareAnglesDegrees = new int[] {
		0, 18, 34, 45, 56, 72, 90, 108, 124, 135, 146, 162, 180, 198, 214, 225, 236, 252, 270, 288, 304, 315, 326, 342
	};
	
	
	private static int[] radiusFourSquareAnglesDegrees = new int[] {
		0, 14, 27, 37, 45, 53, 63, 76, 90, 104, 117, 127, 135, 143, 153, 166, 180, 194, 207, 
		217, 225, 233, 243, 256, 270, 284, 297, 307, 315, 323, 333, 346
	};
	
	
	private static int[] radiusFiveSquareAnglesDegrees = new int[] {
		0, 11, 22, 31, 39, 45, 51, 59, 68, 79, 90, 101, 112, 121, 129, 135, 141, 149, 158, 
		169, 180, 191, 202, 211, 219, 225, 231, 239, 248, 259, 270, 281, 292, 301, 309, 315, 321, 329, 338, 349
	};
	
	private static int[][] radiusSquareAngleArrays = new int[][] {
		radiusOneSquareAnglesDegrees,
		radiusTwoSquareAnglesDegrees,
		radiusThreeSquareAnglesDegrees,
		radiusFourSquareAnglesDegrees,
		radiusFiveSquareAnglesDegrees
	};
	
	public static List<int> SquareAnglesForRadiusAndAngle(int radius, int angle)
	{
		return SquareAnglesForRadiusAndAngle(radius, angle, radius);
	}
	
	public static List<int> SquareAnglesForFullPerimeter(int radius, int startinAngle)
	{
		return SquareAnglesForRadiusAndAngle(radius, startinAngle, radius * 4);
	}
	
	public static List<int> SquareAnglesForRadiusAndAngle(int radius, int angle, int halfConeWidth)
	{
		List<int> result = new List<int>();
		if (radius <= 0)
		{
			result.Add(0);
			return result;
		}
		
		angle = MassageDegrees(angle);
		
		int halfCone= halfConeWidth;
		
		if (radius <= radiusSquareAngleArrays.Length)
		{
			int[] angles = radiusSquareAngleArrays[radius - 1];
			//get approx ang
			int perimeterCount = radius * 8;
			int index = (int)((angle/360f) * perimeterCount);
			
			
			result.Add(angles[index]);
			
			for(int i = 1 ; i <= halfCone; ++i)
			{
				int plusIndex = index + i;
				plusIndex = plusIndex % angles.Length;
				result.Add(angles[plusIndex]);
				
				int mIndex = (index - i) % angles.Length;
				mIndex = mIndex < 0 ? angles.Length + mIndex : mIndex;
				result.Add(angles[mIndex]);
			}
			
			
			return result;
		}
		
		result.Add(0);
		return result;
	}
	
	//CONSIDER: could update to use only angle 0 to 180 
	// just keep track of sign when computing...
	
	public static float Sin(int angleDegrees)
	{
		return SinZeroTo360[MassageDegrees(angleDegrees)];
	}
	
	public static float Cos(int angleDegrees)
	{
		return CosZeroTo360[MassageDegrees(angleDegrees)];
	}
	
	private static int MassageDegrees(int angleDegrees)
	{
		angleDegrees = angleDegrees % 360;
		angleDegrees = angleDegrees < 0 ? angleDegrees + 360 : angleDegrees;
		
		AssertUtil.Assert(angleDegrees >= 0 && angleDegrees < 360, "math problem in curde trig Sin");
		return angleDegrees;
	}
	
	public static string AnglesForPrint()
	{
//		return "USE THIS FOR MAKING THE STATIC ARRAYS. make func public if needed";
		
		string result = "";
		
		List<int>  degrees = GetTanAnglesTest(3);
		
		foreach(int d in degrees)
		{
			result += ", " + d;
		}
		
		degrees.Clear();
		degrees = GetTanAnglesTest(4);
		
		result += "\n\n ";
		
		foreach(int d in degrees)
		{
			result += ", " + d;
		}
		
		degrees.Clear();
		degrees = GetTanAnglesTest(5);
		
		result += "\n\n ";
		
		foreach(int d in degrees)
		{
			result += ", " + d;
		}
		
//		for(int i=0; i < 360; ++i)
//		{
////			double asin = Mathf.Asin(SinZeroTo360[i]) * (360.0/(2*Mathf.PI));
////			result += " : " + i + ", " + asin;
//			
//			if (i % 10 == 0)
//			{
//				result += "\n";
//			}
//			float radians = (float)(i * (2* Mathf.PI/360.0f));
//			float sinVal = Mathf.Sin(radians);
//			result += ", " + sinVal + "f";
//			
//		}
		return result;
	}
	
	public static List<int> GetTanAnglesTest(int sqRadius)
	{
		int x = sqRadius;
		List<int> result = new List<int>();
		
		List<PTwo> tempCoords = new List<PTwo>();
		int z;
		for(z = 0; z < sqRadius; ++z)
		{
			tempCoords.Add(new PTwo(x, z));
		}
		z = sqRadius;
		for(x = sqRadius; x > 0; --x)
		{
			tempCoords.Add(new PTwo(x,z));
		}
		
		List<float> tempResult = new List<float>();
		foreach(PTwo co in tempCoords)
		{
			float div = float.MaxValue;
			if (co.x != 0)
			{
				div = (float)(co.z/(float)co.x);
			}
			float ang = Mathf.Atan(div);
			tempResult.Add(ang);
		}
		
		int addDegrees = 0;
		foreach(float radian in tempResult)
		{
			int deg = (int)(radian * (360f/(Mathf.PI * 2)) + .5f);
			result.Add(deg+addDegrees * 90);
		}
		
		addDegrees++;
		foreach(float radian in tempResult)
		{
			int deg = (int)(radian * (360f/(Mathf.PI * 2)) + .5f);
			result.Add(deg+addDegrees * 90);
		}
		
		addDegrees++;
		foreach(float radian in tempResult)
		{
			int deg = (int)(radian * (360f/(Mathf.PI * 2)) + .5f);
			result.Add(deg+addDegrees * 90);
		}
		
		addDegrees++;
		foreach(float radian in tempResult)
		{
			int deg = (int)(radian * (360f/(Mathf.PI * 2)) + .5f);
			result.Add(deg+addDegrees * 90);
		}
		return result;
		
	}
}