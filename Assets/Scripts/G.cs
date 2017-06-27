using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class G
{
    private static volatile G _instance;

    public Tilemap tilemap = new Tilemap();
	private List<Traveler> travelers = new List<Traveler> ();

    private GameManager _gameManager;
	private WaveManager _waveManager;

    public static G Sys
    {
        get
        {
            if (G._instance == null)
                G._instance = new G();
            return G._instance;
        }
    }

    public GameManager gameManager
    {
        get { return _gameManager; }
        set
        {
            if (_gameManager != null)
                Debug.Log("2 gameManagers instanciated !");
            _gameManager = value;
        }
    }

	public WaveManager waveManager {
		get { return _waveManager; }
		set {
			if (_waveManager != null)
				Debug.Log ("2 waveManagers instanciated !");
			_waveManager = value;
		}
	}

	public void registerTraveler(Traveler t)
	{
		if(!travelers.Contains(t))
			travelers.Add (t);
	}

	public bool removeTraveler(Traveler t)
	{
		return travelers.Remove (t);
	}

	public int travelerCount()
	{
		return travelers.Count;
	}

	public void clear()
	{
		travelers.Clear ();
	}
}