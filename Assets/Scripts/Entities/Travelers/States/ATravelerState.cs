using System;

public abstract class ATravelerState : AState
{
	public ATravelerState (Traveler _traveler, StateType _type) : base (_type)
	{
		traveler = _traveler;
	}

	protected readonly Traveler traveler;
}

