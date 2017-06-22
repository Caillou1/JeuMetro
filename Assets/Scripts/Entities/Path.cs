using System;
using UnityEngine;
using System.Collections.Generic;

public class Path
{
	public Path(Dictionary<TileID, float> _weights, float _smoothAngles)
	{
		points = new List<Vector3> ();
		startPos = new Vector3 ();
		endPos = new Vector3 ();
	}

	public void create(Vector3 start, Vector3 end)
	{
		points = PathFinder.Path (start, end, weights);
		startPos = start;
		endPos = end;
	}

	public bool finished()
	{
		return points.Count == 0;
	}

	public Vector3 next(Vector3 current)
	{
		if (points.Count == 0)
			return endPos;
		var currentPos = new Vector3i (current);
		var nextPos = new Vector3i (points [0]);
		if (currentPos.x == nextPos.x && currentPos.y == nextPos.y && currentPos.z == nextPos.z)
			points.RemoveAt (0);
		if (points.Count == 0)
			return endPos;
		return points [0];
	}

	List<Vector3> points;
	Vector3 startPos;
	Vector3 endPos;
	Dictionary<TileID, float> weights;
	float smoothAngles;
}

