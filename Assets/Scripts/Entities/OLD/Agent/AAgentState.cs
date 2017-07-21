using System;

namespace V1
{
public abstract class AAgentState : AState
{
	public AAgentState (AgentEntity _agent, StateType _type) : base (_type)
	{
		agent = _agent;
	}

	protected readonly AgentEntity agent;
}

}