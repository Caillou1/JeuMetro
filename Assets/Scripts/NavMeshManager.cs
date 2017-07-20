using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;


public class NavMeshManager : MonoBehaviour 
{
	List<NavMeshSurface> navmesh;
	SubscriberList subscriberList = new SubscriberList();

	void Awake () 
	{
		navmesh = GetComponents<NavMeshSurface> ().ToList();

		subscriberList.Add (new Event<BakeNavMeshEvent>.Subscriber (onBakeEvent));
		subscriberList.Subscribe ();
	}

	void onDestroy()
	{
		subscriberList.Unsubscribe ();
	}

	void onBakeEvent(BakeNavMeshEvent e)
	{
		foreach (var n in navmesh)
			n.BuildNavMesh ();
		Debug.Log ("Poop");
	}
}
