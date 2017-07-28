using System;
using UnityEngine;

public class CleanBinAction : AEntityAction<Cleaner>
{
	BinTile binTile;
	float time = 0;

	public CleanBinAction (Cleaner t, Vector3 pos, BinTile tile) : base(t, ActionType.CLEAN_BIN, pos)
	{
		binTile = tile;
	}

	protected override bool Update ()
	{
		time += Time.deltaTime;
		return time > G.Sys.constants.BinCleanTime;
	}

	protected override void End ()
	{
		binTile.waste = 0f;
		binTile.Targetted = false;
	}
}

