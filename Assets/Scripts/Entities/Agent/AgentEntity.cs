using System;
using UnityEngine;
using System.Collections.Generic;
using NRand;
using System.Collections;

public class AgentEntity  : AEntity
{
	public AgentStats Stats;

	protected override void OnEntityAwake ()
	{
		states.Add (new AgentMoveState (this));
		states.Add (new AgentStairsState (this));
		states.Add (new AgentEscalatorState (this));
	}

	public override void BackToMoveState ()
	{
		SetNextState (StateType.AGENT_MOVE);
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
