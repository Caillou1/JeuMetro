﻿using System;
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

	//[BoxGroup("Metros")]
	//public MetroDelay[] metrosDelay = new MetroDelay[]{ };

    [BoxGroup("Waves")]
    public bool EndWithFireAlert = false;
    [BoxGroup("Waves")]
    [ShowIf("EndWithFireAlert")]
    public float FireAlertTime = 0;

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
        public MetroWave(){}
        public MetroWave(Metro metroObject) { MetroObject = metroObject; }
		public Metro MetroObject;
		public GameObject Wave;
        public float delay;
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

    List<List<MetroWave>> realMetroWave = new List<List<MetroWave>>();

	void Start()
	{
		currentWave = -1;
		chronoLastTime = Time.time;
		chronoStartTime = Time.time;

		G.Sys.menuManager.SetWaveNumber (currentWave + 1, waveCounts);

        foreach (var m in metroWaves)
        {
            List<MetroWave> tempWave = new List<MetroWave>();

            for (int i = 0; i < G.Sys.metroCount(); i++)
            {
                MetroWave obj = null;
                foreach (var w in m.array)
                    if (w.MetroObject == G.Sys.metro(i))
                        obj = w;
                if (obj == null)
                    obj = new MetroWave(G.Sys.metro(i));
                tempWave.Add(obj);
            }
            realMetroWave.Add(tempWave);
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

        if (Time.time - chronoStartTime > WaveTime(currentWave))
		{
			currentWave++;
            G.Sys.menuManager.SetWaveNumber(Mathf.Min(currentWave + 1, waveCounts), waveCounts);
			if (currentWave >= waveCounts + 1)
			{
				ended = true;
				G.Sys.menuManager.SetPieTime(1, 0);
				SendScore();
				return;
			}
			else if (currentWave >= waveCounts)
			{
                if (!EndWithFireAlert)
                {
                    ended = true;
                    G.Sys.menuManager.SetPieTime(1, 0);
                    SendScore();
                    return;
                }
                else
                {
                    G.Sys.gameManager.FireAlert = true;
                    Event<StartFireAlertEvent>.Broadcast(new StartFireAlertEvent());
                    Event<BakeNavMeshEvent>.Broadcast(new BakeNavMeshEvent());
                }
			}
			chronoStartTime = Time.time;
		}

		G.Sys.menuManager.SetPieTime ((Time.time - chronoStartTime) / WaveTime (currentWave), Mathf.RoundToInt(WaveTime (currentWave) - (Time.time - chronoStartTime)));

        if (!G.Sys.gameManager.FireAlert)
        {
            float currentTime = Time.time - chronoStartTime;
            float lastTime = chronoLastTime - chronoStartTime;
            if (currentWave >= 0)
                SpawnInWave(currentWave, lastTime, currentTime - lastTime);
            if (currentWave < waveCounts - 1)
                SpawnInWave(currentWave + 1, lastTime - WaveTime(currentWave), currentTime - lastTime);

            chronoLastTime = Time.time;
        }
	}

	void SpawnInWave(int wave, float time, float dt)
	{
		foreach (var v in inWaves[wave].array)
			if (v.Delay > time && v.Delay <= time + dt)
                Spawn (v.Entrance.position, v.Wave, v.Entrance.transform.rotation);

        foreach (var v in realMetroWave[wave]) {
            float startTime = v.delay - G.Sys.constants.MetroComeTime;
            if (startTime > time && startTime <= time + dt)
                v.MetroObject.CallMetro();
            var d = v.delay + 0.2f;
			if (d > time && d <= time + dt)
                Spawn (v.MetroObject.transform.position, v.Wave, v.MetroObject.transform.rotation);
		}
	}

	float getMetroDelay(Metro metro, int wave)
	{
        foreach (var m in realMetroWave[wave])
			if (m.MetroObject == metro)
                return m.delay;
		return 0;
	}

    void Spawn(Vector3 pos, GameObject prefab, Quaternion rot)
	{
		if(prefab != null)
            Instantiate (prefab, pos, rot);
	}

	float WaveTime(int wave)
	{
		if (wave < waveCounts)
			return waveTimes [wave + 1];
        return FireAlertTime;
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
