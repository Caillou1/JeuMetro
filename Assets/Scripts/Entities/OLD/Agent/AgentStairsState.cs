using System;
using UnityEngine;

namespace V1
{
public class AgentStairsState : AAgentState
{
	enum Dir { UP, DOWN }

	Vector3 stairsDir;
	ATile stair;
	Dir direction;
	int state;

	public AgentStairsState (AgentEntity t) : base(t, StateType.AGENT_STAIRS)
	{

	}

	public override int check()
	{
		
		var tile = G.Sys.tilemap.GetTileOfTypeAt(agent.transform.position, TileID.ESCALATOR);
		if (tile == null)
			return 0;
		if (tile.type != TileID.STAIRS)
			return 0;

		var next = new Vector3i(agent.path.next (agent.transform.position));
		if (next.y == new Vector3i (agent.transform.position).y)
			return 0;

		return int.MaxValue;
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

		stair = G.Sys.tilemap.GetTileOfTypeAt(agent.transform.position + dir.toVector3(), TileID.STAIRS);
		if (stair == null) {
			agent.BackToMoveState ();
			return;
		}
		if (stair.type != TileID.STAIRS){
			agent.BackToMoveState ();
			return;
		}

		var pos = new Vector3i (stair.transform.position);
		stairsDir = Orienter.orientationToDir3 (Orienter.angleToOrientation (stair.transform.rotation.eulerAngles.y));
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
				agent.rigidbody.velocity = (stair.transform.position + stairsDir - agent.transform.position).normalized * agent.Stats.MovementSpeed;
				agent.transform.rotation = Quaternion.Euler (0, Quaternion.LookRotation (agent.rigidbody.velocity, Vector3.up).eulerAngles.y, 0);
					
				if (G.Sys.tilemap.tilesOfTypeAt (agent.transform.position, TileID.STAIRS).Count == 0) {
					agent.Updatepath ();
					agent.BackToMoveState ();
				}

				if (new Vector3i (agent.transform.position).equal (new Vector3i (stair.transform.position + stairsDir)))
					state = 1;
			}
			break;
		case 1:
			{
				var dest = stair.transform.position - stairsDir + 2 * Vector3.up;
				agent.rigidbody.velocity = (dest - agent.transform.position).normalized * agent.Stats.MovementSpeed * G.Sys.constants.StairsSpeedMultiplier;
				agent.transform.rotation = Quaternion.Euler (0, Quaternion.LookRotation (new Vector3 (agent.rigidbody.velocity.x, 0, agent.rigidbody.velocity.z), Vector3.up).eulerAngles.y, 0);

				if (G.Sys.tilemap.tilesOfTypeAt (agent.transform.position, TileID.STAIRS).Count == 0) {
					agent.Updatepath ();
					agent.BackToMoveState ();
				}

				if (new Vector3i (dest).equal (new Vector3i (agent.transform.position)))
					state = 2;
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
				agent.rigidbody.velocity = (stair.transform.position + 2 * Vector3.up - agent.transform.position).normalized * agent.Stats.MovementSpeed;
				agent.transform.rotation = Quaternion.Euler (0, Quaternion.LookRotation (agent.rigidbody.velocity, Vector3.up).eulerAngles.y, 0);

				if (new Vector3i (agent.transform.position).equal (new Vector3i (stair.transform.position + 2 * Vector3.up)))
					state = 1;
			}
			break;
		case 1:
			{
				var dest = stair.transform.position + 2 * stairsDir ;
				agent.rigidbody.velocity = (dest - agent.transform.position).normalized * agent.Stats.MovementSpeed * G.Sys.constants.StairsSpeedMultiplier;
				agent.transform.rotation = Quaternion.Euler (0, Quaternion.LookRotation (new Vector3 (agent.rigidbody.velocity.x, 0, agent.rigidbody.velocity.z), Vector3.up).eulerAngles.y, 0);

				if (new Vector3i (dest).equal (new Vector3i (agent.transform.position)))
					state = 2;
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