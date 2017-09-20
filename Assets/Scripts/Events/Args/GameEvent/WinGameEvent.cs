using System;
using UnityEngine;
using System.Collections.Generic;

public class WinGameEvent : EventArgs
{
	public WinGameEvent(Score s)
	{
		score = s;
	}

	public Score score;
}

