using System;
using UnityEngine;

public class CleanerCleanState : ACleanerState
{
	ATile targetToClean;
	Vector3 oldPos;
	float speedMultiplier = 1;
	float timeFromLastWait = 0;

	public CleanerCleanState (CleanerEntity t) : base(t, StateType.CLEANER_CLEAN)
	{

	}

	public override int check()
	{
		var tilesToClean = G.Sys.tilemap.getSurroundingTilesOfTypeAt (cleaner.transform.position, TileID.WASTE, cleaner.Stats.WasteVisibilityRadius);
		if (tilesToClean.Count > 0) {
			targetToClean = tilesToClean [0];
			cleaner.altDestination = targetToClean.transform.position;
			cleaner.altAction = AEntity.ActionType.CLEAN;
			cleaner.Updatepath ();
			return 2;
		} else {
			return int.MinValue;
		}
	}

	public override void start ()
	{
	}

	public override void update()
	{
		timeFromLastWait += Time.deltaTime;
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
			Debug.Log(targetToClean.name + " CLEANED");
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

