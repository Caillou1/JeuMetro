using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

public class WaveManager : MonoBehaviour {
	public Wave[] Vagues;

	private int CurrentWaveIndex;
	private Transform tf;
	private Wave CurrentArrivingWave;
	private float ChronoStartTime;

	void Awake() {
		tf = transform;
		G.Sys.waveManager = this;
		CurrentWaveIndex = 0;
	}

	void Start() {
		G.Sys.menuManager.SetWaveNumber (0, Vagues.Length);
		StartCoroutine (SpawnNext ());
	}

	void Update() {
		if (CurrentArrivingWave != null) {
			float time = CurrentArrivingWave.TimeBeforeWave - (Time.time - ChronoStartTime);
			float timePercentage = (Time.time - ChronoStartTime) / CurrentArrivingWave.TimeBeforeWave;

			G.Sys.menuManager.SetPieTime (timePercentage, (int)time + 1);
		}
	}

	void StartChrono(Wave w) {
		CurrentArrivingWave = w;
		ChronoStartTime = Time.time;
	}

	IEnumerator SpawnNext() {
		if (CurrentWaveIndex < Vagues.Length) {
			var v = Vagues [CurrentWaveIndex];
			StartChrono (v);
			yield return new WaitForSeconds (v.TimeBeforeWave - G.Sys.constants.MetroComeTime);

			for (int i = 0; i < G.Sys.metroCount(); i++)
				G.Sys.metro (i).CallMetro();

			yield return new WaitForSeconds (G.Sys.constants.MetroComeTime);

			foreach (var e in v.Entrees) {
					InstantiateWave (e.Vague, e.Entree.position);
			}
				
			CurrentWaveIndex++;

			G.Sys.menuManager.SetWaveNumber (CurrentWaveIndex, Vagues.Length);

			StartCoroutine (SpawnNext ());
		}
	}

	void InstantiateWave(GameObject wave, Vector3 pos) {
		if(wave != null)
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
	public GameObject Vague;
}

[System.Serializable]
public class Wave {
	public float TimeBeforeWave;
	public Entrance[] Entrees;
}