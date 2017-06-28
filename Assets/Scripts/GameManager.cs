using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NRand;

public class GameManager : MonoBehaviour
{
	public List<GameObject> entities = new List<GameObject>();
	public float minDelay;
	public float maxDelay;

    void Awake()
    {
        G.Sys.gameManager = this;
    }

	void Start ()
    {
		if (entities.Count > 0)
			StartCoroutine (spawnCoroutine()); 
	}
	
	void Update ()
    {
		
	}

	IEnumerator spawnCoroutine()
	{
		StaticRandomGenerator<DefaultRandomGenerator> gen = new StaticRandomGenerator<DefaultRandomGenerator> ();
		UniformIntDistribution dType = new UniformIntDistribution (entities.Count-1);
		UniformFloatDistribution dDelta = new UniformFloatDistribution (minDelay, maxDelay);

		while (true) {
			yield return new WaitForSeconds (dDelta.Next (gen));
			var doors = G.Sys.tilemap.getSpecialTiles (TileID.IN);
			var door = doors [new UniformIntDistribution (doors.Count-1).Next (gen)];
			var e = Instantiate (entities [dType.Next (gen)], door, new Quaternion ());
			var d = G.Sys.tilemap.tilesOfTypeAt (door, TileID.IN) [0];
			e.transform.rotation = Quaternion.LookRotation (d.connectedTiles [0].Second.toVector3 () - e.transform.position, Vector3.up);
		}
	}
}
