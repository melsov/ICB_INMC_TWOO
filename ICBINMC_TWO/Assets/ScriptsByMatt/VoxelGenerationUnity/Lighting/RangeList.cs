using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface iRange
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
}	

/*
 * Collection class that keeps a 
 * list of Type iRange elements
 * and keeps them in order when
 * inserting. 
 * 'Discrete Domain:' doesn't tolerate
 * intersecting ranges
 * */

public class DiscreteDomainRangeList<T> where T : iRange
{
	private List<T> ranges = new List<T>();
	
	public void Add(T rangeItem)
	{
		//ranges ordered from low start to high start
		int insertIndex = 0;
		
		foreach(iRange r in ranges)
		{
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
			throw new Exception("Discrete Domain Range List doesn't put up with flush or overlapping ranges");
		}
		this.Insert(insertIndex, rangeItem);
	}
	
	private void Insert(int index, T item)
	{
		this.ranges.Insert(index, item);
	}
	
	public T this[int i]
	{
		get {
			return this.ranges[i];
		} set {
			this.ranges[i] = value;
		}
	}
	
	public List<T> toList() {
		return ranges;
	}
}
