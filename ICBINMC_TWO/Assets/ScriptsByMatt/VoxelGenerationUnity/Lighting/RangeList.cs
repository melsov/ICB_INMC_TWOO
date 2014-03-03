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
public class DiscreteDomainRangeList<T>  where T : iRange	
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
	
	public void Add(T rangeItem)
	{
		Add (rangeItem, false);
	}
	
	public void AddOverwrite(T rangeItem)
	{
		Add(rangeItem, true);
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
			if (range.startP <= y)
			{
				if (range.contains(y))
				{
					result = range;
				}
				break;
			}
		}
		
		return result;
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
