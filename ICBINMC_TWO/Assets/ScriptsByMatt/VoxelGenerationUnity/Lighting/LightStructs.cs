using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public struct Trapezoid
{
	public SimpleRange startRange;
	public SimpleRange endRange;
	public SimpleRange span;
	
	public Trapezoid(SimpleRange _startRange, SimpleRange _endRange, SimpleRange _span)
	{
		startRange = _startRange; endRange = _endRange; span = _span;
	}
	
	public Trapezoid(SimpleRange _startRange, int startZ)
	{
		this = new Trapezoid(_startRange, _startRange, new SimpleRange(startZ, 1));
	}
	
	public PTwo upperNegCorner()
	{
		return new PTwo(this.startRange.extent(), this.span.start);	
	}
	
	public PTwo lowerNegCorner()
	{
		return new PTwo(this.startRange.start, this.span.start);	
	}
	
	public PTwo upperPosCorner()
	{
		return new PTwo(this.endRange.extent(), this.span.extent());	
	}
	
	public PTwo lowerPosCorner()
	{
		return new PTwo(this.endRange.start, this.span.extent());	
	}
	
	public PTwo midPoint()
	{
		int hmid = span.midPoint();
		int vmid =(int)( (startRange.midPoint() + endRange.midPoint())/2f + .5f);
		return new PTwo(hmid, vmid);
	}
	
	public PTwo[] clockwisePoints() {
		return new PTwo[] {this.lowerNegCorner(), this.upperNegCorner(), this.upperPosCorner(), this.lowerPosCorner() };	
	}
	
	public bool contains(PTwo point) {
		if (!span.contains(point.s))
			return false;
		
		if (startRange.contains(point.t))
		{
			if (endRange.contains(point.t))
				return true;
			else {
				// TODO: fill in better math
				int distToStart = point.s - span.start;
				return (distToStart < (span.range - distToStart));
			}
		} else {
			if (endRange.contains(point.t))
			{
				int distToStart = point.s - span.start;
				return (distToStart > (span.range - distToStart));	
			}
		}
		
		return false;
	}
}

public struct TrapLight
{
	public float upperNeg;
	public float lowerNeg;
	public float upperPos;
	public float lowerPos;
	
	public TrapLight(float un, float ln, float up, float lp) 
	{
		upperNeg = un; lowerNeg = ln; upperPos = up; lowerPos = lp;	
	}
	
	public static TrapLight MediumLightQuad() 
	{
		float level = Window.LIGHT_LEVEL_MAX * .5f;
		return new TrapLight(level,level,level,level);
	}
	
	public float[] negValues()
	{
		return new float[] {lowerNeg, upperNeg};	
	}
	
	public float[] posValues()
	{
		return new float[] {lowerPos, upperPos};	
	}
	
	public float[] clockWiseValues()
	{
		return new float[] {lowerNeg, upperNeg, upperPos, lowerPos};
	}
	
	public void setValueWithClockwiseIndex(float _value, int index) {
		switch(index) {
		case 0:
			lowerNeg = _value;	
			break;
		case 1:
			upperNeg = _value;
			break;
		case 2:
			upperPos = _value;
			break;
		default:
			lowerPos = _value;
			break;
		}
	}
	
	public bool allValuesMaxed()
	{
		return this.upperNeg >= Window.LIGHT_LEVEL_MAX && 
			this.lowerNeg >= Window.LIGHT_LEVEL_MAX && 
			this.upperPos >= Window.LIGHT_LEVEL_MAX && 
			this.lowerPos >= Window.LIGHT_LEVEL_MAX;	
	}
	
	public void setAllValuesToMax()
	{
		this.upperNeg = Window.LIGHT_LEVEL_MAX;
		this.lowerNeg = Window.LIGHT_LEVEL_MAX;  
		this.upperPos = Window.LIGHT_LEVEL_MAX;  
		this.lowerPos = Window.LIGHT_LEVEL_MAX;
	}
	
	public void setAllValuesTo(float _val)
	{
		this.upperNeg = _val;
		this.lowerNeg = _val;
		this.upperPos = _val;
		this.lowerPos = _val;
	}
	
	public float average() {
		return (upperNeg + lowerNeg + upperPos + lowerPos)/4f;	
	}
	
	public float averageNeg() {
		return (upperNeg + lowerNeg)/2f;
	}
	
	public float averagePos() {
		return (upperPos + lowerPos)/2f;	
	}
}

public struct LightPoint
{
	public PTwo point;
	public float lightValue;
	
	public LightPoint(PTwo _point, float _lightValue) {
		point = _point; lightValue = _lightValue;	
	}
}