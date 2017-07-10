using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using NRand;

public class CleanerCleanState : ACleanerState
{
	Vector3 dest = new Vector3();
	Vector3 wastePos = new Vector3 ();
	TileID cleanType = TileID.BIN;

	public CleanerCleanState (CleanerEntity t) : base(t, StateType.CLEANER_CLEAN)
	{
		
	}

	public override int check()
	{
		if(cleaner.altAction == AEntity.ActionType.CLEAN && new Vector3i (dest).equal (new Vector3i (cleaner.transform.position)))
			return int.MaxValue;

		if (cleaner.altAction != AEntity.ActionType.NONE)
			return 0;

		cleanType = TileID.BIN;
		var wastes = G.Sys.tilemap.getSurrondingSpecialTile (cleaner.transform.position, TileID.WASTE, cleaner.Stats.WasteVisibilityRadius, G.Sys.constants.VerticalAmplification);
		Vector3 waste = new Vector3 ();
		if (wastes.Count != 0) {
			waste = wastes[new UniformIntDistribution (wastes.Count - 1).Next (new StaticRandomGenerator<DefaultRandomGenerator> ())];
			var dir = waste - cleaner.transform.position;
			if (new Vector2 (dir.x, dir.z).magnitude + G.Sys.constants.VerticalAmplification * dir.y <= cleaner.Stats.WasteVisibilityRadius)
				cleanType = TileID.WASTE;
		}

		if (cleanType == TileID.BIN) {
			var bins = G.Sys.tilemap.getSurrondingSpecialTile (cleaner.transform.position, TileID.BIN, cleaner.Stats.WasteVisibilityRadius, G.Sys.constants.VerticalAmplification);
			if (bins.Count == 0)
				return 0;
			Vector3 bestPos = new Vector3 ();
			float bestDist = float.MaxValue;

			foreach (var b in bins) {
				var b1 = G.Sys.tilemap.tilesOfTypeAt (b, TileID.BIN);
				if (b1.Count == 0)
					continue;
				var b2 = b1 [0] as BinTile;
				if (b2.isEmpty ())
					continue;	
				var dir = b - cleaner.transform.position;
				var dist = new Vector2 (dir.x, dir.z).magnitude + Mathf.Abs(dir.y * G.Sys.constants.VerticalAmplification);
				if (dist < bestDist) {
					bestDist = dist;
					bestPos = b;
				}
			}

			if (bestDist == float.MaxValue)
				return 0;
			waste = bestPos;
		}
			
		List<Vector3> poss = new List<Vector3> ();
		poss.Add (waste + Vector3.left);
		poss.Add (waste + Vector3.right);
		poss.Add (waste + Vector3.forward);
		poss.Add (waste + Vector3.back);

		wastePos = waste;

		var pos = poss[new UniformIntDistribution(poss.Count-1).Next(new StaticRandomGenerator<DefaultRandomGenerator>())];

		cleaner.altAction = AEntity.ActionType.CLEAN;
		cleaner.altDestination = pos;
		dest = pos;
		cleaner.Updatepath ();

		return 0;
	}

	public override void start ()
	{
		if (cleanType == TileID.WASTE)
			cleaner.StartCoroutine (CleanCoroutine ());
		else
			cleaner.StartCoroutine (CleanBinCoroutine ());
	}

	public override void update()
	{
		cleaner.rigidbody.velocity = Vector3.zero;
	}

	IEnumerator CleanCoroutine()
	{
		yield return new WaitForSeconds (G.Sys.constants.WasteCleanTime);
		var tiles = G.Sys.tilemap.tilesOfTypeAt (wastePos, TileID.WASTE);
		foreach (var t in tiles)
			UnityEngine.Object.Destroy (t.gameObject);
		cleaner.altAction = AEntity.ActionType.NONE;
		cleaner.BackToMoveState();
	}

	IEnumerator CleanBinCoroutine()
	{
		yield return new WaitForSeconds (G.Sys.constants.BinCleanTime);
		var tiles = G.Sys.tilemap.tilesOfTypeAt (wastePos, TileID.BIN);
		foreach (var t in tiles) {
			var bin = t as BinTile;
			bin.waste = 0;
		}
		cleaner.altAction = AEntity.ActionType.NONE;
		cleaner.BackToMoveState();
	}

	public override void end ()
	{
		cleaner.altAction = AEntity.ActionType.NONE;
	}

	public override bool canBeStopped()
	{
		return false;
	}
}

