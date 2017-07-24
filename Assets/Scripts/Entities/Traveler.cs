using System;
using System.Collections.Generic;
using UnityEngine;
using NRand;

public class Traveler : AEntity
{
	public TravelerStats stats = new TravelerStats ();

	[SerializeField]
	string targetName;
	[HideInInspector]
	public TravelerDatas datas = new TravelerDatas ();

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
		if (datas.Lostness > 0.5f && !path.haveAction (ActionType.SIGN)) {
			var sign = G.Sys.tilemap.getNearestSpecialTileOfType (transform.position, TileID.INFOPANEL, G.Sys.constants.VerticalAmplification, G.Sys.constants.TravelerDetectionRadius);
			if (sign.Second) {
				path.addAction (new SignAction (this, sign.First));
			}
		}
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
		var signs = G.Sys.tilemap.getSurrondingSpecialTile (transform.position, TileID.INFOPANEL, G.Sys.constants.TravelerDetectionRadius, G.Sys.constants.VerticalAmplification).Count;

		datas.Lostness = Mathf.Clamp(datas.Lostness + ((signs == 0 || signs > 3) ? 1 : -1) * stats.LostAbility * stats.LostAbility / 20000 * Time.deltaTime, 0, 1);

		if (datas.Lostness > 0.95f && !isLost) {
			isLost = true;
			OnPathFinished ();
		}
		if (datas.Lostness < 0.95f && isLost) {
			isLost = false;
			OnPathFinished ();
		}
		path.lostness = datas.Lostness;
	}

	protected override void OnPathFinished ()
	{
		if (!isLost) {
			path.destnation = target;
			return;
		}

		var gen = new StaticRandomGenerator<DefaultRandomGenerator> ();
		var d = new UniformVector3SphereDistribution (G.Sys.constants.travelerLostRadius);

		for (int i = 0; i < 10; i++) {
			var pos = transform.position + d.Next(gen);
			var tiles = G.Sys.tilemap.at (pos);
			if (tiles.Count == 1 && tiles [0].type == TileID.GROUND) {
				path.destnation = pos;
				return;
			}
		}
		path.destnation = transform.position + d.Next (gen);

	}
}
