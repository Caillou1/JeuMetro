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
			anim.SetBool ("Reverse", _side == EscalatorSide.DOWN);

			foreach (var l in linksUp)
				l.enabled = _side == EscalatorSide.UP;
			foreach (var l in linksDown)
				l.enabled = _side == EscalatorSide.DOWN;

        }
        get { return _side; }
    }

	public void Stop() {
		anim.SetBool ("Stop", true);
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

		G.Sys.tilemap.addTile (transform.position, this, Tilemap.STAIRS_PRIORITY);
		G.Sys.tilemap.addTile (transform.position + dir, this, Tilemap.STAIRS_PRIORITY);
		G.Sys.tilemap.addTile (transform.position + Vector3.up, this, Tilemap.STAIRS_PRIORITY);
		G.Sys.tilemap.addTile (transform.position + Vector3.up + dir, this, Tilemap.STAIRS_PRIORITY);
		G.Sys.tilemap.addTile (transform.position + 2 * Vector3.up, this, Tilemap.STAIRS_PRIORITY);
		G.Sys.tilemap.addTile (transform.position + 2 * Vector3.up + dir, this, Tilemap.STAIRS_PRIORITY);

		side = _side;
    }

	void Start() {
		anim.SetBool ("Reverse", _side == EscalatorSide.DOWN);

		Invoke ("Stop", 5f);
	}

	protected override void OnDestroy()
	{
		GetComponent<DragAndDropEscalator> ().OnDestroy ();
		var dir = Orienter.orientationToDir3(Orienter.angleToOrientation(transform.rotation.eulerAngles.y));

		G.Sys.tilemap.delTile (transform.position, this);
		G.Sys.tilemap.delTile (transform.position + dir, this);
		G.Sys.tilemap.delTile (transform.position + Vector3.up, this);
		G.Sys.tilemap.delTile (transform.position + Vector3.up + dir, this);
		G.Sys.tilemap.delTile (transform.position + 2 * Vector3.up, this);
		G.Sys.tilemap.delTile (transform.position + 2 * Vector3.up + dir, this);
	}
}