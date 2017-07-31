using System;
using UnityEngine;
using System.Collections.Generic;

public class FaintEvent : EventArgs
{
	public FaintEvent(Traveler t)
	{
		traveler = t;
	}

	public Traveler traveler;
}

