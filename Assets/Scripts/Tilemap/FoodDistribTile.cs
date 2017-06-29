using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class FoodDistribTile : ATile
{
	void Awake()
	{
		type = TileID.FOODDISTRIBUTEUR;

		G.Sys.tilemap.addTile (transform.position, this, false, true, Tilemap.DISTRIBUTEUR_PRIORITY);

		foreach (var t in G.Sys.tilemap.at(transform.position))
			t.Connect ();

		G.Sys.tilemap.addSpecialTile (type, transform.position);
    }

	public override void Register ()
	{
		G.Sys.tilemap.addTile (transform.position, this, false, true, Tilemap.DISTRIBUTEUR_PRIORITY);

		foreach (var t in G.Sys.tilemap.at(transform.position))
			t.Connect ();

		G.Sys.tilemap.addSpecialTile (type, transform.position);
	}

	public override void Unregister ()
	{
		G.Sys.tilemap.delTile (transform.position, this);

		foreach (var t in G.Sys.tilemap.at(transform.position))
			t.Connect ();

		G.Sys.tilemap.delSpecialTile (type, transform.position);
	}

	void OnDestroy()
	{
		G.Sys.tilemap.delTile (transform.position, this);

		foreach (var t in G.Sys.tilemap.at(transform.position))
			t.Connect ();

		G.Sys.tilemap.delSpecialTile (type, transform.position);
	}
}