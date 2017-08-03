using System;
using UnityEngine;

public class WaitMetroAction : AEntityAction<Traveler>
{
	WaitZoneTile tile;

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
		return tile.metro.tileEnabled;
	}

	protected override void End ()
	{
		entity.path.addAction (new GetMetroAction (entity, tile.metro.transform.position));
	}
}

