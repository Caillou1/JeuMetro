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
		List<Pair<ATile, Pair<ATile, Vector3i>>> visiteds = new List<Pair<ATile, Pair<ATile, Vector3i>>>();

		nexts.Add (new Pair<ATile, float> (startTile, 0));

		while (nexts.Count > 0) {
			var current = Min (nexts, end, weights);
			nexts.Remove (current);

			foreach (var tile in current.First.connectedTiles) {
				if (isVisited (tile.First, visiteds))
					continue;

				visiteds.Add(new Pair<ATile, Pair<ATile, Vector3i>>(current.First, tile));
				nexts.Add (new Pair<ATile, float> (tile.First, current.Second + distance (new Vector3i (current.First.transform.position), tile.Second)));

				if (tile.First == endTile)
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

	private static bool isVisited(ATile tile, List<Pair<ATile, Pair<ATile, Vector3i>>> visited)
	{
		foreach (var t in visited)
			if (t.Second.First == tile)
				return true;
		return false;
	}

	private static List<Vector3i> createPath(List<Pair<ATile, Pair<ATile, Vector3i>>> visited)
	{
		if (visited.Count == 0)
			return new List<Vector3i> ();
		
		ATile current = visited [visited.Count - 1].Second.First;
		List<Vector3i> poss = new List<Vector3i> ();
		for (int i = visited.Count - 1; i >= 0; i--) {
			if (visited [i].Second.First == current) {
				poss.Add (visited [i].Second.Second);
				current = visited [i].First;
			}
		}
		poss.Add (new Vector3i (current.transform.position));
		poss.Reverse ();
		return poss;
	}
}

