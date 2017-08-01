using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NRand;

public class Agent : AEntity
{
	public AgentStats stats = new AgentStats ();
	[HideInInspector]
	public AgentDatas datas = new AgentDatas ();

	protected override void OnAwake ()
	{
		//path.lostness = 0.5f;
		initializeDatas();
		agent.enabled = false;
	}

	protected override void OnUpdate ()
	{
		
	}

	void OnDestroy() {
		G.Sys.removeAgent (this);
	}

	public override void EnableAgent ()
	{
		base.EnableAgent ();
		G.Sys.registerAgent (this);
	}

	protected override void Check ()
	{
		if (!path.haveAction(ActionType.WAIT_ELEVATOR) && agent.enabled == true && path.destnation.y != transform.position.y) {
			var positions = G.Sys.tilemap.getSpecialTiles (TileID.ELEVATOR);

			if (positions.Count > 0) {
				foreach (var pos in positions) {
					if (Mathf.RoundToInt(pos.y) == Mathf.RoundToInt(transform.position.y)) {
						var elevator = G.Sys.tilemap.GetTileOfTypeAt(pos, TileID.ELEVATOR) as ElevatorTile;
						path.addAction (new WaitForElevatorAction (this, elevator.GetWaitZone (Mathf.RoundToInt(pos.y)), elevator, Mathf.RoundToInt(path.destnation.y)));

						break;
					}
				}
			}
		}
	}

	public void GoHelpTravelerAction(Traveler t) {
		var pos = t.transform.position;
		var dir = Vector3.Normalize (pos - transform.position);
		path.addAction(new HelpTravelerAction(this, pos - new Vector3(dir.x, 0, dir.y), t));
	}

	void initializeDatas()
	{
		datas.Speed = stats.MovementSpeed;
	}

	public void updateDatas(float time)
	{
		if (!path.CanStartAction())
			return;

		updateSpeed ();
	}

	void updateSpeed()
	{
		datas.Speed = stats.MovementSpeed;

		var tiles = G.Sys.tilemap.at (transform.position);
		if (tiles.Exists (t => t.type == TileID.ESCALATOR)) {
			agent.speed = G.Sys.constants.EscalatorSpeed;
		} else if (tiles.Exists (t => t.type == TileID.STAIRS)) {
			agent.speed = G.Sys.constants.StairsSpeedMultiplier * datas.Speed;
		} else
			agent.speed = datas.Speed;
	}

	protected override void OnPathFinished ()
	{
		path.destnation = G.Sys.tilemap.getRandomGroundTile ().transform.position;
	}
}