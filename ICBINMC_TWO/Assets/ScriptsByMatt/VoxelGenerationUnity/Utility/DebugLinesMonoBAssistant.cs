using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DebugLinesMonoBAssistant : MonoBehaviour 
{
	private List<Coord> unitCubes = new List<Coord>();
	private List<CoordLine> lines = new List<CoordLine>();
	
	public void addUnitCubeAt(Coord woco)
	{
		if (!unitCubes.Contains(woco))
		{
			unitCubes.Add(woco);	
		}
	}
	
	public void addCoordLine(Coord start, Coord end)
	{
		CoordLine cl = new CoordLine();
		cl.start = start;
		cl.end = end;
		lines.Add(cl);
	}
	
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		drawUnitCubes();
//		drawTestLines();
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
}
