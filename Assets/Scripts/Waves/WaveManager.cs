using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour {
	public Entrance[] Entrees;

	private int[] WaveIndex;
	private Transform tf;

	void Awake() {
		tf = transform;
		G.Sys.waveManager = this;

		WaveIndex = new int[Entrees.Length];
	}

	void Start() {
		for (int i = 0; i < Entrees.Length; i++) {
			StartCoroutine (SpawnNext (i));
		}
	}

	IEnumerator SpawnNext(int i) {
		var e = Entrees [i];

		if (WaveIndex [i] < e.Vagues.Length) {
			var v = e.Vagues [WaveIndex [i]];
			if (e.Type == EntranceType.Door) {
				yield return new WaitForSeconds (v.TimeBeforeWave);
				InstantiateWave (v.Vague, e.Entree.position);
			} else {
				yield return new WaitForSeconds (v.TimeBeforeWave - G.Sys.constants.MetroComeTime);
				Debug.Log ("Faire venir metro");
				yield return new WaitForSeconds (G.Sys.constants.MetroComeTime);
				InstantiateWave (v.Vague, e.Entree.position);
				yield return new WaitForSeconds (G.Sys.constants.MetroWaitTime);
				Debug.Log ("Faire repartir le métro");
			}

			WaveIndex [i]++;
			StartCoroutine (SpawnNext (i));
		}
	}

	void InstantiateWave(GameObject wave, Vector3 pos) {
		Instantiate (wave, pos, Quaternion.identity, tf);
	}
}

public enum EntranceType {
	Door,
	Metro,
}

[System.Serializable]
public class Entrance {
	public Transform Entree;
	public EntranceType Type;
	public Wave[] Vagues;
}

[System.Serializable]
public class Wave {
	public GameObject Vague;
	public float TimeBeforeWave;
}