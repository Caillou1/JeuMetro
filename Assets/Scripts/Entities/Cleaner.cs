﻿using System;
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

        if (G.Sys.gameManager.FireAlert && G.Sys.tilemap.GetTileOfTypeAt(transform.position, TileID.OUT))
        {
            Destroy(gameObject);
            return;
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
}