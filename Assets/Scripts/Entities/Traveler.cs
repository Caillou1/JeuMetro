using System;
using System.Collections;
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
		initializeDatas();
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

	protected override void Check ()
	{
		checkSigns ();
		checkTiredness ();
		checkWaste ();
		checkHunger ();
		checkTicket ();
	}

	void checkSigns()
	{
		if (datas.Lostness > 0.5f && !path.haveAction (ActionType.SIGN)) {
			var sign = G.Sys.tilemap.getNearestSpecialTileOfType (transform.position, TileID.INFOPANEL, G.Sys.constants.VerticalAmplification, G.Sys.constants.TravelerDetectionRadius);
			if (sign.Second) {
				List<Vector3> validPos = new List<Vector3> ();
				if (G.Sys.tilemap.IsEmptyGround (sign.First + Vector3.left))
					validPos.Add (sign.First + Vector3.left);
				if (G.Sys.tilemap.IsEmptyGround (sign.First + Vector3.right))
					validPos.Add (sign.First + Vector3.right);
				if (G.Sys.tilemap.IsEmptyGround (sign.First + Vector3.forward))
					validPos.Add (sign.First + Vector3.forward);
				if (G.Sys.tilemap.IsEmptyGround (sign.First + Vector3.back))
					validPos.Add (sign.First + Vector3.back);
				path.addAction (new SignAction (this, validPos[new UniformIntDistribution(validPos.Count-1).Next(new StaticRandomGenerator<DefaultRandomGenerator>())], sign.First));
			}
		}
	}

	void checkTiredness()
	{
		if (datas.Tiredness > 0.95f && !path.haveAction(ActionType.FAINT)) {
			path.addAction (new FaintAction (this));
			return;
		}
		if (datas.Tiredness < (0.5f - stats.RestPlaceAttraction / 200) || path.haveAction (ActionType.SIT))
			return;
		var benchs = G.Sys.tilemap.getSurrondingSpecialTile (transform.position, TileID.BENCH, G.Sys.constants.TravelerDetectionRadius, G.Sys.constants.VerticalAmplification);

		List<Pair<BenchTile, BenchTile.Side>> validBenchs = new List<Pair<BenchTile, BenchTile.Side>>();
		foreach (var bPos in benchs) {
			var	b = G.Sys.tilemap.GetTileOfTypeAt (bPos, TileID.BENCH) as BenchTile;
			if (b == null)
				continue;

			foreach (var p in b.freePlaces()) {
				if(!G.Sys.tilemap.IsEmptyGround(b.sideToFrontPos(p)))
					continue;
				validBenchs.Add (new Pair<BenchTile, BenchTile.Side> (b, p));
			}
		}

		if (validBenchs.Count == 0)
			return;
		var bench = validBenchs [new UniformIntDistribution (validBenchs.Count - 1).Next (new StaticRandomGenerator<DefaultRandomGenerator> ())];
		path.addAction (new SitAction (this, bench.First.sideToFrontPos (bench.Second), bench.First, bench.Second));
	}

	void checkWaste()
	{
		if (datas.Waste < 0.01f || path.haveAction(ActionType.THROW_IN_BIN) || path.haveAction(ActionType.THROW_IN_GROUND))
			return;

		if (datas.Dirtiness > 0.95f) {
			if (G.Sys.GetNearestCleaner (transform.position, G.Sys.constants.TravelerDetectionRadius) != null) {
				
				List<Pair<Vector3, ATile>> surrondingTiles = new List<Pair<Vector3, ATile>> ();
				surrondingTiles.Add (new Pair<Vector3, ATile> (transform.position, G.Sys.tilemap.GetTileOfTypeAt (transform.position, TileID.WASTE)));
				surrondingTiles.Add (new Pair<Vector3, ATile> (transform.position + Vector3.left, G.Sys.tilemap.GetTileOfTypeAt (transform.position + Vector3.left, TileID.WASTE)));
				surrondingTiles.Add (new Pair<Vector3, ATile> (transform.position + Vector3.right, G.Sys.tilemap.GetTileOfTypeAt (transform.position + Vector3.right, TileID.WASTE)));
				surrondingTiles.Add (new Pair<Vector3, ATile> (transform.position + Vector3.forward, G.Sys.tilemap.GetTileOfTypeAt (transform.position + Vector3.forward, TileID.WASTE)));
				surrondingTiles.Add (new Pair<Vector3, ATile> (transform.position + Vector3.back, G.Sys.tilemap.GetTileOfTypeAt (transform.position + Vector3.back, TileID.WASTE)));
				if (surrondingTiles [0].Second == null)
					path.addAction (new ThrowInGroundAction (this, transform.position, false));
				else if (surrondingTiles.FindIndex (t => t.Second == null) < 0) {
					path.addAction (new ThrowInGroundAction (this, transform.position, true));
					return;
				}
				else {
					List<Vector3> freeTiles = new List<Vector3> ();
					foreach (var t in surrondingTiles)
						if (t.Second == null)
							freeTiles.Add (t.First);
					path.addAction (new ThrowInGroundAction (this, freeTiles [new UniformIntDistribution (freeTiles.Count - 1).Next (new StaticRandomGenerator<DefaultRandomGenerator> ())], true));
					return;
				}
			}
		}

		var bins = G.Sys.tilemap.getSurrondingSpecialTile (transform.position, TileID.BIN, G.Sys.constants.TravelerDetectionRadius, G.Sys.constants.VerticalAmplification);

		BinTile bestBin = null;
		float bestDistance = float.MaxValue;
		foreach (var binPos in bins) {
			var bin = G.Sys.tilemap.GetTileOfTypeAt (binPos, TileID.BIN) as BinTile;
			if (bin.isFull())
				continue;
			var d = (transform.position - binPos).sqrMagnitude;
			if (d < bestDistance) {
				bestDistance = d;
				bestBin = bin;
			}
		}

		if (bestBin == null)
			return;

		List<Vector3> validPos = new List<Vector3>();
		var pos = bestBin.transform.position;
		if (G.Sys.tilemap.IsEmptyGround (pos + Vector3.forward))
			validPos.Add (pos + Vector3.forward);
		if (G.Sys.tilemap.IsEmptyGround (pos + Vector3.back))
			validPos.Add (pos + Vector3.back);
		if (G.Sys.tilemap.IsEmptyGround (pos + Vector3.left))
			validPos.Add (pos + Vector3.left);
		if (G.Sys.tilemap.IsEmptyGround (pos + Vector3.right))
			validPos.Add (pos + Vector3.right);
		
		path.addAction(new ThrowInBinAction(this, validPos[new UniformIntDistribution(validPos.Count-1).Next(new StaticRandomGenerator<DefaultRandomGenerator>())], bestBin));
	}

	void checkHunger()
	{
		if (datas.Hunger < 0.95f || path.haveAction(ActionType.BUY_FOOD))
			return;

		var tiles = G.Sys.tilemap.getSurrondingSpecialTile (transform.position, TileID.FOODDISTRIBUTEUR, G.Sys.constants.TravelerDetectionRadius, G.Sys.constants.VerticalAmplification);
		FoodDistribTile bestTile = null;
		float bestDistance = float.MaxValue;
		foreach (var value in tiles) {
			var t = G.Sys.tilemap.GetTileOfTypeAt (value, TileID.FOODDISTRIBUTEUR);
			if (t == null)
				continue;
			var dir = Orienter.orientationToDir (Orienter.angleToOrientation (t.transform.rotation.eulerAngles.y));
			if (!G.Sys.tilemap.IsEmptyGround (t.transform.position + new Vector3(-dir.y, 0, dir.x)))
				continue;
			
			var d = (value - transform.position).sqrMagnitude;
			if (d < bestDistance) {
				bestTile = t as FoodDistribTile;
				bestDistance = d;
			}
		}
		if (bestTile == null)
			return;

		if (bestTile.queue > 5) {
			foreach (var value in G.Sys.tilemap.getSurrondingSpecialTile(bestTile.transform.position, TileID.FOODDISTRIBUTEUR, G.Sys.constants.TravelerDetectionRadius, G.Sys.constants.VerticalAmplification)) {
				var v = G.Sys.tilemap.GetTileOfTypeAt(value, TileID.FOODDISTRIBUTEUR) as FoodDistribTile;
				if (v == null)
					return;
				if (v.queue < bestTile.queue)
					bestTile = v;
			}
		}

		var bestDir = Orienter.orientationToDir (Orienter.angleToOrientation (bestTile.transform.rotation.eulerAngles.y));
		path.addAction(new BuyFoodAction(this, bestTile.transform.position + new Vector3(-bestDir.y, 0, bestDir.x), bestTile));
	}


	void checkTicket ()
	{
		if (datas.HasTicket || (datas.Fraud && !G.Sys.GetNearestAgent(transform.position, G.Sys.constants.TravelerDetectionRadius)) ||path.haveAction(ActionType.BUY_TICKET))
			return;

		var tiles = G.Sys.tilemap.getSurrondingSpecialTile (transform.position, TileID.TICKETDISTRIBUTEUR, G.Sys.constants.TravelerDetectionRadius, G.Sys.constants.VerticalAmplification);
		TicketDistribTile bestTile = null;
		float bestDistance = float.MaxValue;
		foreach (var value in tiles) {
			var t = G.Sys.tilemap.GetTileOfTypeAt (value, TileID.TICKETDISTRIBUTEUR);
			if (t == null)
				continue;
			var dir = Orienter.orientationToDir (Orienter.angleToOrientation (t.transform.rotation.eulerAngles.y));
			if (!G.Sys.tilemap.IsEmptyGround (t.transform.position + new Vector3(-dir.y, 0, dir.x)))
				continue;

			var d = (value - transform.position).sqrMagnitude;
			if (d < bestDistance) {
				bestTile = t as TicketDistribTile;
				bestDistance = d;
			}
		}
		if (bestTile == null)
			return;

		if (bestTile.queue > 5) {
			foreach (var value in G.Sys.tilemap.getSurrondingSpecialTile(bestTile.transform.position, TileID.TICKETDISTRIBUTEUR, G.Sys.constants.TravelerDetectionRadius, G.Sys.constants.VerticalAmplification)) {
				var v = G.Sys.tilemap.GetTileOfTypeAt (value, TileID.TICKETDISTRIBUTEUR) as TicketDistribTile;
				if (v == null)
					return;
				if (v.queue < bestTile.queue)
					bestTile = v;
			}
		}

		var bestDir = Orienter.orientationToDir (Orienter.angleToOrientation (bestTile.transform.rotation.eulerAngles.y));
		path.addAction(new BuyTicketAction(this, bestTile.transform.position + new Vector3(-bestDir.y, 0, bestDir.x), bestTile ));

	}

	void initializeDatas()
	{
		var gen = new StaticRandomGenerator<DefaultRandomGenerator> ();
		datas.Speed = stats.MovementSpeed;
		datas.Lostness = stats.LostAbility / 100;
		datas.Tiredness = stats.FaintnessPercentage / 100;
		datas.Dirtiness = 1 - (stats.Cleanliness / 100);
		datas.Waste = new BernoulliDistribution().Next(gen) ? new UniformFloatDistribution (0.25f).Next (gen) : 0;
		datas.Hunger = new UniformFloatDistribution (0.5f).Next (gen);
		datas.HasTicket = new BernoulliDistribution ().Next (gen);
		datas.Fraud = new BernoulliDistribution (stats.FraudPercentage / 100).Next (gen);
		datas.LostNoTicket = false;
		path.canPassControl = datas.HasTicket || datas.Fraud;
	}

	public void updateDatas(float time)
	{
		updateSpeed ();

		if (!path.CanStartAction())
			return;
		updateLostness (time);
		updateTiredness (time);
		updateDirtiness (time);
	}

	void updateSpeed()
	{
		datas.Speed = stats.MovementSpeed * (2 - datas.Tiredness)/2;

		var tiles = G.Sys.tilemap.at (transform.position);
		if (tiles.Exists (t => t.type == TileID.ESCALATOR)) {
			agent.speed = G.Sys.constants.EscalatorSpeed;
		} else if (tiles.Exists (t => t.type == TileID.STAIRS)) {
			agent.speed = G.Sys.constants.StairsSpeedMultiplier * datas.Speed;
		} else
			agent.speed = datas.Speed;

		if ((datas.HasTicket || datas.Fraud) && !path.canPassControl)
			path.canPassControl = true;
	}

	void updateLostness(float time)
	{
		var signs = G.Sys.tilemap.getSurrondingSpecialTile (transform.position, TileID.INFOPANEL, G.Sys.constants.TravelerDetectionRadius, G.Sys.constants.VerticalAmplification).Count;

		datas.Lostness = Mathf.Clamp(datas.Lostness + ((signs == 0 || signs > 3) ? 1 : -1) * stats.LostAbility * stats.LostAbility / 20000 * time, 0, 1);

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

	void updateTiredness(float time)
	{
		datas.Tiredness = Mathf.Min (datas.Tiredness + stats.FaintnessPercentage * stats.FaintnessPercentage / 20000 * time, 1);
		datas.Hunger = Mathf.Min (datas.Hunger + datas.Tiredness * datas.Tiredness * time, 1);
	}

	void updateDirtiness(float time)
	{
		datas.Dirtiness = Mathf.Min (datas.Dirtiness + (1 - (stats.Cleanliness * stats.Cleanliness / 10000)) * datas.Waste * datas.Waste * time);
	}

	protected override void OnPathFinished ()
	{
		
		if (!isLost && path.canPassControl) {
			path.destnation = target;
			return;
		}

		var gen = new StaticRandomGenerator<DefaultRandomGenerator> ();
		var d = new UniformVector3SphereDistribution (G.Sys.constants.travelerLostRadius);

		for (int i = 0; i < 100; i++) {
			var pos = transform.position + d.Next(gen);
			if(G.Sys.tilemap.IsEmptyGround(pos)){
				path.destnation = pos;
				return;
			}
		}
		path.destnation = transform.position + d.Next (gen);
	}
}
