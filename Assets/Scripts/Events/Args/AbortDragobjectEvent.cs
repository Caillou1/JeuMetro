
using System;

public class AbortDragObjectEvent : EventArgs
{
	public AbortDragObjectEvent(TileID _type)
	{
		type = _type;
	}

	public TileID type;
}
