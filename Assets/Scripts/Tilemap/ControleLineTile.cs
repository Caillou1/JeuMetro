using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControleLineTile : ATile 
{
	protected override void Awake ()
	{
		var dir = Orienter.orientationToDir(Orienter.angleToOrientation(transform.rotation.eulerAngles.y));

		type = TileID.CONTROLELINE;
	}

	public override void Connect ()
	{
		
	}

	protected override void OnDestroy ()
	{
		
	}
}
