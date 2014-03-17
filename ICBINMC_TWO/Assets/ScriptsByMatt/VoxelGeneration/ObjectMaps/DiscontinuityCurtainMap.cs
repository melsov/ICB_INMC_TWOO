using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//TODO: discontinuity set...

public struct DiscontinuityNode
{
	public Range1D adjacentContinuity;
	public Range1D discontinuityAirRange;
	public bool adjacencyUnknown;
	
	public DiscontinuityNode(Range1D adjacentContinuity_, Range1D airRange_, bool adjacencyUnknown_) 
	{
		adjacentContinuity = adjacentContinuity_;
		discontinuityAirRange = airRange_;
		adjacencyUnknown = adjacencyUnknown_;
	}
	
	public DiscontinuityNode(Range1D adjacentContinuity_, Range1D airRange_) {
		this = new DiscontinuityNode(adjacentContinuity_, airRange_, false);
	}
	
	public DiscontinuityNode( Range1D airRange_) {
		this = new DiscontinuityNode(Range1D.theErsatzNullRange(), airRange_, true);	
	}
}

public struct DiscontinuityDescription
{
	DiscontinuityNode startNode;
	DiscontinuityNode endNode;
	SimpleRange zRange;
	int xCoord;
	
	public DiscontinuityDescription(DiscontinuityNode start, DiscontinuityNode end, SimpleRange _zrange, int woco_x) {
		startNode = start; endNode = end; zRange = _zrange; xCoord = woco_x;	
	}
}



public class DiscontinuityCurtainMap 
{
	private static readonly DiscontinuityCurtainMap dcm_instance = new DiscontinuityCurtainMap();
	
	private DiscontinuityCurtainMap(){}
	
	private List<DiscontinuityCurtain> curtains = new List<DiscontinuityCurtain>();
	
//	private Dictionary<int, bool>
	private Dictionary<NoiseCoord, List<int> > curtainLookup 
	{
		get {	
			if (curtainLookup == null)
				curtainLookup = new Dictionary<NoiseCoord, List<int>>();
			return curtainLookup;
		}
		set {
			curtainLookup = value;
		}
	}
	
	//singleton
	public static DiscontinuityCurtainMap Instance
	{
		get 
		{
			return dcm_instance; 
		}
	}
	
	public void startTrackingDiscontinuityWithRanges(List<Range1D> heightRangesAtCoord, PTwo patchRelCoord, NoiseCoord noiseCoord)
	{
		//check whether there's discontinuity here
	}
	
	// MUST MAKE JOBS SIMPLER
	public void addToMapWithDiscontinuityPair(DiscontinuityDescription discontinuityDescription)
	{
//		bool shouldAddNew = true;
//		
//		PTwo disPoint = discontinuityPair.discontinuityXZCo;
//		
//		// try to add to a current dcurtain
//		NoiseCoord containingNoiseCoord = noiseCoordForWocoXZ(disPoint);
//		List<int> curtain_indices = this.curtainLookup[containingNoiseCoord ];
//		
//		DiscontinuityCurtain dcurtain = null;
//		bool foundOne = false;
//		foreach(int index in curtain_indices) 
//		{
//			dcurtain = curtains[index];
//			
//			XDiscontinuityPointCoverageStatus xcoverage_status = dcurtain.xCoverageStatusForPoint(disPoint);
//			
//			if (xcoverage_status <= XDiscontinuityPointCoverageStatus.BeyondBoundsXPos) // no chance with this one
//			{
//				continue;
//			}
//			
//			if (xcoverage_status == XDiscontinuityPointCoverageStatus.XAdjacentToLowerLimit)
//				disPoint.s++;
//			else if (xcoverage_status == XDiscontinuityPointCoverageStatus.XAdjacentToUpperLimit)
//				disPoint.s--;
//			
//			ZDiscontinuityPointCoverageStatus zcoverage_status = dcurtain.zCoverageStatusForPoint(disPoint);
//			
//			if (discontinuityPair.adjacencyUnknown)
//			{
//				if (discontinuityPair.adjacentContinuityIsZPosToAir) //looking for a curtain coverage just z pos to air
//				{
//					if (zcoverage_status == ZDiscontinuityPointCoverageStatus.ZAdjacentToStart) //OK we can add...here
//					{
//						
//					}
//				}
//			}
//			
//		}

	}
	
	private List<int> curtainIndicesForWocoXZ(PTwo wocoPointXZ)
	{
		return this.curtainLookup[noiseCoordForWocoXZ(wocoPointXZ)];
	}
	
	private NoiseCoord noiseCoordForWocoXZ(PTwo wocoPointXZ)
	{
		return BlockCollection.noiseCoordForWorldCoord(PTwo.CoordFromPTwoWithY(wocoPointXZ, 0));
	}
	
	
//	public 
}
