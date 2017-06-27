using System;
using UnityEngine;
using System.Collections.Generic;

public class Path
{
	public Path(Dictionary<TileID, float> _weights)
	{
		points = new List<Vector3> ();
		endPos = new Vector3 ();
		weights = _weights;
	}

	public void create(Vector3 start, Vector3 end)
	{
		points = PathFinder.Path (start, end, weights);
		endPos = end;
	}

	public bool finished()
	{
		return points.Count == 0;
	}

	public Vector3 next(Vector3 current)
	{
		debugPath (current);
		if (points.Count == 0)
			return endPos;
		var currentPos = new Vector3i (current);
		var nextPos = new Vector3i (points [0]);
		if (currentPos.x == nextPos.x && currentPos.y == nextPos.y && currentPos.z == nextPos.z)
			points.RemoveAt (0);
		if (points.Count == 0)
			return endPos;
		return points[0];
	}

	void debugPath(Vector3 current)
	{
		for (int i = 0; i < points.Count; i++) {
			var last = i > 0 ? points [i - 1] : current;
			Debug.DrawLine (last, points [i], Color.red);
		}
	}

	public Vector3 target()
	{
		return endPos;
	}

	List<Vector3> points;
	Vector3 endPos;
	Dictionary<TileID, float> weights;
}

