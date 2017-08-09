using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SpeakerTile : ATile
{
	protected override void Awake()
	{
		type = TileID.SPEAKER;

		G.Sys.tilemap.addTile (transform.position, this, Tilemap.SPEAKER_PRIORITY);

		G.Sys.tilemap.addSpecialTile (type, transform.position);
    }

	protected override void OnDestroy()
	{
		G.Sys.tilemap.delTile (transform.position, this);

		G.Sys.tilemap.delSpecialTile (type, transform.position);
	}
		
	public static bool isEmiting()
	{
		if (G.Sys.constants == null)
			return false;
		var t = Time.time % G.Sys.constants.SpeakerWaitTime;
		return t < G.Sys.constants.SpeakerEmissionTime;
	}
}