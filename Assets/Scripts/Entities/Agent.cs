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

    bool endInvoked = false;

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

	void Start() {
		GetComponent<DragAndDropEntity> ().ToggleOutline (false);
		Invoke ("EnableAgent", 1f);

		if (G.Sys.constants.TravelerColors.Count > 0)
			GetComponentInChildren<SkinnedMeshRenderer> ().material.color = G.Sys.constants.TravelerColors [(new UniformIntDistribution (G.Sys.constants.TravelerColors.Count - 1).Next (new StaticRandomGenerator<DefaultRandomGenerator> ()))];
		else
			GetComponentInChildren<SkinnedMeshRenderer> ().material.color = Color.HSVToRGB ((new UniformFloatDistribution (0f, 1f).Next (new StaticRandomGenerator<DefaultRandomGenerator> ())), 1f, 1f);
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

		if (G.Sys.gameManager.FireAlert && G.Sys.travelerCount() == 0 && !endInvoked)
		{
			Invoke("OnAllTravelersExited", new UniformFloatDistribution(G.Sys.constants.MinTravelerFireAlertDelay, G.Sys.constants.MaxTravelerFireAlertDelay).Next(new StaticRandomGenerator<DefaultRandomGenerator>()));
			endInvoked = true;
		}
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

	void OnAllTravelersExited()
	{
		agent.agentTypeID = 0;

		var comp = gameObject.AddComponent<Traveler>();

		var tStats = new TravelerStats();
		tStats.FaintnessPercentage = 0;
		tStats.LostAbility = 0;
		tStats.MovementSpeed = stats.MovementSpeed;
		tStats.Type = TravelerType.CLASSIC;
		comp.stats = tStats;

		var tDatas = new TravelerDatas();
		tDatas.Lostness = 0;
		tDatas.Tiredness = 0;
		comp.datas = tDatas;

		comp.Invoke("StartFireAlert", 0.1f);

		Destroy(this);
	}
}