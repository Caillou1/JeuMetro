using System;
using UnityEngine;

public class SignAction : AEntityAction<Traveler>
{
	float time = 0;
    InfoPanelTile sign;

    public SignAction (Traveler t, Vector3 pos, InfoPanelTile tile) : base(t, ActionType.SIGN, pos)
	{
        sign = tile;
	}

	protected override bool Update ()
	{
        if (sign == null)
            return true;
        
		time += Time.deltaTime;
        return time > G.Sys.constants.ReadSignTime;
	}

	protected override void End ()
	{
		entity.datas.Lostness = 0;
	}
}
