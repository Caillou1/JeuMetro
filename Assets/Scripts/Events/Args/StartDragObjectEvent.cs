
using System;

public class StartDragObjectEvent : EventArgs
{
    public StartDragObjectEvent(TileID _type)
    {
        type = _type;
    }

    public TileID type;
}
