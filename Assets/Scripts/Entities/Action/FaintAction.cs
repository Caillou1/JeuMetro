﻿using System;
using UnityEngine;
using DG.Tweening;

public class FaintAction : AEntityAction<Traveler>
{
	public FaintAction (Traveler t) : base(t, ActionType.FAINT, t.transform.position, 100)
	{
		traveler = t;
	}

	protected override bool Start ()
	{
		if (entity.datas.Tiredness <= 0.95f)
			return true;
		entity.transform.DORotate (new Vector3(90, entity.transform.rotation.eulerAngles.y, 0), 0.5f, RotateMode.Fast);
		Event<FaintEvent>.Broadcast (new FaintEvent (traveler));
		return false;
	}

	protected override bool Update ()
	{
		return entity.datas.Tiredness <= 0.95f;
	}

	protected override void End ()
	{
		entity.transform.DORotate (new Vector3(0, entity.transform.rotation.eulerAngles.y, 0), 0.5f, RotateMode.Fast);
	}

	Traveler traveler;
}