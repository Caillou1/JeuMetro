﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Vector3i
{
    public int x;
    public int y;
    public int z;

    public Vector3i(int _x, int _y, int _z)
    {
        x = _x;
        y = _y;
        z = _z;
    }

    public Vector3i(Vector3 vect)
    {
        x = (int)Mathf.Round(vect.x);
        y = (int)Mathf.Round(vect.y);
        z = (int)Mathf.Round(vect.z);
    }

    public Vector3 toVector3()
    {
        return new Vector3(x, y, z);
    }

    public static Vector3i operator+(Vector3i a, Vector3i b)
    {
        return new Vector3i(a.x + b.x, a.y + b.y, a.z + b.z);
    }
}