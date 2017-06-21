using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawnerDoor : WaveSpawner {
	public DoorWave[] Waves;

	private int CurrentWave;
	private Transform tf;

	void Start() {
		tf = transform;
		G.Sys.waveManager.Register (this);
		CurrentWave = 0;
		StartCoroutine (SpawnNextWave ());
	}

	IEnumerator SpawnNextWave() {
		Instantiate (Waves[CurrentWave].Vague, tf.position, Quaternion.identity, tf);

		if (CurrentWave < Waves.Length - 1) {
			yield return new WaitForSeconds (Waves [CurrentWave].TimeBeforeNext);
			CurrentWave++;
			StartCoroutine (SpawnNextWave ());
		}
	}
}

[System.Serializable]
public class DoorWave {
	public GameObject Vague;
	public float TimeBeforeNext;
}