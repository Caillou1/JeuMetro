﻿using System;

public enum StateType
{
	TRAVELER_MOVE,
	TRAVELER_STAIRS,
	TRAVELER_ESCALATOR,
	TRAVELER_LOST,
	TRAVELER_INFOS,
	TRAVELER_SIT,
	TRAVELER_THROW_WASTE_BIN,
	TRAVELER_THROW_WASTE_GROUND,
	TRAVELER_FOOD,
	TRAVELER_TICKET,
	TRAVELER_BUY_FOOD,
	CLEANER_MOVE,
	CLEANER_STAIRS,
	CLEANER_ESCALATOR,
	CLEANER_CLEAN,
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