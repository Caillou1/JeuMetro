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

    private GameObject wall1;
    private GameObject wall2;
    private GameObject ground;

    SubscriberList subscriberlist = new SubscriberList();

    public EscalatorSide side
    {
        set
        {
			_side = value;
			if(G.Sys.gameManager != null && !G.Sys.gameManager.FireAlert)
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

        wall1 = transform.Find("W1").gameObject;
        wall2 = transform.Find("W2").gameObject;
        ground = transform.Find("Quad").gameObject;
        ground.SetActive(false);

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

        subscriberlist.Add(new Event<StartFireAlertEvent>.Subscriber(onfireAlert));
        subscriberlist.Subscribe();
    }

	void Start() {
        if (!G.Sys.gameManager.FireAlert)
            anim.SetBool("Reverse", _side == EscalatorSide.DOWN);
        else onfireAlert(new StartFireAlertEvent());
	}

	protected override void OnDestroy()
	{
		var dir = Orienter.orientationToDir3(Orienter.angleToOrientation(transform.rotation.eulerAngles.y));

		Vector3i v1 = new Vector3i (transform.position + dir * 2);
		Vector3i v2 = new Vector3i (transform.position - dir + Vector3.up * 2);

		var list = G.Sys.tilemap.tilesOfTypeAt (v1, TileID.PODOTACTILE).Concat (G.Sys.tilemap.tilesOfTypeAt (v2, TileID.PODOTACTILE)).ToList ();

		foreach (var podotactileTile in list)
			(podotactileTile as PodotactileTile).Connect (false);

		G.Sys.tilemap.delTile (transform.position, this);
		G.Sys.tilemap.delTile (transform.position + dir, this);
		G.Sys.tilemap.delTile (transform.position + Vector3.up, this);
		G.Sys.tilemap.delTile (transform.position + Vector3.up + dir, this);
		G.Sys.tilemap.delTile (transform.position + 2 * Vector3.up, this);
		G.Sys.tilemap.delTile (transform.position + 2 * Vector3.up + dir, this);

        subscriberlist.Unsubscribe();
	}

	public bool IsOnEscalatorPath(Vector3i pos) {
		var dir = Orienter.orientationToDir3(Orienter.angleToOrientation(transform.rotation.eulerAngles.y));

		Vector3i v1 = new Vector3i (transform.position + dir * 2);
		Vector3i v2 = new Vector3i (transform.position - dir + Vector3.up * 2);

		return (pos.equal (v1) || pos.equal (v2));
	}

    void onfireAlert(StartFireAlertEvent e)
    {
        Stop();
        ground.SetActive(true);
        wall1.SetActive(false);
        wall2.SetActive(false);
        foreach (var l in linksUp)
            l.enabled = false;
        foreach (var l in linksDown)
            l.enabled = false;
    }
}