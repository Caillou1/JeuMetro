using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class GroundTile : ATile
{
    public override void Connect()
    {
        connectedTiles.Clear();

        Vector3i pos = new Vector3i(transform.position);
        Add(G.Sys.tilemap.At(pos + new Vector3i(0, 0, 1)));
        Add(G.Sys.tilemap.At(pos + new Vector3i(0, 0, -1)));
        Add(G.Sys.tilemap.At(pos + new Vector3i(1, 0, 0)));
        Add(G.Sys.tilemap.At(pos + new Vector3i(-1, 0, 0)));
    }

    void Awake()
    {
        G.Sys.tilemap.SetAt(transform.position, this);
    }
}
