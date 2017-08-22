using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class ExitsTile : ATile
{
    public bool isInForFlux;
	public string exitname;

	protected override void Awake()
    {
        type = TileID.OUT;

		G.Sys.tilemap.addTile (transform.position, this, Tilemap.EXITS_PRIORITY);

		G.Sys.tilemap.addSpecialTile (type, transform.position);
	}
		

	protected override void OnDestroy()
	{
		G.Sys.tilemap.delTile (transform.position, this);
		G.Sys.tilemap.delSpecialTile (type, transform.position);
	}
}

