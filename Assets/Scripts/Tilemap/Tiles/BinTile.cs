﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class BinTile : ATile
{
    GameObject emptyBin;
    GameObject fullBin;

	bool targetted = false;
	private float wasteValue = 0f;
	public float waste {
		get { return wasteValue; }
		set
        { 
            wasteValue = value;
            updateVisual();
        }
	}

    void updateVisual()
    {
        emptyBin.SetActive(!isFull());
        fullBin.SetActive(isFull());
    }

	public bool Targetted {
		get {
			return targetted;
		}
		set {
			targetted = value;
		}
	}

	public float freeSpace()
	{
		return 1 - wasteValue;
	}

	public bool canContain(float _waste)
	{
		return wasteValue + _waste <= 1;
	}

	public bool isEmpty()
	{
		return wasteValue <= 0.1f;
	}

	public bool isFull()
	{
		return wasteValue > 0.99f;
	}

	protected override void Awake()
    {
		type = TileID.BIN;

		G.Sys.tilemap.addTile (transform.position, this, Tilemap.BIN_PRIORITY);

		G.Sys.tilemap.addSpecialTile (type, transform.position);

        emptyBin = transform.Find("Vide").gameObject;
        fullBin = transform.Find("Pleine").gameObject;
        updateVisual();
    }

	protected override void OnDestroy()
	{
		G.Sys.tilemap.delTile (transform.position, this);

		G.Sys.tilemap.delSpecialTile (type, transform.position);
	}
}