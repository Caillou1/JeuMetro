using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ControleLineTile : ATile 
{
	protected override void Awake ()
	{
		var dir = Orienter.orientationToDir3(Orienter.angleToOrientation(transform.rotation.eulerAngles.y));

		type = TileID.CONTROLELINE;

		G.Sys.tilemap.addTile (transform.position, this, false, true, Tilemap.CONTROLE_LINE_PRIORITY);
		G.Sys.tilemap.addTile (transform.position + dir, this, true, false, Tilemap.CONTROLE_LINE_PRIORITY);
		G.Sys.tilemap.addTile (transform.position - dir, this, true, false, Tilemap.CONTROLE_LINE_PRIORITY);
	}

	protected override void OnDestroy ()
	{
		var dir = Orienter.orientationToDir3(Orienter.angleToOrientation(transform.rotation.eulerAngles.y));

		G.Sys.tilemap.delTile (transform.position, this);
		G.Sys.tilemap.delTile (transform.position + dir, this);
		G.Sys.tilemap.delTile (transform.position - dir, this);
	}
}
