using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;

public class ControleLineTile : ATile 
{
    public bool canPassWithoutTicket;

    List<NavMeshLink> backLinks = new List<NavMeshLink>();
    SubscriberList subscriberList = new SubscriberList();

	protected override void Awake ()
	{
		type = TileID.CONTROLELINE;

		G.Sys.tilemap.addTile (transform.position, this, Tilemap.CONTROLE_LINE_PRIORITY);
        Vector3 forward = transform.right;
        var rot = transform.rotation;
        foreach(var l in transform.Find("GameObject").GetComponents<NavMeshLink>())
        {
            if (Vector3.Dot(rot * (l.endPoint - l.startPoint), forward) < 0)
                backLinks.Add(l);
        }
        foreach(var l in backLinks)
        {
            l.enabled = false;
        }

        subscriberList.Add(new Event<StartFireAlertEvent>.Subscriber(onFireAlarmStart));
        subscriberList.Subscribe();
	}

	protected override void OnDestroy ()
	{
		G.Sys.tilemap.delTile (transform.position, this);
        subscriberList.Unsubscribe();
	}

    void onFireAlarmStart(StartFireAlertEvent e)
    {
		foreach (var l in backLinks)
		{
			l.enabled = true;
		}
    }
}
