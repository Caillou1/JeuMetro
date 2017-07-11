using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public static class PathFinder
{
	private const float verticalDistanceMultiplier = 5;

	/// <summary>
	/// Retourne le chemin optimal entre les 2 positions
	/// Si la 2e valeur est égal à faux, le chemin retourné amenne sur la position la plus proche de l'arrivée
	/// </summary>
	/// <param name="start">Start.</param>
	/// <param name="end">End.</param>
	public static Pair<List<Vector3>, bool> Path (Vector3 start, Vector3 end, Dictionary<TileID, float> weights = default(Dictionary<TileID, float>), int maxIteration = int.MaxValue)
	{
		var p = Path (new Vector3i (start), new Vector3i (end), weights, maxIteration);
		List<Vector3> endPath = new List<Vector3> ();
		foreach (var item in p.First)
			endPath.Add (item.toVector3 ());
		return new Pair<List<Vector3>, bool>(endPath, p.Second);
	}

	/// <summary>
	/// Retourne le chemin optimal entre les 2 positions
	/// Si la 2e valeur est égal à faux, le chemin retourné amenne sur la position la plus proche de l'arrivée
	/// </summary>
	/// <param name="start">Start.</param>
	/// <param name="end">End.</param>
	public static Pair<List<Vector3i>, bool> Path(Vector3i start, Vector3i end, Dictionary<TileID, float> weights = default(Dictionary<TileID, float>), int maxIteration = int.MaxValue)
	{
		if (weights == null)
			weights = new Dictionary<TileID, float> ();
		
		if(start.Equals(end))
			return new Pair<List<Vector3i>, bool>(new Vector3i[]{start}.ToList(), true);
		var startTile = G.Sys.tilemap.connectableTile (start);
		var endTile = G.Sys.tilemap.connectableTile (end);
	
		if (startTile == null) {
			var t = G.Sys.tilemap.at (start);
			if (t.Count > 0)
				startTile = t [0];
		}
		if (endTile == null) {
			var t = G.Sys.tilemap.at (end);
			if (t.Count > 0)
				endTile = t [0];
		}

		if (startTile == null || endTile == null)
			return new Pair<List<Vector3i>, bool>(new List<Vector3i>(), false);

		List<Pair<ATile, float>> nexts = new List<Pair<ATile, float>> ();
		List<Pair<ATile, Pair<ATile, Vector3i>>> visiteds = new List<Pair<ATile, Pair<ATile, Vector3i>>>();

		nexts.Add (new Pair<ATile, float> (startTile, 0));

		int iteration = 0;

		while (nexts.Count > 0 && iteration ++ < maxIteration) {
			var current = Min (nexts, start, end);
			nexts.Remove (current);

			foreach (var tile in current.First.connectedTiles) {
				if (isVisited (tile.First, visiteds))
					continue;

				visiteds.Add(new Pair<ATile, Pair<ATile, Vector3i>>(current.First, tile));
				nexts.Add (new Pair<ATile, float> (tile.First, current.Second + moveWeight(tile.First.transform.position - current.First.transform.position, tile.First.transform.position, weights)));

				if (tile.First == endTile) {
					var p = createPath (visiteds);
					p.Insert (0, start);
					return new Pair<List<Vector3i>, bool>(p, true);
				}
			}
		}

		return new Pair<List<Vector3i>, bool>(createNearestPath(visiteds, end, start), false);
	}

	private static float moveWeight(Vector3 dir, Vector3 pos, Dictionary<TileID, float> weights)
	{
		float weight = 0;
		int count = 0;
		foreach (var t in G.Sys.tilemap.at(pos)) {
			if (weights.ContainsKey (t.type))
				weight += weights [t.type];
		}
		if (count == 0)
			weight = 1;
		else
			weight /= count;
		return (new Vector2 (dir.x, dir.z).magnitude + Mathf.Abs (dir.y) * G.Sys.constants.VerticalAmplification) * weight;
	}

	private static float distance(Vector3i start, Vector3i end)
	{
		int x = Mathf.Abs (start.x - end.x);
		int y = Mathf.Abs (start.z - end.z);
		int diagonal = Mathf.Min (x, y);

		return Mathf.Abs (start.y - end.y) * verticalDistanceMultiplier + diagonal * 1.5f + (x + y - 2 * diagonal);
	}

	private static Pair<ATile, float> Min(List<Pair<ATile, float>> list, Vector3i start, Vector3i end)
	{
		float minValue = float.MaxValue;
		Pair<ATile, float> minElement = null;
		foreach(var it in list)
		{
			var dir = end.toVector3() - it.First.transform.position;
			float value = it.Second + new Vector2(dir.x, dir.z).magnitude + Mathf.Abs(dir.y * 5);
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
		poss.Reverse ();
		return poss;
	}

	private static List<Vector3i> createNearestPath(List<Pair<ATile, Pair<ATile, Vector3i>>> visited, Vector3i dest, Vector3i start)
	{
		if (visited.Count == 0)
			return new List<Vector3i> ();
		int bestIndex = -1;
		float bestDist = float.MaxValue;
		for (int i = 0; i < visited.Count; i++) {
			var dir = visited [i].Second.Second - dest;
			var dist = new Vector2 (dir.x, dir.z).magnitude + Mathf.Abs (dir.y) * G.Sys.constants.VerticalAmplification;
			if (dist < bestDist) {
				bestDist = dist;
				bestIndex = i;
			}
		}

		if(visited [bestIndex].Second.Second.Equals(start))
			return new Vector3i[]{ start }.ToList ();

		ATile current = visited [bestIndex].Second.First;
		List<Vector3i> poss = new List<Vector3i> ();
		for (int i = bestIndex; i >= 0; i--) {
			if (visited [i].Second.First == current) {
				poss.Add (visited [i].Second.Second);
				current = visited [i].First;
			}
		}
		poss.Reverse ();
		return poss;
	}
}

