using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NRand;

public class InfosState : AState
{
	Vector3 dest = new Vector3();

	public InfosState (Traveler t) : base(t, StateType.INFOS)
	{
	}

	public override int check()
	{
		if (traveler.altAction == Traveler.ActionType.INFOS && new Vector3i (dest).equal (new Vector3i (traveler.transform.position)))
			return int.MaxValue;
		
		if (traveler.datas.Lostness < 0.5f)
			return 0;

		if (traveler.altAction != Traveler.ActionType.LOST && traveler.altAction != Traveler.ActionType.NONE)
			return 0;

		var signs = G.Sys.tilemap.getSpecialTiles (TileID.INFOPANEL);
		Vector3 bestSign = new Vector3();
		var minDist = float.MaxValue;
		foreach (var s in signs) {
			var dir = s - traveler.transform.position;
			var dist = new Vector2 (dir.x, dir.z).magnitude + 5 * dir.y;
			if (dist < minDist) {
				bestSign = s;
				minDist = dist;
			}
		}
		if (minDist > 5)
			return 0;

		var tiles = G.Sys.tilemap.tilesOfTypeAt (bestSign, TileID.INFOPANEL);
		if (tiles.Count == 0)
			return 0;

		List<Vector3> poss = new List<Vector3> ();
		poss.Add (bestSign + Vector3.left);
		poss.Add (bestSign + Vector3.right);
		poss.Add (bestSign + Vector3.forward);
		poss.Add (bestSign + Vector3.back);

		var pos = poss[new UniformIntDistribution(poss.Count-1).Next(new StaticRandomGenerator<DefaultRandomGenerator>())];

		traveler.altAction = Traveler.ActionType.INFOS;
		traveler.altDestination = pos;
		traveler.altWait = false;
		dest = pos;
		traveler.Updatepath ();

		return 0;
	}

	public override void start ()
	{
		traveler.StartCoroutine (waitCoroutine());
		traveler.rigidbody.velocity = new Vector3 ();
	}

	public IEnumerator waitCoroutine()
	{
		yield return new WaitForSeconds (0.5f);
		traveler.datas.Lostness = 0;
		traveler.Updatepath ();
		traveler.BackToMoveState ();
		traveler.altAction = Traveler.ActionType.NONE;
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

