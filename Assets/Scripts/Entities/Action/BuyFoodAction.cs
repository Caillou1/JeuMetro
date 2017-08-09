using System;
using UnityEngine;

public class BuyFoodAction : AEntityAction<Traveler>
{
	FoodDistribTile foodDistrib;
	float time = 0;
	float maxTime = 0;

	public BuyFoodAction (Traveler t, Vector3 pos, FoodDistribTile tile) : base(t, ActionType.BUY_FOOD, pos)
	{
		foodDistrib = tile;
	}

	protected override bool Start ()
	{
		foodDistrib.queue++;
		maxTime = foodDistrib.queue * 2;
		return false;
	}

	protected override bool Update ()
	{
		time += Time.deltaTime;
		return time > maxTime;
	}

	protected override void End ()
	{
		entity.datas.Hunger = 0;
		entity.datas.Waste += 0.5f;
		entity.datas.Tiredness = 0;
		foodDistrib.queue--;
		G.Sys.gameManager.AddMoney (foodDistrib.price);
		G.Sys.audioManager.PlayBuyFood ();
	}
}