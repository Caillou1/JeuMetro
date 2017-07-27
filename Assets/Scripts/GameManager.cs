using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NRand;

public class GameManager : MonoBehaviour
{
	private Transform tf;
	public List<GameObject> entities = new List<GameObject>();
	public float minDelay;
	public float maxDelay;
	public GameObject wastePrefab;
	public GameObject emptyWall;

	public int StartingMoney = 0;

	private int money;
	private float time;
	private float startTime;
	private float totalTime;

    void Awake()
    {
        G.Sys.gameManager = this;
		StartCoroutine (updateTravelersDatasCoroutine ());
    }

	void Start ()
    {
		Event<BakeNavMeshEvent>.Broadcast (new BakeNavMeshEvent ());
		tf = transform;
		AddMoney (StartingMoney);
		StartCoroutine (spawnCoroutine ());
		G.Sys.tilemap.UpdateGlobalBounds ();
		InstantiateColliders ();
	}

	IEnumerator updateTravelersDatasCoroutine()
	{
		const float time = 0.1f;
		while (true) {
			for(int i = 0 ; i < G.Sys.travelerCount() ; i++)
				G.Sys.traveler(i).updateDatas (time);
			yield return new WaitForSeconds (time);
		}
	}

	void InstantiateColliders() {
		var bounds = G.Sys.tilemap.GlobalBounds ();

		for (int x = (int)bounds.min.x; x <= (int)bounds.max.x; x++) {
			for (int y = (int)bounds.min.y; y <= (int)bounds.max.y; y++) {
				for (int z = (int)bounds.min.z; z <= (int)bounds.max.z; z++) {
					var pos = new Vector3 (x, y, z);

					if (G.Sys.tilemap.HasGroundAt(pos)) {
						if (G.Sys.tilemap.at (pos + Vector3.forward).Count == 0) {
							InstantiateEmptyWallAt (pos + Vector3.forward);
						}
						if (G.Sys.tilemap.at (pos + Vector3.back).Count == 0) {
							InstantiateEmptyWallAt (pos + Vector3.back);
						}
						if (G.Sys.tilemap.at (pos + Vector3.left).Count == 0) {
							InstantiateEmptyWallAt (pos + Vector3.left);
						}
						if (G.Sys.tilemap.at (pos + Vector3.right).Count == 0) {
							InstantiateEmptyWallAt (pos + Vector3.right);
						}
					}
				}
			}
		}
	}

	public void InstantiateEmptyWallAt(Vector3 pos) {
		Instantiate (emptyWall, pos, Quaternion.identity, tf);
	}

	public void StartTimer(float t) {
		startTime = Time.time;
		time = t;
		totalTime = t;
	}
	
	void Update ()
    {
		time = totalTime - Time.time + startTime;
		G.Sys.menuManager.SetPieTime (1f-(time/totalTime), (int)time);
	}

	public void AddMoney(int m) {
		money += m;
		G.Sys.menuManager.SetMoneyNumber (money);
		G.Sys.menuManager.ShowMoneyAdded (m);
	}

	public int GetMoney() {
		return money;
	}

	public bool HaveEnoughMoney(int m) {
		return money >= m;
	}

	IEnumerator spawnCoroutine()
	{
		yield return new WaitForSeconds (5f);
		StaticRandomGenerator<DefaultRandomGenerator> gen = new StaticRandomGenerator<DefaultRandomGenerator> ();
		UniformIntDistribution dType = new UniformIntDistribution (entities.Count-1);
		UniformFloatDistribution dDelta = new UniformFloatDistribution (minDelay, maxDelay);

		while (true) {
			yield return new WaitForSeconds (dDelta.Next (gen));
			var doors = G.Sys.tilemap.getSpecialTiles (TileID.IN);
			var door = doors [new UniformIntDistribution (doors.Count-1).Next (gen)];
			var e = Instantiate (entities [dType.Next (gen)], door, new Quaternion ());
			var dir = new Vector3[]{ Vector3.left, Vector3.right, Vector3.forward, Vector3.back };
			var validDir = new List<Vector3> ();
			foreach(var d in dir)
			{
				var tiles = G.Sys.tilemap.at (door + d);
				if (tiles.Count == 0 && tiles [0].type == TileID.GROUND)
					validDir.Add (door + d);
			}
		
			if(validDir.Count != 0)
				e.transform.rotation = Quaternion.LookRotation (validDir[new UniformIntDistribution(validDir.Count-1).Next(gen)] - e.transform.position, Vector3.up);
		}
	}
}
