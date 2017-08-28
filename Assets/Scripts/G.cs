using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public sealed class G
{
    private static volatile G _instance;

	public Tilemap tilemap = new Tilemap();
    public int levelIndex = 0;
	private List<Traveler> travelers = new List<Traveler> ();
    private List<Traveler> falseTravelers = new List<Traveler>();
	private List<Agent> agents = new List<Agent> ();
	private List<Cleaner> cleaners = new List<Cleaner> ();
	private List<Metro> metros = new List<Metro> ();
	private Dictionary<TileID, int> disposablesCount;

	private Camera _camera;
    private GameManager _gameManager;
	private WaveManager _waveManager;
	private AudioManager _audioManager;
	private CameraController _cameraController;
	private SelectionManager _selectionManager;
	private MenuManager _menuManager;
	private Constants _constants;

	public List<Agent> agentsList { 
		get {
			return agents;
		}
	}

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
			_menuManager.SetTravelerNumber (travelerCount(), G.Sys.gameManager.MaxTravelerBeforeLose);
		}
	}

	public bool removeTraveler(Traveler t)
	{
		var value = travelers.Remove (t);

		_menuManager.SetTravelerNumber (travelerCount(), G.Sys.gameManager.MaxTravelerBeforeLose);
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

	public Agent GetNearestFreeAgent(Vector3 pos, float maxDist = float.MaxValue) {
		Agent closest = null;
		float minDist = float.MaxValue;

		foreach (var a in agents) {
            if (a.IsHelping)
                continue;

			float dist = (pos - a.transform.position).sqrMagnitude;
            if (dist > maxDist || dist >= minDist)
				continue;
            
			NavMeshPath p = new NavMeshPath();
            a.getNavMeshAgent().CalculatePath(pos, p);
            if (p.status != NavMeshPathStatus.PathComplete)
                continue;

			minDist = dist;
			closest = a;
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

	public void AddDisposable(TileID id) {
		if (disposablesCount == null)
			disposablesCount = new Dictionary<TileID, int> ();
		
		if (disposablesCount.ContainsKey (id)) {
			disposablesCount [id]++;
		} else {
			disposablesCount.Add (id, 1);
		}
	}

	public int GetDisposableCount(TileID id) {
		if (disposablesCount != null && disposablesCount.ContainsKey (id)) {
			return disposablesCount [id];
		} else {
			return 0;
		}
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

	public int agentsCount {
		get {
			return agents.Count;
		}
	}

	public int cleanerCount {
		get {
			return cleaners.Count;
		}
	}

	public void registerFalseTraveler(Traveler t)
	{
		if (!falseTravelers.Contains(t))
			falseTravelers.Add(t);
	}

	public bool removeFalseTraveler(Traveler t)
	{
		return falseTravelers.Remove(t);
	}

    public int falseTravelerCount()
	{
		return falseTravelers.Count;
	}
}