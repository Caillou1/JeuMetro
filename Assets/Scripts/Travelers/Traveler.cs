using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NRand;

public class Traveler : MonoBehaviour 
{
	public TravelerStats Stats;

	List<AState> states = new List<AState>();
	int stateIndex = 0;

	[HideInInspector]
	public new Rigidbody rigidbody;

	public float avoidPower = 1.0f;
	[HideInInspector]
	public Vector3 avoidDir;
	[HideInInspector]
	public Path path;
	[HideInInspector]
	public Vector3 destination;

	void Awake()
	{
		G.Sys.registerTraveler (this);
		rigidbody = GetComponent<Rigidbody> ();

		states.Add (new MoveState (this));
		states.Add (new StairsState (this));
		states.Add (new EscalatorState (this));

		configurePathfinder ();
	}

	void Start()
	{
		InitialiseTarget ();
		OnPathFinished ();
		states [stateIndex].start ();
	}

	void Update() 
	{
		checkNextState ();

		states [stateIndex].update ();

		if (path.finished ())
			OnPathFinished ();

		DestroyOnExit ();
	}

	void OnDestroy()
	{
		G.Sys.removeTraveler (this);
	}

	public void BackToMoveState()
	{
		SetNextState (StateType.MOVE);
	}

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

	void OnTriggerStay(Collider c)
	{
		if (c.gameObject.tag != "Entity")
			return;
		var dir = c.transform.position - transform.position;
		float dist = dir.magnitude;

		if (Vector3.Dot (transform.forward, dir) < 0)
			return;

		float isleft = Mathf.Atan2 (dir.z, dir.x) - Mathf.Atan2 (transform.forward.z, transform.forward.x) > 0 ? -1 : 1;
		var direction = new Vector3 (-dir.z, 0, dir.x).normalized;

		avoidDir += direction * (2 - dist) / 2 * avoidPower;
	}

	void configurePathfinder()
	{
		var dico = new Dictionary<TileID, float> ();
		dico.Add (TileID.ESCALATOR, statValueToPathWeight(Stats.EscalatorAttraction));
		dico.Add (TileID.ELEVATOR, statValueToPathWeight(Stats.ElevatorAttraction));
		dico.Add (TileID.STAIRS, statValueToPathWeight(Stats.StairsAttraction));
		dico.Add (TileID.PODOTACTILE, statValueToPathWeight (Stats.TouchComprehension));

		switch (Stats.Type) {
		case TravelerType.WITH_BAG:
			dico [TileID.ELEVATOR] *= 2;
			dico [TileID.STAIRS] /= 10;
			break;
		case TravelerType.WHEELCHAIR:
			dico [TileID.ELEVATOR] *= 10;
			dico [TileID.ESCALATOR] /= 10;
			dico [TileID.STAIRS] /= 10;
			break;
		case TravelerType.BLIND:
			dico [TileID.ELEVATOR] *= 5;
			dico [TileID.ESCALATOR] *= 5;
			dico [TileID.STAIRS] /= 10;
			dico [TileID.PODOTACTILE] *= 10;
			break;
		default:
			break;
		}
		path = new Path (dico);
	}

	float statValueToPathWeight(float value)
	{
		return Mathf.Pow (2, -value / 30);
	}

	void OnPathFinished()
	{
		path.create (transform.position, destination);
	}

	void InitialiseTarget()
	{
		var tiles = G.Sys.tilemap.getSpecialTiles (TileID.OUT);
		if (tiles.Count == 0)
			return;

		destination = tiles[new UniformIntDistribution (tiles.Count-1).Next(new StaticRandomGenerator<DefaultRandomGenerator> ())];
	}

	void DestroyOnExit()
	{
		if (G.Sys.tilemap.haveSpecialTileAt (TileID.OUT, transform.position))
			Destroy (gameObject);
	}
}
