using System;
using UnityEngine;

public class StairsState : ATravelerState
{
	enum Dir { UP, DOWN }

	const float stairsSpeedMultiplier = 0.75f;
	Vector3 stairsDir;
	ATile stair;
	Dir direction;
	int state;

	public StairsState (Traveler t) : base(t, StateType.TRAVELER_STAIRS)
	{

	}

	public override int check()
	{
		var tile = G.Sys.tilemap.connectableTile(traveler.transform.position);
		if (tile == null)
			return 0;
		if (tile.type != TileID.STAIRS)
			return 0;

		var next = new Vector3i(traveler.path.next (traveler.transform.position));
		if (next.y == new Vector3i (traveler.transform.position).y)
			return 0;

		return int.MaxValue;
	}

	public override void start ()
	{
		state = 0;
		var ePos = new Vector3i (traveler.transform.position);
		var next = new Vector3i (traveler.path.next (traveler.transform.position));
		var dir = new Vector3i (next.x - ePos.x, 0, next.z - ePos.z);
		if (Mathf.Abs (dir.x) > Mathf.Abs (dir.z))
			dir.z = 0;
		else
			dir.x = 0;

		dir.x = dir.x > 0 ? 1 : dir.x < 0 ? -1 : 0;
		dir.z = dir.z > 0 ? 1 : dir.z < 0 ? -1 : 0;

		stair = G.Sys.tilemap.GetTileOfTypeAt(traveler.transform.position + dir.toVector3(), TileID.STAIRS);
		if (stair == null) {
			traveler.BackToMoveState ();
			return;
		}
		if (stair.type != TileID.STAIRS){
			traveler.BackToMoveState ();
			return;
		}

		var pos = new Vector3i (stair.transform.position);
		stairsDir = Orienter.orientationToDir3 (Orienter.angleToOrientation (stair.transform.rotation.eulerAngles.y));
		var current = new Vector3i (traveler.transform.position);
		direction = pos.y == current.y ? Dir.UP : Dir.DOWN;
	}

	public override void update()
	{
		if (direction == Dir.UP)
			GoUp ();
		else
			GoDown ();
	}

	public override bool canBeStopped()
	{
		return false;
	}

	void GoUp()
	{
		switch (state) {
		case 0:
			{
				traveler.rigidbody.velocity = (stair.transform.position + stairsDir - traveler.transform.position).normalized * traveler.datas.Speed;
				traveler.transform.rotation = Quaternion.Euler (0, Quaternion.LookRotation (traveler.rigidbody.velocity, Vector3.up).eulerAngles.y, 0);

				if (G.Sys.tilemap.tilesOfTypeAt (traveler.transform.position, TileID.STAIRS).Count == 0) {
					traveler.Updatepath ();
					traveler.BackToMoveState ();
				}

				if (new Vector3i (traveler.transform.position).equal (new Vector3i (stair.transform.position + stairsDir)))
					state = 1;
			}
			break;
		case 1:
			{
				var dest = stair.transform.position - stairsDir + 2 * Vector3.up;
				traveler.rigidbody.velocity = (dest - traveler.transform.position).normalized * traveler.datas.Speed * stairsSpeedMultiplier;
				traveler.transform.rotation = Quaternion.Euler (0, Quaternion.LookRotation (new Vector3 (traveler.rigidbody.velocity.x, 0, traveler.rigidbody.velocity.z), Vector3.up).eulerAngles.y, 0);

				if (new Vector3i (dest).equal (new Vector3i (traveler.transform.position)))
					state = 2;
			}
			break;
		case 2:
			{
				traveler.Updatepath ();
				traveler.BackToMoveState ();
			}
			break;
		default:
			traveler.BackToMoveState ();
			break;
		}
	}

	void GoDown()
	{
		switch (state) {
		case 0:
			{
				traveler.rigidbody.velocity = (stair.transform.position + 2 * Vector3.up - traveler.transform.position).normalized * traveler.datas.Speed;
				traveler.transform.rotation = Quaternion.Euler (0, Quaternion.LookRotation (traveler.rigidbody.velocity, Vector3.up).eulerAngles.y, 0);

				if (G.Sys.tilemap.tilesOfTypeAt (traveler.transform.position, TileID.STAIRS).Count == 0) {
					traveler.Updatepath ();
					traveler.BackToMoveState ();
				}


				if (new Vector3i (traveler.transform.position).equal (new Vector3i (stair.transform.position + 2 * Vector3.up)))
					state = 1;
			}
			break;
		case 1:
			{
				var dest = stair.transform.position + 2 * stairsDir ;
				traveler.rigidbody.velocity = (dest - traveler.transform.position).normalized * traveler.datas.Speed * stairsSpeedMultiplier;
				traveler.transform.rotation = Quaternion.Euler (0, Quaternion.LookRotation (new Vector3 (traveler.rigidbody.velocity.x, 0, traveler.rigidbody.velocity.z), Vector3.up).eulerAngles.y, 0);

				if (new Vector3i (dest).equal (new Vector3i (traveler.transform.position)))
					state = 2;
			}
			break;
		case 2:
			{
				traveler.Updatepath ();
				traveler.BackToMoveState ();
			}
			break;
		default:
			traveler.BackToMoveState ();
			break;
		}
	}
}

