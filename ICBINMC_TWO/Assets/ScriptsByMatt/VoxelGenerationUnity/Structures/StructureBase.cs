using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StructureBase 
{
	// structures are not (necessarily) caves
	// sep class for cave structures.
	// doing a simple implementation
	// just a 2D array of lists of ranges
	
	protected List<Range1D>[,] ranges;
	public PTwo undividedDimensions; // plot without intersection
	public Quad plot;// dimensions;
//	public PTwo patchRelativeOrigin;
	public int y_origin;
	
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
		
		this.undividedDimensions = _patchRelativePlot.dimensions;
		_patchRelativePlot = Quad.Intersection(_patchRelativePlot, new Quad(new PTwo(0,0), noisePatchXZDims) ); 
		
		b.bug("calling set up SB in base class");
		if (_patchRelativePlot.isErsatzNull())
		{
			b.bug("was null");
			this.ranges = new List<Range1D>[0,0];
		}
		else 
		{	
			this.ranges = new List<Range1D>[_patchRelativePlot.dimensions.s, _patchRelativePlot.dimensions.t];	
		}
		
		this.plot = _patchRelativePlot;
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

public class Plinth : StructureBase
{
	public Plinth(PTwo _patchRelOrigin, int y_height, float seed)// : base(_patchRelativePlot)
	{
		Coord dims = new Coord(5, 7, 4); // fake
		
		Quad _prplot = new Quad(_patchRelOrigin, PTwo.PTwoXZFromCoord(dims) );
		
		setupSBB(_prplot, y_height);
		
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
	
	private void setupSBB(Quad _patchRelativePlot, int y_height)  // todo learn to inherit
	{
		this.y_origin = y_height;
		
		this.undividedDimensions = _patchRelativePlot.dimensions;
		//probable bug:
		// intersection to intersecting
		_patchRelativePlot = Quad.Intersection(_patchRelativePlot, new Quad(new PTwo(0,0), noisePatchXZDims) ); 
		
		if (_patchRelativePlot.isErsatzNull())
		{
			b.bug("was null in child class");
			this.ranges = new List<Range1D>[0,0];
		}
		else 
		{	
			this.ranges = new List<Range1D>[_patchRelativePlot.dimensions.s, _patchRelativePlot.dimensions.t];	
		}
		
		this.plot = _patchRelativePlot;
	}
	
}
