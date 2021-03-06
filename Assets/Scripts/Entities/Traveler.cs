﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NRand;
using UnityEngine.AI;

public class Traveler : AEntity
{
	enum ExitType{ DOOR, METRO }

	ExitType exitType;

	public TravelerStats stats = new TravelerStats ();

	[SerializeField]
	string targetName = "";
	//[HideInInspector]
	public TravelerDatas datas = new TravelerDatas ();

	bool isLost = false;
    bool isTicketLost = false;

	private bool CanFall = true;

	private float ArrivalTime;
    private SubscriberList subscriberList = new SubscriberList();

    bool fireAlertStarted = false;

	[HideInInspector]
	public Animator anim;

	private Vector3 lastPos;

	private bool CanLookForElevator;

    private NavMeshPath navmeshPath;

	protected override void OnAwake ()
	{
        navmeshPath = new NavMeshPath();
		CanLookForElevator = true;
        if (!G.Sys.gameManager.FireAlert)
            G.Sys.registerTraveler(this);
        else G.Sys.registerFalseTraveler(this);
		var e = findExit (targetName);
		target = e.First;
		exitType = e.Second;
		path.destnation = target;
		initializeDatas();
		ArrivalTime = Time.time;
		anim = GetComponentInChildren<Animator> ();
        if (!G.Sys.gameManager.FireAlert)
            GetComponentInChildren<SkinnedMeshRenderer>().material.color = G.Sys.constants.GetRandomColor(G.Sys.constants.TravelerSaturation);

        subscriberList.Add(new Event<CollectTravelerTimeEvent>.Subscriber(onCollectTime));
        subscriberList.Add(new Event<StartFireAlertEvent>.Subscriber(onFireAlertStart));
        subscriberList.Subscribe();
	}

	protected override void OnUpdate ()
	{
        if (gameObject.activeSelf)
        {
            anim.SetFloat("MovementSpeed", agent.velocity.magnitude);

            if ((lastPos - transform.position).magnitude >= .001f)
            {
                anim.SetBool("Walking", true);
            }
            else
            {
                anim.SetBool("Walking", false);
            }
            if (fireAlertStarted)
			{
				fireAlertStarted = false;
                StartFireAlert();
            }
        }

		checkOnExit();
		checkOnControleLine();

		lastPos = transform.position;
	}

	void checkOnExit()
	{
        if (exitType == ExitType.DOOR) {
            var tile = G.Sys.tilemap.GetTileOfTypeAt(transform.position, TileID.OUT) as ExitsTile;
            if (tile != null && tile.exitname == targetName) 
			    Destroy (gameObject);
			return;
		}
        if (exitType == ExitType.METRO && !path.haveAction (ActionType.WAIT_METRO) && !path.haveAction(ActionType.GO_TO_METRO) && new Vector3i (transform.position).Equals (new Vector3i (target)) && G.Sys.tilemap.GetTileOfTypeAt (transform.position, TileID.WAIT_ZONE) != null) {
			path.addAction (new WaitMetroAction (this, new Vector3i (transform.position).toVector3 ()));
			return;
		}
	}

    void checkOnControleLine()
    {
        if (G.Sys.gameManager.FireAlert)
            return;

        var tile = G.Sys.tilemap.GetTileOfTypeAt(transform.position, TileID.CONTROLELINE) as ControleLineTile;
        if (tile != null && !tile.canPassWithoutTicket)
		{
			isTicketLost = false;
            stats.HaveTicket = true;
			path.canPassControl = true;
			path.lostness = 0;
		}
    }

	static Pair<Vector3, ExitType> findExit(string name)
	{
		List<Pair<Vector3, ExitType>> validTiles = new List<Pair<Vector3, ExitType>> ();
		foreach (var m in G.Sys.tilemap.getSpecialTiles(TileID.OUT)) {
			var t = G.Sys.tilemap.GetTileOfTypeAt (m, TileID.OUT) as ExitsTile;
			if (t == null)
				continue;
			if (t.exitname == name)
				validTiles.Add (new Pair<Vector3, ExitType>(m, ExitType.DOOR));
		}

		foreach (var m in G.Sys.tilemap.getSpecialTiles(TileID.WAIT_ZONE)) {
			var t = G.Sys.tilemap.GetTileOfTypeAt (m, TileID.WAIT_ZONE) as WaitZoneTile;
			if (t == null)
				continue;
			if (G.Sys.tilemap.GetTileOfTypeAt (m, TileID.GROUND) == null)
				continue;
			if (t.exitname == name)
				validTiles.Add (new Pair<Vector3, ExitType>(m, ExitType.METRO));
		}

		if (validTiles.Count == 0)
			return new Pair<Vector3, ExitType> (Vector3.zero, ExitType.DOOR);

		return validTiles [new UniformIntDistribution (validTiles.Count - 1).Next (new StaticRandomGenerator<DefaultRandomGenerator> ())];
	}
		
	void OnDestroy()
	{
		G.Sys.removeTraveler (this);
        G.Sys.removeFalseTraveler(this);
		G.Sys.gameManager.AddTime (Time.time - ArrivalTime);
        subscriberList.Unsubscribe();
	}

	protected override void Check ()
	{
		checkSigns ();
		checkTiredness ();
		checkWaste ();
		checkHunger ();
		checkTicket ();
		checkElevators ();
		checkStairs ();
	}

	void checkElevators() 
    {
		if (CanLookForElevator && !path.haveAction (ActionType.WAIT_ELEVATOR)) 
        {
			bool CanTakeElevator = false;

			if (stats.Type == TravelerType.WHEELCHAIR)
			{ // Si chaise roulante
				CanTakeElevator = true;
			}
			else if (Mathf.RoundToInt((target - transform.position).y) >= 1 && (new BernoulliDistribution(G.Sys.constants.ElevatorAttraction).Next(new DefaultRandomGenerator())))
			{ // Si il y a un escalier calcule proba
				CanTakeElevator = true;
			}
			else
			{ // Si pas d'autres choix
				NavMeshQueryFilter filter = new NavMeshQueryFilter();
				filter.agentTypeID = agent.agentTypeID;
				filter.areaMask = NavMesh.AllAreas;
				NavMesh.CalculatePath(transform.position, path.destnation, filter, navmeshPath);
				if (navmeshPath.status != NavMeshPathStatus.PathComplete)
					CanTakeElevator = true;
			}
			if (stats.Type != TravelerType.WHEELCHAIR && G.Sys.gameManager.FireAlert)
				CanTakeElevator = false;
            if (CanTakeElevator)
			{
    			var possiblePath = G.Sys.tilemap.GetElevatorsToFloor(transform.position, path.destnation);
    			if (possiblePath.Count > 0) 
                {
				
					for (int i = 0; i < possiblePath.Count; i++) 
                    {
						Vector3 pos = (i == 0) ? possiblePath [i].Second.GetWaitZone (Mathf.RoundToInt (transform.position.y)) : possiblePath [i].Second.GetWaitZone (possiblePath [i - 1].First);
						ElevatorTile tile = possiblePath [i].Second;
                        if (stats.Type != TravelerType.WHEELCHAIR && tile.peopleWaiting > G.Sys.constants.ElevatorMaxPeople)
                            continue;
						int floor = ((i + 1) < possiblePath.Count) ? Mathf.RoundToInt (possiblePath [i + 1].Second.GetWaitZone (possiblePath [i].First).y) : Mathf.RoundToInt (path.destnation.y);
						path.addAction (new WaitForElevatorAction (this, pos, tile, floor, stats.Type != TravelerType.WHEELCHAIR));
					}
				} 
                else 
                {
					CanLookForElevator = false;
					StartCoroutine(DelayedElevator());
				}
			}
		}
	}

	IEnumerator DelayedElevator() {
		yield return new WaitForSeconds (3f);
		CanLookForElevator = true;
	}

	void checkSigns()
	{
        if (stats.Type == TravelerType.BLIND)
            return;
        
		if (datas.Lostness > 0.5f && !path.haveAction (ActionType.SIGN)) {
            var sign = G.Sys.tilemap.getNearestSpecialTileOfType (transform.position, TileID.INFOPANEL, G.Sys.constants.VerticalAmplification, G.Sys.constants.TravelerDetectionRadius * (stats.Malvoyant ? 0.5f : 1.0f) );
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
                var pos = validPos[new UniformIntDistribution(validPos.Count - 1).Next(new StaticRandomGenerator<DefaultRandomGenerator>())];
                agent.CalculatePath(pos, navmeshPath);
                if (navmeshPath.status != NavMeshPathStatus.PathComplete)
                    return;
                path.addAction (new SignAction (this, pos, G.Sys.tilemap.GetTileOfTypeAt(sign.First, TileID.INFOPANEL) as InfoPanelTile));
			}
		}
	}

	void checkStairs() {
		if (stats.Type != TravelerType.WHEELCHAIR && CanFall) {
			var tilesFront = G.Sys.tilemap.at (transform.position + transform.forward);
			var stairs = tilesFront.Find (x => x.type == TileID.STAIRS) as StairsTile;

			if (stairs != null && stairs.IsOnStairsPath (new Vector3i (tf.position)) && !stairs.HasPodotactileOnFloor (Mathf.RoundToInt (transform.position.y))) {
				float fallChance = G.Sys.constants.FallChance;

				if (stats.Malvoyant)
					fallChance = G.Sys.constants.PartialyBlindFallChance;

				if (stats.Type == TravelerType.BLIND)
					fallChance = G.Sys.constants.BlindFallChance;

				var chance = new BernoulliDistribution (fallChance / 100f).Next (new StaticRandomGenerator<DefaultRandomGenerator> ());

				if (chance && !path.haveAction (ActionType.FAINT)) {
					datas.Tiredness = 1f;
					path.addAction (new StairsFallAction (this, stairs));
					CanFall = false;
					StartCoroutine (CanFallDelay ());
				}
			}
		}
	}

	public void GetUp() {
		StartCoroutine (CanFallDelay ());
	}
		
	IEnumerator CanFallDelay() {
		yield return new WaitForSeconds (3f);
		CanFall = true;
	}

	void checkTiredness()
	{
		if (stats.Type != TravelerType.WHEELCHAIR && datas.Tiredness > 0.95f && !path.haveAction()) {
			var tiles = G.Sys.tilemap.at (new Vector3i (transform.position + transform.forward));

			foreach (var tile in tiles) {
				if (tile.type == TileID.STAIRS) {
					path.addAction (new StairsFallAction (this, tile as StairsTile));
					return;
				}
			}

			path.addAction (new FaintAction (this));
			return;
		}
        if (datas.Tiredness < (1f - stats.RestPlaceAttraction / 100f) || path.haveAction(ActionType.SIT) || G.Sys.gameManager.FireAlert)
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
        agent.CalculatePath(bench.First.sideToFrontPos(bench.Second), navmeshPath);
        if (navmeshPath.status != NavMeshPathStatus.PathComplete)
            return;
		path.addAction (new SitAction (this, bench.First.sideToFrontPos (bench.Second), bench.First, bench.Second));
	}

	void checkWaste()
	{
		if (G.Sys.gameManager.FireAlert)
			return;
        
		if (datas.Waste < 0.01f || path.haveAction(ActionType.THROW_IN_BIN) || path.haveAction(ActionType.THROW_IN_GROUND))
			return;

        bool haveAgentNearby = G.Sys.GetNearestCleaner(transform.position, G.Sys.constants.TravelerDetectionRadius) != null ||
                                G.Sys.GetNearestAgent(transform.position, G.Sys.constants.TravelerDetectionRadius) != null;

        if (datas.Dirtiness > 0.95f && !haveAgentNearby) {
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

        if (!stats.IgnoreBin || haveAgentNearby)
        {
            var bins = G.Sys.tilemap.getSurrondingSpecialTile(transform.position, TileID.BIN, G.Sys.constants.TravelerDetectionRadius, G.Sys.constants.VerticalAmplification);

            BinTile bestBin = null;
            float bestDistance = float.MaxValue;
            foreach (var binPos in bins)
            {
                var bin = G.Sys.tilemap.GetTileOfTypeAt(binPos, TileID.BIN) as BinTile;
                if (bin.isFull())
                    continue;
                var d = (transform.position - binPos).sqrMagnitude;
                if (d < bestDistance)
                {
                    bestDistance = d;
                    bestBin = bin;
                }
            }

            if (bestBin == null)
                return;

            List<Vector3> validPos = new List<Vector3>();
            var pos = bestBin.transform.position;
            if (G.Sys.tilemap.IsEmptyGround(pos + Vector3.forward))
                validPos.Add(pos + Vector3.forward);
            if (G.Sys.tilemap.IsEmptyGround(pos + Vector3.back))
                validPos.Add(pos + Vector3.back);
            if (G.Sys.tilemap.IsEmptyGround(pos + Vector3.left))
                validPos.Add(pos + Vector3.left);
            if (G.Sys.tilemap.IsEmptyGround(pos + Vector3.right))
                validPos.Add(pos + Vector3.right);

            var targetPos = validPos[new UniformIntDistribution(validPos.Count - 1).Next(new StaticRandomGenerator<DefaultRandomGenerator>())];
			agent.CalculatePath(targetPos, navmeshPath);
			if (navmeshPath.status != NavMeshPathStatus.PathComplete)
				return;

            path.addAction(new ThrowInBinAction(this, pos, bestBin));
        }
	}

	void checkHunger()
	{
		if (G.Sys.gameManager.FireAlert)
			return;
        
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

        if (bestTile.queue > G.Sys.constants.QueueMax) {
			foreach (var value in G.Sys.tilemap.getSurrondingSpecialTile(bestTile.transform.position, TileID.FOODDISTRIBUTEUR, G.Sys.constants.TravelerDetectionRadius, G.Sys.constants.VerticalAmplification)) {
				var v = G.Sys.tilemap.GetTileOfTypeAt(value, TileID.FOODDISTRIBUTEUR) as FoodDistribTile;
				if (v == null)
					return;
				if (v.queue < bestTile.queue)
					bestTile = v;
			}
		}

		var bestDir = Orienter.orientationToDir (Orienter.angleToOrientation (bestTile.transform.rotation.eulerAngles.y));
        var pos = bestTile.transform.position + new Vector3(-bestDir.y, 0, bestDir.x);
		agent.CalculatePath(pos, navmeshPath);
		if (navmeshPath.status != NavMeshPathStatus.PathComplete)
			return;
		path.addAction(new BuyFoodAction(this, pos, bestTile));
	}


	void checkTicket ()
	{
		if (G.Sys.gameManager.FireAlert)
			return;
        
		if (stats.HaveTicket || (datas.Fraud && !G.Sys.GetNearestAgent(transform.position, G.Sys.constants.TravelerDetectionRadius)) ||path.haveAction(ActionType.BUY_TICKET))
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

        if (bestTile.queue > G.Sys.constants.QueueMax) {
			foreach (var value in G.Sys.tilemap.getSurrondingSpecialTile(bestTile.transform.position, TileID.TICKETDISTRIBUTEUR, G.Sys.constants.TravelerDetectionRadius, G.Sys.constants.VerticalAmplification)) {
				var v = G.Sys.tilemap.GetTileOfTypeAt (value, TileID.TICKETDISTRIBUTEUR) as TicketDistribTile;
				if (v == null)
					return;
				if (v.queue < bestTile.queue)
					bestTile = v;
			}
		}

		var bestDir = Orienter.orientationToDir (Orienter.angleToOrientation (bestTile.transform.rotation.eulerAngles.y));
		var pos = bestTile.transform.position + new Vector3(-bestDir.y, 0, bestDir.x);
		if (agent != null && navmeshPath != null) {
			agent.CalculatePath (pos, navmeshPath);
			if (navmeshPath.status != NavMeshPathStatus.PathComplete)
				return;
		
			path.addAction (new BuyTicketAction (this, pos, bestTile));
		}
	}

	void initializeDatas()
	{
		var gen = new StaticRandomGenerator<DefaultRandomGenerator>();
        stats.FaintnessPercentage += new UniformFloatDistribution(-stats.DeltaFainteness, stats.DeltaFainteness).Next(gen);
        stats.FaintnessPercentage = Mathf.Clamp(stats.FaintnessPercentage, 0, 100);
        stats.Cleanliness += new UniformFloatDistribution(-stats.DeltaCleanliness, stats.DeltaCleanliness).Next(gen);
        stats.Cleanliness = Mathf.Clamp(stats.Cleanliness, 0, 100);

		datas.Speed = stats.MovementSpeed;
		datas.Lostness = stats.LostAbility / 100;
        datas.Tiredness = stats.FaintnessPercentage / 100;
        datas.Dirtiness = 1 - (stats.Cleanliness / 100);
        datas.Waste = stats.wastes/100;
        datas.Hunger = new BernoulliDistribution(stats.Hunger / 100).Next(gen) ? 1 : 0;
		datas.Fraud = new BernoulliDistribution (stats.FraudPercentage / 100).Next (gen);
		path.canPassControl = stats.HaveTicket || datas.Fraud;
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
        if (!G.Sys.gameManager.FireAlert && tiles.Exists (t => t.type == TileID.ESCALATOR)) {
			agent.speed = G.Sys.constants.EscalatorSpeed;
		} else if (tiles.Exists (t => t.type == TileID.STAIRS) || (G.Sys.gameManager.FireAlert && tiles.Exists(t => t.type == TileID.ESCALATOR))) {
			agent.speed = G.Sys.constants.StairsSpeedMultiplier * datas.Speed;
		} else
			agent.speed = datas.Speed;

        if (stats.HaveTicket && isTicketLost)
            isTicketLost = false;

		if ((stats.HaveTicket || datas.Fraud) && !path.canPassControl)
			path.canPassControl = true;
	}

	void updateLostness(float time)
	{
		
		var signs = G.Sys.tilemap.getSurrondingSpecialTile (transform.position, TileID.INFOPANEL, G.Sys.constants.TravelerDetectionRadius, G.Sys.constants.VerticalAmplification).Count;
		var sound = G.Sys.tilemap.getSurrondingSpecialTile (transform.position, TileID.SPEAKER, G.Sys.constants.TravelerDetectionRadius, G.Sys.constants.VerticalAmplification).Count;
		if (!SpeakerTile.isEmiting())
			sound = 0;
		if (stats.Type != TravelerType.BLIND) {
			float coefSign = 1;
			float coefSound = 1;

			if (stats.Deaf)
				coefSound = 0;

			if (stats.Malvoyant)
				coefSign /= 2;
			if (stats.Type == TravelerType.CLASSIC || stats.Type == TravelerType.WHEELCHAIR || stats.Type == TravelerType.WITH_BAG)
				coefSound /= 2;
			float total = signs * coefSign + sound * coefSound;

            datas.Lostness = Mathf.Clamp (datas.Lostness + ((total <= 0.1f || signs > 3f) ? 1 : -1) * stats.LostAbility * stats.LostAbility / 20000 * time * G.Sys.constants.lostMultiplier, 0, 1);
		} else {
			TileID[] validTile = new TileID[]{ TileID.PODOTACTILE, TileID.CONTROLELINE, TileID.ELEVATOR, TileID.ESCALATOR, TileID.OUT, TileID.METRO};
			bool isOn = false;
			foreach (var t in validTile)
				if (G.Sys.tilemap.GetTileOfTypeAt (transform.position, t) != null) {
					isOn = true;
					break;
				}
			if (isOn)
				datas.Lostness = 0;
			else if (sound == 0 || sound > 3)
				datas.Lostness = 1;
			else
				datas.Lostness = 0;
		}

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
        if (!isLost && !path.isLastPathNeedPassControl() && !isTicketLost) {
			path.destnation = target;
			return;
		}

        if (path.isLastPathNeedPassControl())
            isTicketLost = true;

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

    void onCollectTime(CollectTravelerTimeEvent e)
    {
        G.Sys.gameManager.AddTime(Time.time - ArrivalTime, false);
    }

    void onFireAlertStart(StartFireAlertEvent e)
    {
        Invoke("StartFireAlert", new UniformFloatDistribution(G.Sys.constants.MinTravelerFireAlertDelay, G.Sys.constants.MaxTravelerFireAlertDelay).Next(new StaticRandomGenerator<DefaultRandomGenerator>()));
        /*if (gameObject.activeSelf)
            StartCoroutine(StartFireAlertCoroutine());
        else fireAlertStarted = true;*/
    }

    void StartFireAlert()
    {
        if (!gameObject.activeSelf)
        {
            fireAlertStarted = true;
            return;
        }

		stats.MovementSpeed *= G.Sys.constants.fireAlertSpeedMultiplier;
		if (stats.Type != TravelerType.WHEELCHAIR)
			path.abortAllAndActiveActionElse(new ActionType[] { ActionType.FAINT, ActionType.SIGN });
		else
			path.abortAllAndActiveActionElse(new ActionType[] { ActionType.FAINT, ActionType.SIGN, ActionType.WAIT_ELEVATOR });

		float bestDist = float.MaxValue;
		Vector3 posDoor = Vector3.zero;
		foreach (var d in G.Sys.tilemap.getSpecialTiles(TileID.OUT))
		{
			float dist = (d - transform.position).sqrMagnitude;
			if (dist < bestDist)
			{
				bestDist = dist;
				posDoor = d;
			}
		}

		target = posDoor;
		exitType = ExitType.DOOR;
		var door = G.Sys.tilemap.GetTileOfTypeAt(posDoor, TileID.OUT) as ExitsTile;
		if (door != null)
			targetName = door.exitname;

        if(!path.haveAction())
            agent.enabled = true;

		stats.HaveTicket = true;
		isTicketLost = false;
		path.canPassControl = true;
		path.destnation = target;
    }
}
