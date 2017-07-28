﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using V1;

public sealed class G
{
    private static volatile G _instance;

	public Tilemap tilemap = new Tilemap();
	private List<Traveler> travelers = new List<Traveler> ();
	private List<Agent> agents = new List<Agent> ();

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

	public Agent GetNearestAgent(Vector3 pos) {
		Agent closest = null;
		float minDist = float.MaxValue;

		foreach (var a in agents) {
			float dist = Vector3.Distance (pos, a.transform.position);
			if (dist < minDist) {
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
		
	public void clear()
	{
		travelers.Clear ();
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