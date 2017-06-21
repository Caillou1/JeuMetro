using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TileInfos
{
	public TileInfos(ATile _tile, bool _canBeConnected, bool _preventConnexions, int _priority)
	{
		priority = _priority;
		canBeConnected = _canBeConnected;
		preventConnexions = _preventConnexions;
		tile = _tile;
	}

	public int priority;
	public bool canBeConnected;
	public bool preventConnexions;
	public ATile tile;
}

public class Tilemap
{
	public const int GROUND_PRIORITY = 0;
	public const int ESCALATOR_PRIORITY = 10;

	private Dictionary<Vector3i, List<TileInfos>> tiles = new Dictionary<Vector3i, List<TileInfos>> ();

	public void addTile(Vector3 pos, ATile tile, int priority = 0)
	{
		addTile (new Vector3i (pos), tile, priority);
	}

	public void addTile(Vector3i pos, ATile tile, int priority = 0)
	{
		addTile (pos, tile, true, false, priority);
	}

	public void addTile(Vector3 pos, ATile tile, bool canBeConnected, int priority = 0)
	{
		addTile (new Vector3i (pos), tile, canBeConnected, priority);
	}

	public void addTile(Vector3i pos, ATile tile, bool canBeConnected, int priority = 0)
	{
		addTile (pos, tile, canBeConnected, false, priority);
	}

	public void addTile(Vector3 pos, ATile tile, bool canBeConnected, bool preventConnexions, int priority = 0)
	{
		addTile (new Vector3i (pos), tile, canBeConnected, preventConnexions, priority);
	}

	public void addTile(Vector3i pos, ATile tile, bool canBeConnected, bool preventConnexions, int priority = 0)
	{
		if (!tiles.ContainsKey (pos))
			tiles.Add (pos, new List<TileInfos> ());
		if(!tiles[pos].Exists (it => it.tile == tile))
			tiles [pos].Add (new TileInfos(tile, canBeConnected, preventConnexions, priority));
	}

	public bool delTile(Vector3 pos, ATile tile)
	{
		return delTile (new Vector3i (pos), tile);
	}

	public bool delTile(Vector3i pos, ATile tile)
	{
		if (!tiles.ContainsKey (pos))
			return false;
		return tiles [pos].RemoveAll (it => it.tile == tile) > 0;
	}

	public List<ATile> at(Vector3 pos)
	{
		return at (new Vector3i (pos));
	}

	public List<ATile> at(Vector3i pos)
	{
		if (!tiles.ContainsKey (pos))
			return new List<ATile> ();
		List<ATile> list = new List<ATile>();
		foreach (var t in tiles[pos])
			list.Add (t.tile);
		return list;
	}

	public bool connectable(Vector3 pos)
	{
		return connectable (new Vector3i (pos));
	}

	public bool connectable(Vector3i pos)
	{
		if (!tiles.ContainsKey (pos))
			return false;
		bool isConnectable = false;
		foreach (var tile in tiles[pos]) {
			if (tile.preventConnexions)
				return false;
			if (tile.canBeConnected)
				isConnectable = true;
		}
		return isConnectable;
	}

	public List<ATile> connectableTiles(Vector3 pos)
	{
		return connectableTiles (new Vector3i (pos));
	}

	public List<ATile> connectableTiles(Vector3i pos)
	{
		List<ATile> list = new List<ATile> ();
		if (!tiles.ContainsKey (pos))
			return list;
		foreach (var tile in tiles[pos]) {
			if (tile.preventConnexions)
				return new List<ATile> ();
			if (tile.canBeConnected)
				list.Add (tile.tile);
		}
		return list;
	}

	public ATile connectableTile(Vector3 pos)
	{
		return connectableTile (new Vector3i (pos));
	}

	public ATile connectableTile(Vector3i pos)
	{
		int bestValue = int.MinValue;
		bool bestCanBeConnected = false;
		ATile bestTile = null;
		if (!tiles.ContainsKey (pos))
			return null;
		foreach (var tile in tiles[pos]) {
			if (tile.preventConnexions)
				return null;
			if (tile.priority > bestValue) {
				bestTile = tile.tile;
				bestCanBeConnected = tile.canBeConnected;
				bestValue = tile.priority;
			}
		}
		return bestCanBeConnected ? bestTile : null;
	}

	public TileInfos tileInfosOf(ATile tile, Vector3 pos)
	{
		return tileInfosOf (tile, new Vector3i (pos));
	}

	public TileInfos tileInfosOf(ATile tile, Vector3i pos)
	{
		if (!tiles.ContainsKey (pos))
			return null;
		return tiles [pos].Find (it => it.tile == tile);
	}
}
