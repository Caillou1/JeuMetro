using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class WasteState : ATravelerState
{
	Vector3 dest = new Vector3();
	Vector3 binPos = new Vector3();
	bool inBin = true;

	public WasteState (Traveler t) : base (t, StateType.TRAVELER_THROW_WASTE)
	{
		
	}

	public override int check()
	{
		if (traveler.altAction == Traveler.ActionType.THROW_WASTE && new Vector3i (dest).equal (new Vector3i (traveler.transform.position)))
			return int.MaxValue;

		if (traveler.datas.Waste < 0.1f)
			return 0;
		
		if (traveler.altAction != Traveler.ActionType.LOST && traveler.altAction != Traveler.ActionType.NONE)
			return 0;

		//todobodododod
		return 0;

	}

	public override void start ()
	{
		if (inBin)
			traveler.StartCoroutine (waitBinCoroutine ());
		else
			traveler.StartCoroutine (waitGroundCoroutine ());
	}

	public IEnumerator waitBinCoroutine()
	{
		yield return new WaitForSeconds (0.5f);
		var bin = G.Sys.tilemap.tilesOfTypeAt (binPos, TileID.BIN);
		if (bin.Count > 0) {
			(bin [0] as BinTile).waste += traveler.datas.Waste;
			traveler.datas.Waste = 0;
			traveler.Updatepath ();
			traveler.BackToMoveState ();
			traveler.altAction = Traveler.ActionType.NONE;
		}
	}

	public IEnumerator waitGroundCoroutine()
	{
		yield return new WaitForSeconds (0.5f);
		var bin = G.Sys.tilemap.tilesOfTypeAt (binPos, TileID.BIN);
		if (bin.Count > 0) {
			UnityEngine.Object.Instantiate (G.Sys.gameManager.wastePrefab, new Vector3i (traveler.transform.position).toVector3 (), new Quaternion ());
			traveler.datas.Waste = 0;
			traveler.Updatepath ();
			traveler.BackToMoveState ();
			traveler.altAction = Traveler.ActionType.NONE;
		}
	}

	public override void update()
	{
		traveler.rigidbody.velocity = new Vector3 ();
	}

	public override bool canBeStopped()
	{
		return false;
	}


}
