using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class BinTile : ATile
{
	void Awake()
    {
		type = TileID.BIN;

		G.Sys.tilemap.addTile (transform.position, this, false, true, Tilemap.BIN_PRIORITY);

		foreach (var t in G.Sys.tilemap.at(transform.position))
			t.Connect ();
    }

	public override void Connect (){}

	void OnDestroy()
	{
		G.Sys.tilemap.delTile (transform.position, this);

		foreach (var t in G.Sys.tilemap.at(transform.position))
			t.Connect ();
	}
}