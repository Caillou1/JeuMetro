using System;
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
	[HideInInspector]
	public TravelerDatas datas = new TravelerDatas ();

	bool isLost = false;
    bool isTicketLost = false;

	private bool CanFall = true;

	private float ArrivalTime;
    private SubscriberList subscriberList = new SubscriberList();

	private Animator anim;

	private Vector3 lastPos;

	private bool isFallen = false;

	private bool CanLookForElevator;

	protected override void OnAwake ()
	{
		CanLookForElevator = true;
		G.Sys.registerTraveler (this);
		var e = findExit (targetName);
		target = e.First;
		exitType = e.Second;
		path.destnation = target;
		initializeDatas();
		ArrivalTime = Time.time;
		anim = GetComponentInChildren<Animator> ();

		if (G.Sys.constants.TravelerColors.Count > 0)
			GetComponentInChildren<SkinnedMeshRenderer> ().material.color = G.Sys.constants.TravelerColors [(new UniformIntDistribution (G.Sys.constants.TravelerColors.Count - 1).Next (new StaticRandomGenerator<DefaultRandomGenerator> ()))];
		else
			GetComponentInChildren<SkinnedMeshRenderer> ().material.color = Color.HSVToRGB ((new UniformFloatDistribution (0f, 1f).Next (new StaticRandomGenerator<DefaultRandomGenerator> ())), 1f, 1f);
        subscriberList.Add(new Event<CollectTravelerTimeEvent>.Subscriber(onCollectTime));
        subscriberList.Add(new Event<StartFireAlertEvent>.Subscriber(onFireAlertStart));
        subscriberList.Subscribe();
	}

	protected override void OnUpdate ()
	{
		checkOnExit ();
        checkOnControleLine();

		anim.SetFloat ("MovementSpeed", agent.velocity.magnitude);
		anim.SetBool ("Falling", isFallen);

		if ((lastPos - transform.position).magnitude >= .001f) {
			anim.SetBool ("Walking", true);
		} else {
			anim.SetBool ("Walking", false);
		}

		lastPos = transform.position;
	}

	void checkOnExit()
	{
        if (exitType == ExitType.DOOR && G.Sys.tilemap.GetTileOfTypeAt(transform.position, TileID.OUT) != null) {
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
		if (G.Sys.tilemap.GetTileOfTypeAt(transform.position, TileID.CONTROLELINE) != null)
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

	void checkElevators() {
		if (CanLookForElevator && !path.haveAction (ActionType.WAIT_ELEVATOR)) {
			var possiblePath = G.Sys.tilemap.GetElevatorsToFloor (transform.position, path.destnation);
			if (possiblePath.Count > 0) {
				bool CanTakeElevator = false;

				if (stats.Type == TravelerType.WHEELCHAIR) { // Si chaise roulante
					CanTakeElevator = true;
				} else if (path.HaveTileOnPath(TileID.STAIRS) && (new BernoulliDistribution (G.Sys.constants.ElevatorAttraction).Next (new DefaultRandomGenerator ()))) { // Si il y a un escalier calcule proba
					CanTakeElevator = true;
				} else { // Si pas d'autres choix
					NavMeshPath p = new NavMeshPath ();
					NavMesh.CalculatePath (transform.position, path.destnation, NavMesh.AllAreas, p);
					if (p.status != NavMeshPathStatus.PathComplete)
						CanTakeElevator = true;
				}

				if (CanTakeElevator) {
					for (int i = 0; i < possiblePath.Count; i++) {
						Vector3 pos = (i == 0) ? possiblePath [i].Second.GetWaitZone (Mathf.RoundToInt (transform.position.y)) : possiblePath [i].Second.GetWaitZone (possiblePath [i - 1].First);
						ElevatorTile tile = possiblePath [i].Second;
						int floor = ((i + 1) < possiblePath.Count) ? Mathf.RoundToInt (possiblePath [i + 1].Second.GetWaitZone (possiblePath [i].First).y) : Mathf.RoundToInt (path.destnation.y);
						int priority = (possiblePath.Count - i) * 2;
						path.addAction (new WaitForElevatorAction (this, pos, tile, floor, priority));
					}
				} else {
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
                path.addAction (new SignAction (this, validPos[new UniformIntDistribution(validPos.Count - 1).Next(new StaticRandomGenerator<DefaultRandomGenerator>())], G.Sys.tilemap.GetTileOfTypeAt(sign.First, TileID.INFOPANEL) as InfoPanelTile));
			}
		}
	}

	void checkStairs() {
		if (CanFall) {
			var tilesFront = G.Sys.tilemap.at (transform.position + transform.forward);
			var stairs = tilesFront.Find (x => x.type == TileID.STAIRS) as StairsTile;

			if (stairs != null && !stairs.HasPodotactileOnFloor(Mathf.RoundToInt(transform.position.y))) {
				float fallChance = G.Sys.constants.FallChance;

                if (stats.Malvoyant)
                    fallChance = G.Sys.constants.PartialyBlindFallChance;

				if (stats.Type == TravelerType.BLIND)
                    fallChance = G.Sys.constants.BlindFallChance;

                var chance = new BernoulliDistribution(fallChance / 100f).Next(new StaticRandomGenerator<DefaultRandomGenerator>());

				if (chance && !path.haveAction (ActionType.FAINT)) {
					datas.Tiredness = 1f;
					isFallen = true;
					path.addAction (new StairsFallAction (this, stairs));
					CanFall = false;
				}
			}
		}
	}

	public void GetUp() {
		StartCoroutine (CanFallDelay ());
		isFallen = false;
	}
		
	IEnumerator CanFallDelay() {
		yield return new WaitForSeconds (3f);
		CanFall = true;
	}

	void checkTiredness()
	{
		if (datas.Tiredness > 0.95f && !path.haveAction(ActionType.FAINT)) {
			var tiles = G.Sys.tilemap.at (new Vector3i (transform.position + transform.forward));

			foreach (var tile in tiles) {
				if (tile.type == TileID.STAIRS) {
					path.addAction (new StairsFallAction (this, tile as StairsTile));
					isFallen = true;
					return;
				}
			}

			isFallen = true;
			path.addAction (new FaintAction (this));
			return;
		}
		if (datas.Tiredness < (1f - stats.RestPlaceAttraction / 100f) || path.haveAction (ActionType.SIT))
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

            path.addAction(new ThrowInBinAction(this, validPos[new UniformIntDistribution(validPos.Count - 1).Next(new StaticRandomGenerator<DefaultRandomGenerator>())], bestBin));
        }
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
		path.addAction(new BuyFoodAction(this, bestTile.transform.position + new Vector3(-bestDir.y, 0, bestDir.x), bestTile));
	}


	void checkTicket ()
	{
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
		path.addAction(new BuyTicketAction(this, bestTile.transform.position + new Vector3(-bestDir.y, 0, bestDir.x), bestTile ));

	}

	void initializeDatas()
	{
		var gen = new StaticRandomGenerator<DefaultRandomGenerator>();
        stats.FaintnessPercentage += new UniformFloatDistribution(-stats.DeltaFainteness, stats.DeltaFainteness).Next(gen);
        stats.Cleanliness += new UniformFloatDistribution(-stats.DeltaCleanliness, stats.DeltaCleanliness).Next(gen);

		datas.Speed = stats.MovementSpeed;
		datas.Lostness = stats.LostAbility / 100;
        datas.Tiredness = stats.FaintnessPercentage / 100;
        datas.Dirtiness = 1 - (stats.Cleanliness / 100);
        datas.Waste = stats.wastes/100;
        datas.Hunger = new BernoulliDistribution(stats.Hunger / 100).Next(gen) ? 1 : 0;
		datas.Fraud = new BernoulliDistribution (stats.FraudPercentage / 100).Next (gen);
		datas.LostNoTicket = false;
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
		if (tiles.Exists (t => t.type == TileID.ESCALATOR)) {
			agent.speed = G.Sys.constants.EscalatorSpeed;
		} else if (tiles.Exists (t => t.type == TileID.STAIRS)) {
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

			datas.Lostness = Mathf.Clamp (datas.Lostness + ((total <= 0.1f || signs > 3f) ? 1 : -1) * stats.LostAbility * stats.LostAbility / 20000 * time, 0, 1);
		} else {
			TileID[] validTile = new TileID[]{ TileID.PODOTACTILE, TileID.CONTROLELINE, TileID.ELEVATOR, TileID.ESCALATOR, TileID.IN, TileID.OUT, TileID.METRO};
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
        
    }
}
