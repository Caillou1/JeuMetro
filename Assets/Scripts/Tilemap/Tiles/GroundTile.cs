using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class GroundTile : ATile
{
	protected override void Awake()
    {
		type = TileID.GROUND;

		G.Sys.tilemap.addTile (transform.position, this, Tilemap.GROUND_PRIORITY);

		G.Sys.tilemap.AddGroundTile ();
    }

	protected override void OnDestroy()
	{
		G.Sys.tilemap.delTile (transform.position, this);
	}
}
