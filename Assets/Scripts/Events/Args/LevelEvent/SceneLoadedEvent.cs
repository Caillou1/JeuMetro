using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SceneLoadedEvent : EventArgs
{
    public SceneLoadedEvent(Menu _menu)
    {
        menu = _menu;
    }

    public Menu menu;
}
