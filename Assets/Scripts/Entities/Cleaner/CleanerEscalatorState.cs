using System;
using UnityEngine;

public class CleanerEscalatorState : ACleanerState
{
	enum Dir { UP, DOWN }

	Vector3 stairsDir;
	ATile escalator;
	Dir direction;
	int state;

	public CleanerEscalatorState (CleanerEntity t) : base(t, StateType.CLEANER_ESCALATOR)
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

		var tile = G.Sys.tilemap.GetTileOfTypeAt(cleaner.transform.position + dir.toVector3(), TileID.ESCALATOR);
		if (tile == null)
			return 0;
		if (tile.type != TileID.ESCALATOR)
			return 0;

		if (next.y == ePos.y)
			return 0;

		var escalator = tile as EscalatorTile;
		if(next.y > ePos.y && escalator.side == EscalatorSide.UP) 
			return int.MaxValue;
		if (next.y < ePos.y && escalator.side == EscalatorSide.DOWN)
			return int.MaxValue;
		
		return 0;
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

		escalator = G.Sys.tilemap.GetTileOfTypeAt(cleaner.transform.position + dir.toVector3(), TileID.ESCALATOR);
		if (escalator == null) {
			cleaner.BackToMoveState ();
			return;
		}
		if (escalator.type != TileID.ESCALATOR){
			cleaner.BackToMoveState ();
			return;
		}

		var pos = new Vector3i (escalator.transform.position);
		stairsDir = Orienter.orientationToDir3 (Orienter.angleToOrientation (escalator.transform.rotation.eulerAngles.y));
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
				cleaner.rigidbody.velocity = (escalator.transform.position + stairsDir - cleaner.transform.position).normalized * cleaner.Stats.MovementSpeed;
				cleaner.transform.rotation = Quaternion.Euler (0, Quaternion.LookRotation (cleaner.rigidbody.velocity, Vector3.up).eulerAngles.y, 0);

				if (G.Sys.tilemap.tilesOfTypeAt (cleaner.transform.position, TileID.ESCALATOR).Count == 0) {
					cleaner.Updatepath ();
					cleaner.BackToMoveState ();
				}

				if (new Vector3i (cleaner.transform.position).equal (new Vector3i (escalator.transform.position + stairsDir)))
					state = 1;
			}
			break;
		case 1:
			{
				if (escalator != null) {
					var dest = escalator.transform.position - stairsDir + 2 * Vector3.up;
					cleaner.rigidbody.velocity = (dest - cleaner.transform.position).normalized * G.Sys.constants.EscalatorSpeed;
					cleaner.transform.rotation = Quaternion.Euler (0, Quaternion.LookRotation (new Vector3 (cleaner.rigidbody.velocity.x, 0, cleaner.rigidbody.velocity.z), Vector3.up).eulerAngles.y, 0);

					if (new Vector3i (dest).equal (new Vector3i (cleaner.transform.position)))
						state = 2;
				}
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
				cleaner.rigidbody.velocity = (escalator.transform.position + 2 * Vector3.up - cleaner.transform.position).normalized * cleaner.Stats.MovementSpeed;
				cleaner.transform.rotation = Quaternion.Euler (0, Quaternion.LookRotation (cleaner.rigidbody.velocity, Vector3.up).eulerAngles.y, 0);

				if (G.Sys.tilemap.tilesOfTypeAt (cleaner.transform.position, TileID.ESCALATOR).Count == 0) {
					cleaner.Updatepath ();
					cleaner.BackToMoveState ();
				}

				if (new Vector3i (cleaner.transform.position).equal (new Vector3i (escalator.transform.position + 2 * Vector3.up)))
					state = 1;
			}
			break;
		case 1:
			{
				if (escalator != null) {
					var dest = escalator.transform.position + 2 * stairsDir;
					cleaner.rigidbody.velocity = (dest - cleaner.transform.position).normalized * G.Sys.constants.EscalatorSpeed;
					cleaner.transform.rotation = Quaternion.Euler (0, Quaternion.LookRotation (new Vector3 (cleaner.rigidbody.velocity.x, 0, cleaner.rigidbody.velocity.z), Vector3.up).eulerAngles.y, 0);

					if (new Vector3i (dest).equal (new Vector3i (cleaner.transform.position)))
						state = 2;
				}
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

