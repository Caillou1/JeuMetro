using System;
using UnityEngine;

public class BuyFoodAction : AEntityAction<Traveler>
{
	FoodDistribTile foodDistrib;
	float time = 0;

	public BuyFoodAction (Traveler t, Vector3 pos, FoodDistribTile tile) : base(t, ActionType.BUY_FOOD, pos)
	{
		foodDistrib = tile;
	}

	protected override bool Update ()
	{
		time += Time.deltaTime;
		return time > 2;
	}

	protected override void End ()
	{
		entity.datas.Hunger = 0;
		entity.datas.Waste += 0.5f;
		entity.datas.Tiredness = 0;
		G.Sys.gameManager.AddMoney (foodDistrib.price);
	}
}