using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public interface iRange : IEquatable<iRange>
{
	int startP{
		get; set;	
	}
	
	int rangeP{
		get; set;
	}
	
	int extent();
	
	bool contains(int i);
	bool contains(iRange other);
	OverlapState overlapStateWith(iRange other);
	bool intersectsWith(iRange other);
	iRange intersection(iRange other);
	
	bool isErsatzNull();
	iRange theErsatzNullIRange();
	string toString();
}	

/*
 * Collection class that keeps a 
 * list of Type iRange elements
 * and keeps them in order when
 * inserting. 
 * 'Discrete Domain:' doesn't tolerate
 * intersecting ranges
 * */


//public class DiscreteDomainRangeList<T> : ICollection<T>, ICollection where T : iRange	
public class DiscreteDomainRangeList<T>  where T : iRange, new()	
{
	private List<T> _ranges = new List<T>();

	public int Count{
		get {
			return _ranges.Count;
		}
	}

	public void Clear() {
		_ranges.Clear();
	}

	public bool Remove(T rangeItem){
		return _ranges.Remove(rangeItem);
	}
	
	public void RemoveAt(int index){
		_ranges.RemoveAt(index);
	}
	
	public void RemoveWithRangeEqualTo(SimpleRange withinRange)
	{
		for(int i = 0; i < _ranges.Count; ++i)
		{
			T r = _ranges[i];
			if (r.rangeP == withinRange.range && r.startP == withinRange.startP)
			{
				_ranges.RemoveAt(i);
				return;
			}
		}
	}
	
	public void RemoveStartAbove(int y)
	{
		for(int i = _ranges.Count - 1; i >= 0 ; --i)
		{
			T r = _ranges[i];
			if (r.startP >= y)
			{
				_ranges.RemoveAt(i);
				b.bug("removed a range with start above: " + r.toString());
			} else {
				b.bug("done removing");
				break;
			}
		}
	}
	
	public void Add(T rangeItem)
	{
		Add (rangeItem, false);
	}
	
	public void AddOverwrite(T rangeItem)
	{
		Add(rangeItem, true);
	}
	
	//NOISE PATCH STYLE
	public T Incorporate(int y, bool YIsAnAddition)
	{
		b.bug("incorp");
		if (YIsAnAddition)
		{
			return addPoint(y);
//			return true;
		}
		return subtractPoint(y);
	}
	
	public int highestExtent()
	{
		if (_ranges.Count == 0)
			return -1;
		T highest = _ranges[_ranges.Count - 1];
		return highest.extent();
	}
	
	private T addPoint(int y)
	{
		
		if (_ranges.Count == 0)
		{
			T range = new T();
			range.startP = y;
			range.rangeP = 1;
			_ranges.Add(range);
			return range;
		}
		
		T rOD;
		for (int heightsIndex = 0; heightsIndex < _ranges.Count ; ++heightsIndex)
		{
			rOD = _ranges[heightsIndex];
			
			if (rOD.extent() == y)
			{
				rOD.rangeP++;
				//is there another range just above relCo y?
				if (heightsIndex < _ranges.Count - 1)
				{
					T nextRangeAbove = _ranges[heightsIndex + 1];
					if (nextRangeAbove.startP - 1 == y) {
						//combine above and below
						rOD.rangeP += nextRangeAbove.rangeP;
						_ranges.RemoveAt(heightsIndex + 1);
					}
					
				}
				_ranges[heightsIndex] = rOD;
				break;
			}
			else if (rOD.startP - 1 == y) 
			{
				rOD.startP--; rOD.rangeP++;
				_ranges[heightsIndex] = rOD;
				break;
			}
			else if (rOD.extent() < y) // more than one block above a height range (already know its not directly above)
			{
				// two cases: 
				//1. there's another range in the list: 
				//		a. relco y is one below that range (we are jumping the gun): continue
				//		b. relco y is greater than or eq. to the next range's start (we are jumping the gun): continue
				//		(else c. relco y is more than one below: see case two) 
				//2. we didn't jump the gun: whether or not there's a range above, 
				// this block has no blocks one above or below
				// add it at h index + 1
				
				if (heightsIndex < _ranges.Count - 1)
				{
					T nextRangeAbove = _ranges[heightsIndex + 1];
					if (y >= nextRangeAbove.startP - 1) // cases 1a and 1b
						continue;
				}
				
				//case 2
				T rangeForRelCoY = new T(); //(relCo.y, 1, bb.type);
				rangeForRelCoY.rangeP = 1;
				rangeForRelCoY.startP = y;
				_ranges.Insert(heightsIndex + 1, rangeForRelCoY);
				return rangeForRelCoY;
//				break;
			}
			
			if (rOD.contains(y) )
				throw new Exception("confusing: adding a block to an already solid area?? \n containg range: " + 
					rOD.toString() + " y: " + y);
		}
		return default(T);
	}

	
	private T subtractPoint(int y)
	{
		b.bug("subtracting at: " + y);
		bool checkGotAContainingRange = false;
		T rOD;
		for (int heightsIndex = 0; heightsIndex < _ranges.Count ; ++heightsIndex)
		{
			rOD = _ranges[heightsIndex];
			if (rOD.contains(y))
			{
				checkGotAContainingRange = true;
				if (y == rOD.startP) {
					rOD.startP ++;	
					rOD.rangeP--; // corner case: range now zero: (check for this later)
					b.bug("got y at start");
					_ranges[heightsIndex] = rOD;
				} else if (y == rOD.extent() - 1 ) {
					b.bug("got y is == extent minus one");
					rOD.rangeP--;
					_ranges[heightsIndex] = rOD;
				} else {
					b.bug("got y in the middle");
					int newBelowRange = y - rOD.startP;
					T newAboveRange = new T ();
					newAboveRange.startP = y + 1;
					newAboveRange.rangeP = rOD.rangeP = newBelowRange - 1;
					rOD.rangeP = newBelowRange;
					_ranges.Insert(heightsIndex + 1, newAboveRange);
					_ranges[heightsIndex] = rOD;
					return newAboveRange;
				}
				
				if (rOD.rangeP == 0) {// no more blocks here?
					_ranges.RemoveAt(heightsIndex);	
				}
				else {
					_ranges[heightsIndex] = rOD;
				}
				
				break; 
			}
		}
		
//		if (!checkGotAContainingRange){
//			return false;
//		}
		return default(T);
	}
	
	private void Add(T rangeItem, bool wantOverwrite)
	{
		//ranges ordered from low start to high start
		int insertIndex = 0;
		
		for(int i = 0; i < _ranges.Count; ++i)
		{
			iRange r = _ranges[i];
			//if r is 'beyond' rangeItem
			//insert at the curr insert index
			if (rangeItem.startP > r.extent())
			{
				insertIndex++;
				continue;
			}
			if (rangeItem.extent() < r.startP)
			{
				break;
			}
			OverlapState overlState = rangeItem.overlapStateWith(r);
			if (OverLapUtil.OverlapExists(overlState))
			{
				//want overwrite??
				_ranges.RemoveAt(i);
				insertIndex = i;
//				throw new Exception("Discrete Domain Range List doesn't put up with flush or overlapping ranges");
			}
		}
		this.Insert(insertIndex, rangeItem);
	}
	
	public T rangeContaining(int y)
	{
		T result = default(T);
		if (result != null && result.GetType().IsValueType) {
			result = (T)result.theErsatzNullIRange();
		}
		
		for(int i =0; i < _ranges.Count; ++i)
		{
			T range = _ranges[i];
			if (range.contains(y))
			{
				result = range;
			}
		}
		
		return result;
	}
	
		
	public T rangeWithExtentMinusOneEqual(int y)
	{
		return rangeWithStartExtentMinusOneEqual(y, false);
	}
	
	public T rangeWithStartEqual(int y)
	{
		return rangeWithStartExtentMinusOneEqual(y, true);
	}
	
	private T rangeWithStartExtentMinusOneEqual(int y, bool wantStart)
	{
		T result = nullOrErsatzNull();
		for(int i = 0; i < _ranges.Count; ++i)
		{
			T range = _ranges[i];
			
			if (wantStart && range.startP == y)
				return range;
			else if (!wantStart && range.extent() - 1 == y)
				return range;
		}
		return result;
	}
	
	public T rangeWithStartAndExtentMinusOneEqual(int y)
	{
		T result = nullOrErsatzNull();
		for(int i = 0; i < _ranges.Count; ++i)
		{
			T range = _ranges[i];
			
			if (range.rangeP == 1 && range.startP == y)
				return range;
		}
		return result;
	}
	
	private T nullOrErsatzNull()
	{
		T result = default(T);
		Type type_ = typeof(T);
		if (type_.IsValueType)
			return (T)result.theErsatzNullIRange();
		
		return result; 
//		if (result != null && result.GetType().IsValueType) {
//			result = (T)result.theErsatzNullIRange();
//		}	
	}
//	#endregion
	
	private void Insert(int index, T item)
	{
		this._ranges.Insert(index, item);
	}
	
	public T this[int i]
	{
		get {
			return this._ranges[i];
		} set {
			this._ranges[i] = value;
		}
	}
	
	public List<T> toList() {
		return _ranges;
	}
}
