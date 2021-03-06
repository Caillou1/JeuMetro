﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TicketDistribTile : ATile
{
	public int price = 1;
	[HideInInspector]
	public int queue = 0;

	protected override void Awake()
	{
		type = TileID.TICKETDISTRIBUTEUR;

		G.Sys.tilemap.addTile (transform.position, this, Tilemap.DISTRIBUTEUR_PRIORITY);

		G.Sys.tilemap.addSpecialTile (type, transform.position);
    }

	protected override void OnDestroy()
	{
		G.Sys.tilemap.delTile (transform.position, this);

		G.Sys.tilemap.delSpecialTile (type, transform.position);
	}
}