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

		G.Sys.tilemap.addTile (transform.position, this, false, true, Tilemap.STAIRS_PRIORITY);
		G.Sys.tilemap.addTile (transform.position + dir, this, false, true, Tilemap.STAIRS_PRIORITY);
		G.Sys.tilemap.addTile (transform.position + 2 * dir, this, true, false, Tilemap.STAIRS_PRIORITY);
		G.Sys.tilemap.addTile (transform.position + Vector3.up, this, false, true, Tilemap.STAIRS_PRIORITY);
		G.Sys.tilemap.addTile (transform.position + Vector3.up + dir, this, false, true, Tilemap.STAIRS_PRIORITY);
		G.Sys.tilemap.addTile (transform.position + 2 * Vector3.up, this, false, true, Tilemap.STAIRS_PRIORITY);
		G.Sys.tilemap.addTile (transform.position + 2 * Vector3.up - dir, this, true, false, Tilemap.STAIRS_PRIORITY);

		foreach(var t in G.Sys.tilemap.at(transform.position))
			t.Connect();
		foreach(var t in G.Sys.tilemap.at(transform.position + dir))
			t.Connect();
		foreach(var t in G.Sys.tilemap.at(transform.position + 2 * dir))
			t.Connect();
		foreach(var t in G.Sys.tilemap.at(transform.position + Vector3.up))
			t.Connect();
		foreach(var t in G.Sys.tilemap.at(transform.position + Vector3.up + dir))
			t.Connect();
		foreach(var t in G.Sys.tilemap.at(transform.position + 2 * Vector3.up))
			t.Connect();
		foreach(var t in G.Sys.tilemap.at(transform.position + 2 * Vector3.up - dir))
			t.Connect();
	}

	public override void Connect ()
	{
		base.Connect ();

		List<Pair<ATile, Vector3i>> connexions = new List<Pair<ATile, Vector3i>>();

		var dir = Orienter.orientationToDir3(Orienter.angleToOrientation(transform.rotation.eulerAngles.y));
		var perpendicularDir = new Vector3 (-dir.z, dir.y, dir.x);

		Add(transform.position + 3 * dir, connexions);
		Add(transform.position + 2 * dir + perpendicularDir, connexions);
		Add(transform.position + 2 * dir - perpendicularDir, connexions);

		Add(transform.position + 2 * Vector3.up - 2 * dir, connexions);
		Add(transform.position + 2 * Vector3.up - dir + perpendicularDir, connexions);
		Add(transform.position + 2 * Vector3.up - dir - perpendicularDir, connexions);

		applyConnexions (connexions);
	}

	protected override void OnDestroy()
	{
		var dir = Orienter.orientationToDir3(Orienter.angleToOrientation(transform.rotation.eulerAngles.y));

		G.Sys.tilemap.delTile (transform.position, this);
		G.Sys.tilemap.delTile (transform.position + dir, this);
		G.Sys.tilemap.delTile (transform.position + 2 * dir, this);
		G.Sys.tilemap.delTile (transform.position + Vector3.up, this);
		G.Sys.tilemap.delTile (transform.position + Vector3.up + dir, this);
		G.Sys.tilemap.delTile (transform.position + 2 * Vector3.up, this);
		G.Sys.tilemap.delTile (transform.position + 2 * Vector3.up - dir, this);

		foreach(var t in G.Sys.tilemap.at(transform.position))
			t.Connect();
		foreach(var t in G.Sys.tilemap.at(transform.position + dir))
			t.Connect();
		foreach(var t in G.Sys.tilemap.at(transform.position + 2 * dir))
			t.Connect();
		foreach(var t in G.Sys.tilemap.at(transform.position + Vector3.up))
			t.Connect();
		foreach(var t in G.Sys.tilemap.at(transform.position + Vector3.up + dir))
			t.Connect();
		foreach(var t in G.Sys.tilemap.at(transform.position + 2 * Vector3.up))
			t.Connect();
		foreach(var t in G.Sys.tilemap.at(transform.position + 2 * Vector3.up - dir))
			t.Connect();

		foreach (var t in connectedTiles)
			t.First.targetOf.Remove (this);
		foreach (var t in targetOf.ToList())
			t.Connect ();
	}
}
