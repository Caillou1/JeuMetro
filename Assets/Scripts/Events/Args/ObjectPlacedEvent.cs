using System;
using UnityEngine;
using System.Collections.Generic;

public class ObjectPlacedEvent : EventArgs
{
    public ObjectPlacedEvent(List<Vector3> _points, TileID _type)
	{
		points = new List<Vector3i> ();
		foreach(var p in _points)
			points.Add(new Vector3i(p));
        type = _type;
	}

    public ObjectPlacedEvent (List<Vector3i> _points, TileID _type)
	{
		points = _points;
        type = _type;
	}

	public List<Vector3i> points;
    public TileID type;
}

