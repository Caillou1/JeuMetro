using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour {
	public GameObject Traveler;
	public Exit[] Exits;

	private int ActualWave;
	private Transform tf;

	void Awake () {
		G.Sys.waveManager = this;
	}

	void Start() {
		tf = transform;

		for (int i = 0; i < Exits.Length; i++) {
			Exits [i].ActualWave = 0;
			StartCoroutine (SpawnNextWave (i));
		}
	}

	IEnumerator SpawnNextWave(int ExitNumber) {
		var obj = Instantiate (Traveler, Exits [ExitNumber].exit.position, Quaternion.identity, tf);
		var stats = obj.GetComponent<TravelerStats> ();
		if (stats != null)
			stats.SetStats (Exits [ExitNumber].Waves [Exits [ExitNumber].ActualWave].Stats);
		else
			Debug.Log ("TravelerStats Component Missing");

		if (Exits [ExitNumber].ActualWave < Exits [ExitNumber].Waves.Length) {
			yield return new WaitForSeconds (Exits [ExitNumber].Waves [Exits [ExitNumber].ActualWave].TimeBeforeNext);
			Exits [ExitNumber].ActualWave++;
			StartCoroutine (SpawnNextWave (ExitNumber));
		}
	}
}

[System.Serializable]
public class Exit {
	public Transform exit;
	public Wave[] Waves;
	public int ActualWave;
}

[System.Serializable]
public class Wave {
	public TravelerStats Stats;
	public float TimeBeforeNext;
}
