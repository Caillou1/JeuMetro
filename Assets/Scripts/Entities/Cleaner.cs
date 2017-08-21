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
		G.Sys.removeCleaner (this);
	}

	public override void EnableAgent ()
	{
		base.EnableAgent ();
		G.Sys.registerCleaner (this);
	}

	protected override void Check ()
	{
        if (!path.haveAction(ActionType.CLEAN_WASTE) && !path.haveAction(ActionType.CLEAN_BIN))
        {
            CheckWaste();
            CheckBin();
        }
	}

    void CheckWaste()
    {
        WasteTile wasteTile = null;
        foreach(var w in G.Sys.tilemap.getSurrondingSpecialTile(transform.position, TileID.WASTE, G.Sys.constants.WorkerDetectionRadius, G.Sys.constants.VerticalAmplification))
        {
            var waste = G.Sys.tilemap.GetTileOfTypeAt(w, TileID.WASTE) as WasteTile;
            if (waste.Targetted)
                continue;

			NavMeshPath p = new NavMeshPath();
            agent.CalculatePath(w, p);
			if (p.status != NavMeshPathStatus.PathComplete)
				continue;

            wasteTile = waste;
            break;
        }

        if (wasteTile == null)
            return;

        wasteTile.Targetted = true;
        path.addAction(new CleanWasteAction(this, wasteTile.transform.position, wasteTile));
    }

    void CheckBin()
    {
        BinTile binTile = null;

        foreach (var w in G.Sys.tilemap.getSurrondingSpecialTile(transform.position, TileID.BIN, G.Sys.constants.WorkerDetectionRadius, G.Sys.constants.VerticalAmplification))
		{
            var bin = G.Sys.tilemap.GetTileOfTypeAt(w, TileID.BIN) as BinTile;
            if (bin.Targetted || bin.waste < 0.5f)
				continue;

			NavMeshPath p = new NavMeshPath();
			agent.CalculatePath(w, p);
			if (p.status != NavMeshPathStatus.PathComplete)
				continue;

			binTile = bin;
			break;
		}

		if (binTile == null)
			return;

		binTile.Targetted = true;
        path.addAction(new CleanBinAction(this, binTile.transform.position, binTile));
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