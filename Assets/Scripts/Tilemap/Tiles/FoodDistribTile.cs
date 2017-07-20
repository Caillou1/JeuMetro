using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class FoodDistribTile : ATile
{
	protected override void Awake()
	{
		type = TileID.FOODDISTRIBUTEUR;

		G.Sys.tilemap.addTile (transform.position, this, false, true, Tilemap.DISTRIBUTEUR_PRIORITY);

		foreach (var t in G.Sys.tilemap.at(transform.position))
			t.Connect ();

		G.Sys.tilemap.addSpecialTile (type, transform.position);
    }

	protected override void OnDestroy()
	{
		G.Sys.tilemap.delTile (transform.position, this);

		foreach (var t in G.Sys.tilemap.at(transform.position))
			t.Connect ();

		G.Sys.tilemap.delSpecialTile (type, transform.position);
	}
}