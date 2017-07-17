using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour {
	public Wave[] Vagues;

	private int CurrentWaveIndex;
	private Transform tf;

	void Awake() {
		G.Sys.waveManager = this;
		CurrentWaveIndex = 0;
		tf = transform;
	}

	void Start() {
		StartCoroutine (SpawnNext ());
		G.Sys.menuManager.SetWaveNumber (0, Vagues.Length);
	}

	IEnumerator SpawnNext() {
		if (CurrentWaveIndex < Vagues.Length) {
			G.Sys.gameManager.StartTimer (Vagues [CurrentWaveIndex].TimeBeforeWave);

			yield return new WaitForSeconds (Vagues [CurrentWaveIndex].TimeBeforeWave);

			G.Sys.menuManager.SetWaveNumber (CurrentWaveIndex+1, Vagues.Length);

			foreach (var e in Vagues[CurrentWaveIndex].Entrees) {
				Instantiate (e.Vague, e.Entree.position, Quaternion.identity, tf);
			}

			CurrentWaveIndex++;

			StartCoroutine (SpawnNext ());
		}
	}
}

[System.Serializable]
public class Wave {
	public Enter[] Entrees;
	public float TimeBeforeWave;
}

[System.Serializable]
public class Enter {
	public Transform Entree;
	public GameObject Vague;
}