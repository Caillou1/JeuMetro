using System.Collections;
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
	[Range(0f,100f)]
	public float RestPlaceAttraction;
	[Range(0f,100f)]
	public float Cleanliness;
	[Range(0f,100f)]
	public float FaintnessPercentage;
	[Range(0f,100f)]
	public float FraudPercentage;
	[Range(0f,100f)]
	public float LostAbility;
    [Range(0f, 100f)]
	public float Hunger;
	[Range(0f, 100f)]
	public float wastes;
	public float MovementSpeed;
	public bool Malvoyant;
	public bool Deaf;
    public bool HaveTicket;
    public bool IgnoreBin;

	public void SetStats(TravelerStats stats) {
		Type = stats.Type;
		RestPlaceAttraction = stats.RestPlaceAttraction;
		Cleanliness = stats.Cleanliness;
		FaintnessPercentage = stats.FaintnessPercentage;
		LostAbility = stats.LostAbility;
		MovementSpeed = stats.MovementSpeed;
		Malvoyant = stats.Malvoyant;
		Deaf = stats.Deaf;
        Hunger = stats.Hunger;
        HaveTicket = stats.HaveTicket;
        wastes = stats.wastes;
	}
}