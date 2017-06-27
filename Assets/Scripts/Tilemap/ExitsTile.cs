using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class ExitsTile : ATile
{
	[Tooltip("Must be IN OUT or METRO")]
	public TileID id = TileID.IN;

	void Awake()
	{
		if (id != TileID.IN && id != TileID.OUT && id != TileID.METRO)
			throw new ArgumentOutOfRangeException ("The tile must be IN, OUT or METRO !");
		
		type = id;

		G.Sys.tilemap.addTile (transform.position, this, true, false, Tilemap.EXITS_PRIORITY);

		foreach (var t in G.Sys.tilemap.at(transform.position))
			t.Connect ();

		G.Sys.tilemap.addSpecialTile (type, transform.position);
	}

	public override void Connect ()
	{
		Vector3i pos = new Vector3i(transform.position);

		List<Pair<ATile, Vector3i>> list = new List<Pair<ATile, Vector3i>>();
		Add(pos + new Vector3i(0, 0, 1), list);
		Add(pos + new Vector3i(0, 0, -1), list);
		Add(pos + new Vector3i(1, 0, 0), list);
		Add(pos + new Vector3i(-1, 0, 0), list);

		applyConnexions (list);
	}

	void OnDestroy()
	{
		G.Sys.tilemap.delTile (transform.position, this);
		foreach (var t in G.Sys.tilemap.at(transform.position))
			t.Connect ();
		foreach (var t in connectedTiles)
			t.First.targetOf.Remove (this);
		foreach (var t in targetOf.ToList())
			t.Connect ();

		G.Sys.tilemap.delSpecialTile (type, transform.position);
	}
}

