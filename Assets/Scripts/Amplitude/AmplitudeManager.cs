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

    bool _firstSceneLoaded = false;

    DateTime _levelStartTime;

    int _placedObjectCount;
    int _removedObjectCount;
    int _missplacedObjectCount;
    int _levelIndex;

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
        _subscriberList.Add(new Event<QuitLevelEvent>.Subscriber(OnLevelQuit));
        _subscriberList.Add(new Event<QuitTutorialEvent>.Subscriber(OnTutorialQuit));
		_subscriberList.Subscribe();

        _amplitude = Amplitude.instance;
        _amplitude.apikey = ApiKey;

        _levelStartTime = DateTime.Now;
		_levelIndex = 0; 
        _placedObjectCount = 0;
		_missplacedObjectCount = 0;
        _removedObjectCount = 0;
	}

    void OnSceneLoaded(SceneLoadedEvent e)
	{
        if (_firstSceneLoaded)
            return;
		_firstSceneLoaded = true; 

        var data = new StartSessionData();
		data.GameVersion = G.Sys.Version;
		_amplitude.SendEvent(StartSessionStr, data);
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
        data.Levelindex = _levelIndex;
        data.Time = (float)time;
        data.Restart = e.restarted;
        _amplitude.SendEvent(QuitTutorialStr, data);
    }

    void OnLevelQuit(QuitLevelEvent e)
    {
        Debug.Log("Poop");
    }

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

    class QuitLevelData
    {
        public int LevelIndex;
        public int PlacedObjectCount;
        public float Time;
        public int Wave;
        public float WaveTime;
        public bool Restart;
    }
}

