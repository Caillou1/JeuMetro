using System;
using UnityEngine;
using UnityEngine.AI;

public class MetroTile : ATile
{
	NavMeshModifier modifier;
	bool _enabled = false;

	protected override void Awake()
	{
		type = TileID.METRO;

		G.Sys.tilemap.addTile (transform.position, this, Tilemap.EXITS_PRIORITY);

		modifier = GetComponent<NavMeshModifier> ();
	}

	protected override void OnDestroy()
	{
		G.Sys.tilemap.delTile (transform.position, this);
	}

	public bool tileEnabled 
	{ 
		get { return _enabled; } 
		set { 
			if (_enabled == value)
				return;
			_enabled = value; 
			modifier.enabled = !_enabled;
			Event<BakeNavMeshEvent>.Broadcast (new BakeNavMeshEvent ());
		}
	}
}

