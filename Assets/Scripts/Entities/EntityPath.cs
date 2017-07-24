﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using NRand;
using DG.Tweening;

public class EntityPath
{
	const float dLostness = 0.1f;

	NavMeshAgent _agent;
	MonoBehaviour _behavior;

	float _lostness = 0;
	float _oldLostness = 0;

	Vector3 _destination;

	List<Vector3> _points = new List<Vector3> ();
	List<AAction> _actions = new List<AAction> ();

	bool pathPending = false;
	AAction currentAction = null;
	bool isOnOffMeshLink = false;

	bool finished = true;

	public EntityPath (NavMeshAgent a, float lostness = 0)
	{
		_agent = a;
		_behavior = _agent.GetComponent<MonoBehaviour> ();
		if (_behavior == null)
			_agent.autoTraverseOffMeshLink = true;
		else
			_agent.autoTraverseOffMeshLink = false;
		_lostness = lostness;
		_oldLostness = lostness;

		//updatePath ();
	}

	public Vector3 destnation {
		get{ return _destination; }
		set {
			_destination = value;
			updatePath ();
		}
	}

	public float lostness {
		get{ return _lostness; }
		set{
			_lostness = value;
			if (Mathf.Abs (_oldLostness - _lostness) > dLostness) {
				_oldLostness = value;
				updatePath ();
			}
		}
	}

	void addAction(AAction action)
	{
		_actions.Add (action);
		if (finished)
			updateAgentPath ();
	}

	void updatePath()
	{
		_agent.SetDestination (_destination);
		if (_agent.pathPending)
			pathPending = true;
		else
			onPathCreated ();
	}

	void onPathCreated()
	{
		pathPending = false;
		_points.Clear ();

		if (!_agent.hasPath)
			return;

		float distBetweenPoints = G.Sys.constants.travelerLostVariance / lostness;

		if (lostness != 0) {
			var path = _agent.path;
			float dist = 0;
			float lastCornerDist = 0;
			float lastPoint = 0;
			Vector3 lastpoint = _agent.transform.position;
			var corners = path.corners.ToList ();
			corners.Add (_destination);

			var gen = new StaticRandomGenerator<DefaultRandomGenerator> ();
			var d = new UniformVector2CircleDistribution (G.Sys.constants.travelerLostVariance);

			foreach (var pos in corners) {
				lastCornerDist = (lastpoint - pos).magnitude;
				while (dist + lastCornerDist - lastPoint >= distBetweenPoints) {
					float pointOnCorner = lastPoint - dist + distBetweenPoints;
					float distOnCorner = pointOnCorner / lastCornerDist;
					var p = lastpoint * distOnCorner + pos * (1 - distOnCorner);
					for (int i = 0; i < 10; i++) {
						var dPos = d.Next (gen);
						var target = p + new Vector3(dPos.x, 0, dPos.y);
						var tiles = G.Sys.tilemap.at (target);
						if (tiles.Count == 1 && tiles [0].type == TileID.GROUND) {
							_points.Add (target);
							break;
						}
					}
					lastPoint = dist + pointOnCorner;
				}
				dist += lastCornerDist;
				lastpoint = pos;
			}
		}

		_points.Add (_destination);

		updateAgentPath ();
	}

	void updateAgentPath()
	{
		if (_points.Count == 0 && _actions.Count == 0) {
			finished = true;
			currentAction = null;
			return;
		}

		if (_points.Count == 0) {
			currentAction = _actions [0];
			_actions.RemoveAt (0);
			_agent.SetDestination (currentAction.pos);
			return;
		}

		if (_actions.Count > 0) {
			float minDist = float.MaxValue;
			for (int i = 0; i < _points.Count; i++) {
				float dist = (_actions [0].pos - _points [i]).sqrMagnitude;
				if (dist < minDist)
					minDist = dist;
			}
			if ((_agent.transform.position - _actions [0].pos).sqrMagnitude < minDist) {
				currentAction = _actions [0];
				_agent.SetDestination (currentAction.pos);
				_actions.RemoveAt (0);
			} else {
				currentAction = null;
				_agent.SetDestination (_points [0]);
				_points.RemoveAt (0);
			}

		} else {
			currentAction = null;
			_agent.SetDestination (_points [0]);
			_points.RemoveAt (0);
		}
	}

	public void Update()
	{
		
		debugPath ();

		if (_agent.isOnOffMeshLink && !isOnOffMeshLink)
			onLink ();

		if (pathPending && !_agent.pathPending)
			onPathCreated();

		if (_agent.remainingDistance < 0.1f) {
			if (currentAction == null)
				updateAgentPath ();
			else if (!currentAction.Exec ())
				updateAgentPath ();
		}
	}

	void debugPath()
	{
		for (int i = 0; i <= _points.Count; i++) {
			var oldPoint = i == 0 ? _agent.transform.position : i == 1 ? _agent.destination : _points [i - 2];
			var point = i == 0 ? _agent.destination : _points [i - 1];
			Debug.DrawLine(oldPoint, point, Color.red);
		}
	}

	public bool Finished()
	{
		return finished;
	}

	public bool IsOnAction()
	{
		return _agent.pathStatus == NavMeshPathStatus.PathComplete && currentAction != null;
	}

	public bool haveAction(ActionType type)
	{
		if (currentAction != null && currentAction.type == type)
			return true;
		foreach (var a in _actions)
			if (a.type == type)
				return true;
		return false;
	}

	void onLink()
	{
		if (_behavior == null) {
			_agent.autoTraverseOffMeshLink = true;
			return;
		}
		isOnOffMeshLink = true;

		_behavior.StartCoroutine (onLinkCoroutine ());

		/*var data = _agent.currentOffMeshLinkData;
		_agent.CompleteOffMeshLink ();
		var dir = (data.endPos - data.startPos).normalized;
		_agent.transform.position += dir * Time.deltaTime * _agent.speed;
		Debug.Log (data.startPos + " " + data.endPos);*/
		
		/*if (agent.isOnOffMeshLink)

			OffMeshLinkData data = agent.currentOffMeshLinkData;
		Vector3 startPos = agent.transform.position;
		Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;
		float normalizedTime = 0.0f;
		while (normalizedTime < 1.0f)
		{
			float yOffset = m_Curve.Evaluate(normalizedTime);
			agent.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * Vector3.up;
			normalizedTime += Time.deltaTime / duration;
			yield return null;
		}*/
	}

	IEnumerator onLinkCoroutine()
	{
		var data = _agent.currentOffMeshLinkData;
		_agent.updatePosition = false;
		_agent.CompleteOffMeshLink ();

		Vector3 startPos = _agent.transform.position;
		Vector3 startLinkPos = data.startPos + Vector3.up * _agent.baseOffset;
		Vector3 endPos = data.endPos + Vector3.up * _agent.baseOffset;
		Vector3 dir = startLinkPos - startPos;
		float norm = dir.magnitude;

		float time = 0;

		while (time < 1) {
			_agent.transform.position = startPos + dir * time;
			float dist = Time.deltaTime * _agent.speed;
			time += dist / norm;
			yield return new WaitForFixedUpdate ();
		}

		time = 0;
		dir = endPos - startLinkPos;
		norm = dir.magnitude;
		_agent.transform.position = startLinkPos;

		while (time < 1) {
			_agent.transform.position = startLinkPos + dir * time;
			float dist = Time.deltaTime * _agent.speed;
			time += dist / norm;
			yield return new WaitForFixedUpdate ();
		}
		
		_agent.updatePosition = true;
		_agent.transform.position = endPos;
		isOnOffMeshLink = false;
	}
}