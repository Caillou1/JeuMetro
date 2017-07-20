using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class BenchTile : ATile
{
	public enum Side
	{
		LEFT,
		RIGHT,
	}

	private AEntity leftEntity = null;
	private AEntity rightEntity = null;

	protected override void Awake()
	{
		var dir = Orienter.orientationToDir(Orienter.angleToOrientation(transform.rotation.eulerAngles.y));

		type = TileID.BENCH;

		G.Sys.tilemap.addTile (transform.position, this, Tilemap.BENCH_PRIORITY);
		G.Sys.tilemap.addTile (transform.position + new Vector3(dir.x, 0, dir.y), this, Tilemap.BENCH_PRIORITY);

		G.Sys.tilemap.addSpecialTile (type, transform.position);
		G.Sys.tilemap.addSpecialTile (type, transform.position + new Vector3 (dir.x, 0, dir.y));
    }

	protected override void OnDestroy()
	{
		var dir = Orienter.orientationToDir(Orienter.angleToOrientation(transform.rotation.eulerAngles.y));

		G.Sys.tilemap.delTile (transform.position, this);
		G.Sys.tilemap.delTile (transform.position + new Vector3(dir.x, 0, dir.y), this);

		G.Sys.tilemap.delSpecialTile (type, transform.position);
		G.Sys.tilemap.delSpecialTile (type, transform.position + new Vector3 (dir.x, 0, dir.y));
	}

	public bool sit(Side s, AEntity e)
	{
		var eSit = s == Side.LEFT ? leftEntity : rightEntity;

		if (eSit == e)
			return true;
		if (eSit != null)
			return false;

		if (s == Side.LEFT)
			leftEntity = e;
		else
			rightEntity = e;

		return true;
	}

	public void leave(Side s, AEntity e)
	{
		if ((s == Side.LEFT ? leftEntity : rightEntity) != e)
			return;

		if (s == Side.LEFT)
		leftEntity = null;
		else
			rightEntity = null;
	}

	public bool canSit(Side s)
	{
		return (s == Side.LEFT ? leftEntity : rightEntity) == null;
	}

	public List<Side> freePlaces()
	{
		List<Side> sides = new List<Side> ();
		if (leftEntity == null)
			sides.Add (Side.LEFT);
		if (rightEntity == null)
			sides.Add (Side.RIGHT);
		return sides;
	}

	public Side posToSide(Vector3 pos)
	{
		if (new Vector3i (pos).equal (new Vector3i (transform.position)))
			return Side.LEFT;
		return Side.RIGHT;
	}

	public Vector3 sideToPos(Side s)
	{
		return transform.position + (s == Side.LEFT ? Vector3.zero : Orienter.orientationToDir3 (Orienter.angleToOrientation (transform.rotation.eulerAngles.y))) ;
	}
}