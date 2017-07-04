using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using NRand;

public class CleanerCleanState : ACleanerState
{
	Vector3 dest = new Vector3();
	Vector3 wastePos = new Vector3 ();

	/*ATile targetToClean;
	Vector3 oldPos;
	float speedMultiplier = 1;
	float timeFromLastWait = 0;*/

	public CleanerCleanState (CleanerEntity t) : base(t, StateType.CLEANER_CLEAN)
	{
		
	}

	public override int check()
	{
		if(cleaner.altAction == AEntity.ActionType.CLEAN && new Vector3i (dest).equal (new Vector3i (cleaner.transform.position)))
			return int.MaxValue;

		if (cleaner.altAction != AEntity.ActionType.NONE)
			return 0;

		var wastes = G.Sys.tilemap.getSpecialTiles (TileID.WASTE);
		Vector3 bestTile = new Vector3 ();
		var minDist = float.MaxValue;
		foreach (var t in wastes) {
			var dir = t - cleaner.transform.position;
			var dist = new Vector2 (dir.x, dir.z).magnitude + 5 * dir.y;
			if (dist < minDist) {
				bestTile = t;
				minDist = dist;
			}
		}

		if (minDist > 5)
			return 0;

		var tiles = G.Sys.tilemap.tilesOfTypeAt (bestTile, TileID.WASTE);
		if (tiles.Count == 0)
			return 0;
			
		List<Vector3> poss = new List<Vector3> ();
		poss.Add (bestTile + Vector3.left);
		poss.Add (bestTile + Vector3.right);
		poss.Add (bestTile + Vector3.forward);
		poss.Add (bestTile + Vector3.back);

		wastePos = bestTile;

		var pos = poss[new UniformIntDistribution(poss.Count-1).Next(new StaticRandomGenerator<DefaultRandomGenerator>())];

		cleaner.altAction = AEntity.ActionType.CLEAN;
		cleaner.altDestination = pos;
		dest = pos;
		cleaner.Updatepath ();

		return 0;
	}

	public override void start ()
	{
		cleaner.StartCoroutine (CleanCoroutine ());
	}

	public override void update()
	{
		cleaner.rigidbody.velocity = Vector3.zero;

		/*timeFromLastWait += Time.deltaTime;
		Debug.Log ("dest : " + cleaner.destination + " -- alt dest : " + cleaner.altDestination + " -- real dest : " + cleaner.path.getEndPos () + " -- alt action : " + cleaner.altAction);

		var target = cleaner.path.next (cleaner.transform.position);
		if ((cleaner.transform.position - target).magnitude > .5f && (cleaner.transform.position - target).magnitude > (oldPos - target).magnitude)
			cleaner.Updatepath ();
		target += cleaner.avoidDir;
		oldPos = cleaner.transform.position;
		var dir = Vector3.Slerp (cleaner.transform.forward, target - cleaner.transform.position, Time.deltaTime * cleaner.Stats.RotationSpeed).normalized;
		cleaner.transform.rotation = Quaternion.Euler (0, Quaternion.LookRotation (dir, Vector3.up).eulerAngles.y, 0);

		Debug.DrawRay (cleaner.transform.position, cleaner.transform.forward, Color.blue);

		cleaner.rigidbody.velocity = cleaner.transform.forward.normalized * cleaner.datas.Speed * speedMultiplier;
		cleaner.avoidDir = new Vector3 ();

		if((new Vector3i(cleaner.transform.position).equal(new Vector3i(targetToClean.transform.position))))
			Debug.Log(targetToClean.name + " CLEANED");*/
	}

	IEnumerator CleanCoroutine()
	{
		yield return new WaitForSeconds (1.0f);
		var tiles = G.Sys.tilemap.tilesOfTypeAt (wastePos, TileID.WASTE);
		foreach (var t in tiles)
			UnityEngine.Object.Destroy (t.gameObject);
		cleaner.altAction = AEntity.ActionType.NONE;
		cleaner.BackToMoveState();
	}

	public override void end ()
	{
		Debug.Log ("END CLEAN");
		cleaner.altAction = AEntity.ActionType.NONE;
	}

	public override bool canBeStopped()
	{
		return false;
	}
}

