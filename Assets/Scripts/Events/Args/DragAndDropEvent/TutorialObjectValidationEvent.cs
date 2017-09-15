using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TutorialObjectValidationEvent : EventArgs
{
    public TutorialObjectValidationEvent(ObjectifType _type, Vector3 _pos, bool _missplaced)
    {
        type = _type;
        missplaced = _missplaced;
        pos = _pos;
    }

    public ObjectifType type;
    public bool missplaced;
    public Vector3 pos;
}
