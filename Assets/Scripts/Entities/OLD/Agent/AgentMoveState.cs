using System;
using UnityEngine;
using System.Collections;
using NRand;

namespace V1
{
public class AgentMoveState : AAgentState
{
	const float delayMultiplier = 30;
	const float waitMultiplier = 1;

	Vector3 oldPos;
	float speedMultiplier = 1;
	float timeFromLastWait = 0;

	public AgentMoveState (AgentEntity ae) : base(ae, StateType.AGENT_MOVE)
	{
		
	}

	public override int check()
	{
		return 1;
	}

	public override void update()
	{
		timeFromLastWait += Time.deltaTime;

		var target = agent.path.next (agent.transform.position);
		if ((agent.transform.position - target).magnitude > 1.5f && (agent.transform.position - target).magnitude > (oldPos - target).magnitude)
			agent.Updatepath ();
		oldPos = agent.transform.position;
		var dir = Vector3.Slerp (agent.transform.forward, target - agent.transform.position, Time.deltaTime * agent.Stats.RotationSpeed).normalized;
		agent.transform.rotation = Quaternion.Euler (0, Quaternion.LookRotation (dir, Vector3.up).eulerAngles.y, 0);

		Debug.DrawRay (agent.transform.position, agent.transform.forward, Color.blue);

		agent.rigidbody.velocity = agent.transform.forward.normalized * agent.Stats.MovementSpeed * speedMultiplier;
	}

	public override bool canBeStopped()
	{
		return true;
	}
}
}
