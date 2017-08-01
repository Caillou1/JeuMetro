using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CleanerStats {
	public float MovementSpeed;
	[HideInInspector]
	public float RotationSpeed;
	public float WasteVisibilityRadius;

	public void SetStats(CleanerStats stats) {
		MovementSpeed = stats.MovementSpeed;
		RotationSpeed = stats.RotationSpeed;
	}
}