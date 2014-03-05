
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class LightColumnMap
{
	private static PTwo NoisePatchDims = new PTwo(NoisePatch.patchDimensions.x, NoisePatch.patchDimensions.z);
	private DiscreteDomainRangeList<LightColumn>[] m_lightColumns = new DiscreteDomainRangeList<LightColumn>[NoisePatch.patchDimensions.x*NoisePatch.patchDimensions.z];
//	private 

	public DiscreteDomainRangeList<LightColumn> this[PTwo patchRelPoint] 
	{
		get {
			return this[patchRelPoint.s,patchRelPoint.t ];
		} set  {
			this[patchRelPoint.s, patchRelPoint.t ] = value;
		}
	}
	
	public DiscreteDomainRangeList<LightColumn> this[int x, int z] 
	{
		get{
			//check out of bounds and ask a singleton that will make for the ranges
			if (!NoisePatchDims.isIndexSafe(new PTwo(x,z)))
			{
				return ChunkManager.lightColumnWorldMap.lightColumnsAtWoco(x,z); 
			}
			DiscreteDomainRangeList<LightColumn> lcolms = m_lightColumns[x * NoisePatch.patchDimensions.x + z];
			if (lcolms == null) {
				lcolms = new DiscreteDomainRangeList<LightColumn>();
				m_lightColumns[x * NoisePatch.patchDimensions.x + z] = lcolms;
			}
			return lcolms;
		}
		set {
			m_lightColumns[x * NoisePatch.patchDimensions.x + z ] = value;
		}
	}
	
	private DiscreteDomainRangeList<LightColumn> lightColumnListAtOrNull(int x, int z)
	{
		if (!NoisePatchDims.isIndexSafe(new PTwo(x,z)))
		{
			return ChunkManager.lightColumnWorldMap.lightColumnsAtWoco(x,z);
		}
		return m_lightColumns[x * NoisePatchDims.s + z];
	}
	
	public void addColumnAt(LightColumn newlcolm, int x, int z)
	{
		DiscreteDomainRangeList<LightColumn> lcolms = this[x,z];
		
		lcolms.Add(newlcolm);
		this[x,z] = lcolms;
	}
	
	public void addReplaceColumn(LightColumn col)
	{
		DiscreteDomainRangeList<LightColumn> cols = this[col.coord];
		cols.RemoveWithRangeEqualTo(col.range);
		
		cols.Add(col);
		this[col.coord] = cols;
	}
	
	public void clearColumnsAt(int x, int z)
	{
		DiscreteDomainRangeList<LightColumn> lcolms = this[x,z];
		
		lcolms.Clear();
		this[x,z] = lcolms;
	}
	
	public void removeColumnsAbove(int x, int y, int z)
	{
		DiscreteDomainRangeList<LightColumn> licoms = this[x,z];
		licoms.RemoveStartAbove(y);
		this[x,z] = licoms;
	}
	
	public int countAt(int x, int z)
	{
		DiscreteDomainRangeList<LightColumn> cols = this.lightColumnListAtOrNull(x,z);
		if (cols == null)
			return 0;
		
		return cols.Count;
	}
	
	
	public LightColumn columnContaining(int x, int y, int z)
	{
		DiscreteDomainRangeList<LightColumn> cols = this.lightColumnListAtOrNull(x,z); 
		if (cols == null)
			return null;
		
		return cols.rangeContaining(y);
	}
	
	public LightColumn columnContaining(Coord patchRelCo)
	{
		return columnContaining(patchRelCo.x,patchRelCo.y, patchRelCo.z);
	}
	
	public List<DiscreteDomainRangeList<LightColumn>> columnsWithin(Quad areaXZ) 
	{
		areaXZ = Quad.Intersection(areaXZ, new PTwo(NoisePatch.patchDimensions.x, NoisePatch.patchDimensions.z));
		
		int startIndex = areaXZ.origin.s * NoisePatchDims.s + areaXZ.origin.t;
		
		List<DiscreteDomainRangeList<LightColumn>> result = new List<DiscreteDomainRangeList<LightColumn>>();
		
		for(int i =0; i < areaXZ.dimensions.t ; ++i )
		{
			result.AddRange(m_lightColumns.Skip(startIndex).Take(areaXZ.dimensions.t));
			startIndex += NoisePatchDims.s;
		}
		
		return result;
	}
	
//	public List<DiscreteDomainRangeList<LightColumn>> columnsWithinBorderOfQuadInDirection(Quad areaXZ, Direction dir) 
//	{
//		return columnsOnBorderOfQuadInDirection(areaXZ, dir, true);
//	}
//	
//	public List<DiscreteDomainRangeList<LightColumn>> columnsOutsideBorderOfQuadInDirection(Quad areaXZ, Direction dir) 
//	{
//		return columnsOnBorderOfQuadInDirection(areaXZ, dir, false);
//	}
//	
//	private List<DiscreteDomainRangeList<LightColumn>> columnsOnBorderOfQuadInDirection(Quad areaXZ, Direction dir, bool wantWithin) 
//	{
//		SimpleRange xRange = areaXZ.sRange();
//		SimpleRange zRange = areaXZ.tRange();
//		SimpleRange longRange;
//		SimpleRange shortRange;
//		int shortRangeStart;
//		Axis axis = DirectionUtil.AxisForDirection(dir);
//		if (axis == Axis.X)
//		{
//			longRange = zRange;
//			shortRange = xRange;
//		} else {
//			longRange = xRange;
//			shortRange = zRange;
//		}
//		if (DirectionUtil.IsPosDirection(dir))
//		{
//			shortRangeStart = wantWithin ? shortRange.extentMinusOne() : shortRange.extent();
//		} else {
//			shortRangeStart = wantWithin ? shortRange.start : shortRange.start - 1;
//		}
//		shortRange = new SimpleRange(shortRangeStart, 1);
//		
//		Quad area = (axis == Axis.X) ? new Quad(new PTwo( shortRange.start, longRange.start), new PTwo(shortRange.range, longRange.range)) :
//			new Quad(new PTwo( longRange.start,shortRange.start), new PTwo(longRange.range, shortRange.range));
//		
//		return columnsWithin(new Quad(area));
//	}
	
	public List<LightColumn> lightColumnsAdjacentToAndFlushWith(LightColumn colm)
	{
		return lightColumnsAdjacentToAndFlushWithSimpleRangeAndPoint(colm.range, colm.coord);
	}
	
	public List<LightColumn> lightColumnsAdjacentToAndFlushWithSimpleRangeAndPoint(SimpleRange colmRange, PTwo coord)
	{
//		PTwo coord = colm.coord;
		List<LightColumn> result = new List<LightColumn>();
		
		foreach(PTwo surroundingCo in DirectionUtil.SurroundingPTwoCoordsFromPTwo(coord))
		{
			DiscreteDomainRangeList<LightColumn> adjRangeList = this.lightColumnListAtOrNull(surroundingCo.s, surroundingCo.t);
			if (adjRangeList == null)
				continue;
			for(int i = 0; i < adjRangeList.Count; ++i)
			{
				LightColumn adjLightColumn = adjRangeList[i];
				
				OverlapState overlap = colmRange.overlapStateWith(adjLightColumn.range);
				if (OverLapUtil.OverlapExists(overlap))
				{
					result.Add(adjLightColumn);
				}
			}
		}
		return result;
	}
	
	public List<LightColumn> lightColumnsAdjacentToAndAtleastPartiallyAbove(Coord coord)
	{
		List<LightColumn> result = new List<LightColumn>();

		foreach(PTwo surroundingCo in DirectionUtil.SurroundingPTwoCoordsFromPTwo(PTwo.PTwoXZFromCoord(coord)))
		{
			DiscreteDomainRangeList<LightColumn> adjRangeList = this.lightColumnListAtOrNull(surroundingCo.s, surroundingCo.t);
			if (adjRangeList == null)
				continue;
			for(int i = 0; i < adjRangeList.Count; ++i)
			{
				LightColumn adjLightColumn = adjRangeList[i];
				if (adjLightColumn.extent() > coord.y)
				{
					b.bug("actually added one");
					result.Add(adjLightColumn);
				}
			}
		}
		return result;
	}
}
