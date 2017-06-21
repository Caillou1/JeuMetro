using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravelerStats : MonoBehaviour {
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
}
