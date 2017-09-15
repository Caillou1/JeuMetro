
using System;
using UnityEngine; 

public enum AgentType
{
    CLEANER,
    AGENT,
}

public class AgentPlacedEvent : EventArgs
{
    public AgentPlacedEvent(Vector3 _pos, AgentType _type)
    {
        type = _type;
        pos = _pos;
    }

    public AgentType type;
    public Vector3 pos;
}
