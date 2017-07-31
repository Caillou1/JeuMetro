using System;
using UnityEngine;

namespace V1
{
public class AgentEscalatorState : AAgentState
{
	enum Dir { UP, DOWN }

	Vector3 stairsDir;
	ATile escalator;
	Dir direction;
	int state;

	public AgentEscalatorState (AgentEntity t) : base(t, StateType.AGENT_ESCALATOR)
	{
		
	}

	public override int check()
	{
		var tile = G.Sys.tilemap.GetTileOfTypeAt(agent.transform.position, TileID.ESCALATOR);
		if (tile == null)
			return 0;
		if (tile.type != TileID.ESCALATOR)
			return 0;

		var tPos = new Vector3i (agent.transform.position);
		var next = new Vector3i(agent.path.next (agent.transform.position));
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
		var ePos = new Vector3i (agent.transform.position);
		var next = new Vector3i (agent.path.next (agent.transform.position));
		var dir = new Vector3i (next.x - ePos.x, 0, next.z - ePos.z);
		if (Mathf.Abs (dir.x) > Mathf.Abs (dir.z))
			dir.z = 0;
		else
			dir.x = 0;

		dir.x = dir.x > 0 ? 1 : dir.x < 0 ? -1 : 0;
		dir.z = dir.z > 0 ? 1 : dir.z < 0 ? -1 : 0;

		escalator = G.Sys.tilemap.GetTileOfTypeAt(agent.transform.position + dir.toVector3(), TileID.ESCALATOR);
		if (escalator == null) {
			agent.BackToMoveState ();
			return;
		}
		if (escalator.type != TileID.ESCALATOR){
			agent.BackToMoveState ();
			return;
		}

		var pos = new Vector3i (escalator.transform.position);
		stairsDir = Orienter.orientationToDir3 (Orienter.angleToOrientation (escalator.transform.rotation.eulerAngles.y));
		var current = new Vector3i (agent.transform.position);
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
				agent.rigidbody.velocity = (escalator.transform.position + stairsDir - agent.transform.position).normalized * agent.Stats.MovementSpeed;
				agent.transform.rotation = Quaternion.Euler (0, Quaternion.LookRotation (agent.rigidbody.velocity, Vector3.up).eulerAngles.y, 0);

				if (G.Sys.tilemap.tilesOfTypeAt (agent.transform.position, TileID.ESCALATOR).Count == 0) {
					agent.Updatepath ();
					agent.BackToMoveState ();
				}

				if (new Vector3i (agent.transform.position).equal (new Vector3i (escalator.transform.position + stairsDir)))
					state = 1;
			}
			break;
		case 1:
			{
				if (escalator != null) {
					var dest = escalator.transform.position - stairsDir + 2 * Vector3.up;
					agent.rigidbody.velocity = (dest - agent.transform.position).normalized * G.Sys.constants.EscalatorSpeed;
					agent.transform.rotation = Quaternion.Euler (0, Quaternion.LookRotation (new Vector3 (agent.rigidbody.velocity.x, 0, agent.rigidbody.velocity.z), Vector3.up).eulerAngles.y, 0);

					if (new Vector3i (dest).equal (new Vector3i (agent.transform.position)))
						state = 2;
				}
			}
			break;
		case 2:
			{
				agent.Updatepath ();
				agent.BackToMoveState ();
			}
			break;
		default:
			agent.BackToMoveState ();
			break;
		}
	}

	void GoDown()
	{
		switch (state) {
		case 0:
			{
				agent.rigidbody.velocity = (escalator.transform.position + 2 * Vector3.up - agent.transform.position).normalized * agent.Stats.MovementSpeed;
				agent.transform.rotation = Quaternion.Euler (0, Quaternion.LookRotation (agent.rigidbody.velocity, Vector3.up).eulerAngles.y, 0);

				if (G.Sys.tilemap.tilesOfTypeAt (agent.transform.position, TileID.ESCALATOR).Count == 0) {
					agent.Updatepath ();
					agent.BackToMoveState ();
				}

				if (new Vector3i (agent.transform.position).equal (new Vector3i (escalator.transform.position + 2 * Vector3.up)))
					state = 1;
			}
			break;
		case 1:
			{
				if (escalator != null) {
					var dest = escalator.transform.position + 2 * stairsDir;
					agent.rigidbody.velocity = (dest - agent.transform.position).normalized * G.Sys.constants.EscalatorSpeed;
					agent.transform.rotation = Quaternion.Euler (0, Quaternion.LookRotation (new Vector3 (agent.rigidbody.velocity.x, 0, agent.rigidbody.velocity.z), Vector3.up).eulerAngles.y, 0);

					if (new Vector3i (dest).equal (new Vector3i (agent.transform.position)))
						state = 2;
				}
			}
			break;
		case 2:
			{
				agent.Updatepath ();
				agent.BackToMoveState ();
			}
			break;
		default:
			agent.BackToMoveState ();
			break;
		}
	}
}
}

