using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class StairsTile : ATile
{
	protected override void Awake()
	{
		type = TileID.STAIRS;

		var dir = Orienter.orientationToDir3(Orienter.angleToOrientation(transform.rotation.eulerAngles.y));

		G.Sys.tilemap.addTile (transform.position, this, Tilemap.STAIRS_PRIORITY);
		G.Sys.tilemap.addTile (transform.position + dir, this, Tilemap.STAIRS_PRIORITY);
		G.Sys.tilemap.addTile (transform.position + Vector3.up, this, Tilemap.STAIRS_PRIORITY);
		G.Sys.tilemap.addTile (transform.position + Vector3.up + dir, this, Tilemap.STAIRS_PRIORITY);
		G.Sys.tilemap.addTile (transform.position + 2 * Vector3.up, this, Tilemap.STAIRS_PRIORITY);
		G.Sys.tilemap.addTile (transform.position + 2 * Vector3.up + dir, this, Tilemap.STAIRS_PRIORITY);
	}

	protected override void OnDestroy()
	{
		var dir = Orienter.orientationToDir3(Orienter.angleToOrientation(transform.rotation.eulerAngles.y));

		G.Sys.tilemap.delTile (transform.position, this);
		G.Sys.tilemap.delTile (transform.position + dir, this);
		G.Sys.tilemap.delTile (transform.position + Vector3.up, this);
		G.Sys.tilemap.delTile (transform.position + Vector3.up + dir, this);
		G.Sys.tilemap.delTile (transform.position + 2 * Vector3.up, this);
		G.Sys.tilemap.delTile (transform.position + 2 * Vector3.up + dir, this);
	}
}
