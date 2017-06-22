using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class GroundTile : ATile
{
    public override void Connect()
    {
        Vector3i pos = new Vector3i(transform.position);

		List<ATile> list = new List<ATile> ();
		Add(G.Sys.tilemap.connectableTile(pos + new Vector3i(0, 0, 1)), list);
		Add(G.Sys.tilemap.connectableTile(pos + new Vector3i(0, 0, -1)), list);
		Add(G.Sys.tilemap.connectableTile(pos + new Vector3i(1, 0, 0)), list);
		Add(G.Sys.tilemap.connectableTile(pos + new Vector3i(-1, 0, 0)), list);
		Add(G.Sys.tilemap.connectableTile(pos + new Vector3i(1, 0, 1)), list);
		Add(G.Sys.tilemap.connectableTile(pos + new Vector3i(1, 0, -1)), list);
		Add(G.Sys.tilemap.connectableTile(pos + new Vector3i(-1, 0, 1)), list);
		Add(G.Sys.tilemap.connectableTile(pos + new Vector3i(-1, 0, -1)), list);

		applyConnexions (list);
    }

    void Awake()
    {
		type = TileID.GROUND;

		G.Sys.tilemap.addTile (transform.position, this, true, false, Tilemap.GROUND_PRIORITY);
		Connect ();
    }

	void OnDestroy()
	{
		G.Sys.tilemap.delTile (transform.position, this);
		foreach (var t in targetOf.ToList())
			t.Connect ();
	}
}
