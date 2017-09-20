using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StartTutorialEvent : EventArgs 
{
    public StartTutorialEvent(int _index)
    {
        index = _index;
    }

    public int index;
}
