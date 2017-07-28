using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

using System.Threading;


public class NavMeshManager : MonoBehaviour 
{
	List<NavMeshSurface> navmesh;
	SubscriberList subscriberList = new SubscriberList();

	bool needToBuild = false;

	void Awake () 
	{
		navmesh = GetComponents<NavMeshSurface> ().ToList();

		subscriberList.Add (new Event<BakeNavMeshEvent>.Subscriber (onBakeEvent));
		subscriberList.Subscribe ();

		StartCoroutine (buildNavmeshCoroutine ());
	}

	void onDestroy()
	{
		subscriberList.Unsubscribe ();
	}

	void onBakeEvent(BakeNavMeshEvent e)
	{
		needToBuild = true;
	}

	IEnumerator buildNavmeshCoroutine()
	{
		while (true) {
			if (!needToBuild) {
				yield return new WaitForEndOfFrame ();
			} else {
				needToBuild = false;
				foreach (var n in navmesh) {
					n.BuildNavMesh ();
					yield return new WaitForEndOfFrame ();
				}
			}
		}
	}
}
