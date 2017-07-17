using System;
using UnityEngine;
using System.Collections.Generic;
using NRand;

public class Path
{

	bool pathValid = false;

	public Path(Dictionary<TileID, float> _weights)
	{
		points = new List<Vector3> ();
		endPos = new Vector3 ();
		weights = _weights;
	}

	public void create(Vector3 start, Vector3 end, float variations = 0)
	{
		var path = PathFinder.Path (start, end, weights);
		points = path.First;
		pathValid = false;
		if (path.Second) {
			pathValid = true;
			endPos = end;
		}
		else if (path.First.Count > 0)
			endPos = path.First [path.First.Count - 1];
		else
			endPos = end;
	
		if (variations * points.Count >= 1 && points.Count > 0)
			variancePath ((int)(variations * points.Count));
	}

	public bool finished()
	{
		return points.Count == 0;
	}

	public Vector3 getEndPos() {
		return endPos;
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

	void variancePath(int variations)
	{
		if (points.Count < 3)
			return;

		List<Pair<int, Vector3>> newPoints = new List<Pair<int, Vector3>>();

		var gen = new StaticRandomGenerator<DefaultRandomGenerator> ();
		var dPoint = new UniformIntDistribution (1, points.Count - 2);
		var r = G.Sys.constants.travelerLostVariance;
		var dDest = new UniformVector3BoxDistribution (-r, r, 0, 0, -r, r);

		for (int i = 0; i < variations; i++) {
			var rDest = new Vector3i(dDest.Next (gen)).toVector3();
			if (rDest == Vector3.zero)
				continue;
			var point = dPoint.Next (gen);
			if (newPoints.Find (it => it.First == point) != null)
				continue;
			if (PathFinder.Path (points [point], points [point] + rDest, weights, 20).Second)
				continue;
			newPoints.Add(new Pair<int, Vector3>(point, points [point] + rDest));
		}

		var pos = points [0];
		points.Clear ();
		newPoints.Sort (new ComparePair ());

		foreach (var p in newPoints) {

			points.AddRange(PathFinder.Path(pos, p.Second, weights).First);

			if (points.Count != 0)
				points.RemoveAt (points.Count - 1);
			
			pos = p.Second;
		}
		points.AddRange (PathFinder.Path (pos, endPos, weights).First);
	}

	public bool isOnPath(Vector3i pos)
	{
		return isOnPath (pos.toVector3 ());
	}

	public bool isOnPath(Vector3 pos)
	{
		var posI = new Vector3i (pos);
		foreach (var p in points) {
			if (new Vector3i (p).equal (posI))
				return true;
		}

		return false;
	}

	public bool haveTileOnPath(TileID tile)
	{
		foreach(var p in points)
			if(G.Sys.tilemap.tilesOfTypeAt(p, tile).Count != 0)
				return true;
		return false;
	}

	List<Vector3> points;
	Vector3 endPos;
	Dictionary<TileID, float> weights;

	class ComparePair : IComparer<Pair<int, Vector3>>
			{
		public int Compare( Pair<int, Vector3> x, Pair<int, Vector3> y )  
		{
			return x.First.CompareTo(y.First);
		}

	}

	public bool isPathValid()
	{
		return pathValid;
	}

	public bool haveOnPath(TileID id)
	{
		foreach (var p in points) {
			if (G.Sys.tilemap.haveSpecialTileAt (id, p))
				return true;
		}
		return false;
	}

	public List<Vector3> posOnPath(TileID id)
	{
		List<Vector3> pos = new List<Vector3>();

		foreach (var p in points) {
			if(G.Sys.tilemap.haveSpecialTileAt(id, p))
				pos.Add(p);
		}
		return pos;
	}
}

