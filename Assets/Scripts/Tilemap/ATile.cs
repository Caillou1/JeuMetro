using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TileID
{
	GROUND,
	WALL,
	BIN,
	ESCALATOR,
	BENCH,
	FOODDISTRIBUTEUR,
	PODOTACTILE,
	INFOPANEL,
	TICKETDISTRIBUTEUR,
	STAIRS,
	IN,
	OUT,
	METRO,
	ELEVATOR,
	SPEAKER,
	WASTE,
	EMPTYWALL,
	CONTROLELINE,
}

public abstract class ATile : MonoBehaviour
{
    public ATile()
    {
		connectedTiles = new List<Pair<ATile, Vector3i>> ();
    }

	public virtual void Connect()
	{
		
	}

	protected abstract void Awake();
	protected abstract void OnDestroy (); 

	public void Unregister()
	{
		OnDestroy (); 
		connectedTiles.Clear ();
	}

	public void Register() { Awake (); }

	static protected void Add(Vector3 pos, List<Pair<ATile, Vector3i>> list, bool addNull = false)
	{
		Add (new Vector3i (pos), list, addNull);
	}

	static protected void Add(Vector3i pos, List<Pair<ATile, Vector3i>> list, bool addNull = false)
    {
		if (!G.Sys.tilemap.connectable (pos)) {
			if (addNull)
				list.Add (new Pair<ATile, Vector3i> ());
			return;
		}

		var cTile = G.Sys.tilemap.connectableTile (pos);

		TileID[] validTiles = { TileID.GROUND, TileID.ESCALATOR, TileID.STAIRS };

		var tiles = G.Sys.tilemap.at (pos);
		foreach (var t in tiles) {
			if (t == cTile) {
				list.Add (new Pair<ATile, Vector3i> (t, pos));
				continue;
			}
			if (!validTiles.Contains (t.type))
				continue;
			list.Add (new Pair<ATile, Vector3i> (t, pos));
		}


		/*var tile = G.Sys.tilemap.connectableTile (pos);
		if (tile != null || addNull)
			list.Add(new Pair<ATile, Vector3i>(tile, pos));*/
    }

	protected static bool validConnexions(List<Pair<ATile, Vector3i>> first, List<Pair<ATile, Vector3i>> second)
	{
		if (first == null || second == null)
			return false;

		if (first.Count != second.Count)
			return false;

		for (int i = 0; i < first.Count; i++)
			if (first [i].First != second [i].First)
				return false;
		return true;
	}

	protected void applyConnexions(List<Pair<ATile, Vector3i>> list)
	{
		if (!validConnexions (list, connectedTiles)) {
			var oldList = connectedTiles.ToList();
			connectedTiles = list;
			foreach (var t in oldList) {
				t.First.targetOf.Remove (this);
				t.First.Connect ();
			}
			foreach (var t in list) {
				t.First.targetOf.Add (this);
				t.First.Connect ();
			}
		}
	}

	public List<Pair<ATile, Vector3i>> connectedTiles { get; protected set; }
	[HideInInspector]
	public List<ATile> targetOf = new List<ATile> (); 
	public TileID type { get; protected set; }
}
