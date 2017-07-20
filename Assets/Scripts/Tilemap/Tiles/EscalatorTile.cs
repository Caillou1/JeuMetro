using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AI;

public enum EscalatorSide
{ UP, DOWN }

public class EscalatorTile : ATile
{
    [SerializeField]
    private EscalatorSide _side = EscalatorSide.UP;

	private Animator anim;

	private List<NavMeshLink> linksUp = new List<NavMeshLink> ();
	private List<NavMeshLink> linksDown = new List<NavMeshLink> ();

    public EscalatorSide side
    {
        set
        {
            _side = value;

			foreach (var l in linksUp)
				l.enabled = _side == EscalatorSide.UP;
			foreach (var l in linksDown)
				l.enabled = _side == EscalatorSide.DOWN;

			var dir = Orienter.orientationToDir3(Orienter.angleToOrientation(transform.rotation.eulerAngles.y));

			var up = G.Sys.tilemap.tileInfosOf(this, transform.position + 2 * Vector3.up - dir);
			var down = G.Sys.tilemap.tileInfosOf (this, transform.position + 2 * dir);
			up.canBeConnected = side == EscalatorSide.DOWN;
			up.preventConnexions = side != EscalatorSide.DOWN;
			down.canBeConnected = side == EscalatorSide.UP;
			down.preventConnexions = side != EscalatorSide.UP;

			Event<ObjectPlacedEvent>.Broadcast (new ObjectPlacedEvent (new Vector3i[]{
				new Vector3i(transform.position),
				new Vector3i(transform.position + dir),
				new Vector3i(transform.position + 2 * dir),
				new Vector3i(transform.position + Vector3.up),
				new Vector3i(transform.position + Vector3.up + dir),
				new Vector3i(transform.position + 2 * Vector3.up), 
				new Vector3i(transform.position + 2 * Vector3.up - dir),
				new Vector3i(transform.position + 2 * Vector3.up + dir)

			}.ToList()));

            Connect();
        }
        get { return _side; }
    }

	public void SetSide(EscalatorSide s) {
		_side = s;
		anim.SetBool ("Reverse", _side == EscalatorSide.DOWN);
	}
		
    public override void Connect()
    {
		List<Pair<ATile, Vector3i>> connexions = new List<Pair<ATile, Vector3i>>();

		var dir = Orienter.orientationToDir3(Orienter.angleToOrientation(transform.rotation.eulerAngles.y));
		var perpendicularDir = new Vector3 (-dir.z, dir.y, dir.x);
		if (side == EscalatorSide.DOWN) {
			Add (transform.position + 3 * dir, connexions);
			Add (transform.position + 2 * dir + perpendicularDir, connexions);
			Add (transform.position + 2 * dir - perpendicularDir, connexions);
			var tiles = G.Sys.tilemap.at (transform.position + 2 * dir);
			foreach (var t in tiles) {
				if (t.type != TileID.ESCALATOR && t.type != TileID.STAIRS)
					continue;
				if (t == this)
					continue;
				connexions.Add (new Pair<ATile, Vector3i> (t, new Vector3i (transform.position + 2 * dir)));
			}
		} else {
			Add (transform.position + 2 * Vector3.up - 2 * dir, connexions);
			Add (transform.position + 2 * Vector3.up - dir + perpendicularDir, connexions);
			Add (transform.position + 2 * Vector3.up - dir - perpendicularDir, connexions);
			var tiles = G.Sys.tilemap.at (transform.position + 2 * Vector3.up - dir);
			foreach (var t in tiles) {
				if (t.type != TileID.ESCALATOR && t.type != TileID.STAIRS)
					continue;
				if (t == this)
					continue;
				connexions.Add (new Pair<ATile, Vector3i> (t, new Vector3i (transform.position + 2 * Vector3.up - dir)));
			}
		}

		applyConnexions (connexions);
    }

	protected override void Awake()
	{
		foreach (var l in GetComponents<NavMeshLink>()) {
			if (l.bidirectional)
				continue;
			if (l.endPoint.y < l.startPoint.y)
				linksDown.Add (l);
			else
				linksUp.Add (l);
		}

		anim = transform.Find ("mesh").GetComponent<Animator> ();

		type = TileID.ESCALATOR;

		var dir = Orienter.orientationToDir3(Orienter.angleToOrientation(transform.rotation.eulerAngles.y));

		G.Sys.tilemap.addTile (transform.position, this, false, true, Tilemap.STAIRS_PRIORITY);
		G.Sys.tilemap.addTile (transform.position + dir, this, false, true, Tilemap.STAIRS_PRIORITY);
		G.Sys.tilemap.addTile (transform.position + 2 * dir, this, side == EscalatorSide.UP, false, Tilemap.STAIRS_PRIORITY);
		G.Sys.tilemap.addTile (transform.position + Vector3.up, this, false, true, Tilemap.STAIRS_PRIORITY);
		G.Sys.tilemap.addTile (transform.position + Vector3.up + dir, this, false, true, Tilemap.STAIRS_PRIORITY);
		G.Sys.tilemap.addTile (transform.position + 2 * Vector3.up, this, false, true, Tilemap.STAIRS_PRIORITY);
		G.Sys.tilemap.addTile (transform.position + 2 * Vector3.up - dir, this, side == EscalatorSide.DOWN, false, Tilemap.STAIRS_PRIORITY);
		G.Sys.tilemap.addTile (transform.position + 2 * Vector3.up + dir, this, false, true, Tilemap.STAIRS_PRIORITY);


		//Supprime les bandes podotactiles en dessous des escalators
		var podo = G.Sys.tilemap.tilesOfTypeAt (transform.position, TileID.PODOTACTILE);
		podo = podo.Concat (G.Sys.tilemap.tilesOfTypeAt (transform.position + dir, TileID.PODOTACTILE)).ToList ();

		foreach (var p in podo) {
			Destroy (p.gameObject);
		}

		foreach(var t in G.Sys.tilemap.at(transform.position))
			t.Connect();
		foreach(var t in G.Sys.tilemap.at(transform.position + dir))
			t.Connect();
		foreach(var t in G.Sys.tilemap.at(transform.position + 2 * dir))
			t.Connect();
		foreach(var t in G.Sys.tilemap.at(transform.position + Vector3.up))
			t.Connect();
		foreach(var t in G.Sys.tilemap.at(transform.position + Vector3.up + dir))
			t.Connect();
		foreach(var t in G.Sys.tilemap.at(transform.position + 2 * Vector3.up))
			t.Connect();
		foreach(var t in G.Sys.tilemap.at(transform.position + 2 * Vector3.up - dir))
			t.Connect();

		side = _side;
    }

	void Start() {
		anim.SetBool ("Reverse", _side == EscalatorSide.DOWN);

		Event<BakeNavMeshEvent>.Broadcast (new BakeNavMeshEvent ());
	}

	protected override void OnDestroy()
	{
		GetComponent<DragAndDropEscalator> ().OnDestroy ();
		var dir = Orienter.orientationToDir3(Orienter.angleToOrientation(transform.rotation.eulerAngles.y));

		G.Sys.tilemap.delTile (transform.position, this);
		G.Sys.tilemap.delTile (transform.position + dir, this);
		G.Sys.tilemap.delTile (transform.position + 2 * dir, this);
		G.Sys.tilemap.delTile (transform.position + Vector3.up, this);
		G.Sys.tilemap.delTile (transform.position + Vector3.up + dir, this);
		G.Sys.tilemap.delTile (transform.position + 2 * Vector3.up, this);
		G.Sys.tilemap.delTile (transform.position + 2 * Vector3.up - dir, this);
		G.Sys.tilemap.delTile (transform.position + 2 * Vector3.up + dir, this);

		foreach(var t in G.Sys.tilemap.at(transform.position))
			t.Connect();
		foreach(var t in G.Sys.tilemap.at(transform.position + dir))
			t.Connect();
		foreach(var t in G.Sys.tilemap.at(transform.position + 2 * dir))
			t.Connect();
		foreach(var t in G.Sys.tilemap.at(transform.position + Vector3.up))
			t.Connect();
		foreach(var t in G.Sys.tilemap.at(transform.position + Vector3.up + dir))
			t.Connect();
		foreach(var t in G.Sys.tilemap.at(transform.position + 2 * Vector3.up))
			t.Connect();
		foreach(var t in G.Sys.tilemap.at(transform.position + 2 * Vector3.up - dir))
			t.Connect();

		foreach (var t in connectedTiles)
			t.First.targetOf.Remove (this);
		foreach (var t in targetOf.ToList())
			t.Connect ();
	}
}