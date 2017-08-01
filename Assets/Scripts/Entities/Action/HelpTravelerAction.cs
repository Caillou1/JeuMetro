using System;
using UnityEngine;

public class HelpTravelerAction : AEntityAction<Agent>
{
	Traveler traveler;
	float time = 0;

	public HelpTravelerAction (Agent a, Vector3 pos, Traveler t) : base(a, ActionType.HELP_TRAVELER, pos)
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
		traveler.datas.Tiredness = 0f;
	}
}

