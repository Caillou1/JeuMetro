using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FinishWaveEvent : EventArgs
{
    public FinishWaveEvent(int _wave)
    {
        wave = _wave;
    }

    public int wave;
}
