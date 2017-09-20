
using System;

public class AbortDragAgentEvent : EventArgs
{
	public AbortDragAgentEvent(AgentType _type)
	{
		type = _type;
	}

	public AgentType type;
}
