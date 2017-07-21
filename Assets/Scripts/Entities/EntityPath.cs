using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using NRand;

public class EntityPath
{
	const float dLostness = 0.1f;

	NavMeshAgent _agent;

	float _lostness;
	float _oldLostness;

	Vector3 _destination;

	List<Vector3> _points = new List<Vector3> ();
	List<AAction> _actions = new List<AAction> ();

	bool pathPending = false;
	AAction currentAction = null;

	bool finished = true;

	public EntityPath (NavMeshAgent a, float lostness = 0)
	{
		_agent = a;
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
			var d = new UniformVector3SphereDistribution (G.Sys.constants.travelerLostVariance);

			foreach (var pos in corners) {
				lastCornerDist = (lastpoint - pos).magnitude;
				while (dist + lastCornerDist - lastPoint >= distBetweenPoints) {
					float pointOnCorner = lastPoint - dist + distBetweenPoints;
					float distOnCorner = pointOnCorner / lastCornerDist;
					var p = lastpoint * distOnCorner + pos * (1 - distOnCorner);
					for (int i = 0; i < 10; i++) {
						var target = p + d.Next (gen);
						if (G.Sys.tilemap.HasGroundAt (target)) {
							_points.Add (target);
							break;
						}
					}
					lastPoint = dist + pointOnCorner;
				}
				dist += lastCornerDist;
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
		if (pathPending && !_agent.pathPending)
			onPathCreated();

		if (_agent.pathStatus == NavMeshPathStatus.PathComplete) {
			if (currentAction == null)
				updateAgentPath ();
			else if (!currentAction.Exec ())
				updateAgentPath ();
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
}
