using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class BenchTile : ATile
{
	void Awake()
    {
		type = TileID.BENCH;

		var dir = Orienter.orientationToDir(Orienter.angleToOrientation(transform.rotation.eulerAngles.y));
		G.Sys.tilemap.addTile (transform.position, this, false, true, Tilemap.BENCH_PRIORITY);
		G.Sys.tilemap.addTile (transform.position + new Vector3(dir.x, 0, dir.y), this, false, true, Tilemap.BENCH_PRIORITY);

		foreach (var t in G.Sys.tilemap.at(transform.position))
			t.Connect ();
		foreach (var t in G.Sys.tilemap.at(transform.position + new Vector3(dir.x, 0, dir.y)))
			t.Connect ();
    }

	public override void Connect (){}

	void OnDestroy()
	{
		var dir = Orienter.orientationToDir(Orienter.angleToOrientation(transform.rotation.eulerAngles.y));

		G.Sys.tilemap.delTile (transform.position, this);
		G.Sys.tilemap.delTile (transform.position + new Vector3(dir.x, 0, dir.y), this);

		foreach (var t in G.Sys.tilemap.at(transform.position))
			t.Connect ();
		foreach (var t in G.Sys.tilemap.at(transform.position + new Vector3(dir.x, 0, dir.y)))
			t.Connect ();
	}
}