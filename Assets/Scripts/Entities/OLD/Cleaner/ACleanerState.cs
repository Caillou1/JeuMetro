using System;

namespace V1
{
public abstract class ACleanerState : AState
{
	public ACleanerState (CleanerEntity _cleaner, StateType _type) : base (_type)
	{
		cleaner = _cleaner;
	}

	protected readonly CleanerEntity cleaner;
}
}
