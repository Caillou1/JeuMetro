﻿using System;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public abstract class AEntity : MonoBehaviour 
{
	protected NavMeshAgent agent;
	public EntityPath path;
	protected Vector3 target = Vector3.zero;

	void Awake()
	{
		agent = GetComponent<NavMeshAgent> ();
		path = new EntityPath (agent);

		StartCoroutine (checkCoroutine ());

		OnAwake ();
	}

	protected virtual void OnAwake()
	{

	}

	void Start()
	{
		OnStart ();
	}

	public virtual void EnableAgent() {
		agent.enabled = true;
	}

	public void SetIsStopped(bool b) {
		agent.isStopped = b;
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

	IEnumerator checkCoroutine()
	{
		while (true) {
			if (!path.IsOnAction () && path.CanStartAction())
				Check ();

			yield return new WaitForSeconds (0.2f);
		}
	}

	protected virtual void Check()
	{

	}
}
