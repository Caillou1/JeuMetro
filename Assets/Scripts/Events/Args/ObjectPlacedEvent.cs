using System;
using UnityEngine;
using System.Collections.Generic;

public class ObjectPlacedEvent : EventArgs
{
	public ObjectPlacedEvent(List<Vector3> _points)
	{
		points = new List<Vector3i> ();
		foreach(var p in _points)
			points.Add(new Vector3i(p);
	}

	public ObjectPlacedEvent (List<Vector3i> _points)
	{
		points = _points;
	}

	public List<Vector3i> points;
}

