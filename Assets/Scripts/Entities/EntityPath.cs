using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using NRand;
using DG.Tweening;

public class EntityPath
{
	const float dLostness = 0.15f;

	NavMeshAgent _agent;
	MonoBehaviour _behavior;

	float _lostness = 0;
	float _oldLostness = 0;

	bool _canPassControl = true;
	bool _lastPathBrokePassControl = false;

	Vector3 _destination;

	List<Vector3> _points = new List<Vector3> ();
	List<AAction> _actions = new List<AAction> ();

	AAction currentAction = null;
	bool onAction = false;
	bool isOnOffMeshLink = false;

	bool finished = true;

    int framsFromLastLink = 0;
    const int framsFromLastLinkTrigger = 4;

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
            _destination = new Vector3i(value).toVector3();
			updatePath ();
		}
	}

	public void clearPath()
	{
		_points.Clear ();
		_agent.destination = _agent.transform.position;
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

	public bool canPassControl {
		set {
			_canPassControl = value;
			updatePath ();
		}
		get{ return _canPassControl; }
	}

	public bool isLastPathNeedPassControl()
	{
		return _lastPathBrokePassControl;
	}

	public void addAction(AAction action)
	{
		if (action.priority < 0)
			_actions.Add (action);
		else {
			if (action.priority > currentAction.priority && !onAction) {
				_actions.Insert (0, currentAction);
				currentAction = action;
				_agent.SetDestination (currentAction.pos);
			} else {
				bool inserted = false;
				for (int i = 0; i < _actions.Count; i++) {
					if (_actions [i].priority < action.priority) {
						_actions.Insert (i, action);
						inserted = true;
						break;
					}
				}
				if (!inserted)
					_actions.Add (action);
			}
		}

		if (finished)
			updateAgentPath ();
	}

	public void abortNextAction()
	{
		if (_actions.Count > 0)
			_actions.RemoveAt (0);
	}

	public void abortAllActions()
	{
		_actions.Clear ();
	}

	public void removeActionOfType(ActionType type)
	{
		_actions.RemoveAll (a => a.type == type);
	}

	void updatePath()
	{
		finished = false;
        NavMeshPath aPath = new NavMeshPath();
        _agent.CalculatePath(_destination, aPath);
        _agent.SetPath(aPath);
        onPathCreated();
	}

    public void abortAllAndActiveActionElse(ActionType[] actions)
    {
        if (currentAction != null && !actions.Contains(currentAction.type))
            currentAction = null;
        List<AAction> newActions = new List<AAction>();
        foreach(var a in _actions)
            if (actions.Contains(a.type))
                newActions.Add(a);
        _actions = newActions;
    }

	void onPathCreated()
	{
		_lastPathBrokePassControl = false;
		_points.Clear ();

		if (!_agent.hasPath)
			return;

		if (lostness < 0.05f && _canPassControl) {
			_points.Add (_destination);
			updateAgentPath ();
			return;
		}

		float distBetweenPoints = G.Sys.constants.travelerLostVariance / lostness;		
		var path = _agent.path;
		float dist = 0;
		float lastCornerDist = 0;
		float lastPoint = 0;
		Vector3 lastpoint = _agent.transform.position;
		var corners = path.corners.ToList ();
		if (!canPassControl) {
			for (int i = 0; i < corners.Count - 1; i++) {
				if (haveControleLineBetween (corners [i], corners [i + 1])) {
					if (i == 0)
						_destination = _agent.transform.position;
					else _destination = (corners [i - 1] - corners [i]).normalized + corners [i];
					corners.RemoveRange (i, corners.Count - i);
					_lastPathBrokePassControl = true;
					break;
				}
			}
		}
			
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

		_points.Add (_destination);

		updateAgentPath ();
	}

	bool haveControleLineBetween(Vector3 pos1, Vector3 pos2)
	{
        var tile = G.Sys.tilemap.GetTileOfTypeAt((pos1 + pos2) / 2, TileID.CONTROLELINE) as ControleLineTile;;
        return tile != null && !tile.canPassWithoutTicket; 
	}

	void updateAgentPath()
	{
        if (!_agent.isOnNavMesh)
            return;
        
		NavMeshPath aPath = new NavMeshPath();

		if (currentAction != null) {
	        _agent.CalculatePath(currentAction.pos, aPath);
	        _agent.SetPath(aPath);
			return;
		}
		
		if (_points.Count == 0 && _actions.Count == 0) {
			finished = true;
			currentAction = null;
			return;
		}
		finished = false;

		if (_points.Count == 0) {
			currentAction = _actions [0];
			_actions.RemoveAt (0);
			_agent.CalculatePath(currentAction.pos, aPath);
			_agent.SetPath(aPath);
			return;
		}

		currentAction = null;
		_agent.CalculatePath(_points[0], aPath);
		_points.RemoveAt (0);
	}

	public void Update()
	{
        framsFromLastLink++;
        if (isOnOffMeshLink)
            framsFromLastLink = 0;

		debugPath ();

		if (_agent.isOnOffMeshLink && !isOnOffMeshLink)
			onLink ();

		if (currentAction == null && _actions.Count > 0)
			checkAction ();
		
		if (new Vector3i(_agent.transform.position).Equals(new Vector3i(_agent.destination))) {
			if (currentAction == null)
				updateAgentPath ();
			else if (framsFromLastLink >= framsFromLastLinkTrigger){
				_agent.updatePosition = false;
				_agent.updateRotation = false;
				_agent.updateUpAxis = false;
				onAction = true;
			}
            else
            {
				NavMeshPath aPath = new NavMeshPath();
				_agent.CalculatePath(currentAction.pos, aPath);
				_agent.SetPath(aPath);
            }
		}

		if (onAction) {
			_agent.nextPosition = _agent.transform.position;
			if (currentAction.Exec ()) {
				onAction = false;
				currentAction = null;
				_agent.updatePosition = true;
				_agent.updateRotation = true;
				_agent.updateUpAxis = true;
				updateAgentPath ();
			}
		}
	}

	void checkAction()
	{
        if (!_agent.isOnNavMesh || framsFromLastLink < framsFromLastLinkTrigger)
			return;
		if ((_agent.transform.position - _agent.destination).sqrMagnitude > (_agent.transform.position - _actions [0].pos).sqrMagnitude) {
			_points.Insert (0, _agent.destination);
			currentAction = _actions [0];
			_actions.RemoveAt (0);
			_agent.SetDestination (currentAction.pos);
		}
	}

	void debugPath()
	{
		/*for (int i = 0; i <= _points.Count; i++) {
			var oldPoint = i == 0 ? _agent.transform.position : i == 1 ? _agent.destination : _points [i - 2];
			var point = i == 0 ? _agent.destination : _points [i - 1];
			Debug.DrawLine(oldPoint, point, Color.red);
		}*/
		Vector3 pos = _agent.transform.position;
		foreach (var p in _agent.path.corners)
		{
			Debug.DrawLine (pos, p, Color.red);
			pos = p;
		}
		if (currentAction != null)
			Debug.DrawLine (_agent.transform.position, currentAction.pos, Color.magenta);
		Debug.DrawLine (_agent.transform.position, destnation, Color.cyan);
	}

	public bool Finished()
	{
		return finished && !isOnOffMeshLink;
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
			_agent.nextPosition = _agent.transform.position;
			float dist = Time.deltaTime * _agent.speed;
			time += dist / norm;
			if (G.Sys.tilemap.GetTileOfTypeAt (_agent.transform.position, TileID.CONTROLELINE) != null) {
				_canPassControl = true;
				_lastPathBrokePassControl = false;
			}
			
			yield return new WaitForFixedUpdate ();
		}

		_agent.transform.position = endPos;
		_agent.updatePosition = true;
		_agent.enabled = false;
		yield return new WaitForEndOfFrame ();
		_agent.enabled = true;
		isOnOffMeshLink = false;
	}

	public bool CanStartAction()
	{
		return !isOnOffMeshLink && G.Sys.tilemap.IsEmptyGround (_agent.transform.position) && !onAction;
	}

    public bool HaveTileOnPath(TileID id, float minChunkLenght = 0.5f)
    {
        if (finished)
            return false;
        
        float minChunk = minChunkLenght * minChunkLenght;
        var corners = _agent.path.corners.ToList();
        corners.Insert(0, _agent.transform.position);
        foreach (var c in corners)
            if (G.Sys.tilemap.GetTileOfTypeAt(c, id)!=null)
                return true;
        List<Pair<Vector3, Vector3>> segments = new List<Pair<Vector3, Vector3>>();
        for (int i = 0; i < corners.Count()-1; i++)
        {
            segments.Add(new Pair<Vector3, Vector3>(corners[i], corners[i+1]));
        }

        int it = 0;
        while(segments.Count() > 0)
        {
            for (int i = 0; i < segments.Count(); i++)
            {
                var segment = segments[i];
                int chunkCount = 1 << it;
                var chunk = (segment.Second - segment.First) / (chunkCount << 1);
                for (int j = 0; j < chunkCount; j++)
                {
                    var pos = segment.First + chunk * (j + 0.5f);
					if (G.Sys.tilemap.GetTileOfTypeAt(pos, id) != null)
						return true;
                }

                if(chunk.sqrMagnitude <= minChunk)
                {
                    segments.RemoveAt(i);
                    i--;
                    if (segments.Count() == 0)
                        break;
                }
            }
            it++;
        }

        return false;
    }

    public bool canGoTo(Vector3 pos)
    {
        NavMeshPath path = new NavMeshPath();
        _agent.CalculatePath(pos, path);
        return path.status == NavMeshPathStatus.PathComplete;
    }
}
