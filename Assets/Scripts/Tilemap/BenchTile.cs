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
		G.Sys.tilemap.addTile (transform.position + new Vector3(dir.y, 0, dir.x), this, false, true, Tilemap.BENCH_PRIORITY);
    }

	public override void Connect ()
	{
		
	}

	void OnDestroy()
	{


		/*var dir = Orienter.orientationToDir(Orienter.angleToOrientation(transform.rotation.eulerAngles.y));
		G.Sys.tilemap.delTile (transform.position, this);
		G.Sys.tilemap.delTile (transform.position + new Vector3 (dir.x, 0, dir.y), this);
		G.Sys.tilemap.delTile (transform.position + new Vector3 (-dir.x, 1, -dir.y), this);
		G.Sys.tilemap.delTile (transform.position + new Vector3 (dir.x, 0, dir.y), this);
		G.Sys.tilemap.delTile (transform.position + new Vector3 (-2 * dir.x, 1, -2 * dir.y), this);
		G.Sys.tilemap.delTile (transform.position + new Vector3 (2 * dir.x, 0, 2 * dir.y), this);
		foreach (var t in targetOf.ToList())
			t.Connect ();*/
	}
}