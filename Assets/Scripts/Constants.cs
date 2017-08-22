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
	public float HelpTime =5;
	[Tooltip("Temps que met un métro pour arriver à quai")]
	public float MetroComeTime = 3;
	[Tooltip("Temps qu'attend un métro à quai")]
	public float MetroWaitTime = 5;
	[Tooltip("Temps que met un ascenseur pour arriver à un étage")]
	public float ElevatorComeTime = 2;
	[Tooltip("Temps qu'attend un ascenseur à un étage s'il doit aller à un autre étage après")]
	public float ElevatorWaitTime = 4;
	[Tooltip("Nombre de personnes maximum dans un ascenseur")]
	public int ElevatorMaxPeople = 10;
	[Tooltip("Temps entre 2 emission par les éméteurs sonor")]
	public float SpeakerWaitTime = 5;
	[Tooltip("Temps d'émission des éméteurs sonor")]
	public float SpeakerEmissionTime = 2;
    [Tooltip("Chance de 0 à 100 qu'un voyageur a de tomber dans les escaliers s'il n'est pas aveugle ou malvoyant")]
    public float FallChance = 0.5f;
    [Tooltip("Chance de 0 à 100 qu'un voyageur aveugle a de tomber dans les escaliers si il n'y a pas de bande podotactile")]
    public float BlindFallChance = 80;
    [Tooltip("Chance de 0 à 100 qu'un voyageur malvoyant a de tomber dans les escaliers s'il n'y a pas de bande podotactile")]
    public float PartialyBlindFallChance = 40;
    [Tooltip("Temps que met un voyageur pour acheter de la nouriture ou un ticket")]
    public float BuyTime = 2;
    [Tooltip("Temps que met un voyageur pour lire un panneau d'information")]
    public float ReadSignTime = 2;
    [Tooltip("Temps d'assoyage des voyageurs")]
    public float SitTime = 3;
    [Tooltip("Temps pour jeuter un déchet au sol")]
    public float WasteGroundTime = 1;
    [Tooltip("Temps pour jeuter un dechet dans une poubelle")]
    public float WasteBinTime = 1;
    [Tooltip("Taille max de queue avant de rechercher un autre distributeur")]
    public int QueueMax = 5;
	public float ElevatorAttraction = .1f;
    [Tooltip("Multiplieur de vitesse lors de l'alerte incendie pour les voyageurs et agents")]
    public float fireAlertSpeedMultiplier = 2;

	public List<Color> TravelerColors;

	void Awake()
	{
		G.Sys.constants = this;
	}
}
