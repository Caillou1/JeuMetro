using System;
using UnityEngine;

public enum ActionType
{
	SIGN,
}

public abstract class AAction
{
	public AAction(ActionType t, Vector3 p)
	{
		type = t;
		pos = p;
	}
	public abstract bool Exec();

	public readonly ActionType type;
	public readonly Vector3 pos;
}

public abstract class AEntityAction<T> : AAction where T : AEntity
{
	public AEntityAction(T e, ActionType t, Vector3 p) : base(t, p)
	{
		entity = e;
	}
		
	protected T entity;
}
