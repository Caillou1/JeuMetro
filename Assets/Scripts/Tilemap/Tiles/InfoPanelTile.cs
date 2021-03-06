﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class InfoPanelTile : ATile
{
	protected override void Awake()
	{
		type = TileID.INFOPANEL;

		G.Sys.tilemap.addTile (transform.position, this, Tilemap.INFOPANEL_PRIORITY);

		G.Sys.tilemap.addSpecialTile (type, transform.position);
    }

	protected override void OnDestroy()
	{
		G.Sys.tilemap.delTile (transform.position, this);

		G.Sys.tilemap.delSpecialTile (type, transform.position);
	}
}