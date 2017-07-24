using System;
using UnityEngine;

public class SignAction : AEntityAction<Traveler>
{
	float time = 0;
	public SignAction (Traveler t, Vector3 pos) : base(t, ActionType.SIGN, pos)
	{
		
	}

	public override bool Exec ()
	{
		time += Time.deltaTime;
		if (time >= 2) {
			entity.datas.Lostness = 0;
			return true;
		}
		return false;
	}
}
