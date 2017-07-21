using System;
using UnityEngine;
using UnityEngine.AI;

public abstract class AEntity : MonoBehaviour 
{
	protected NavMeshAgent agent;
	protected EntityPath path;
	protected Vector3 target = Vector3.zero;

	void Awake()
	{
		agent = GetComponent<NavMeshAgent> ();
		path = new EntityPath (agent);

		OnAwake ();
	}

	protected virtual void OnAwake()
	{

	}

	void Start()
	{
		OnStart ();
	}

	protected virtual void OnStart()
	{

	}

	void Update()
	{
		if (path.Finished())
			OnPathFinished ();

		path.Update ();

		OnUpdate ();
	}

	protected virtual void OnUpdate()
	{

	}

	protected virtual void OnPathFinished()
	{

	}

	protected virtual void Check()
	{

	}
}
