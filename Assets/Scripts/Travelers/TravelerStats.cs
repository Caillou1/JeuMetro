using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TravelerStats {
	[Range(0f,100f)]
	public float ElevatorAttraction;
	[Range(0f,100f)]
	public float EscalatorAttraction;
	[Range(0f,100f)]
	public float StairsAttraction;
	[Range(0f,100f)]
	public float RestPlaceAttraction;
	[Range(0f,100f)]
	public float Cleanliness;
	[Range(0f,100f)]
	public float VisualComprehension;
	[Range(0f,100f)]
	public float AudioComprehension;
	[Range(0f,100f)]
	public float TouchComprehension;
	[Range(0f,100f)]
	public float FaintnessPercentage;
	public float MovementSpeed;

	public void SetStats(TravelerStats stats) {
		ElevatorAttraction = stats.ElevatorAttraction;
		EscalatorAttraction = stats.EscalatorAttraction;
		StairsAttraction = stats.StairsAttraction;
		RestPlaceAttraction = stats.RestPlaceAttraction;
		Cleanliness = stats.Cleanliness;
		VisualComprehension = stats.VisualComprehension;
		AudioComprehension = stats.AudioComprehension;
		TouchComprehension = stats.TouchComprehension;
		FaintnessPercentage = stats.FaintnessPercentage;
		MovementSpeed = stats.MovementSpeed;
	}
}
