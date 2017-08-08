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
		score.SetValues(G.Sys.gameManager.GetAverageTime (), G.Sys.gameManager.GetMoney (), G.Sys.tilemap.CalculateFreeSpacePercentage ());
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
	[BoxGroup("AverageTime")]
	public float BronzeAverageTime;
	[BoxGroup("AverageTime")]
	public float SilverAverageTime;
	[BoxGroup("AverageTime")]
	public float GoldAverageTime;

	[BoxGroup("MoneyLeft")]
	public int BronzeMoneyLeft;
	[BoxGroup("MoneyLeft")]
	public int SilverMoneyLeft;
	[BoxGroup("MoneyLeft")]
	public int GoldMoneyLeft;

	[BoxGroup("FreeSpace")]
	public float BronzeFreeSpace;
	[BoxGroup("FreeSpace")]
	public float SilverFreeSpace;
	[BoxGroup("FreeSpace")]
	public float GoldFreeSpace;

	[HideInInspector]
	public float AverageTime;
	[HideInInspector]
	public MedalType MedalAverageTime;
	[HideInInspector]
	public int MoneyLeft; 
	[HideInInspector]
	public MedalType MedalMoneyLeft;
	[HideInInspector]
	public float FreeSpacePercentage;
	[HideInInspector]
	public MedalType MedalFreeSpacePercentage;

	public void SetValues(float avgT, int money, float freeSpace) {
		AverageTime = avgT;
		MoneyLeft = money;
		FreeSpacePercentage = freeSpace;

		if (AverageTime <= GoldAverageTime)
			MedalAverageTime = MedalType.Gold;
		else if (AverageTime <= SilverAverageTime)
			MedalAverageTime = MedalType.Silver;
		else if (AverageTime <= BronzeAverageTime)
			MedalAverageTime = MedalType.Bronze;
		else
			MedalAverageTime = MedalType.None;

		if (MoneyLeft >= GoldMoneyLeft)
			MedalMoneyLeft = MedalType.Gold;
		else if (MoneyLeft >= SilverMoneyLeft)
			MedalMoneyLeft = MedalType.Silver;
		else if (MoneyLeft >= BronzeMoneyLeft)
			MedalMoneyLeft = MedalType.Bronze;
		else
			MedalMoneyLeft = MedalType.None;

		if (FreeSpacePercentage >= GoldFreeSpace)
			MedalFreeSpacePercentage = MedalType.Gold;
		else if (FreeSpacePercentage >= SilverFreeSpace)
			MedalFreeSpacePercentage = MedalType.Silver;
		else if (FreeSpacePercentage >= BronzeFreeSpace)
			MedalFreeSpacePercentage = MedalType.Bronze;
		else
			MedalFreeSpacePercentage = MedalType.None;
	}

    public float nextTimeMedalValue()
    {
        if (AverageTime <= GoldAverageTime)
            return 0;
        if (AverageTime <= SilverAverageTime)
            return GoldAverageTime;
        if (AverageTime <= BronzeAverageTime)
            return SilverAverageTime;
        return BronzeAverageTime;
    }

    public int nextMoneyMedalValue()
    {
        if (MoneyLeft >= GoldMoneyLeft)
            return 0;
        if (MoneyLeft >= SilverMoneyLeft)
            return GoldMoneyLeft;
        if (MoneyLeft >= BronzeMoneyLeft)
            return SilverMoneyLeft;
        return BronzeMoneyLeft;
    }

    public float nextSpaceMedalValue()
    {
        if (FreeSpacePercentage >= GoldFreeSpace)
            return 0;
        if (FreeSpacePercentage >= SilverFreeSpace)
            return GoldFreeSpace;
        if (FreeSpacePercentage >= BronzeFreeSpace)
            return SilverFreeSpace;
        return BronzeFreeSpace;
    }

    public MedalType nextMedal(MedalType m)
    {
        switch(m)
        {
            case MedalType.Bronze:
                return MedalType.Silver;
            case MedalType.Silver:
                return MedalType.Gold;
            case MedalType.Gold:
                return MedalType.None;
            default: //MedalType.None
                return MedalType.Bronze;
        }
    }
}

public enum MedalType {
	None,
	Bronze,
	Silver,
	Gold
}
