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

		G.Sys.removeCleaner (this);
	}

	protected override void OnEntityDestroy ()
	{
		G.Sys.removeCleaner (this);
	}

	public override void BackToMoveState ()
	{
		SetNextState (StateType.CLEANER_MOVE);
	}

	protected override void configurePathfinder ()
	{
		var dico = new Dictionary<TileID, float> ();

		path = new EntityPath (dico);
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

	protected override void ForcePath ()
	{
		var dest = altAction != ActionType.NONE ? altDestination : destination;
		var startOffset = new Vector3[]{ Vector3.forward, Vector3.back, Vector3.left, Vector3.right };

		foreach (var s in startOffset) {
			path.create (transform.position + s, dest, 0);
			if (path.isPathValid ())
				return;
		}
	}
}
