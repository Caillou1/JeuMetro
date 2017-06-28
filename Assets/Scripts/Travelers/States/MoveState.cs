using System;
using UnityEngine;

public class MoveState : AState
{
	Vector3 oldPos;

	public MoveState (Traveler t) : base(t, StateType.MOVE)
	{
		
	}

	public override int check()
	{
		return 1;
	}

	public override void update()
	{
		var target = traveler.path.next (traveler.transform.position);
		if ((traveler.transform.position - target).magnitude > 1.5f && (traveler.transform.position - target).magnitude > (oldPos - target).magnitude)
			traveler.path.create(traveler.transform.position, traveler.path.target());
		target += traveler.avoidDir;
		oldPos = traveler.transform.position;
		var dir = Vector3.Slerp (traveler.transform.forward, target - traveler.transform.position, Time.deltaTime * traveler.Stats.RotationSpeed).normalized;
		traveler.transform.rotation = Quaternion.Euler (0, Quaternion.LookRotation (dir, Vector3.up).eulerAngles.y, 0);

		Debug.DrawRay (traveler.transform.position, traveler.transform.forward, Color.blue);

		traveler.rigidbody.velocity = traveler.transform.forward.normalized * traveler.Stats.MovementSpeed;
		traveler.avoidDir = new Vector3 ();

	}

	public override bool canBeStopped()
	{
		return true;
	}
}

