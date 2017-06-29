using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class G
{
    private static volatile G _instance;
    public Tilemap tilemap = new Tilemap();
    private GameManager _gameManager;
	private WaveManager _waveManager;
	private AudioManager _audioManager;
	private CameraController _cameraController;
	private SelectionManager _selectionManager;

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
}