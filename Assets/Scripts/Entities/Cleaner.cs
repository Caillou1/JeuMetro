using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NRand;

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
		CheckWaste ();
		CheckBin ();
	}

	void CheckWaste() {
		if (!path.haveAction (ActionType.CLEAN_WASTE)) {
			var wastes = G.Sys.tilemap.getSurrondingSpecialTile (transform.position, TileID.WASTE, stats.WasteVisibilityRadius, G.Sys.constants.VerticalAmplification);
			Vector3 wastePos = Vector3.zero;
			WasteTile wasteTile = null;

			bool trouve = false;

			while (!trouve && wastes.Count > 0) {
				wastePos = wastes [new UniformIntDistribution (wastes.Count - 1).Next (new StaticRandomGenerator<DefaultRandomGenerator> ())];
				wasteTile = G.Sys.tilemap.GetTileOfTypeAt (wastePos, TileID.WASTE) as WasteTile;
				if (wasteTile.Targetted) {
					wastes.Remove (wastePos);
				} else {
					wasteTile.Targetted = true;
					trouve = true;
				}
			}

			if(trouve)
				path.addAction (new CleanWasteAction (this, wastePos, wasteTile));
		}
	}

	void CheckBin() {
		if (!path.haveAction (ActionType.CLEAN_BIN)) {
			var bins = G.Sys.tilemap.getSurrondingSpecialTile (transform.position, TileID.BIN, stats.WasteVisibilityRadius, G.Sys.constants.VerticalAmplification);
			Vector3 binPos = Vector3.zero;
			BinTile binTile = null;

			bool trouve = false;

			while (!trouve && bins.Count > 0) {
				binPos = bins [new UniformIntDistribution (bins.Count - 1).Next (new StaticRandomGenerator<DefaultRandomGenerator> ())];
				binTile = G.Sys.tilemap.GetTileOfTypeAt (binPos, TileID.BIN) as BinTile;
				if (binTile.Targetted || binTile.waste < .5f) {
					bins.Remove (binPos);
				} else if(!binTile.Targetted && binTile.waste >= .5f) {
					binTile.Targetted = true;
					trouve = true;
				}
			}

			if(trouve)
				path.addAction (new CleanBinAction (this, binPos, binTile));
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

	protected override void OnPathFinished ()
	{
		path.destnation = G.Sys.tilemap.getRandomGroundTile ().transform.position;
	}
}