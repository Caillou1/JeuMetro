using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Awake()
    {
        G.Sys.gameManager = this;
    }

	void Start ()
    {
        
        G.Sys.tilemap.ConnectAll();
	}
	
	void Update ()
    {
		
	}
}
