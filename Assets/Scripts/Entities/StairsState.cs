using System;
using UnityEngine;

public class StairsState : AState
{
	public StairsState (Traveler T) : base(T, StateType.STAIRS)
	{

	}

	public override int check()
	{
		var tile = G.Sys.tilemap.connectableTile(traveler.transform.position);
		if (tile == null)
			return 0;

		var next = traveler.path.next (traveler.transform.position);
		return 0;
	}

	public override void start ()
	{
		
	}

	public override void update()
	{
		
	}

	public override bool canBeStopped()
	{
		return false;
	}
}

