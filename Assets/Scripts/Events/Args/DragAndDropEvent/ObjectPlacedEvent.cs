using System;
using UnityEngine;
using System.Collections.Generic;

public class ObjectPlacedEvent : EventArgs
{
    public ObjectPlacedEvent(List<Vector3> _points, TileID _type, bool _bought)
	{
		points = new List<Vector3i> ();
		foreach(var p in _points)
			points.Add(new Vector3i(p));
        type = _type;
        bought = _bought;
	}

    public ObjectPlacedEvent (List<Vector3i> _points, TileID _type, bool _bought)
	{
		points = _points;
        type = _type;
        bought = _bought;
	}

	public List<Vector3i> points;
    public TileID type;
    public bool bought;
}

