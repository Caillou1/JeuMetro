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

			var dir = Orienter.orientationToDir(Orienter.angleToOrientation(transform.rotation.eulerAngles.y));

			var up = G.Sys.tilemap.tileInfosOf(this, transform.position + new Vector3(-dir.x, 1, -dir.y));
			var down = G.Sys.tilemap.tileInfosOf (this, transform.position + new Vector3 (dir.x, 0, dir.y));
			up.canBeConnected = side == EscalatorSide.DOWN;
			up.preventConnexions = side != EscalatorSide.DOWN;
			down.canBeConnected = side == EscalatorSide.UP;
			down.preventConnexions = side != EscalatorSide.UP;

            Connect();
        }
        get { return _side; }
    }

    public override void Connect()
    {
		if (this == null)
			Debug.Log ("null");
		
		List<ATile> connexions = new List<ATile>();

        var dir = Orienter.orientationToDir(Orienter.angleToOrientation(transform.rotation.eulerAngles.y));
        if (side == EscalatorSide.UP)
			Add(G.Sys.tilemap.connectableTile(transform.position + new Vector3(-dir.x * 2, 1, -dir.y * 2)), connexions);
		else Add(G.Sys.tilemap.connectableTile(transform.position + new Vector3(dir.x * 2, 0, dir.y * 2)), connexions);

		applyConnexions (connexions);
    }

    void Awake()
    {
		type = TileID.ESCALATOR;

        var dir = Orienter.orientationToDir(Orienter.angleToOrientation(transform.rotation.eulerAngles.y));

		G.Sys.tilemap.addTile (transform.position, this, false, true, Tilemap.ESCALATOR_PRIORITY);
		G.Sys.tilemap.addTile (transform.position + new Vector3 (-dir.x, 0, -dir.y), this, false, true, Tilemap.ESCALATOR_PRIORITY);
		G.Sys.tilemap.addTile (transform.position + new Vector3 (-dir.x, 1, -dir.y), this, side == EscalatorSide.DOWN, side != EscalatorSide.DOWN, Tilemap.ESCALATOR_PRIORITY);
		G.Sys.tilemap.addTile (transform.position + new Vector3 (dir.x, 0, dir.y), this, side == EscalatorSide.UP, side != EscalatorSide.UP, Tilemap.ESCALATOR_PRIORITY);
		G.Sys.tilemap.addTile (transform.position + new Vector3 (-2 * dir.x, 1, -2 * dir.y), this, side == EscalatorSide.DOWN, side != EscalatorSide.DOWN, Tilemap.LOW_PRIORITY);
		G.Sys.tilemap.addTile (transform.position + new Vector3 (2 * dir.x, 0, 2 * dir.y), this, side == EscalatorSide.DOWN, side != EscalatorSide.DOWN, Tilemap.LOW_PRIORITY);

		foreach(var t in G.Sys.tilemap.at(transform.position))
			t.Connect();
		foreach (var t in G.Sys.tilemap.at(transform.position + new Vector3 (-dir.x, 0, -dir.y)))
			t.Connect ();
		foreach (var t in G.Sys.tilemap.at(transform.position + new Vector3 (-dir.x, 1, -dir.y)))
			t.Connect ();
		foreach (var t in G.Sys.tilemap.at(transform.position + new Vector3 (dir.x, 0, dir.y)))
			t.Connect ();
		foreach (var t in G.Sys.tilemap.at(transform.position + new Vector3 (-2 * dir.x, 1, -2 * dir.y)))
			t.Connect ();
		foreach (var t in G.Sys.tilemap.at(transform.position + new Vector3 (2 * dir.x, 0, 2 * dir.y)))
			t.Connect ();
    }

	void OnDestroy()
	{
		var dir = Orienter.orientationToDir(Orienter.angleToOrientation(transform.rotation.eulerAngles.y));

		G.Sys.tilemap.delTile (transform.position, this);
		G.Sys.tilemap.delTile (transform.position + new Vector3 (-dir.x, 0, -dir.y), this);
		G.Sys.tilemap.delTile (transform.position + new Vector3 (-dir.x, 1, -dir.y), this);
		G.Sys.tilemap.delTile (transform.position + new Vector3 (dir.x, 0, dir.y), this);
		G.Sys.tilemap.delTile (transform.position + new Vector3 (-2 * dir.x, 1, -2 * dir.y), this);
		G.Sys.tilemap.delTile (transform.position + new Vector3 (2 * dir.x, 0, 2 * dir.y), this);

		foreach(var t in G.Sys.tilemap.at(transform.position))
			t.Connect();
		foreach (var t in G.Sys.tilemap.at(transform.position + new Vector3 (-dir.x, 0, -dir.y)))
			t.Connect ();
		foreach (var t in G.Sys.tilemap.at(transform.position + new Vector3 (-dir.x, 1, -dir.y)))
			t.Connect ();
		foreach (var t in G.Sys.tilemap.at(transform.position + new Vector3 (dir.x, 0, dir.y)))
			t.Connect ();
		foreach (var t in G.Sys.tilemap.at(transform.position + new Vector3 (-2 * dir.x, 1, -2 * dir.y)))
			t.Connect ();
		foreach (var t in G.Sys.tilemap.at(transform.position + new Vector3 (2 * dir.x, 0, 2 * dir.y)))
			t.Connect ();
		
		foreach (var t in connectedTiles)
			t.targetOf.Remove (this);
		foreach (var t in targetOf.ToList())
			t.Connect ();
	}
}