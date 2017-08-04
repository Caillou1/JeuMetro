using System;
using UnityEngine;

public class GetMetroAction : AEntityAction<Traveler>
{
	public GetMetroAction (Traveler t, Vector3 pos) : base(t, ActionType.GO_TO_METRO, pos)
	{
		
	}

	protected override bool Update ()
	{
		GameObject.Destroy (entity.gameObject);
		return true;
	}
}

