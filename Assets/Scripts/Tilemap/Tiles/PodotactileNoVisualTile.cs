using UnityEngine;
using System.Collections;

public class PodotactileNoVisualTile : ATile
{
	protected override void Awake()
	{
        type = TileID.PODOTACTILE;

		G.Sys.tilemap.addTile(transform.position, this, Tilemap.LOW_PRIORITY);

		G.Sys.tilemap.addSpecialTile(type, transform.position);
	}

	protected override void OnDestroy()
	{
		G.Sys.tilemap.delTile(transform.position, this);
		G.Sys.tilemap.delSpecialTile(TileID.PODOTACTILE, transform.position);
	}

}
