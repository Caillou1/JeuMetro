using System;
using UnityEngine;

public class SignAction : AEntityAction<Traveler>
{
	float time = 0;
	Vector3 signPos = new Vector3();

	public SignAction (Traveler t, Vector3 pos, Vector3 sign) : base(t, ActionType.SIGN, pos)
	{
		signPos = sign;
	}

	protected override bool Update ()
	{
		time += Time.deltaTime;
        return time > G.Sys.constants.ReadSignTime;
	}

	protected override void End ()
	{
		entity.datas.Lostness = 0;
	}
}
