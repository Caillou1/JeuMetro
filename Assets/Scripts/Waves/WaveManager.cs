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

			foreach (var e in v.Entrees) {
				if (e.Type == EntranceType.Metro) {
					e.Metro.CallMetro ();
					DOVirtual.DelayedCall (G.Sys.constants.MetroComeTime, () => {
						InstantiateWave (e.Vague, e.Entree.position);
					});
				}
			}

			yield return new WaitForSeconds (G.Sys.constants.MetroComeTime);

			foreach (var e in v.Entrees) {
				if (e.Type == EntranceType.Door) {
					InstantiateWave (e.Vague, e.Entree.position);
				}
			}
				
			CurrentWaveIndex++;

			G.Sys.menuManager.SetWaveNumber (CurrentWaveIndex, Vagues.Length);

			StartCoroutine (SpawnNext ());
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
	public GameObject Vague;
	[ShowIf("IsMetro")]
	public Metro Metro;


	private bool IsMetro() {
		return Type == EntranceType.Metro;
	}
}

[System.Serializable]
public class Wave {
	public Entrance[] Entrees;
	public float TimeBeforeWave;
}