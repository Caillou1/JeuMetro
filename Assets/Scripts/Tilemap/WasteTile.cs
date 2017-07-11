using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WasteTile : ATile
{
	protected override void Awake()
    {
		/*if (G.Sys.tilemap.at (transform.position).Count != 1) {
			Destroy (gameObject);
		}*/

		type = TileID.WASTE;

		G.Sys.tilemap.addTile (transform.position, this, false, true, Tilemap.LOW_PRIORITY);

		foreach (var t in G.Sys.tilemap.at(transform.position))
			t.Connect ();

		G.Sys.tilemap.addSpecialTile (type, transform.position);
    }

	protected override void OnDestroy()
	{
		G.Sys.tilemap.delTile (transform.position, this);

		foreach (var t in G.Sys.tilemap.at(transform.position))
			t.Connect ();

		G.Sys.tilemap.delSpecialTile (type, transform.position);
	}
}