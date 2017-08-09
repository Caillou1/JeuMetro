using System;
using UnityEngine;

public enum ActionType
{
	SIGN,
	SIT,
	FAINT,
	THROW_IN_BIN,
	THROW_IN_GROUND,
	BUY_FOOD,
	BUY_TICKET,
	CLEAN_WASTE,
	CLEAN_BIN,
	HELP_TRAVELER,
	WAIT_ELEVATOR,
	WAIT_METRO,
	GO_TO_METRO,
}

public abstract class AAction
{
	bool started = false;
	public readonly int priority = -1;

	public AAction(ActionType t, Vector3 p, int _priority = -1)
	{
		type = t;
		pos = p;
	}

	protected virtual bool Start()
	{
		return false;
	}

	protected abstract bool Update ();
	public bool Exec()
	{
		if (!started) {
			started = true;
			if (Start ())
				return true;
		}
		bool value = Update ();
		if (value)
			End ();
		return value;
	}

	protected virtual void End()
	{

	}

	public readonly ActionType type;
	public readonly Vector3 pos;
}

public abstract class AEntityAction<T> : AAction where T : AEntity
{
	public AEntityAction(T e, ActionType t, Vector3 p, int priority = -1) : base(t, p, priority)
	{
		entity = e;
	}
		
	protected T entity;
}
