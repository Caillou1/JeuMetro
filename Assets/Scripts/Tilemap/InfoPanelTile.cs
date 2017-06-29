using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class InfoPanelTile : ATile
{
	void Awake()
	{
		type = TileID.INFOPANEL;

		G.Sys.tilemap.addTile (transform.position, this, false, true, Tilemap.INFOPANEL_PRIORITY);

		foreach (var t in G.Sys.tilemap.at(transform.position))
			t.Connect ();
    }

	public override void Register ()
	{
		G.Sys.tilemap.addTile (transform.position, this, false, true, Tilemap.INFOPANEL_PRIORITY);

		foreach (var t in G.Sys.tilemap.at(transform.position))
			t.Connect ();
	}

	public override void Unregister ()
	{
		G.Sys.tilemap.delTile (transform.position, this);

		foreach (var t in G.Sys.tilemap.at(transform.position))
			t.Connect ();
	}

	public override void Connect (){}

	void OnDestroy()
	{
		G.Sys.tilemap.delTile (transform.position, this);

		foreach (var t in G.Sys.tilemap.at(transform.position))
			t.Connect ();
	}
}