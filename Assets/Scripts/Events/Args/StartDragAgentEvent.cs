
using System;

public class StartDragAgentEvent : EventArgs
{
    public StartDragAgentEvent(AgentType _type)
    {
        type = _type;
    }

    public AgentType type;
}
