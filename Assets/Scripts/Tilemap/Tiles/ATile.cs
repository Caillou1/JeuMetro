using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TileID
{
	GROUND,
	WALL,
	BIN,
	ESCALATOR,
	BENCH,
	FOODDISTRIBUTEUR,
	PODOTACTILE,
	INFOPANEL,
	TICKETDISTRIBUTEUR,
	STAIRS,
	OUT,
	METRO,
	ELEVATOR,
	SPEAKER,
	WASTE,
	EMPTYWALL,
	CONTROLELINE,
	WAIT_ZONE,
}

public abstract class ATile : MonoBehaviour
{
	protected abstract void Awake();
	protected abstract void OnDestroy (); 

	public void Unregister()
	{
		OnDestroy (); 
        OnUnregister();
	}

    protected virtual void OnUnregister()
    {
        
    }


	public void Register() 
    {
        OnRegister();
        Awake (); 
    }

	protected virtual void OnRegister()
	{

	}

	public TileID type { get; protected set; }
}
