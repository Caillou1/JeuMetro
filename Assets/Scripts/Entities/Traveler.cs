using System;
using System.Collections.Generic;
using UnityEngine;
using NRand;

public class Traveler : AEntity
{
	public TravelerStats stats = new TravelerStats ();

	[SerializeField]
	string targetName;
	TravelerDatas datas = new TravelerDatas ();

	bool isLost = false;

	protected override void OnAwake ()
	{
		G.Sys.registerTraveler (this);
		target = findExit (targetName);
		path.destnation = target;
		//path.lostness = 0.5f;
		initializeDatas();
	}

	protected override void OnUpdate ()
	{
		updateDatas ();
		updateSpeed ();
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

	protected override void Check ()
	{

	}

	void updateSpeed()
	{
		var tiles = G.Sys.tilemap.at (transform.position);
		if (tiles.Exists (t => t.type == TileID.ESCALATOR)) {
			agent.speed = G.Sys.constants.EscalatorSpeed;
		} else if (tiles.Exists (t => t.type == TileID.STAIRS)) {
			agent.speed = G.Sys.constants.StairsSpeedMultiplier * datas.Speed;
		} else
			agent.speed = datas.Speed;
	}

	void initializeDatas()
	{
		datas.Speed = stats.MovementSpeed;
		datas.Lostness = stats.LostAbility / 100;
	}

	void updateDatas()
	{
		if (datas.Lostness > 0.95f && !isLost) {
			isLost = true;
			OnPathFinished ();
		}
		if (datas.Lostness < 0.95f && isLost) {
			isLost = false;
			OnPathFinished ();
		}
	}

	protected override void OnPathFinished ()
	{
		if (!isLost) {
			path.destnation = target;
			return;
		}

		path.destnation = transform.position + new UniformVector3SphereDistribution (G.Sys.constants.travelerLostRadius).Next (new StaticRandomGenerator<DefaultRandomGenerator> ());
	}
}
