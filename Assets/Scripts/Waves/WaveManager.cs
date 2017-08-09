﻿﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using System.Linq;

public class WaveManager : MonoBehaviour {
	public Score score;

	[BoxGroup("Waves")]
	public int waveCounts;
	[BoxGroup("Waves")]
	[Tooltip("Temps d'attente avant le lancement de la vague. Le dernier temps détermine le temps de la derniere vague")]
	public float[] waveTimes = new float[]{ };
	[BoxGroup("Waves")]
	public InWaveArray[] inWaves = new InWaveArray[]{ };
	[BoxGroup("Waves")]
	public MetroWaveArray[] metroWaves = new MetroWaveArray[]{ };

	[BoxGroup("Metros")]
	public MetroDelay[] metrosDelay = new MetroDelay[]{ };

	void OnValidate()
	{
		if (waveTimes.Length != waveCounts + 1)
		{
			Array.Resize(ref waveTimes, waveCounts + 1);
		}

		if (inWaves.Length != waveCounts)
		{
			Array.Resize(ref inWaves, waveCounts);
		}

		if (metroWaves.Length != waveCounts)
		{
			Array.Resize(ref metroWaves, waveCounts);
		}
	}

	[System.Serializable]
	public class InWave
	{
		public Transform Entrance;
		public GameObject Wave;
		public float Delay;
	}
	[System.Serializable]
	public class InWaveArray
	{
		public InWave[] array = new InWave[]{ };
	}

	[System.Serializable]
	public class MetroWave
	{
		public Metro MetroObject;
		public GameObject Wave;
	}

	[System.Serializable]
	public class MetroWaveArray
	{
		public MetroWave[] array = new MetroWave[]{ };
	}

	[System.Serializable]
	public class MetroDelay
	{
		public MetroDelay(){}
		public MetroDelay(Metro m, float d){ MetroObject = m; Delay = d;}
		public Metro MetroObject;
		public float Delay;
	}

	int currentWave;
	float chronoStartTime;
	float chronoLastTime;
	bool ended = false;

	List<MetroDelay> realMetroDelays = new List<MetroDelay>();

	void Start()
	{
		currentWave = -1;
		chronoLastTime = Time.time;
		chronoStartTime = Time.time;

		G.Sys.menuManager.SetWaveNumber (currentWave + 1, waveCounts);

		for (int i = 0; i < G.Sys.metroCount(); i++) {
			MetroDelay obj = null;
			foreach (var m in metrosDelay)
				if (m.MetroObject == G.Sys.metro (i))
					obj = m;
			if (obj == null)
				realMetroDelays.Add (new MetroDelay (G.Sys.metro (i), 0));
			else
				realMetroDelays.Add (obj);
		}
	}

	void SendScore() {
        Event<CollectTravelerTimeEvent>.Broadcast(new CollectTravelerTimeEvent());
        score.SetValues(G.Sys.gameManager.GetAverageTime (), G.Sys.gameManager.GetEarnedMoney (), G.Sys.tilemap.getMaxUsedSpace (), 0);
        float totalScore = 0;
        totalScore += 1/(score.AverageTime / score.GoldAverageTime) * 1000;
        totalScore += score.MoneyLeft / (float)score.GoldMoneyLeft * 1000;
        totalScore += 1 / ((1f + score.SpaceUsed) / (1f + score.GoldSurface)) * 1000;
        score.ScoreValue = (int)totalScore;

		Event<WinGameEvent>.Broadcast (new WinGameEvent (score));
	}

	void Update()
	{
		if (ended)
			return;
		
		if (Time.time - chronoStartTime > WaveTime (currentWave)) {
			currentWave++;
			G.Sys.menuManager.SetWaveNumber (currentWave + 1, waveCounts);
			if (currentWave >= waveCounts) {
				ended = true;
				G.Sys.menuManager.SetPieTime (1, 0);
				SendScore ();
				return;
			}
			chronoStartTime = Time.time;
		}

		G.Sys.menuManager.SetPieTime ((Time.time - chronoStartTime) / WaveTime (currentWave), Mathf.RoundToInt(WaveTime (currentWave) - (Time.time - chronoStartTime)));

		float currentTime = Time.time - chronoStartTime;
		float lastTime = chronoLastTime - chronoStartTime;
		if(currentWave >= 0)
			SpawnInWave (currentWave, lastTime, currentTime - lastTime);
		if (currentWave < waveCounts - 1)
			SpawnInWave (currentWave + 1, lastTime - WaveTime (currentWave), currentTime - lastTime);

		chronoLastTime = Time.time;
	}

	void SpawnInWave(int wave, float time, float dt)
	{
		foreach (var v in inWaves[wave].array)
			if (v.Delay > time && v.Delay <= time + dt)
				Spawn (v.Entrance.position, v.Wave);

		foreach (var m in realMetroDelays) {
			float startTime = m.Delay - G.Sys.constants.MetroComeTime;
			if (startTime > time && startTime <= time + dt) {
				m.MetroObject.CallMetro ();
			}
		}

		foreach (var v in metroWaves[wave].array) {
			var d = getMetroDelay (v.MetroObject);
			if (d > time && d < time + dt)
				Spawn (v.MetroObject.transform.position, v.Wave);
		}
	}

	float getMetroDelay(Metro metro)
	{
		foreach (var m in metrosDelay)
			if (m.MetroObject == metro)
				return m.Delay;
		return 0;
	}

	void Spawn(Vector3 pos, GameObject prefab)
	{
		if(prefab != null)
			Instantiate (prefab, pos, Quaternion.identity);
	}

	float WaveTime(int wave)
	{
		if (wave < waveCounts)
			return waveTimes [wave + 1];
		return 0;
	}
}

[System.Serializable]
public class Score {
	public float GoldAverageTime;
	public int GoldMoneyLeft;
    public int GoldSurface;

	[HideInInspector]
	public float AverageTime;
    [HideInInspector]
    public bool HaveTimeMedal;
	[HideInInspector]
	public int MoneyLeft;
	[HideInInspector]
	public bool HaveMoneyMedal; 
	[HideInInspector]
	public int SpaceUsed;
    [HideInInspector]
    public bool HaveSurfaceMedal;
    [HideInInspector]
    public int ScoreValue;

	public void SetValues(float avgT, int money, int spaceUsed, int score) {
		AverageTime = avgT;
		MoneyLeft = money;
        SpaceUsed = spaceUsed;
        ScoreValue = score;

        HaveTimeMedal = AverageTime <= GoldAverageTime;
        HaveMoneyMedal = MoneyLeft >= GoldMoneyLeft;
        HaveSurfaceMedal = spaceUsed <= GoldSurface;
	}
}
