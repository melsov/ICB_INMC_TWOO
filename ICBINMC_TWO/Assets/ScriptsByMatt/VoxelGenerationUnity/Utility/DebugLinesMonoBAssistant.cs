using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public struct Column : IEquatable<Column>
{
	public SimpleRange range;
	public PTwo xz;
	public int handyInteger;
	
	public bool Equals(Column other)
	{
		return this.range.start == other.range.start && this.range.range == other.range.range && PTwo.Equal(this.xz, other.xz);
	}
}

public class DebugLinesMonoBAssistant : MonoBehaviour 
{
	private List<Coord> unitCubes = new List<Coord>();
	private List<CoordLine> lines = new List<CoordLine>();
	private List<Window> windows = new List<Window>();
	private List<Column> columns = new List<Column>();
	
	public void addUnitCubeAt(Coord woco)
	{
		if (!unitCubes.Contains(woco))
		{
			unitCubes.Add(woco);	
		}
	}
	
	public void addColumn(Column column)
	{
		int index = columns.IndexOf(column);
		if (index >= 0)
		{
			Column col = columns[index];
			col.handyInteger = column.handyInteger;
			columns[index] = col;
		} else {
			columns.Add(column);
		}
//		if (!columns.Contains(column))
//			columns.Add(column);
	}
	
	public void addColumn(Column column, int handyInt)
	{
		int index = columns.IndexOf(column);
		if (index >= 0)
		{
			Column col = columns[index];
			col.handyInteger = handyInt; // column.handyInteger;
			columns[index] = col;
		} else {
			columns.Add(column);
		}
//		if (!columns.Contains(column))
//			columns.Add(column);
	}
	
	public void addColumn(SimpleRange range, PTwo _xz, int debugInteger)
	{
		Column column = new Column();
		column.range = range;
		column.xz = _xz;
		column.handyInteger = debugInteger;
		
		if (!columns.Contains(column))
			columns.Add(column);
	}
	
	public void removeColumn(Column col)
	{
		columns.Remove(col);
	}
	
	public void clearColumns()
	{
		columns.Clear();	
	}
	
	public void addCoordLine(Coord start, Coord end)
	{
		CoordLine cl = new CoordLine();
		cl.start = start;
		cl.end = end;
		
		lines.Add(cl);
	}
	
	public void addWindowToDraw(Window win)
	{
		windows.Add(win);	
	}
	
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
//		drawUnitCubes();
//		drawTestLines();
//		drawWindows();
		drawColumns();
	}
	
	void drawUnitCubes()
	{
		foreach(Coord woco in unitCubes)
		{
			DebugLinesUtil.DrawUnitCubeAt(woco);	
		}
	}
	
	void drawTestLines()
	{
		foreach(CoordLine line in lines)
		{
			DebugLinesUtil.debugLine(line.start, line.end, Color.cyan);
		}
	}
	
	void drawWindows()
	{
		for(int i = 0; i < windows.Count; ++i)
		{
			DebugLinesUtil.DrawWindow(windows[i]); //, Color.green, Color.cyan);	
		}
	}
	
	void drawColumns()
	{
//		foreach(Column colm in columns)
		for(int i=0; i < columns.Count; ++i)
		{
			Column colm = columns[i];
			DebugLinesUtil.DrawDebugColumn(colm);
		}
	}
}
