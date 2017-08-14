using System;
using UnityEngine;
using NRand;

public class ThrowInGroundAction : AEntityAction<Traveler>
{
	float time = 0;
	bool bigWaste = false;

	public ThrowInGroundAction (Traveler t, Vector3 pos, bool big) : base(t, ActionType.THROW_IN_GROUND, pos)
	{
		bigWaste = big;
	}

	protected override bool Start ()
	{
		return entity.datas.Waste == 0;
	}

	protected override bool Update ()
	{
		time += Time.deltaTime;
        return time >= G.Sys.constants.WasteGroundTime;
	}

	protected override void End ()
	{
		if (!G.Sys.tilemap.IsEmptyGround (entity.transform.position))
			return;
		
		var waste = G.Sys.tilemap.GetTileOfTypeAt (entity.transform.position, TileID.WASTE) as WasteTile;

		if (bigWaste && waste != null)
			waste.big = true;
		else if (waste != null)
			return;
		else
			GameObject.Instantiate(G.Sys.gameManager.wastePrefab, new Vector3i(entity.transform.position).toVector3(), Quaternion.Euler(0, new UniformIntDistribution(3).Next(new StaticRandomGenerator<DefaultRandomGenerator>()) * 90, 0));
		entity.datas.Dirtiness = 1 - (entity.stats.Cleanliness / 100);
		entity.datas.Waste = 0;
		G.Sys.audioManager.PlayTrash ();
		Event<BakeNavMeshEvent>.Broadcast (new BakeNavMeshEvent ());
	}
}

