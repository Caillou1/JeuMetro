using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AgentRemovedEvent : EventArgs
{
	public AgentRemovedEvent(Vector3 _pos, AgentType _type, bool _bought)
	{
		type = _type;
		pos = _pos;
        bought = _bought;
	}

	public AgentType type;
	public Vector3 pos;
    public bool bought;
}
