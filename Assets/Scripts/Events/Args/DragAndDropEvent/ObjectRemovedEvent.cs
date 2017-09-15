using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectRemovedEvent : EventArgs 
{
	public ObjectRemovedEvent(Vector3 _point, TileID _type, bool _bought)
	{
        point = new Vector3i(_point);
		type = _type;
		bought = _bought;
	}

	public ObjectRemovedEvent(Vector3i _point, TileID _type, bool _bought)
	{
		point = _point;
		type = _type;
		bought = _bought;
	}

	public Vector3i point;
	public TileID type;
	public bool bought;
}
