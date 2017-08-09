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

	private Animator anim;
	private Vector3 lastPos;

	public bool IsHelping {
		get {
			return path.haveAction (ActionType.HELP_TRAVELER);
		}
	}

	protected override void OnAwake ()
	{
		//path.lostness = 0.5f;
		initializeDatas();
		agent.enabled = false;
		anim = GetComponentInChildren<Animator> ();
	}

	protected override void OnUpdate ()
	{
		anim.SetFloat ("MovementSpeed", agent.velocity.magnitude);

		if ((lastPos - transform.position).magnitude >= .001f) {
			anim.SetBool ("Walking", true);
		} else {
			anim.SetBool ("Walking", false);
		}

		lastPos = transform.position;
	}

	void OnDestroy() {
		G.Sys.removeAgent (this);
	}

	public override void EnableAgent ()
	{
		base.EnableAgent ();
		G.Sys.registerAgent (this);
	}

	public void GoHelpTravelerAction(Traveler t) {
		var pos = t.transform.position;
		var dir = Vector3.Normalize (pos - transform.position);
		path.addAction(new HelpTravelerAction(this, pos - new Vector3(dir.x, 0, dir.y)*.5f, t));
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