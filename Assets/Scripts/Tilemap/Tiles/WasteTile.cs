﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WasteTile : ATile
{
	private bool _canBeCleaned;

	public bool CanBeCleaned {
		get {
			return _canBeCleaned;
		}
		set {
			_canBeCleaned = value;
		}
	}

	protected override void Awake()
    {
		_canBeCleaned = true;

		type = TileID.WASTE;

		G.Sys.tilemap.addTile (transform.position, this, Tilemap.LOW_PRIORITY);

		G.Sys.tilemap.addSpecialTile (type, transform.position);
    }

	protected override void OnDestroy()
	{
		G.Sys.tilemap.delTile (transform.position, this);
		G.Sys.tilemap.delSpecialTile (type, transform.position);
	}
}