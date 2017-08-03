﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TravelerType
{
	CLASSIC,
	WITH_BAG,
	WHEELCHAIR,
	BLIND
}

[System.Serializable]
public class TravelerStats {
	public TravelerType Type;
	[HideInInspector]
	[Range(-100f,100f)]
	public float ElevatorAttraction;
	[HideInInspector]
	[Range(-100f,100f)]
	public float EscalatorAttraction;
	[HideInInspector]
	[Range(-100f,100f)]
	public float StairsAttraction;
	[Range(-100f,100f)]
	public float RestPlaceAttraction;
	[Range(-100f,100f)]
	public float VisualComprehension;
	[Range(-100f,100f)]
	public float AudioComprehension;
	[Range(-100f,100f)]
	public float TouchComprehension;
	[Range(0f,100f)]
	public float Cleanliness;
	[Range(0f,100f)]
	public float FaintnessPercentage;
	[Range(0f,100f)]
	public float FraudPercentage;
	[Range(0f,100f)]
	public float LostAbility;
	public float MovementSpeed;
	[HideInInspector]
	public float RotationSpeed;

	public void SetStats(TravelerStats stats) {
		Type = stats.Type;
		ElevatorAttraction = stats.ElevatorAttraction;
		EscalatorAttraction = stats.EscalatorAttraction;
		StairsAttraction = stats.StairsAttraction;
		RestPlaceAttraction = stats.RestPlaceAttraction;
		Cleanliness = stats.Cleanliness;
		VisualComprehension = stats.VisualComprehension;
		AudioComprehension = stats.AudioComprehension;
		TouchComprehension = stats.TouchComprehension;
		FaintnessPercentage = stats.FaintnessPercentage;
		LostAbility = stats.LostAbility;
		MovementSpeed = stats.MovementSpeed;
		RotationSpeed = stats.RotationSpeed;
	}
}