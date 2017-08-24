using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;
using DG.Tweening;

public class ControleLineTile : ATile 
{
    public bool canPassWithoutTicket;

    List<NavMeshLink> backLinks = new List<NavMeshLink>();
    SubscriberList subscriberList = new SubscriberList();

    Transform doorLeft;
    Transform doorRight;

    List<Tween> currentTweens = new List<Tween>();

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

        var portique = transform.Find("portique 1");
        doorLeft = portique.Find("porte_01");
        doorRight = portique.Find("porte_02");

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

    public void Open(float startTime)
    {
        const float rotateAngle = 85.0f;
        const float inRotationTime = 0.3f;
        const float outRotationTime = 1.5f;
        const float waitTime = 0.1f;

        foreach (var t in currentTweens)
            if (t != null)
                t.Kill();
        currentTweens.Clear();

        currentTweens.Add(DOVirtual.DelayedCall(startTime, () =>
        {
            currentTweens.Add(doorLeft.DOLocalRotate(new Vector3(0, rotateAngle, 0), inRotationTime).OnComplete(() =>
            {
                currentTweens.Add(DOVirtual.DelayedCall(waitTime, () =>
                { currentTweens.Add(doorLeft.DOLocalRotate(Vector3.zero, outRotationTime).SetEase(Ease.OutElastic)); }));
            }));

            currentTweens.Add(doorRight.DOLocalRotate(new Vector3(0, -rotateAngle, 0), inRotationTime).OnComplete(() =>
            {
                currentTweens.Add(DOVirtual.DelayedCall(waitTime, () =>
                { currentTweens.Add(doorRight.DOLocalRotate(Vector3.zero, outRotationTime).SetEase(Ease.OutElastic)); }));
            }));
        }));
    }
}
