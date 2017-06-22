using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawnerMetro : WaveSpawner {
	public GameObject[] Waves;

	private int CurrentWave;
	private Transform tf;

	void Start() {
		tf = transform;
		G.Sys.waveManager.Register (this);
		CurrentWave = 0;
	}

	void SpawnNextWave() {
		if (CurrentWave < Waves.Length) {
			Instantiate (Waves [CurrentWave], tf.position, Quaternion.identity, tf);
		}
	}
}