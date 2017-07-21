using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V1
{
[System.Serializable]
public class CleanerStats {
	public float MovementSpeed;
	public float RotationSpeed;
	public int WasteVisibilityRadius;

	public void SetStats(CleanerStats stats) {
		MovementSpeed = stats.MovementSpeed;
		RotationSpeed = stats.RotationSpeed;
	}
}
}
