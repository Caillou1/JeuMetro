using System;
using UnityEngine;
using NRand;

public class ThrowInGroundAction : AEntityAction<Traveler>
{
	float time = 0;

	public ThrowInGroundAction (Traveler t) : base(t, ActionType.THROW_IN_GROUND, t.transform.position)
	{
		
	}

	protected override bool Start ()
	{
		return entity.datas.Waste == 0;
	}

	protected override bool Update ()
	{
		time += Time.deltaTime;
		return time >= 2;
	}

	protected override void End ()
	{
		GameObject.Instantiate(G.Sys.gameManager.wastePrefab, entity.transform.position, Quaternion.Euler(0, new UniformIntDistribution(3).Next(new StaticRandomGenerator<DefaultRandomGenerator>()) * 90, 0));
		entity.datas.Dirtiness = 1 - (entity.stats.Cleanliness / 100);
		Event<BakeNavMeshEvent>.Broadcast (new BakeNavMeshEvent ());
	}
}

