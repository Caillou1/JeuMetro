using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class EmptyWallTile : ATile
{
	protected override void Awake()
    {
		type = TileID.EMPTYWALL;

		G.Sys.tilemap.addTile (transform.position, this, Tilemap.LOW_PRIORITY);
    }

	protected override void OnDestroy()
	{
		G.Sys.tilemap.delTile (transform.position, this);
	}
}