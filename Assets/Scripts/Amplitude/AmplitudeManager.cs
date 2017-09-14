using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class AmplitudeManager
{
    private const string ApiKey = "5df5e4c504c6f123761f6348e28427db";

    private const string StartSessionStr = "StartSession";
    private const string StartTutorialStr = "StartTutorial";
    private const string QuitTutorialStr = "QuitTutorial";
    private const string FinishTutorialStr = "FinishTutorial";
    private const string StartLevelStr = "StartLevel";
    private const string FinishLevelStr = "FinishLevel";
    private const string LooseLevelStr = "LooseLevel";
    private const string QuitLevelStr = "QuitLevel";

	private Amplitude _amplitude;
	private SubscriberList _subscriberList = new SubscriberList();

	public AmplitudeManager()
	{
		_subscriberList.Subscribe();

        _amplitude = Amplitude.instance;
        _amplitude.apikey = ApiKey;
	}

    public void StartSession()
    {
        var data = new StartSessionData();
        data.GameVersion = G.Sys.Version;
        _amplitude.SendEvent(StartSessionStr, data);
    }

    [Serializable]
    class StartSessionData
    {
        public string GameVersion;
    }

    [Serializable]
    class StartLevelData
    {
        public int LevelIndex;
    }

    [Serializable]
    class QuitTutorialData
    {
        public int Levelindex;
        public float Time;
    }

    [Serializable]
    class FinishTutorialData
    {
        public int LevelIndex;
        public int PlacedObjectCount;
        public int MisplacedObjectCount;
        public float Time;
    }

    [Serializable]
    class FinishLevelData
    {
        public int LevelIndex;
        public int PlacedObjectCount;
        public float Time;
        public bool MedalTime;
        public bool MedalMoney;
        public bool MedalSurface;
        public int Score;
        public float ValueTime;
        public int ValueMoney;
        public int ValueSurface;
        public bool ValueFireAlert;
    }

    class QuitLevelData
    {
        public int LevelIndex;
        public int PlacedObjectCount;
        public float Time;
        public int Wave;
        public float WaveTime;
    }
}

