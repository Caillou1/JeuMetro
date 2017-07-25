﻿using System;
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
		if (time >= 2) {
			entity.datas.Lostness = 0;
			return true;
		}
		return false;
	}
}
