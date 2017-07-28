using System;
using UnityEngine;

public class CleanWasteAction : AEntityAction<Cleaner>
{
	WasteTile wasteTile;
	float time = 0;

	public CleanWasteAction (Cleaner t, Vector3 pos, WasteTile tile) : base(t, ActionType.CLEAN_WASTE, pos)
	{
		wasteTile = tile;
	}

	protected override bool Update ()
	{
		time += Time.deltaTime;
		return time > G.Sys.constants.WasteCleanTime;
	}

	protected override void End ()
	{
		if(wasteTile != null)
			GameObject.Destroy (wasteTile.gameObject);
	}
}

