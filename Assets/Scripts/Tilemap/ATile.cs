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
	TICKETDISTRIBUTEUR
}

public abstract class ATile : MonoBehaviour
{
    public ATile()
    {
        connectedTiles = new List<ATile>();
    }

    public abstract void Connect();

	static protected void Add(ATile tile, List<ATile> list)
    {
        if (tile != null)
            list.Add(tile);
    }

	protected static bool validConnexions(List<ATile> first, List<ATile> second)
	{
		if (first == null || second == null)
			return false;

		if (first.Count != second.Count)
			return false;

		for (int i = 0; i < first.Count; i++)
			if (first [i] != second [i])
				return false;
		return true;
	}

	protected void applyConnexions(List<ATile> list)
	{
		if (!validConnexions (list, connectedTiles)) {
			var oldList = connectedTiles.ToList();
			connectedTiles = list;
			foreach (var t in oldList) {
				t.targetOf.Remove (this);
				t.Connect ();
			}
			foreach (var t in list) {
				t.targetOf.Add (this);
				t.Connect ();
			}
		}
	}

    public List<ATile> connectedTiles { get; protected set; }
	public List<ATile> targetOf = new List<ATile> (); 
	public TileID type { get; protected set; }
}
