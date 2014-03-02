using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightColumn  
{
	private List<LightColumn> m_InfluencedColumns = new List<LightColumn>();
	public byte lightLevel;
	public SimpleRange range;
	public PTwo coord;
	public bool needsUpdate = true;
	
	public List<LightColumn> influencedColumns
	{
		get {
			return m_InfluencedColumns;
		}
	}
	
	public Coord baseCoord{
		get {
			return new Coord(coord.s, range.start, coord.t);	
		}
	}
	
	public LightColumn(PTwo _coord, byte _lightLevel)
	{
		lightLevel = _lightLevel;
		coord = _coord;
	}
	
	
}
