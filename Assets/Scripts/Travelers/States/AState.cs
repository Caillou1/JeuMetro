using System;

public enum StateType
{
	MOVE,
	STAIRS,
	ESCALATOR,
	LOST,
	INFOS,
	SIT,
	THROW_WASTE_BIN,
	THROW_WASTE_GROUND,
	FOOD,
	TICKET,
}

public abstract class AState
{
	public AState (Traveler _traveler, StateType _type)
	{
		traveler = _traveler;
		type = _type;
	}

	public virtual void start(){}
	public virtual void end(){}
	public abstract void update();
	public abstract int check();
	public abstract bool canBeStopped();

	protected readonly Traveler traveler;
	public readonly StateType type;
}

