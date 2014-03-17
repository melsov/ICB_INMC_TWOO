using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RangeTable 
{
	//class acts like a 2D array
	// but is really a list with a lookup table that's implemented with a 2D array
	
	// maybe this is a structure table???
	
	private List<Range1D> ranges = new List<Range1D>();
	
	private int[,] indexLookup;
	private PTwo dimensions;
//	private const int SPECIAL_LOOKUP_SHIFT = 100*100;
	
	public RangeTable(PTwo _dims) 
	{
		indexLookup = new int[_dims.s, _dims.t];
		dimensions = _dims;
	}
	
	//scotch for now!
	
//	public List<Range1D> this[PTwo pt]
//	{
//		get {
//				
//		}
//		set {
//			
//		}
//	}
//	
//	private List<Range1D> getRangesAt(PTwo pt)
//	{
//		
//	}
//	
//	private void setRangesAt(PTwo pt)
//	{
//		
//	}
//	
//	private void addRangesAt(PTwo pt)
//	{
//			
//	}
}
