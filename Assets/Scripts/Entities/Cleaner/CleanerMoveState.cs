using System;
using UnityEngine;
using System.Collections;
using NRand;

public class CleanerMoveState : ACleanerState
{
	const float delayMultiplier = 30;
	const float waitMultiplier = 1;

	Vector3 oldPos;
	float speedMultiplier = 1;
	float timeFromLastWait = 0;

	public CleanerMoveState (CleanerEntity ce) : base(ce, StateType.CLEANER_MOVE)
	{
		
	}

	public override int check()
	{
		return 1;
	}

	public override void update()
	{
		timeFromLastWait += Time.deltaTime;

		var target = cleaner.path.next (cleaner.transform.position);
		if ((cleaner.transform.position - target).magnitude > 1.5f && (cleaner.transform.position - target).magnitude > (oldPos - target).magnitude)
			cleaner.Updatepath ();
		oldPos = cleaner.transform.position;
		var dir = Vector3.Slerp (cleaner.transform.forward, target - cleaner.transform.position, Time.deltaTime * cleaner.Stats.RotationSpeed).normalized;
		cleaner.transform.rotation = Quaternion.Euler (0, Quaternion.LookRotation (dir, Vector3.up).eulerAngles.y, 0);

		Debug.DrawRay (cleaner.transform.position, cleaner.transform.forward, Color.blue);

		cleaner.rigidbody.velocity = cleaner.transform.forward.normalized * cleaner.Stats.MovementSpeed * speedMultiplier;
	}

	public override bool canBeStopped()
	{
		return true;
	}
}

