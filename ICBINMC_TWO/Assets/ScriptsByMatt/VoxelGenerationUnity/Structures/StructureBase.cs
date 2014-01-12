#define TREE_OFFSET_METHOD
//#define NO_RANDOMNESS

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class StructureBase 
{
	// structures are not (necessarily) caves
	// sep class for cave structures.
	// doing a simple implementation
	// just a 2D array of lists of ranges
	
	protected List<Range1D>[,] ranges;
	public PTwo undividedDimensions; // plot without intersection
	public Quad plot;// dimensions;
	public Quad groundLevelPlot;
//	public PTwo patchRelativeOrigin;
	public int y_origin;
//	public uint precedence; // decides whic structures appear in the case of an overlap
#if TREE_OFFSET_METHOD	
	protected PTwo constructOffsetStart;
	protected PTwo constructOffsetEnd;
#endif
	
	protected static PTwo noisePatchXZDims = new PTwo(NoisePatch.patchDimensions.x, NoisePatch.patchDimensions.z);
	
	public StructureBase() {}
	
	public StructureBase(PTwo _dims, PTwo _patchRelativeOrigin, int y_height) : this(new Quad(_patchRelativeOrigin, _dims), y_height)
	{
		
	}
	
	public StructureBase(Quad _patchRelativePlot, int y_height) 
	{
		setupSB(_patchRelativePlot, y_height);
	}
	
	protected virtual void setupSB(Quad _patchRelativePlot, int y_height) 
	{
		this.y_origin = y_height;
		
#if TREE_OFFSET_METHOD
		PTwo offset = PTwo.Min(_patchRelativePlot.origin, new PTwo(0,0) ); // negative origin should contribute to the offset
		this.constructOffsetStart =	PTwo.Abs(offset);
#endif
		
		this.undividedDimensions = _patchRelativePlot.dimensions;
		_patchRelativePlot = Quad.Intersection(_patchRelativePlot, new Quad(new PTwo(0,0), noisePatchXZDims) ); 
		
#if TREE_OFFSET_METHOD
		this.constructOffsetEnd = this.undividedDimensions - _patchRelativePlot.dimensions; //can't be negative
		if (this.constructOffsetEnd.hasANegativeElement())
			throw new Exception("confusing. undivided dimensions was smaller than patch rel plot dimensions?");
		this.constructOffsetEnd = this.constructOffsetEnd - this.constructOffsetStart; // we don't want to offset dims on the end and the start (basically)
#endif
		
		if (_patchRelativePlot.isErsatzNull())
		{
			throw new Exception ("Structure plot was null was null. patch rel plot: " + _patchRelativePlot.toString() + "construct offset: " + constructOffsetStart.toString() );
			this.ranges = new List<Range1D>[0,0];
		}
		else 
		{	
			this.ranges = new List<Range1D>[_patchRelativePlot.dimensions.s, _patchRelativePlot.dimensions.t];	
		}
		
		this.plot = _patchRelativePlot;
		this.groundLevelPlot = this.plot;
	}
	
	public List<Range1D> this[PTwo pt]
	{
		get {
			if (ranges[pt.s, pt.t] == null)
				ranges[pt.s, pt.t] = new List<Range1D>();
			return ranges[pt.s, pt.t];	
		}
		set {
			ranges[pt.s, pt.t] = value;
		}
	}
	
	public PTwo getOrigin () {
		return plot.origin;
	}
	
	public PTwo getExtent() {
		return plot.extent();	
	}
	
	public PTwo getDimensions() {
		return plot.dimensions;	
	}
	
	public bool wasNotTruncated() {
		return PTwo.Equal(this.undividedDimensions, this.plot.dimensions);	
	}
	
	public bool tDimensionWasTruncated() {
		return this.undividedDimensions.t > this.plot.dimensions.t;	
	}
	
	public bool sDimensionWasTruncated() {
		return this.undividedDimensions.s > this.plot.dimensions.s;	
	}
}

public class Tree : StructureBase
{
	public Tree(PTwo _patchRelOrigin, int y_height, float seed)
	{
		int structure_height = 8;
		structure_height += (int) (Utils.DecimatLeftShift(seed, 1) * structure_height * .33f);
		
		int length = 6;
		length +=(int)(Utils.DecimatLeftShift(seed,3) * length * .33f);
		if (length % 2 == 0)
			length++;
		
		Coord dimensions = new Coord(length, structure_height, length); // TEST
		
		int rel_trunk_coord = length/2;
		PTwo center_coord = new PTwo(rel_trunk_coord, rel_trunk_coord);
		
		int trunk_height = 3 + (int) (1 * Utils.DecimatLeftShift(seed, 5) );
		
#if NO_RANDOMNESS
		structure_height = 8;
		length = 6;
		trunk_height = 3;
		dimensions = new Coord(length * 0 + 2, structure_height, length); // TEST
		center_coord = new PTwo(rel_trunk_coord * 0 + 1, rel_trunk_coord);
#endif
		
		

#if TREE_OFFSET_METHOD
		//shift the origin to put the trunk at the coord that noise patch chose
		_patchRelOrigin = _patchRelOrigin - center_coord; // no, need a different way?
#endif
		
		
		
		Quad _patchRelPlot = new Quad(_patchRelOrigin, PTwo.PTwoXZFromCoord(dimensions) );
		
		setupSB(_patchRelPlot, y_height);
		
		//ground level plot
		this.groundLevelPlot = Quad.UnitQuadWithPoint(center_coord); 
		
//		if (this.plot.dimensions.isIndexSafe(center_coord))
//		{
//			Range1D trunk_range = new Range1D(0, structure_height - 1, BlockType.TreeTrunk); // change later to wood
//			Range1D top_leaves = new Range1D(structure_height - 1, 1, BlockType.TreeLeaves); // change later
//			List<Range1D> center_list = new List<Range1D>() {trunk_range, top_leaves};
//			ranges[center_coord.s , center_coord.t ] = center_list;
//		}
		
		int number_of_tapers = 3;
		int foliage_base_height = structure_height - trunk_height - number_of_tapers; 
		
		
		PTwo relation_to_center;
		PTwo cur_coord;
		
		int i = 0; int j = 0;
		int i_end = this.plot.dimensions.s;
		int j_end = this.plot.dimensions.t;
#if TREE_OFFSET_METHOD
		i = this.constructOffsetStart.s;
		
		i_end = this.undividedDimensions.s - this.constructOffsetEnd.s;
		j_end = this.undividedDimensions.t - this.constructOffsetEnd.t;
		
		
#endif
		
		
		for (; i < i_end; ++i)
		{
			j = 0;
#if TREE_OFFSET_METHOD
			j = this.constructOffsetStart.t;		
#endif
			for (; j < j_end; ++j)
			{
				if (i == center_coord.s && j == center_coord.t) //center, already done	
				{
					Range1D trunk_range = new Range1D(0, structure_height - 1, BlockType.TreeTrunk); // change later to wood
					Range1D top_leaves = new Range1D(structure_height - 1, 1, BlockType.TreeLeaves); // change later
					List<Range1D> center_list = new List<Range1D>() {trunk_range, top_leaves};
					ranges[center_coord.s - constructOffsetStart.s , center_coord.t - constructOffsetStart.t ] = center_list;
					continue;
				}
				
				cur_coord = new PTwo(i,j);
				relation_to_center = center_coord - PTwo.Abs(cur_coord - center_coord);
				
				if (relation_to_center.s + relation_to_center.t <= 1) //corners
					continue;
				
				int foliage_height = foliage_base_height + Mathf.Min ( number_of_tapers,  PTwo.LesserDimension(relation_to_center));
				int foliage_start = trunk_height;
				
				if (PTwo.LesserDimension(relation_to_center) < 1) {
					foliage_start++;
					foliage_height--;
				}
				
				List<Range1D> ij_ranges = new List<Range1D>() {new Range1D(foliage_start, foliage_height, BlockType.TreeLeaves)};
				
#if TREE_OFFSET_METHOD
				ranges[i - constructOffsetStart.s, j - constructOffsetStart.t] = ij_ranges;
#else
				ranges[i,j] = ij_ranges;
#endif
			}
		}
		
	}
}

public class Plinth : StructureBase
{
	public Plinth(PTwo _patchRelOrigin, int y_height, float seed)// : base(_patchRelativePlot)
	{
		Coord dims = new Coord(5, 7, 4); // fake
		
		Quad _prplot = new Quad(_patchRelOrigin, PTwo.PTwoXZFromCoord(dims) );
		
//		setupSBB(_prplot, y_height);
		setupSB(_prplot, y_height);
		
		int i = 0;
		int j = 0;
		
		for (; i < this.plot.dimensions.s; ++i)
		{
			for (; j < this.plot.dimensions.t; ++j)
			{
				Range1D column = new Range1D(0, dims.y, BlockType.Sand);
				List<Range1D> rangs = new List<Range1D>();
				rangs.Add(column);
				ranges[i,j] = rangs; // TODO: convert to range table
			}
		}
	}
	
//	private void setupSBB(Quad _patchRelativePlot, int y_height)  // todo learn to inherit
//	{
//		this.y_origin = y_height;
//		
//		this.undividedDimensions = _patchRelativePlot.dimensions;
//		
//		_patchRelativePlot = Quad.Intersection(_patchRelativePlot, new Quad(new PTwo(0,0), noisePatchXZDims) ); 
//		
//		if (_patchRelativePlot.isErsatzNull())
//		{
//			b.bug("was null in child class");
//			this.ranges = new List<Range1D>[0,0];
//		}
//		else 
//		{	
//			this.ranges = new List<Range1D>[_patchRelativePlot.dimensions.s, _patchRelativePlot.dimensions.t];	
//		}
//		
//		this.plot = _patchRelativePlot;
//	}
	
}
