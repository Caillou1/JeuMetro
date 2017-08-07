using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V1
{
public class SitState : ATravelerState
{
	Vector3 dest = new Vector3();
	Vector3 benchPos = new Vector3();

	public SitState (Traveler t) : base(t, StateType.TRAVELER_SIT)
	{
		
	}

	public override int check()
	{
		if (traveler.altAction == Traveler.ActionType.SIT) {
			var benchs = G.Sys.tilemap.tilesOfTypeAt (benchPos, TileID.BENCH);
			if (benchs.Count == 0) {
				traveler.altAction = AEntity.ActionType.NONE;
				traveler.Updatepath ();
				return 0;
			}
			var bench = benchs [0] as BenchTile;
			if (!bench.canSit (bench.posToSide (benchPos))) {
				traveler.altAction = AEntity.ActionType.NONE;
				traveler.Updatepath ();
				return 0;
			}
			if ( new Vector3i (dest).equal (new Vector3i (traveler.transform.position)))
				return int.MaxValue;
		}
		
		if(traveler.datas.Tiredness < 0.5f)
			return 0;

		if (traveler.altAction != Traveler.ActionType.LOST && traveler.altAction != Traveler.ActionType.NONE)
			return 0;

		List<Pair<BenchTile, BenchTile.Side>> validPlaces = new List<Pair<BenchTile, BenchTile.Side>>();
		foreach (var b in G.Sys.tilemap.getSpecialTiles(TileID.BENCH)) {
			var tiles = G.Sys.tilemap.tilesOfTypeAt (b, TileID.BENCH);
			if (tiles.Count == 0)
				continue;
			var t = tiles [0] as BenchTile;
			foreach (var p in t.freePlaces())
				validPlaces.Add (new Pair<BenchTile, BenchTile.Side> (t, p));
		}

		if (validPlaces.Count == 0)
			return 0;

		float bestDist = float.MaxValue;
		Vector3 bestPos = Vector3.zero;
		Vector2 bestDir = Vector2.zero;
		foreach (var p in validPlaces) {
			var pos = p.First.sideToPos (p.Second);
			var dist = (traveler.transform.position - pos).magnitude;
			if (dist < bestDist) {
				bestDist = dist;
				bestPos = pos;
				bestDir = Orienter.orientationToDir (Orienter.angleToOrientation (p.First.transform.rotation.eulerAngles.y));
			}
		}

		if (bestDist > 5)
			return 0;

		dest = bestPos + new Vector3 (-bestDir.y, 0, bestDir.x);
		benchPos = bestPos;
		traveler.altDestination = dest;
		traveler.altAction = AEntity.ActionType.SIT;
		traveler.altWait = false;
		traveler.Updatepath ();
		return 0;
	}

	public override void start ()
	{
		traveler.StartCoroutine (waitCoroutine ());
	}

	public IEnumerator waitCoroutine()
	{
		var benchs = G.Sys.tilemap.tilesOfTypeAt (benchPos, TileID.BENCH);
		if (benchs.Count == 0)
			yield return null;
		//var bench = benchs [0] as BenchTile;
		//bench.sit (bench.posToSide (benchPos), traveler);
		
		yield return new WaitForSeconds (1.0f);
		traveler.datas.Tiredness = 0;
		traveler.altAction = Traveler.ActionType.NONE;
		traveler.Updatepath ();
		traveler.BackToMoveState ();

		//bench.leave (bench.posToSide (benchPos), traveler);
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
}
