using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using NRand;

namespace V1
{
public class AgentHelpState : AAgentState
{
	Vector3 dest = new Vector3();
	Vector3 wastePos = new Vector3 ();

	public AgentHelpState (AgentEntity t) : base(t, StateType.AGENT_HELP)
	{
		
	}

	public override int check()
	{
		return 0;
		/*
		if(agent.altAction == AEntity.ActionType.CLEAN && new Vector3i (dest).equal (new Vector3i (agent.transform.position)))
			return int.MaxValue;

		if (agent.altAction != AEntity.ActionType.NONE)
			return 0;

		var wastes = G.Sys.tilemap.getSurrondingSpecialTile (agent.transform.position, TileID.WASTE, agent.Stats.WasteVisibilityRadius, G.Sys.constants.VerticalAmplification);
		Vector3 waste = new Vector3 ();
		if (wastes.Count != 0) {
			METHODE DE PIERRE
			foreach (var w in wastes) {
				var tile = G.Sys.tilemap.GetTileOfTypeAt (w, TileID.WASTE);
				var wasteTile = tile as WasteTile;
				if (wasteTile != null) {
					if (wasteTile.CanBeCleaned) {
						wasteTile.CanBeCleaned = false;
						waste = w;
						break;
					}
				}
			}

			ANCIENNE METHODE
			waste = wastes[new UniformIntDistribution (wastes.Count - 1).Next (new StaticRandomGenerator<DefaultRandomGenerator> ())];
			var dir = waste - agent.transform.position;
			if (new Vector2 (dir.x, dir.z).magnitude + G.Sys.constants.VerticalAmplification * dir.y <= agent.Stats.WasteVisibilityRadius)
				cleanType = TileID.WASTE;
			}

		if (cleanType == TileID.BIN) {
			var bins = G.Sys.tilemap.getSurrondingSpecialTile (agent.transform.position, TileID.BIN, agent.Stats.WasteVisibilityRadius, G.Sys.constants.VerticalAmplification);
			if (bins.Count == 0)
				return 0;
			Vector3 bestPos = new Vector3 ();
			float bestDist = float.MaxValue;

			foreach (var b in bins) {
				var b1 = G.Sys.tilemap.tilesOfTypeAt (b, TileID.BIN);
				if (b1.Count == 0)
					continue;
				var b2 = b1 [0] as BinTile;
				if (b2.isEmpty ())
					continue;	
				var dir = b - agent.transform.position;
				var dist = new Vector2 (dir.x, dir.z).magnitude + Mathf.Abs(dir.y * G.Sys.constants.VerticalAmplification);
				if (dist < bestDist) {
					bestDist = dist;
					bestPos = b;
				}
			}

			if (bestDist == float.MaxValue)
				return 0;
			waste = bestPos;
		}
			
		List<Vector3> poss = new List<Vector3> ();
		poss.Add (waste + Vector3.left);
		poss.Add (waste + Vector3.right);
		poss.Add (waste + Vector3.forward);
		poss.Add (waste + Vector3.back);

		wastePos = waste;

		var pos = poss[new UniformIntDistribution(poss.Count-1).Next(new StaticRandomGenerator<DefaultRandomGenerator>())];

		agent.altAction = AEntity.ActionType.CLEAN;
		agent.altDestination = pos;
		dest = pos;
		agent.Updatepath ();

		return 0;
		*/
	}

	public override void start ()
	{
		
	}

	public override void update()
	{
		agent.rigidbody.velocity = Vector3.zero;
	}

	IEnumerator HelpCoroutine()
	{
		yield return new WaitForSeconds (G.Sys.constants.HelpTime);
		Debug.Log ("help traveler");
	}

	public override void end ()
	{
		agent.altAction = AEntity.ActionType.NONE;
	}

	public override bool canBeStopped()
	{
		return false;
	}
}

}