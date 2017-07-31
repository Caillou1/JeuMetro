using System;
using UnityEngine;

public class HelpTraveler : AEntityAction<Agent>
{
	Traveler traveler;
	float time = 0;

	public HelpTraveler (Agent a, Vector3 pos, Traveler t) : base(a, ActionType.HELP_TRAVELER, pos)
	{
		traveler = t;
	}

	protected override bool Update ()
	{
		time += Time.deltaTime;
		return time > G.Sys.constants.HelpTime;
	}

	protected override void End ()
	{
		traveler.stats.FaintnessPercentage = 0f;
	}
}

