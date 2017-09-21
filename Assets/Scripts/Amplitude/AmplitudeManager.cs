using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class AmplitudeManager
{
    private const string ApiKeyTests = "5df5e4c504c6f123761f6348e28427db";
    private const string ApiKey = "d3eb320c4e366e3b6eee24d28bfe7ce8";

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

    bool _firstSceneLoaded = false;

    DateTime _levelStartTime;
    DateTime _waveStartTime;

    int _placedObjectCount;
    int _removedObjectCount;
    int _missplacedObjectCount;
    int _levelIndex;
    int _currentWave;
    bool _finished;

	public AmplitudeManager()
	{
        _subscriberList.Add(new Event<SceneLoadedEvent>.Subscriber(OnSceneLoaded));
        _subscriberList.Add(new Event<StartTutorialEvent>.Subscriber(OnTutorialStart));
        _subscriberList.Add(new Event<FinishTutorialEvent>.Subscriber(OnTutorialEnd));
        _subscriberList.Add(new Event<ObjectPlacedEvent>.Subscriber(OnPlaceObjectEvent));
        _subscriberList.Add(new Event<AgentPlacedEvent>.Subscriber(OnPlaceAgentEvent));
        _subscriberList.Add(new Event<ObjectRemovedEvent>.Subscriber(OnRemoveObjectEvent));
        _subscriberList.Add(new Event<AgentRemovedEvent>.Subscriber(OnRemoveAgentEvent));
        _subscriberList.Add(new Event<TutorialObjectValidationEvent>.Subscriber(OnTutorialItemValidationEvent));
        _subscriberList.Add(new Event<QuitTutorialEvent>.Subscriber(OnTutorialQuit));
		_subscriberList.Add(new Event<StartLevelEvent>.Subscriber(OnLevelStart));
		_subscriberList.Add(new Event<QuitLevelEvent>.Subscriber(OnLevelQuit));
        _subscriberList.Add(new Event<WinGameEvent>.Subscriber(OnFinishLevel));
        _subscriberList.Add(new Event<LooseLevelEvent>.Subscriber(OnLooseLevel));
		_subscriberList.Subscribe();

        _amplitude = Amplitude.instance;
		switch (Application.platform)
		{
			case RuntimePlatform.LinuxEditor:
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.WindowsEditor:
				_amplitude.apikey = ApiKeyTests;
                break;
			default:
				_amplitude.apikey = ApiKey;
				break;
		}

        _levelStartTime = DateTime.Now;
		_levelIndex = 0; 
        _placedObjectCount = 0;
		_missplacedObjectCount = 0;
        _removedObjectCount = 0;
        _finished = false;
        _amplitude.appVersion = G.Version;
	}

    // --- Event that send datas to Amplitude ---

    void OnSceneLoaded(SceneLoadedEvent e)
	{
        if (_firstSceneLoaded)
            return;
		_firstSceneLoaded = true; 

		_amplitude.SendEvent(StartSessionStr);
	}

    void OnTutorialStart(StartTutorialEvent e)
    {
        _levelStartTime = DateTime.Now;
        _placedObjectCount = 0;
        _missplacedObjectCount = 0;
        _removedObjectCount = 0;
        _levelIndex = e.index;

        var data = new StartLevelData();
        data.LevelIndex = e.index;
        _amplitude.SendEvent(StartTutorialStr, data);
    }

    void OnTutorialEnd(FinishTutorialEvent e)
    {
        var time = (DateTime.Now - _levelStartTime).TotalSeconds;

        var data = new FinishTutorialData();
        data.LevelIndex = _levelIndex;
        data.Time = (float)time;
        data.MisplacedObjectCount = _missplacedObjectCount;
        data.PlacedObjectCount = _placedObjectCount;
        data.RemovedObjectCount = _removedObjectCount;
        _amplitude.SendEvent(FinishTutorialStr, data);
    }

    void OnTutorialQuit(QuitTutorialEvent e)
    {
        var time = (DateTime.Now - _levelStartTime).TotalSeconds;

        var data = new QuitTutorialData();
        data.LevelIndex = _levelIndex;
        data.Time = (float)time;
        data.Restart = e.restarted;
        _amplitude.SendEvent(QuitTutorialStr, data);
    }

    void OnLevelStart(StartLevelEvent e)
    {
		_levelStartTime = DateTime.Now;
        _waveStartTime = DateTime.Now;
        _currentWave = 0;
		_placedObjectCount = 0;
		_missplacedObjectCount = 0;
		_removedObjectCount = 0;
		_levelIndex = e.index;
        _finished = false;

        var data = new StartLevelData();
        data.LevelIndex = e.index;
        _amplitude.SendEvent(StartLevelStr, data);
    }

    void OnLevelQuit(QuitLevelEvent e)
    {
        if (_finished)
            return;
        
        var time = (DateTime.Now - _levelStartTime).TotalSeconds;
        var waveTime = (DateTime.Now - _waveStartTime).TotalSeconds;

        var data = new QuitLevelData();
        data.LevelIndex = _levelIndex;
        data.PlacedObjectCount = _placedObjectCount;
        data.RemovedObjectCount = _removedObjectCount;
        data.Restart = e.restarted;
        data.Time = (float)time;
        data.Wave = _currentWave;
        data.WaveTime = (float)waveTime;
        _amplitude.SendEvent(QuitLevelStr, data);
    }

	void OnFinishLevel(WinGameEvent e)
    {
        _finished = true;

        var time = (DateTime.Now - _levelStartTime).TotalSeconds;

        var data = new FinishLevelData();
        data.LevelIndex = _levelIndex;
        data.MedalMoney = e.score.HaveMoneyMedal;
        data.MedalSurface = e.score.HaveSurfaceMedal;
        data.MedalTime = e.score.HaveTimeMedal;
        data.PlacedObjectCount = _placedObjectCount;
        data.RemovedObjectCount = _removedObjectCount;
        data.Score = e.score.ScoreValue;
        data.Time = (float)time;
        data.ValueFireAlert = e.score.WonFireAlert;
        data.ValueMoney = e.score.MoneyLeft;
        data.ValueSurface = e.score.SpaceUsed;
        data.ValueTime = e.score.AverageTime;
        _amplitude.SendEvent(FinishLevelStr, data);
    }

    void OnLooseLevel(LooseLevelEvent e)
    {
        _finished = true;

		var time = (DateTime.Now - _levelStartTime).TotalSeconds;
		var waveTime = (DateTime.Now - _waveStartTime).TotalSeconds;

        var data = new LooseLevelData();
		data.LevelIndex = _levelIndex;
		data.PlacedObjectCount = _placedObjectCount;
		data.RemovedObjectCount = _removedObjectCount;
		data.Time = (float)time;
		data.Wave = _currentWave;
		data.WaveTime = (float)waveTime;
        _amplitude.SendEvent(LooseLevelStr, data);
    }

	// --- Stats events ---

	void OnPlaceObjectEvent(ObjectPlacedEvent e)
    {
        if(!e.bought)
            _placedObjectCount++;
    }

    void OnPlaceAgentEvent(AgentPlacedEvent e)
    {
        _placedObjectCount++;
    }

    void OnRemoveObjectEvent(ObjectRemovedEvent e)
    {
        if(e.bought)
            _removedObjectCount++;
    }

    void OnRemoveAgentEvent(AgentRemovedEvent e)
    {
        if (e.bought)
            _removedObjectCount++;
    }

    void OnTutorialItemValidationEvent(TutorialObjectValidationEvent e)
    {
        if (e.missplaced)
            _missplacedObjectCount++;
    }

    void OnFinishWaveEvent(FinishWaveEvent e)
    {
        _currentWave = e.wave + 1;
        _waveStartTime = DateTime.Now;
    }

    // --- Datas ---

    [Serializable]
    class StartLevelData
    {
        public int LevelIndex;
    }

    [Serializable]
    class QuitTutorialData
    {
        public int LevelIndex;
        public float Time;
        public bool Restart;
    }

    [Serializable]
    class FinishTutorialData
    {
        public int LevelIndex;
        public int PlacedObjectCount;
        public int MisplacedObjectCount;
        public int RemovedObjectCount;
        public float Time;
    }

    [Serializable]
    class FinishLevelData
    {
        public int LevelIndex;
        public int PlacedObjectCount;
        public int RemovedObjectCount;
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

    class LooseLevelData
    {
		public int LevelIndex;
		public int PlacedObjectCount;
        public int RemovedObjectCount;
		public float Time;
		public int Wave;
		public float WaveTime;
    }

    class QuitLevelData
    {
        public int LevelIndex;
        public int PlacedObjectCount;
        public int RemovedObjectCount;
        public float Time;
        public int Wave;
        public float WaveTime;
        public bool Restart;
    }
}

