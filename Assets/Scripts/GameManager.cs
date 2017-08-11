using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NRand;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;

public class GameManager : MonoBehaviour
{
	private Transform tf;
	public List<GameObject> entities = new List<GameObject>();
	public float minDelay;
	public float maxDelay;
	public GameObject wastePrefab;
	public GameObject emptyWall;
	public bool enableTravelers;
	public bool SetMaxTravelers;
	[ShowIf("SetMaxTravelers")]
	public int MaxTravelers;

	public int StartingMoney = 0;

	private float TotalTime = 0;
	private int TravelersThatLeftStation = 0;

	private int money;
    private int earnedMoney = 0;

	private SubscriberList subscriberList = new SubscriberList();

	private List<Traveler> faintingTravelers;

    [HideInInspector]
    public bool FireAlert = false;

    void Awake()
    {
        G.Sys.gameManager = this;
		StartCoroutine (updateTravelersDatasCoroutine ());
		faintingTravelers = new List<Traveler> ();
		subscriberList.Add(new Event<FaintEvent>.Subscriber(OnTravelerFaint));
		subscriberList.Add(new Event<NavMeshBakedEvent>.Subscriber(OnNavMeshBaked));
		subscriberList.Subscribe ();
    }

	void Start ()
    {
		Event<BakeNavMeshEvent>.Broadcast (new BakeNavMeshEvent ());
		tf = transform;
		AddMoney(StartingMoney);
		if (enableTravelers)
		    StartCoroutine (spawnCoroutine ());
		G.Sys.tilemap.UpdateGlobalBounds ();
		//PathCalculator = tf.Find ("PathCalculator");
		InstantiateColliders ();
		StartCoroutine (checkFaintingTravelers ());
	}

	public void OnNavMeshBaked(NavMeshBakedEvent e) {
		G.Sys.tilemap.CreateElevatorsConnections ();
	}

	void OnDestroy() {
		subscriberList.Unsubscribe ();
	}

	void OnTravelerFaint(FaintEvent e) {
		var a = G.Sys.GetNearestFreeAgent (e.traveler.transform.position);
		if (a != null) {
			a.GoHelpTravelerAction (e.traveler);
		} else {
			if (!faintingTravelers.Contains (e.traveler)) {
				faintingTravelers.Add (e.traveler);
			}
		}
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

	IEnumerator checkFaintingTravelers() {
		while (true) {
			if (faintingTravelers.Count > 0) {
				var a = G.Sys.GetNearestFreeAgent (faintingTravelers[0].transform.position);
				if (a != null) {
					a.GoHelpTravelerAction (faintingTravelers[0]);
					faintingTravelers.RemoveAt (0);
				}
			}

			yield return new WaitForSeconds (.5f);
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

	public void AddMoney(int m) {
		money += m;
        if(m > 0)
            earnedMoney += m;
		G.Sys.menuManager.SetMoneyNumber (money);
		G.Sys.menuManager.ShowMoneyAdded (m);
	}

	public int GetMoney() {
		return money;
	}

    public int GetEarnedMoney()
    {
        return earnedMoney - StartingMoney;
    }

	public bool HaveEnoughMoney(int m) {
		return money >= m;
	}

    public void AddTime(float t, bool addTraveler = true) {
		if (addTraveler) 
            TravelersThatLeftStation++;
		TotalTime += t;
	}

	public float GetAverageTime() {
		return TotalTime / TravelersThatLeftStation;
	}

	IEnumerator spawnCoroutine()
	{
		yield return new WaitForSeconds (5f);
		StaticRandomGenerator<DefaultRandomGenerator> gen = new StaticRandomGenerator<DefaultRandomGenerator> ();
		UniformIntDistribution dType = new UniformIntDistribution (entities.Count-1);
		UniformFloatDistribution dDelta = new UniformFloatDistribution (minDelay, maxDelay);

		while (true) {
			yield return new WaitForSeconds (dDelta.Next (gen));
			if (!SetMaxTravelers || (SetMaxTravelers && G.Sys.travelerCount() < MaxTravelers)) {
					var doors = G.Sys.tilemap.getSpecialTiles (TileID.IN);
					var door = doors [new UniformIntDistribution (doors.Count - 1).Next (gen)];
					var e = Instantiate (entities [dType.Next (gen)], door, new Quaternion ());
					var dir = new Vector3[]{ Vector3.left, Vector3.right, Vector3.forward, Vector3.back };
					var validDir = new List<Vector3> ();
					foreach (var d in dir) {
						var tiles = G.Sys.tilemap.at (door + d);
						if (tiles.Count == 0 && tiles [0].type == TileID.GROUND)
							validDir.Add (door + d);
					}
			
					if (validDir.Count != 0)
						e.transform.rotation = Quaternion.LookRotation (validDir [new UniformIntDistribution (validDir.Count - 1).Next (gen)] - e.transform.position, Vector3.up);
			}
		}
	}
}
