using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ATile : MonoBehaviour
{
    public ATile()
    {
        connectedTiles = new List<ATile>();
    }

    public abstract void Connect();

    protected void Add(ATile tile)
    {
        if (tile != null)
            connectedTiles.Add(tile);
    }

    public List<ATile> connectedTiles { get; protected set; }

    public bool empty { get; set; }
}
