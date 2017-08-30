using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NRand;
using UnityEngine.AI;

public class Cleaner : AEntity
{
	public CleanerStats stats = new CleanerStats ();
	[HideInInspector]
	public CleanerDatas datas = new CleanerDatas ();

	private Animator anim;
	private Vector3 lastPos;

    bool endInvoked = false;

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

        GetComponentInChildren<SkinnedMeshRenderer>().material.color = G.Sys.constants.GetRandomColor(G.Sys.constants.agentSaturation);
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
		G.Sys.removeCleaner (this);
	}

	public override void EnableAgent ()
	{
		base.EnableAgent ();
		G.Sys.registerCleaner (this);
	}

	protected override void Check ()
	{
		if (enabled && !path.haveAction(ActionType.CLEAN_WASTE) && !path.haveAction(ActionType.CLEAN_BIN))
        {
            CheckWaste();
            CheckBin();
        }
	}

    void CheckWaste()
    {
        WasteTile wasteTile = null;
        float dist = float.MaxValue;
		var wastes = new List<Vector3> ();

		if (G.Sys.constants != null) {
			wastes = G.Sys.tilemap.getSurrondingSpecialTile (transform.position, TileID.WASTE, G.Sys.constants.WorkerDetectionRadius, G.Sys.constants.VerticalAmplification);
		}

        foreach(var w in wastes)
        {
            var waste = G.Sys.tilemap.GetTileOfTypeAt(w, TileID.WASTE) as WasteTile;
            if (waste.Targetted)
                continue;

			NavMeshPath p = new NavMeshPath();
            agent.CalculatePath(w, p);
			if (p.status != NavMeshPathStatus.PathComplete)
				continue;

            float d = (transform.position - w).sqrMagnitude;
            if (d < dist)
            {
				wasteTile = waste;
                dist = d;
            }

            break;
        }

		if (wasteTile == null) {
			return;
		}

        wasteTile.Targetted = true;
        path.addAction(new CleanWasteAction(this, wasteTile.transform.position, wasteTile));
    }

    void CheckBin()
    {
        BinTile binTile = null;
		if (G.Sys.constants != null) {
			foreach (var w in G.Sys.tilemap.getSurrondingSpecialTile(transform.position, TileID.BIN, G.Sys.constants.WorkerDetectionRadius, G.Sys.constants.VerticalAmplification)) {
				var bin = G.Sys.tilemap.GetTileOfTypeAt (w, TileID.BIN) as BinTile;
				if (bin.Targetted || bin.waste < 0.5f)
					continue;

				NavMeshPath p = new NavMeshPath ();
				agent.CalculatePath (w, p);
				if (p.status != NavMeshPathStatus.PathComplete)
					continue;

				binTile = bin;
				break;
			}

			if (binTile == null)
				return;

			binTile.Targetted = true;
			path.addAction (new CleanBinAction (this, binTile.transform.position, binTile));
		}
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

    protected override void OnPathFinished()
    {
        path.destnation = G.Sys.tilemap.getRandomGroundTile().transform.position;
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