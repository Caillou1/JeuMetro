using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum Orientation
{
    UP,
    DOWN,
    LEFT,
    RIGHT
}

public static class Orienter
{
    public static Orientation angleToOrientation(float angle)
    {
        angle %= 360.0f;
		if (angle < 0)
			angle += 360.0f;
		
        if (angle < 45.0f || angle > 315.0f)
            return Orientation.RIGHT;
        if (angle < 135.0f)
            return Orientation.DOWN;
        if (angle < 225.0f)
            return Orientation.LEFT;
        return Orientation.UP;
    }

    public static float orientationToAngle(Orientation o)
    {
        switch(o)
        {
            case Orientation.RIGHT:
                return 0.0f;
            case Orientation.DOWN:
                return 90.0f;
            case Orientation.LEFT:
                return 180.0f;
            default:
                return 270.0f;
        }
    }

    public static Vector2 orientationToDir(Orientation o)
    {
        switch(o)
        {
            case Orientation.RIGHT:
                return new Vector2(1, 0);
            case Orientation.DOWN:
                return new Vector2(0, -1);
            case Orientation.LEFT:
                return new Vector2(-1, 0);
            default:
                return new Vector2(0, 1);
        }
    }

	public static Vector3 orientationToDir3(Orientation o)
	{
		Vector2 v = orientationToDir (o);
		return new Vector3 (v.x, 0, v.y);
	}

	public static Vector3 getSideVector3(Vector3 v)
	{
		return new Vector3 (v.z, 0, v.x);
	}
}




