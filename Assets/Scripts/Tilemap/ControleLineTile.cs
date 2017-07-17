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

		foreach(var t in G.Sys.tilemap.at(transform.position))
			t.Connect();
		foreach(var t in G.Sys.tilemap.at(transform.position + dir))
			t.Connect();
		foreach(var t in G.Sys.tilemap.at(transform.position - dir))
			t.Connect();
	}

	public override void Connect ()
	{
		List<Pair<ATile, Vector3i>> connexions = new List<Pair<ATile, Vector3i>>();

		var dir = Orienter.orientationToDir3(Orienter.angleToOrientation(transform.rotation.eulerAngles.y));
		var perpendicularDir = new Vector3 (-dir.z, dir.y, dir.x);
		Add (transform.position + 2 * dir, connexions);
		Add (transform.position + dir + perpendicularDir, connexions);
		Add (transform.position + dir - perpendicularDir, connexions);

		Add (transform.position - 2 * dir, connexions);
		Add (transform.position - dir + perpendicularDir, connexions);
		Add (transform.position - dir - perpendicularDir, connexions);


		applyConnexions (connexions);
	}

	protected override void OnDestroy ()
	{
		var dir = Orienter.orientationToDir3(Orienter.angleToOrientation(transform.rotation.eulerAngles.y));

		G.Sys.tilemap.delTile (transform.position, this);
		G.Sys.tilemap.delTile (transform.position + dir, this);
		G.Sys.tilemap.delTile (transform.position - dir, this);


		foreach(var t in G.Sys.tilemap.at(transform.position))
			t.Connect();
		foreach(var t in G.Sys.tilemap.at(transform.position + dir))
			t.Connect();
		foreach(var t in G.Sys.tilemap.at(transform.position - dir))
			t.Connect();

		foreach (var t in connectedTiles)
			t.First.targetOf.Remove (this);
		foreach (var t in targetOf.ToList())
			t.Connect ();
	}
}
