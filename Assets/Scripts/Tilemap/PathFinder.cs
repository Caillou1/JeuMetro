using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public static class PathFinder
{
	private const float verticalDistanceMultiplier = 5;

	/// <summary>
	/// Retourne le chemin optimal entre les 2 positions
	/// </summary>
	/// <param name="start">Start.</param>
	/// <param name="end">End.</param>
	public static List<Vector3> Path (Vector3 start, Vector3 end, Dictionary<TileID, float> weights = default(Dictionary<TileID, float>))
	{
		var p = Path (new Vector3i (start), new Vector3i (end));
		List<Vector3> endPath = new List<Vector3> ();
		foreach (var item in p)
			endPath.Add (item.toVector3 ());
		return endPath;
	}

	/// <summary>
	/// Retourne le chemin optimal entre les 2 positions
	/// </summary>
	/// <param name="start">Start.</param>
	/// <param name="end">End.</param>
	public static List<Vector3i> Path(Vector3i start, Vector3i end, Dictionary<TileID, float> weights = default(Dictionary<TileID, float>))
	{
		if (weights == null)
			weights = new Dictionary<TileID, float> ();
		
		if(start == end)
			return new List<Vector3i>(new Vector3i[]{start});
		var startTile = G.Sys.tilemap.connectableTile (start);
		var endTile = G.Sys.tilemap.connectableTile (end);

		if (startTile == null || endTile == null)
			return new List<Vector3i> ();

		List<Pair<ATile, float>> nexts = new List<Pair<ATile, float>> ();
		List<Pair<ATile, ATile>> visiteds = new List<Pair<ATile, ATile>>();

		nexts.Add (new Pair<ATile, float> (startTile, 0));

		while (nexts.Count > 0) {
			var current = Min (nexts, end, weights);
			nexts.Remove (current);

			foreach (var tile in current.First.connectedTiles) {
				if (isVisited (tile, visiteds))
					continue;

				visiteds.Add(new Pair<ATile, ATile>(current.First, tile));
				nexts.Add (new Pair<ATile, float> (tile, current.Second + distance (new Vector3i (current.First.transform.position), new Vector3i (tile.transform.position))));

				if (tile == endTile)
					return createPath (visiteds);
			}
		}

		return new List<Vector3i> ();
	}

	private static float distance(Vector3i start, Vector3i end)
	{
		return Mathf.Abs (start.y - end.y) * verticalDistanceMultiplier + new Vector2 (start.x - end.x, start.z - end.z).magnitude;
	}

	private static Pair<ATile, float> Min(List<Pair<ATile, float>> list, Vector3i end, Dictionary<TileID, float> weights)
	{
		float minValue = float.MaxValue;
		Pair<ATile, float> minElement = null;
		foreach(var it in list)
		{
			float weight = 0;
			int count = 0;
			var tiles = G.Sys.tilemap.at (it.First.transform.position);
			foreach (var tile in tiles)
				if (weights.ContainsKey (it.First.type)) {
					weight = weights [it.First.type];
					count++;
				}
			if (count == 0)
				weight = 1;
			else
				weight /= count;

			float value = it.Second + distance (new Vector3i (it.First.transform.position), end) * weight;
			if (value < minValue) {
				minValue = value;
				minElement = it;
			}
		}
		return minElement;
	}

	private static bool isVisited(ATile tile, List<Pair<ATile, ATile>> visited)
	{
		foreach (var t in visited)
			if (t.Second == tile)
				return true;
		return false;
	}

	private static List<Vector3i> createPath(List<Pair<ATile, ATile>> visited)
	{
		List<ATile> thepath = new List<ATile> ();
		thepath.Add (visited [visited.Count - 1].Second);

		for (int i = visited.Count - 1; i >= 0; i--) {
			if (visited [i].Second == thepath [thepath.Count - 1])
				thepath.Add (visited [i].First);
		}

		thepath.Reverse ();
		List<Vector3i> poss = new List<Vector3i> ();
		foreach (var p in thepath)
			poss.Add (new Vector3i (p.transform.position));

		return poss;
	}
}

