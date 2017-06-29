﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class BenchTile : ATile
{
	protected override void Awake()
	{
		var dir = Orienter.orientationToDir(Orienter.angleToOrientation(transform.rotation.eulerAngles.y));

		type = TileID.BENCH;

		G.Sys.tilemap.addTile (transform.position, this, false, true, Tilemap.BENCH_PRIORITY);
		G.Sys.tilemap.addTile (transform.position + new Vector3(dir.x, 0, dir.y), this, false, true, Tilemap.BENCH_PRIORITY);

		foreach (var t in G.Sys.tilemap.at(transform.position))
			t.Connect ();
		foreach (var t in G.Sys.tilemap.at(transform.position + new Vector3(dir.x, 0, dir.y)))
			t.Connect ();

		G.Sys.tilemap.addSpecialTile (type, transform.position);
		G.Sys.tilemap.addSpecialTile (type, transform.position + new Vector3 (dir.x, 0, dir.y));
    }

	protected override void OnDestroy()
	{
		var dir = Orienter.orientationToDir(Orienter.angleToOrientation(transform.rotation.eulerAngles.y));

		G.Sys.tilemap.delTile (transform.position, this);
		G.Sys.tilemap.delTile (transform.position + new Vector3(dir.x, 0, dir.y), this);

		foreach (var t in G.Sys.tilemap.at(transform.position))
			t.Connect ();
		foreach (var t in G.Sys.tilemap.at(transform.position + new Vector3(dir.x, 0, dir.y)))
			t.Connect ();

		G.Sys.tilemap.delSpecialTile (type, transform.position);
		G.Sys.tilemap.delSpecialTile (type, transform.position + new Vector3 (dir.x, 0, dir.y));
	}
}