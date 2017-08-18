using System;
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

    public NavMeshAgent getNavMeshAgent()
    {
        return agent;
    }

	public static Vector3 getAlertTarget(Vector3 pos)
	{
        var tilesIn = G.Sys.tilemap.getSpecialTiles(TileID.IN);
        var tilesOut = G.Sys.tilemap.getSpecialTiles(TileID.OUT);
        return Vector3.zero;
	}
}
