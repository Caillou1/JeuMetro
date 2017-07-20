using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class ExitsTile : ATile
{
	[Tooltip("Must be IN OUT or METRO")]
	public TileID id = TileID.IN;

	protected override void Awake()
	{
		if (id != TileID.IN && id != TileID.OUT && id != TileID.METRO)
			throw new ArgumentOutOfRangeException ("The tile must be IN, OUT or METRO !");
		
		type = id;

		G.Sys.tilemap.addTile (transform.position, this, Tilemap.EXITS_PRIORITY);

		G.Sys.tilemap.addSpecialTile (type, transform.position);
	}
		

	protected override void OnDestroy()
	{
		G.Sys.tilemap.delTile (transform.position, this);
		G.Sys.tilemap.delSpecialTile (type, transform.position);
	}
}

