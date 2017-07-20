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
	IN,
	OUT,
	METRO,
	ELEVATOR,
	SPEAKER,
	WASTE,
	EMPTYWALL,
	CONTROLELINE,
}

public abstract class ATile : MonoBehaviour
{
	protected abstract void Awake();
	protected abstract void OnDestroy (); 

	public void Unregister()
	{
		OnDestroy (); 
	}

	public void Register() { Awake (); }


	public TileID type { get; protected set; }
}
