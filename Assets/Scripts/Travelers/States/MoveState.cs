using System;
using UnityEngine;
using System.Collections;
using NRand;

public class MoveState : ATravelerState
{
	const float delayMultiplier = 30;
	const float waitMultiplier = 1;

	Vector3 oldPos;
	float speedMultiplier = 1;
	float timeFromLastWait = 0;
	float delayToNextWait = 0;
	bool waiting = false;

	public MoveState (Traveler t) : base(t, StateType.TRAVELER_MOVE)
	{
		
	}

	public override int check()
	{
		return 1;
	}

	public override void start ()
	{
		delayToNextWait = new UniformFloatDistribution(Mathf.Max(1 - traveler.datas.Lostness * delayMultiplier / 2, 1)
												     , Mathf.Max(1 - traveler.datas.Lostness * delayMultiplier, 2)).Next(new StaticRandomGenerator<DefaultRandomGenerator>());
	}

	public override void update()
	{
		timeFromLastWait += Time.deltaTime;

		var target = traveler.path.next (traveler.transform.position);
		if ((traveler.transform.position - target).magnitude > 1.5f && (traveler.transform.position - target).magnitude > (oldPos - target).magnitude)
			traveler.Updatepath ();
		target += traveler.avoidDir;
		oldPos = traveler.transform.position;
		var dir = Vector3.Slerp (traveler.transform.forward, target - traveler.transform.position, Time.deltaTime * traveler.Stats.RotationSpeed).normalized;
		traveler.transform.rotation = Quaternion.Euler (0, Quaternion.LookRotation (dir, Vector3.up).eulerAngles.y, 0);

		Debug.DrawRay (traveler.transform.position, traveler.transform.forward, Color.blue);

		traveler.rigidbody.velocity = traveler.transform.forward.normalized * traveler.datas.Speed * speedMultiplier;
		traveler.avoidDir = new Vector3 ();

		if (traveler.datas.Lostness > 0.5 && delayToNextWait < timeFromLastWait && !waiting && traveler.altWait)
			traveler.StartCoroutine (waitCoroutine ((traveler.datas.Lostness * waitMultiplier)));
	}

	IEnumerator waitCoroutine(float time)
	{
		waiting = true;
		float t = -0.5f;
		while (t < 0) {
			speedMultiplier = speedWaitMultiplier (t);
			t += Time.deltaTime;
			yield return new WaitForEndOfFrame ();
		}
		t = 0;
		speedMultiplier = 0;
		yield return new WaitForSeconds (time);
		while (t < 0.5f) {
			speedMultiplier = speedWaitMultiplier (t);
			t += Time.deltaTime;
			yield return new WaitForEndOfFrame ();
		}
		speedMultiplier = 1;
		timeFromLastWait = 0;
		delayToNextWait = new UniformFloatDistribution(Mathf.Max(1 - traveler.datas.Lostness * delayMultiplier / 2, 1)
													 , Mathf.Max(1 - traveler.datas.Lostness * delayMultiplier, 2)).Next(new StaticRandomGenerator<DefaultRandomGenerator>());
		waiting = false;
	}

	public override bool canBeStopped()
	{
		return true;
	}

	float speedWaitMultiplier(float time)
	{
		if (time > 0.5f || time < -0.5f)
			return 1;
		return Mathf.Abs(time) * 2;
	}
}

