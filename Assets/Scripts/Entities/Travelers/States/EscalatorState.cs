using System;
using UnityEngine;

public class EscalatorState : ATravelerState
{
	enum Dir { UP, DOWN }

	const float escalatorSpeed = 3.0f;
	Vector3 stairsDir;
	ATile escalator;
	Dir direction;
	int state;

	public EscalatorState (Traveler t) : base(t, StateType.TRAVELER_ESCALATOR)
	{
		
	}

	public override int check()
	{
		var tile = G.Sys.tilemap.connectableTile(traveler.transform.position);
		if (tile == null)
			return 0;
		if (tile.type != TileID.ESCALATOR)
			return 0;

		var tPos = new Vector3i (traveler.transform.position);
		var next = new Vector3i(traveler.path.next (traveler.transform.position));
		if (next.y == tPos.y)
			return 0;

		var escalator = tile as EscalatorTile;
		if(next.y > tPos.y && escalator.side == EscalatorSide.UP) 
			return int.MaxValue;
		if (next.y < tPos.y && escalator.side == EscalatorSide.DOWN)
			return int.MaxValue;

		return 0;
	}

	public override void start ()
	{
		state = 0;
		escalator = G.Sys.tilemap.connectableTile(traveler.transform.position);
		if (escalator == null) {
			traveler.BackToMoveState ();
			return;
		}
		if (escalator.type != TileID.ESCALATOR){
			traveler.BackToMoveState ();
			return;
		}

		var pos = new Vector3i (escalator.transform.position);
		stairsDir = Orienter.orientationToDir3 (Orienter.angleToOrientation (escalator.transform.rotation.eulerAngles.y));
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
				traveler.rigidbody.velocity = (escalator.transform.position + stairsDir - traveler.transform.position).normalized * traveler.datas.Speed;
				traveler.transform.rotation = Quaternion.Euler (0, Quaternion.LookRotation (traveler.rigidbody.velocity, Vector3.up).eulerAngles.y, 0);

				if (G.Sys.tilemap.tilesOfTypeAt (traveler.transform.position, TileID.ESCALATOR).Count == 0) {
					traveler.Updatepath ();
					traveler.BackToMoveState ();
				}

				if (new Vector3i (traveler.transform.position).equal (new Vector3i (escalator.transform.position + stairsDir)))
					state = 1;
			}
			break;
		case 1:
			{
				var dest = escalator.transform.position - stairsDir + 2 * Vector3.up;
				traveler.rigidbody.velocity = (dest - traveler.transform.position).normalized * G.Sys.constants.EscalatorSpeed;
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
				traveler.rigidbody.velocity = (escalator.transform.position + 2 * Vector3.up - traveler.transform.position).normalized * traveler.datas.Speed;
				traveler.transform.rotation = Quaternion.Euler (0, Quaternion.LookRotation (traveler.rigidbody.velocity, Vector3.up).eulerAngles.y, 0);

				if (G.Sys.tilemap.tilesOfTypeAt (traveler.transform.position, TileID.ESCALATOR).Count == 0) {
					traveler.Updatepath ();
					traveler.BackToMoveState ();
				}

				if (new Vector3i (traveler.transform.position).equal (new Vector3i (escalator.transform.position + 2 * Vector3.up)))
					state = 1;
			}
			break;
		case 1:
			{
				var dest = escalator.transform.position + 2 * stairsDir ;
				traveler.rigidbody.velocity = (dest - traveler.transform.position).normalized * G.Sys.constants.EscalatorSpeed;
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

