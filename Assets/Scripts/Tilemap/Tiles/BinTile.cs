﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class BinTile : ATile
{
	private float wasteValue = 0;
	public float waste {
		get { return wasteValue; }
		set{ wasteValue = value; }
	}

	public bool canContain(float _waste)
	{
		return wasteValue + _waste <= 1;
	}

	public bool isEmpty()
	{
		return wasteValue <= 0.1f;
	}

	protected override void Awake()
    {
		type = TileID.BIN;

		G.Sys.tilemap.addTile (transform.position, this, Tilemap.BIN_PRIORITY);

		G.Sys.tilemap.addSpecialTile (type, transform.position);
    }

	protected override void OnDestroy()
	{
		G.Sys.tilemap.delTile (transform.position, this);

		G.Sys.tilemap.delSpecialTile (type, transform.position);
	}
}