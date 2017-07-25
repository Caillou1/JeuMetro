using System;
using UnityEngine;

public class SitAction : AEntityAction<Traveler>
{
	float time = 0;
	BenchTile bench;
	BenchTile.Side benchSide;

	public SitAction (Traveler t, Vector3 pos, BenchTile b, BenchTile.Side side) : base(t, ActionType.SIT, pos)
	{
		bench = b;
		benchSide = side;
	}

	protected override bool Start ()
	{
		return !bench.sit (benchSide, entity);
	}

	protected override bool Update ()
	{
		time += Time.deltaTime;

		if (time > 5) {
			entity.datas.Tiredness = 0;
			bench.leave (benchSide, entity);
			return true;
		}
		return false;
	}

}
