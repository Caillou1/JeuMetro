using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using NRand;

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

	public Vector3 GetDownOfStairs() {
		var dir = Orienter.orientationToDir3(Orienter.angleToOrientation(transform.rotation.eulerAngles.y));
		float randomX, randomZ;
		randomX = (new UniformFloatDistribution (-.5f, .5f).Next (new StaticRandomGenerator<DefaultRandomGenerator> ()));
		randomZ = (new UniformFloatDistribution (-.5f, .5f).Next (new StaticRandomGenerator<DefaultRandomGenerator> ()));

		return (transform.position + dir * 2 + new Vector3(randomX, 0, randomZ));
	}

	public bool HasPodotactileOnFloor(int floor) {
		var dir = Orienter.orientationToDir3 (Orienter.angleToOrientation (transform.rotation.eulerAngles.y));
		Vector3 posToCheck;

		if (floor == Mathf.RoundToInt (transform.position.y)) {
			posToCheck = transform.position + dir * 2;
		} else {
			posToCheck = transform.position + Vector3.up * 2 - dir;
		}
			
		return G.Sys.tilemap.at (posToCheck).Exists (x => x.type == TileID.PODOTACTILE);
	}

	public bool IsOnStairsPath(Vector3i pos) {
		var dir = Orienter.orientationToDir3(Orienter.angleToOrientation(transform.rotation.eulerAngles.y));

		var right = new Vector3 (dir.z, dir.y, -dir.x);

		Vector3i v1 = new Vector3i (transform.position + dir * 2);
		Vector3i v2 = new Vector3i (transform.position + dir * 2 + right);
		Vector3i v3 = new Vector3i (transform.position + dir * 2 - right);
		Vector3i v4 = new Vector3i (transform.position - dir + Vector3.up * 2);
		Vector3i v5 = new Vector3i (transform.position - dir + Vector3.up * 2 + right);
		Vector3i v6 = new Vector3i (transform.position - dir + Vector3.up * 2 - right);

		return (pos.equal (v1) || pos.equal (v4) || pos.equal(v2) || pos.equal(v3) || pos.equal(v5) || pos.equal(v6));
	}
}
