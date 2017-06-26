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
		UniformIntDistribution dType = new UniformIntDistribution (entities.Count);
		UniformFloatDistribution dDelta = new UniformFloatDistribution (minDelay, maxDelay);

		while (true) {
			yield return new WaitForSeconds (dDelta.Next (gen));
			var doors = G.Sys.tilemap.getSpecialTiles (TileID.IN);
			var door = doors [new UniformIntDistribution (doors.Count).Next (gen)];
			Instantiate (entities [dType.Next (gen)], door, new Quaternion ());
		}
	}
}
