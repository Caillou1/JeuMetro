using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using NRand;

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

		if (traveler.datas.Dirtiness > 0.95f) {
			var tiles = G.Sys.tilemap.at (traveler.transform.position);
			if (tiles.Count != 1 || tiles [0].type != TileID.GROUND)
				return 0;
				
			inBin = false;
			return int.MaxValue;
		}

		var bins = G.Sys.tilemap.getSurrondingSpecialTile (traveler.transform.position, TileID.BIN, 5, 5);
		if (bins.Count == 0)
			return 0;

		var bestBin = new Vector3 ();
		var bestDist = float.MaxValue;
		foreach (var b in bins) {
			var tiles = G.Sys.tilemap.tilesOfTypeAt (b, TileID.BIN);
			if (tiles.Count == 0)
				continue;
			var bin = tiles [0] as BinTile;
			if (!bin.canContain (traveler.datas.Waste / 5.0f))
				continue;
			var dir = traveler.transform.position - b;
			var dist = new Vector2 (dir.x, dir.z).magnitude + Mathf.Abs (dir.y);
			if (dist < bestDist) {
				bestDist = dist;
				bestBin = b;
			}
		}
		if (bestDist == float.MaxValue)
			return 0;

		List<Vector3> poss = new List<Vector3> ();
		poss.Add (bestBin + Vector3.left);
		poss.Add (bestBin + Vector3.right);
		poss.Add (bestBin + Vector3.forward);
		poss.Add (bestBin + Vector3.back);

		binPos = bestBin;

		var pos = poss[new UniformIntDistribution(poss.Count-1).Next(new StaticRandomGenerator<DefaultRandomGenerator>())];

		traveler.altAction = Traveler.ActionType.THROW_WASTE;
		traveler.altDestination = pos;
		traveler.altWait = false;
		dest = pos;
		traveler.Updatepath ();

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
			(bin [0] as BinTile).waste += traveler.datas.Waste / 5.0f;
			traveler.datas.Waste = 0;
		}
			traveler.Updatepath ();
			traveler.BackToMoveState ();
			traveler.altAction = Traveler.ActionType.NONE;
	}

	public IEnumerator waitGroundCoroutine()
	{
		yield return new WaitForSeconds (0.5f);
		UnityEngine.Object.Instantiate (G.Sys.gameManager.wastePrefab, new Vector3i (traveler.transform.position).toVector3 (), new Quaternion ());
		traveler.datas.Waste = 0;
		traveler.Updatepath ();
		traveler.BackToMoveState ();
		traveler.altAction = Traveler.ActionType.NONE;
		Event<ObjectPlacedEvent>.Broadcast (new ObjectPlacedEvent (new Vector3[]{ new Vector3i (traveler.transform.position).toVector3 () }.ToList()));
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
