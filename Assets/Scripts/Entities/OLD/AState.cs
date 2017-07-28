using System;

namespace V1
{
public enum StateType
{
	TRAVELER_MOVE,
	TRAVELER_STAIRS,
	TRAVELER_ESCALATOR,
	TRAVELER_LOST,
	TRAVELER_INFOS,
	TRAVELER_SIT,
	TRAVELER_THROW_WASTE,
	TRAVELER_FOOD,
	TRAVELER_TICKET,
	TRAVELER_BUY_FOOD,
	TRAVELER_BUY_TICKET,
	CLEANER_MOVE,
	CLEANER_STAIRS,
	CLEANER_ESCALATOR,
	CLEANER_CLEAN,
	AGENT_MOVE,
	AGENT_STAIRS,
	AGENT_ESCALATOR,
	AGENT_HELP,
}

public abstract class AState
{
	public AState (StateType _type)
	{
		type = _type;
	}
		
	public virtual void start(){}
	public virtual void end(){}
	public abstract void update();
	public abstract int check();
	public abstract bool canBeStopped();

	public readonly StateType type;
}
}
