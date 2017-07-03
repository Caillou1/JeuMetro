using System;
using NRand;
using UnityEngine;

public class LostState : ATravelerState
{
	public LostState (Traveler t) : base(t, StateType.TRAVELER_LOST)
	{
	}

	public override int check()
	{
		if(traveler.datas.Lostness < 0.95f)
			return 0;


		var pos = traveler.transform.position - traveler.destination;
		if (new Vector2 (pos.x, pos.z).magnitude < 5)
			return 0;

		if (traveler.altAction == Traveler.ActionType.NONE)
			createLostPath ();
		else if (traveler.altAction == Traveler.ActionType.LOST && new Vector3i (traveler.altDestination).equal (new Vector3i (traveler.transform.position)))
			createLostPath ();

		return 0;
	}

	void createLostPath()
	{
		traveler.altAction = Traveler.ActionType.LOST;
		var dir = new UniformVector3SphereDistribution (5).Next (new StaticRandomGenerator<DefaultRandomGenerator> ());
		dir.y /= 4;
		traveler.altDestination = new Vector3i (traveler.transform.position + dir).toVector3 ();
		traveler.Updatepath ();
	}
		
	public override void update()
	{

	}

	public override bool canBeStopped()
	{
		return true;
	}
}

