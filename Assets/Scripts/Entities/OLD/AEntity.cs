using System;
using UnityEngine;
using System.Collections.Generic;

namespace V1
{
public abstract class AEntity  : MonoBehaviour
{
	public enum ActionType
	{
		NONE,
		SIT,
		THROW_WASTE,
		FOOD,
		TICKET,
		LOST,
		INFOS,
		BUY_FOOD,
		BUY_TICKET,
		NO_TICKET,
		CLEAN,
	}

	protected List<AState> states = new List<AState>();
	protected int stateIndex = 0;

	[HideInInspector]
	public new Rigidbody rigidbody;

	[HideInInspector]
	public EntityPath path;
	[HideInInspector]
	public Vector3 destination;
	[HideInInspector]
	public Vector3 altDestination;
	[HideInInspector]
	public ActionType altAction;
	[HideInInspector]
	public bool altWait = true;

	protected SubscriberList subscriberList = new SubscriberList();

	protected float timeFromLastFinishedPath = 10.0f;

	void Awake()
	{
		rigidbody = GetComponent<Rigidbody> ();
		
		configurePathfinder ();

		subscriberList.Add(new Event<ObjectPlacedEvent>.Subscriber(onPlaceObject));
		subscriberList.Subscribe ();

		OnEntityAwake ();
	}

	protected virtual void OnEntityAwake(){}

	void Start()
	{
		InitialiseTarget ();
		OnPathFinished ();
		states [stateIndex].start ();

		OnEntityStart ();
	}

	protected virtual void OnEntityStart(){}

	void Update() 
	{
		timeFromLastFinishedPath += Time.deltaTime;

		checkNextState ();

		if (path.finished ()) {
			OnPathFinished ();
		}

		states [stateIndex].update ();

		OnEntityUpdate ();
	}

	protected virtual void OnEntityUpdate () {}

	void OnDestroy()
	{
		subscriberList.Unsubscribe ();
		OnEntityDestroy ();
	}

	protected virtual void OnEntityDestroy(){}

	public abstract void BackToMoveState ();

	public void SetNextState(StateType type)
	{
		int nextIndex = -1;
		for (int i = 0; i < states.Count; i++) {
			if (states [i].type == type)
				nextIndex = i;
		}

		if (nextIndex == -1)
			return;

		enableNextState (nextIndex);
	}

	void enableNextState(int index)
	{
		if (index == stateIndex)
			return;
		states [stateIndex].end ();
		stateIndex = index;
		states [stateIndex].start ();
	}

	void checkNextState()
	{
		if (!states [stateIndex].canBeStopped())
			return;
		int bestWeight = int.MinValue;
		int bestIndex = -1;
		for (int i = 0; i < states.Count; i++) {
			int v = states [i].check ();
			if (v > bestWeight) {
				bestIndex = i;
				bestWeight = v;
			}
		}

		if (bestIndex == -1)
			return;

		enableNextState (bestIndex);
	}

	/*void OnTriggerStay(Collider c)
	{
		if (c.gameObject.tag != "Entity")
			return;
		var dir = c.transform.position - transform.position;
		float dist = dir.magnitude;

		if (Vector3.Dot (transform.forward, dir) < 0)
			return;

		//float isleft = Mathf.Atan2 (dir.z, dir.x) - Mathf.Atan2 (transform.forward.z, transform.forward.x) > 0 ? -1 : 1;
		var direction = new Vector3 (-dir.z, 0, dir.x).normalized;

		avoidDir += direction * (2 - dist) / 2 * avoidPower;

		OnEntityTriggerStay (c);
	}*/

	protected virtual void OnEntityTriggerStay(Collider c){}

	protected abstract void configurePathfinder ();

	void OnPathFinished()
	{
		altAction = ActionType.NONE;
		altWait = true;
		if (!path.isPathValid() && timeFromLastFinishedPath > 1.0f) {
				timeFromLastFinishedPath = 0.0f;
				Updatepath ();
			if (!path.isPathValid ())
				ForcePath ();
		}
		else
			Updatepath ();
		OnEntityPathFinished ();
	}

	protected virtual void OnEntityPathFinished() {}

	protected abstract void InitialiseTarget();

	public abstract void Updatepath ();

	void onPlaceObject(ObjectPlacedEvent e)
	{
		bool onPath = false;
		foreach (var p in e.points) {
			if (path.isOnPath (p)) {
				onPath = true;
				break;
			}
		}
		if (onPath)
			Updatepath ();
	}

	protected abstract void ForcePath();
}
}
