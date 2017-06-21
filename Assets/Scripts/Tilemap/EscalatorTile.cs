using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum EscalatorSide
{ UP, DOWN }

public class EscalatorTile : ATile
{
    [SerializeField]
    private EscalatorSide _side = EscalatorSide.UP;

    public EscalatorSide side
    {
        set
        {
            _side = value;
            Connect();
        }
        get { return _side; }
    }

    public override void Connect()
    {
        connectedTiles.Clear();
        var dir = Orienter.orientationToDir(Orienter.angleToOrientation(transform.rotation.eulerAngles.y));
        if (side == EscalatorSide.UP)
            Add(G.Sys.tilemap.At(transform.position + new Vector3(dir.x * 2, 1, dir.y * 2)));
        else Add(G.Sys.tilemap.At(transform.position - new Vector3(dir.x * 2, 0, dir.y * 2)));

    }

    void Awake()
    {
        var dir = Orienter.orientationToDir(Orienter.angleToOrientation(transform.rotation.eulerAngles.y));
        G.Sys.tilemap.SetAt(transform.position, this);
        G.Sys.tilemap.SetAt(transform.position + new Vector3(dir.x, 0, dir.y), this);
        G.Sys.tilemap.SetAt(transform.position + new Vector3(dir.x, 1, dir.y), this);
        G.Sys.tilemap.SetAt(transform.position - new Vector3(dir.x, 0, dir.y), this);
    }
}