using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ControleLineTile : ATile 
{
	protected override void Awake ()
	{
		type = TileID.CONTROLELINE;

		G.Sys.tilemap.addTile (transform.position, this, Tilemap.CONTROLE_LINE_PRIORITY);
	}

	protected override void OnDestroy ()
	{
		G.Sys.tilemap.delTile (transform.position, this);
	}
}
