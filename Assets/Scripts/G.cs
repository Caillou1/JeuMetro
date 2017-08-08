using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class G
{
    private static volatile G _instance;

	public Tilemap tilemap = new Tilemap();
    public int levelIndex = 0;
	private List<Traveler> travelers = new List<Traveler> ();
	private List<Agent> agents = new List<Agent> ();
	private List<Cleaner> cleaners = new List<Cleaner> ();
	private List<Metro> metros = new List<Metro> ();

	private Camera _camera;
    private GameManager _gameManager;
	private WaveManager _waveManager;
	private AudioManager _audioManager;
	private CameraController _cameraController;
	private SelectionManager _selectionManager;
	private MenuManager _menuManager;
	private Constants _constants;


    public static G Sys
    {
        get
        {
            if (G._instance == null)
                G._instance = new G();
            return G._instance;
        }
    }

	public Camera MainCamera {
		get {
			if(_camera == null)
				_camera = Camera.main;
			return _camera;
		}
	}

    public GameManager gameManager
    {
        get { return _gameManager; }
        set
        {
            if (_gameManager != null)
                Debug.Log("2 gameManagers instantiated !");
            _gameManager = value;
        }
    }

	public WaveManager waveManager {
		get { return _waveManager; }
		set {
			if (_waveManager != null)
				Debug.Log ("2 waveManagers instantiated !");
			_waveManager = value;
		}
	}

	public void registerTraveler(Traveler t)
	{
		if (!travelers.Contains (t)) {
			travelers.Add (t);
			_menuManager.SetTravelerNumber (travelerCount());
		}
	}

	public bool removeTraveler(Traveler t)
	{
		var value = travelers.Remove (t);
		_menuManager.SetTravelerNumber (travelerCount());
		return value;
	}

	public void registerAgent(Agent a) {
		if (!agents.Contains (a)) {
			agents.Add (a);
		}
	}

	public bool removeAgent(Agent a) {
		return agents.Remove (a);
	}

	public void registerCleaner(Cleaner c) {
		if (!cleaners.Contains (c)) {
			cleaners.Add (c);
		}
	}

	public bool removeCleaner(Cleaner c) {
		return cleaners.Remove (c);
	}

	public Agent GetNearestAgent(Vector3 pos, float maxDist = float.MaxValue) {
		Agent closest = null;
		float minDist = float.MaxValue;

		foreach (var a in agents) {
			float dist = (pos - a.transform.position).sqrMagnitude;
			if (dist < minDist && dist < maxDist) {
				minDist = dist;
				closest = a;
			}
		}

		return closest;
	}

	public Cleaner GetNearestCleaner(Vector3 pos, float maxDist = float.MaxValue) {
		Cleaner closest = null;
		float minDist = float.MaxValue;

		foreach (var a in cleaners) {
			float dist = (pos - a.transform.position).sqrMagnitude;
			if (dist < minDist && dist < maxDist) {
				minDist = dist;
				closest = a;
			}
		}

		return closest;
	}

	public int travelerCount()
	{
		return travelers.Count;
	}

	public Traveler traveler(int index)
	{
		return travelers [index];
	}

	public void registerMetro(Metro t)
	{
		if (!metros.Contains (t))
			metros.Add (t);
	}

	public bool removeMetro(Metro t)
	{
		return metros.Remove (t);
	}

	public int metroCount()
	{
		return metros.Count;
	}

	public Metro metro(int index)
	{
		return metros [index];
	}
		
	public void clear()
	{
		travelers.Clear ();
		agents.Clear ();
		cleaners.Clear ();
		metros.Clear ();
	}

	public AudioManager audioManager {
		get { return _audioManager; }
		set {
			if (_audioManager != null)
				Debug.Log ("2 audioManagers instantiated");
			_audioManager = value;
		}
	}

	public CameraController cameraController {
		get { return _cameraController; }
		set{
			if (_cameraController != null)
				Debug.Log ("2 cameraController instantiated !");
			_cameraController = value;
		}
	}

	public SelectionManager selectionManager {
		get { return _selectionManager; }
		set {
			if (_selectionManager != null)
				Debug.Log ("2 selectionManager instantiated !");
			_selectionManager = value;
		}
	}

	public MenuManager menuManager {
		get { return _menuManager; }
		set {
			if (_menuManager != null)
				Debug.Log ("2 menuManager instantiated !");
			_menuManager = value;
		}
	}

	public Constants constants {
		get { return _constants; }
		set {
			if (_constants != null)
				Debug.Log ("2 constants instanciated !");
			_constants = value;
		}
	}
}