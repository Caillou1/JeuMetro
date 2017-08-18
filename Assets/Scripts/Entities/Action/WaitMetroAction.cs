using System;
using UnityEngine;

public class WaitMetroAction : AEntityAction<Traveler>
{
	WaitZoneTile tile;
    int frames = 0;

	public WaitMetroAction(Traveler t, Vector3 pos) : base(t, ActionType.WAIT_METRO, pos)
	{

	}

	protected override bool Start ()
	{
		tile = G.Sys.tilemap.GetTileOfTypeAt (entity.transform.position, TileID.WAIT_ZONE) as WaitZoneTile;
		return tile == null || tile.metro == null;
	}

	protected override bool Update ()
	{
        return tile.metro.tileEnabled && frames ++ > 6;
	}

	protected override void End ()
	{
		entity.path.addAction (new GetMetroAction (entity, tile.metro.transform.position));
	}
}

