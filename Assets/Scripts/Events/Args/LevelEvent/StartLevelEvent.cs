using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StartLevelEvent : EventArgs 
{
	public StartLevelEvent(int _index)
	{
		index = _index;
	}

	public int index;
}
