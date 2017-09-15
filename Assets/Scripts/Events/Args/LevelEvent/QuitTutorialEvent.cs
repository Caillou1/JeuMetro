using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class QuitTutorialEvent : EventArgs
{
	public QuitTutorialEvent(bool _restarted)
	{
		restarted = _restarted;
	}

	public bool restarted;
}
