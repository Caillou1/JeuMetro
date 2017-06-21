using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class G
{
    private static volatile G _instance;
    public Tilemap tilemap = new Tilemap();
    private GameManager _gameManager;

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
    
}