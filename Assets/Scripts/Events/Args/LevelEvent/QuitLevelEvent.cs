using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class QuitLevelEvent : EventArgs
{
    public QuitLevelEvent(bool _restarted)
    {
        restarted = _restarted;
    }

    public bool restarted;
}
