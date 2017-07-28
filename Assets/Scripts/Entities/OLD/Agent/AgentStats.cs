using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V1
{
[System.Serializable]
public class AgentStats {
	public float MovementSpeed;
	public float RotationSpeed;
	public int TravelerVisibilityRadius;

	public void SetStats(AgentStats stats) {
		MovementSpeed = stats.MovementSpeed;
		RotationSpeed = stats.RotationSpeed;
	}
}
}