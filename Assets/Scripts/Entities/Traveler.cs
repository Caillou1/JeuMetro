using System;
using System.Collections.Generic;
using UnityEngine;
using NRand;

public class Traveler : AEntity
{
	[SerializeField]
	string targetName;

	protected override void OnAwake ()
	{
		G.Sys.registerTraveler (this);
		target = findExit (targetName);
		path.destnation = target;
		//path.lostness = 0.5f;
	}

	protected override void OnUpdate ()
	{
		if (G.Sys.tilemap.haveSpecialTileAt (TileID.OUT, transform.position))
			Destroy (gameObject);
	}

	static Vector3 findExit(string name)
	{
		List<Vector3> validTiles = new List<Vector3> ();
		foreach (var m in G.Sys.tilemap.getSpecialTiles(TileID.OUT)) {
			var t = G.Sys.tilemap.GetTileOfTypeAt (m, TileID.OUT) as ExitsTile;
			if (t == null)
				continue;
			if (t.exitname == name)
				validTiles.Add (m);
		}

		foreach (var m in G.Sys.tilemap.getSpecialTiles(TileID.METRO)) {
			var t = G.Sys.tilemap.GetTileOfTypeAt (m, TileID.METRO) as ExitsTile;
			if (t == null)
				continue;
			if (t.exitname == name)
				validTiles.Add (m);
		}

		return validTiles [new UniformIntDistribution (validTiles.Count - 1).Next (new StaticRandomGenerator<DefaultRandomGenerator> ())];
	}
		
	void OnDestroy()
	{
		G.Sys.removeTraveler (this);
	}
}
