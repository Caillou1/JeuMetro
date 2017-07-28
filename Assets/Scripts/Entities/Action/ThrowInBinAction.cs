using System;
using UnityEngine;

public class ThrowInBinAction : AEntityAction<Traveler>
{
	BinTile bin;
	float time = 0;

	public ThrowInBinAction (Traveler t, Vector3 pos, BinTile tile) : base(t, ActionType.THROW_IN_BIN, pos)
	{
		bin = tile;
	}

	protected override bool Start ()
	{
		return entity.datas.Waste == 0;
	}

	protected override bool Update ()
	{
		time += Time.deltaTime;
		return time >= 2;
	}

	protected override void End ()
	{
		if (bin.freeSpace () < entity.datas.Waste) {
			entity.datas.Waste -= bin.freeSpace ();
			bin.waste = 1;
		} else {
			bin.waste += entity.datas.Waste;
			entity.datas.Waste = 0;
		}
		entity.datas.Dirtiness = 1 - (entity.stats.Cleanliness / 100);
	}
}
