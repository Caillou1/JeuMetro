using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour 
{
	[Tooltip("Détermine le poids d'un déplacement vertical comparé au déplacement horisontal")]
	public float VerticalAmplification = 5;
	[Tooltip("Le rayon de détéction des objets à proximité pour un voyageur")]
	public float TravelerDetectionRadius = 6;
	[Tooltip("Le rayon de détéction des objets à proximité pour un agent")]
	public float WorkerDetectionRadius = 6;
	[Tooltip("Le temps que prend un agent d'entretien pour nettoyer un déchet au sol")]
	public float WasteCleanTime = 1;
	[Tooltip("Le temps que prend un agent d'entretien pour vider une poubelle")]
	public float BinCleanTime = 1;
	[Tooltip("Vitesse des entités dans les escalators")]
	public float EscalatorSpeed = 2;
	[Tooltip("Modificateur de vitesse des entités dans les escaliers")]
	public float StairsSpeedMultiplier = 0.75f;
	[Tooltip("Rayon de déplacement du voyageur lorsqu'il est perdu")]
	public float travelerLostRadius = 6;
	[Tooltip("Utilisé pour définir un chemin chaotique lorsqu'un voyageur est un peut perdu")]
	public float travelerLostVariance = 3;
	[Tooltip("Temps que met un agent pour aider un voyageur ayant fait un malaise")]
	public float HelpTime;
	[Tooltip("Temps que met un métro pour arriver à quai")]
	public float MetroComeTime;
	[Tooltip("Temps qu'attend un métro à quai")]
	public float MetroWaitTime;
	[Tooltip("Temps que met un ascenseur pour arriver à un étage")]
	public float ElevatorComeTime;
	[Tooltip("Temps qu'attend un ascenseur à un étage s'il doit aller à un autre étage après")]
	public float ElevatorWaitTime;
	[Tooltip("Temps entre 2 emission par les éméteurs sonor")]
	public float SpeakerWaitTime;
	[Tooltip("Temps d'émission des éméteurs sonor")]
	public float SpeakerEmissionTime;

	void Awake()
	{
		G.Sys.constants = this;
	}
}
