using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class GroundTile : ATile
{
    public override void Connect()
    {
		base.Connect ();

		Vector3i pos = new Vector3i(transform.position);

		List<Pair<ATile, Vector3i>> list = new List<Pair<ATile, Vector3i>> ();
		if (!G.Sys.tilemap.connectable (transform.position)) {
			applyConnexions (list);
			return;
		}

		Add (pos + new Vector3i (0, 0, 1), list, true);
		Add (pos + new Vector3i(0, 0, -1), list, true);
		Add (pos + new Vector3i(1, 0, 0), list, true);
		Add (pos + new Vector3i(-1, 0, 0), list, true);

		if(list[0].First != null && list[2].First != null)
			Add(pos + new Vector3i(1, 0, 1), list);
		if(list[2].First != null && list[1].First != null)
			Add(pos + new Vector3i(1, 0, -1), list);
		if(list[3].First != null && list[0].First != null)
			Add(pos + new Vector3i(-1, 0, 1), list);
		if(list[3].First != null && list[1].First != null)
			Add(pos + new Vector3i(-1, 0, -1), list);
		list.RemoveAll (t => t.First == null);

		applyConnexions (list);
    }

	protected override void Awake()
    {
		type = TileID.GROUND;

		G.Sys.tilemap.addTile (transform.position, this, true, false, Tilemap.GROUND_PRIORITY);
		foreach (var t in G.Sys.tilemap.at(transform.position))
			t.Connect ();
    }

	protected override void OnDestroy()
	{
		G.Sys.tilemap.delTile (transform.position, this);
		foreach (var t in G.Sys.tilemap.at(transform.position))
			t.Connect ();
		foreach (var t in connectedTiles)
			t.First.targetOf.Remove (this);
		foreach (var t in targetOf.ToList())
			t.Connect ();
	}
}
