using System;
using UnityEngine;

namespace V1
{
public class CleanerStairsState : ACleanerState
{
	enum Dir { UP, DOWN }

	Vector3 stairsDir;
	ATile stair;
	Dir direction;
	int state;

	public CleanerStairsState (CleanerEntity t) : base(t, StateType.CLEANER_STAIRS)
	{

	}

	public override int check()
	{
		var ePos = new Vector3i (cleaner.transform.position);
		var next = new Vector3i (cleaner.path.next (cleaner.transform.position));
		var dir = new Vector3i (next.x - ePos.x, 0, next.z - ePos.z);
		if (Mathf.Abs (dir.x) > Mathf.Abs (dir.z))
			dir.z = 0;
		else
			dir.x = 0;

		dir.x = dir.x > 0 ? 1 : dir.x < 0 ? -1 : 0;
		dir.z = dir.z > 0 ? 1 : dir.z < 0 ? -1 : 0;

		var tile = G.Sys.tilemap.GetTileOfTypeAt(cleaner.transform.position + dir.toVector3(), TileID.STAIRS);
		if (tile == null)
			return 0;
		if (tile.type != TileID.STAIRS)
			return 0;

		if (next.y == ePos.y)
			return 0;

		return int.MaxValue;
	}

	public override void start ()
	{
		state = 0;
		var ePos = new Vector3i (cleaner.transform.position);
		var next = new Vector3i (cleaner.path.next (cleaner.transform.position));
		var dir = new Vector3i (next.x - ePos.x, 0, next.z - ePos.z);
		if (Mathf.Abs (dir.x) > Mathf.Abs (dir.z))
			dir.z = 0;
		else
			dir.x = 0;

		dir.x = dir.x > 0 ? 1 : dir.x < 0 ? -1 : 0;
		dir.z = dir.z > 0 ? 1 : dir.z < 0 ? -1 : 0;

		stair = G.Sys.tilemap.GetTileOfTypeAt(cleaner.transform.position + dir.toVector3(), TileID.STAIRS);
		if (stair == null) {
			cleaner.BackToMoveState ();
			return;
		}
		if (stair.type != TileID.STAIRS){
			cleaner.BackToMoveState ();
			return;
		}

		var pos = new Vector3i (stair.transform.position);
		stairsDir = Orienter.orientationToDir3 (Orienter.angleToOrientation (stair.transform.rotation.eulerAngles.y));
		var current = new Vector3i (cleaner.transform.position);
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
				cleaner.rigidbody.velocity = (stair.transform.position + stairsDir - cleaner.transform.position).normalized * cleaner.Stats.MovementSpeed;
				cleaner.transform.rotation = Quaternion.Euler (0, Quaternion.LookRotation (cleaner.rigidbody.velocity, Vector3.up).eulerAngles.y, 0);
					
				if (G.Sys.tilemap.tilesOfTypeAt (cleaner.transform.position, TileID.STAIRS).Count == 0) {
					cleaner.Updatepath ();
					cleaner.BackToMoveState ();
				}

				if (new Vector3i (cleaner.transform.position).equal (new Vector3i (stair.transform.position + stairsDir)))
					state = 1;
			}
			break;
		case 1:
			{
				var dest = stair.transform.position - stairsDir + 2 * Vector3.up;
				cleaner.rigidbody.velocity = (dest - cleaner.transform.position).normalized * cleaner.Stats.MovementSpeed * G.Sys.constants.StairsSpeedMultiplier;
				cleaner.transform.rotation = Quaternion.Euler (0, Quaternion.LookRotation (new Vector3 (cleaner.rigidbody.velocity.x, 0, cleaner.rigidbody.velocity.z), Vector3.up).eulerAngles.y, 0);

				if (G.Sys.tilemap.tilesOfTypeAt (cleaner.transform.position, TileID.STAIRS).Count == 0) {
					cleaner.Updatepath ();
					cleaner.BackToMoveState ();
				}

				if (new Vector3i (dest).equal (new Vector3i (cleaner.transform.position)))
					state = 2;
			}
			break;
		case 2:
			{
				cleaner.Updatepath ();
				cleaner.BackToMoveState ();
			}
			break;
		default:
			cleaner.BackToMoveState ();
			break;
		}
	}

	void GoDown()
	{
		switch (state) {
		case 0:
			{
				cleaner.rigidbody.velocity = (stair.transform.position + 2 * Vector3.up - cleaner.transform.position).normalized * cleaner.Stats.MovementSpeed;
				cleaner.transform.rotation = Quaternion.Euler (0, Quaternion.LookRotation (cleaner.rigidbody.velocity, Vector3.up).eulerAngles.y, 0);

				if (new Vector3i (cleaner.transform.position).equal (new Vector3i (stair.transform.position + 2 * Vector3.up)))
					state = 1;
			}
			break;
		case 1:
			{
				var dest = stair.transform.position + 2 * stairsDir ;
				cleaner.rigidbody.velocity = (dest - cleaner.transform.position).normalized * cleaner.Stats.MovementSpeed * G.Sys.constants.StairsSpeedMultiplier;
				cleaner.transform.rotation = Quaternion.Euler (0, Quaternion.LookRotation (new Vector3 (cleaner.rigidbody.velocity.x, 0, cleaner.rigidbody.velocity.z), Vector3.up).eulerAngles.y, 0);

				if (new Vector3i (dest).equal (new Vector3i (cleaner.transform.position)))
					state = 2;
			}
			break;
		case 2:
			{
				cleaner.Updatepath ();
				cleaner.BackToMoveState ();
			}
			break;
		default:
			cleaner.BackToMoveState ();
			break;
		}
	}
}

}