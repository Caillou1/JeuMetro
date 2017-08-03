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
		if (!path.haveAction (ActionType.WAIT_ELEVATOR)) {
			var possiblePath = G.Sys.tilemap.GetElevatorsToFloor (transform.position, path.destnation);
			if (possiblePath.Count > 0) {
				for (int i = 0; i < possiblePath.Count; i++) {
					Vector3 pos = (i == 0) ? possiblePath [i].Second.GetWaitZone (Mathf.RoundToInt (transform.position.y)) : possiblePath [i].Second.GetWaitZone (possiblePath [i - 1].First);
					ElevatorTile tile = possiblePath [i].Second;
					int floor = ((i + 1) < possiblePath.Count) ? Mathf.RoundToInt (possiblePath [i + 1].Second.GetWaitZone (possiblePath [i].First).y) : Mathf.RoundToInt (path.destnation.y);
					int priority = (possiblePath.Count - i) * 2;

					path.addAction (new WaitForElevatorAction (this, pos, tile, floor, priority));
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