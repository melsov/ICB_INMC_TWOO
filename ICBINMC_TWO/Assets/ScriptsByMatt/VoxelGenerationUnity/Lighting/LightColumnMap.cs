
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class LightColumnMap
{
	private static PTwo NoisePatchDims = new PTwo(NoisePatch.patchDimensions.x, NoisePatch.patchDimensions.z);
	private DiscreteDomainRangeList<LightColumn>[] m_lightColumns = new DiscreteDomainRangeList<LightColumn>[NoisePatch.patchDimensions.x*NoisePatch.patchDimensions.z];
	
	public List<LightColumn> this[PTwo patchRelPoint] 
	{
		get {
			return this[patchRelPoint.s,patchRelPoint.t ];
		}
		set  {
			this[patchRelPoint.s, patchRelPoint.t ] = value;
		}
	}
	
	public List<LightColumn> this[int x, int z] 
	{
		get{
			List<LightColumn> lcolms = m_lightColumns[x * NoisePatch.patchDimensions.x + z];
			if (lcolms == null) {
				lcolms = new List<LightColumn>();
				m_lightColumns[x * NoisePatch.patchDimensions.x + z] = lcolms;
			}
			return lcolms;
		}
		set {
			m_lightColumns[x * NoisePatch.patchDimensions.x + z ] = value;
		}
	}
	
	public void addColumnAt(LightColumn newlcolm, int x, int z)
	{
		List<LightColumn> lcolms = this[x,z];
		
		int i = 0;
//		LightColumn justBelow = null;
		foreach(LightColumn lcol in lcolms)
		{
			if (newlcolm.range.start > lcol.range.start)
			{
//				justBelow = lcol;
				if (SimpleRange.RangesIntersect(newlcolm.range, lcol.range)) {
					throw new Exception("problems. Light col ranges intersect.");
				}
				break;
			}
			++i;
		}
		lcolms.Insert(i, newlcolm);
		this[x,z] = lcolms;
	}
	
	public LightColumn columnContaining(int x, int y, int z)
	{
		List<LightColumn> cols = this[x,z];
		LightColumn result = null;
		if (cols == null)
			return result;
		
		foreach(LightColumn col in cols)
		{
			if (col.range.start <= y)
			{
				if (col.range.contains(y))
				{
					result = col;
				}
				break;
			}
			
			
		}
		
		return result;
	}
	
	public LightColumn columnContaining(Coord patchRelCo)
	{
		return columnContaining(patchRelCo.x,patchRelCo.y, patchRelCo.z);
	}
	
	public List<List<LightColumn>> columnsWithin(Quad areaXZ) 
	{
		areaXZ = Quad.Intersection(areaXZ, new PTwo(NoisePatch.patchDimensions.x, NoisePatch.patchDimensions.z));
		
		int startIndex = areaXZ.origin.s * NoisePatchDims.s + areaXZ.origin.t;
		
		List<List<LightColumn>> result = new List<List<LightColumn>>();
		
		for(int i =0; i < areaXZ.dimensions.t ; ++i )
		{
			result.AddRange(m_lightColumns.Skip(startIndex).Take(areaXZ.dimensions.t));
			startIndex += NoisePatchDims.s;
		}
		
		return result;
	}
	
	public List<LightColumn>[] columnsSurroundingPerimeterOf(Quad areaXZ) 
	{
		return null;
	}
}
