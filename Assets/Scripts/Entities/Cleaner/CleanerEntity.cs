using System;
using UnityEngine;
using System.Collections.Generic;
using NRand;
using System.Collections;

public class CleanerEntity  : AEntity
{
	public CleanerStats Stats;

	protected override void OnEntityAwake ()
	{
		states.Add (new CleanerMoveState (this));
		states.Add (new CleanerStairsState (this));
		states.Add (new CleanerEscalatorState (this));
		states.Add (new CleanerCleanState (this));
	}

	public override void BackToMoveState ()
	{
		SetNextState (StateType.CLEANER_MOVE);
	}

	protected override void configurePathfinder ()
	{
		var dico = new Dictionary<TileID, float> ();

		path = new Path (dico);
	}

	protected override void OnEntityPathFinished ()
	{
		InitialiseTarget ();
	}

	protected override void InitialiseTarget ()
	{
		var t = G.Sys.tilemap.getRandomGroundTile ();
		if (t != null) {
			destination = t.transform.position;
		}
	}

	public override void Updatepath ()
	{
		if (altAction != ActionType.NONE) {
			path.create (transform.position, altDestination, 0);
		} else {
			altWait = true;
			path.create (transform.position, destination, 0);
		}
	}
}
