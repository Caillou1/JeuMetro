using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour {
	private List<WaveSpawner> Spawners;

	void Awake() {
		G.Sys.waveManager = this;
	}

	public void Register(WaveSpawner ws) {
		if (Spawners == null) {
			Spawners = new List<WaveSpawner> ();
		}

		Spawners.Add (ws);
	}
}
