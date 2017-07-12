using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NRand;

public class Traveler : AEntity 
{
	public TravelerStats Stats;
	[HideInInspector]
	public TravelerDatas datas = new TravelerDatas ();

	float lostNessOnLastPath = 0;

	protected override void OnEntityAwake ()
	{
		states.Add (new MoveState (this));
		states.Add (new StairsState (this));
		states.Add (new EscalatorState (this));
		states.Add (new LostState (this));
		states.Add (new InfosState (this));
		states.Add (new SitState (this));
		states.Add (new BuyFoodState (this));
		states.Add (new WasteState (this));
		states.Add (new BuyTicketState (this));

		G.Sys.registerTraveler (this);
		configureDatasFromStats ();
	}

	protected override void OnEntityUpdate ()
	{
		updateDatas ();
		DestroyOnExit ();

		Debug.DrawRay (transform.position, Vector3.up * datas.Dirtiness, Color.magenta);
	}

	protected override void OnEntityDestroy ()
	{
		G.Sys.removeTraveler (this);
	}

	public override void BackToMoveState ()
	{
		SetNextState (StateType.TRAVELER_MOVE);
	}

	protected override void configurePathfinder()
	{
		var dico = new Dictionary<TileID, float> ();
		dico.Add (TileID.ESCALATOR, statValueToPathWeight(Stats.EscalatorAttraction));
		dico.Add (TileID.ELEVATOR, statValueToPathWeight(Stats.ElevatorAttraction));
		dico.Add (TileID.STAIRS, statValueToPathWeight(Stats.StairsAttraction));
		dico.Add (TileID.PODOTACTILE, statValueToPathWeight (Stats.TouchComprehension));
		dico.Add (TileID.WASTE, 20);

		switch (Stats.Type) {
		case TravelerType.WITH_BAG:
			dico [TileID.ELEVATOR] *= 2;
			dico [TileID.STAIRS] /= 10;
			break;
		case TravelerType.WHEELCHAIR:
			dico [TileID.ELEVATOR] *= 10;
			dico [TileID.ESCALATOR] /= 10;
			dico [TileID.STAIRS] /= 10;
			break;
		case TravelerType.BLIND:
			dico [TileID.ELEVATOR] *= 5;
			dico [TileID.ESCALATOR] *= 5;
			dico [TileID.STAIRS] /= 10;
			dico [TileID.PODOTACTILE] *= 10;
			dico [TileID.WASTE] = 2;
			break;
		default:
			break;
		}
		path = new Path (dico);
	}

	void configureDatasFromStats()
	{
		var gen = new StaticRandomGenerator<DefaultRandomGenerator> ();
		datas.Speed = Stats.MovementSpeed / (Stats.FaintnessPercentage / 100f + 1);
		datas.Dirtiness = new UniformFloatDistribution (0, 0.5f - Stats.Cleanliness / 200f).Next (gen);
		datas.Lostness = Stats.LostAbility / 100f;
		//datas.Waste = new UniformFloatDistribution (0, 0.5f).Next (new StaticRandomGenerator<DefaultRandomGenerator> ());;
		datas.Tiredness = Stats.FaintnessPercentage / 100f;
		datas.Hunger = new UniformFloatDistribution (0, 1).Next (gen);
		datas.HasTicket = new BernoulliDistribution (0.5f).Next (gen);
	}

	void updateDatas()
	{
		if (datas.Waste == 0)
			datas.Dirtiness = new UniformFloatDistribution (0, 0.5f - Stats.Cleanliness / 200f).Next (new StaticRandomGenerator<DefaultRandomGenerator> ());
		else
			datas.Dirtiness += ((0.5f - Stats.Cleanliness / 200f) * datas.Waste) * Time.deltaTime;
		datas.Dirtiness = Mathf.Min (datas.Dirtiness, 1);

		var infoPannels = G.Sys.tilemap.getSpecialTiles (TileID.INFOPANEL);
		float infoPannelsPower = 0;
		foreach (var i in infoPannels) {
			var dist = (i - transform.position);
			var d = new Vector2 (dist.x, dist.z).magnitude + Mathf.Abs(5 * dist.y);
			if (d > 6)
				continue;
			infoPannelsPower += 1 - (d / 6);
		}
		infoPannelsPower = Mathf.Max(infoPannelsPower < 1 ? infoPannelsPower : 1.5f - 0.5f * infoPannelsPower, 0);
		var lostness = infoPannelsPower < 0.5f ? Stats.LostAbility / 100f * Stats.LostAbility / 100f * (0.5f - infoPannelsPower) : -infoPannelsPower + 0.5f;
		datas.Lostness += lostness * Time.deltaTime;
		datas.Lostness = Mathf.Clamp (datas.Lostness, 0, 1);
		if (Mathf.Abs (datas.Lostness - lostNessOnLastPath) > 0.2f)
			Updatepath ();

		datas.Speed = Stats.MovementSpeed / (datas.Tiredness + 1);
		datas.Tiredness += Stats.FaintnessPercentage / 100f * Stats.FaintnessPercentage / 100f * Time.deltaTime;
		datas.Tiredness = Mathf.Min (datas.Tiredness, 1);
	}

	float statValueToPathWeight(float value)
	{
		return Mathf.Pow (2, -value / 30);
	}

	protected override void InitialiseTarget()
	{
		var tiles = G.Sys.tilemap.getSpecialTiles (TileID.OUT);
		if (tiles.Count == 0)
			return;

		destination = tiles[new UniformIntDistribution (tiles.Count-1).Next(new StaticRandomGenerator<DefaultRandomGenerator> ())];
	}

	void DestroyOnExit()
	{
		if (G.Sys.tilemap.haveSpecialTileAt (TileID.OUT, transform.position))
			Destroy (gameObject);
	}

	public override void Updatepath ()
	{
		if (altAction != ActionType.NONE) {
			path.create (transform.position, altDestination, datas.Lostness);
		} else {
			altWait = true;
			path.create (transform.position, destination, datas.Lostness);
		}

		lostNessOnLastPath = datas.Lostness;
	}
}
