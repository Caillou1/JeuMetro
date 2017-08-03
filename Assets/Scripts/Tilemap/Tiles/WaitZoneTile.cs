using System;


public class WaitZoneTile : ATile
{
	public string exitname;
	public MetroTile metro;

	protected override void Awake()
	{
		type = TileID.WAIT_ZONE;

		G.Sys.tilemap.addTile (transform.position, this, Tilemap.EXITS_PRIORITY);
		G.Sys.tilemap.addSpecialTile (type, transform.position);

		if (string.IsNullOrEmpty (exitname)) {
			var comp = OutMetroName.findInParent (transform);
			if (comp != null)
				exitname = comp.exitName;
		}
	}

	protected override void OnDestroy()
	{
		G.Sys.tilemap.delTile (transform.position, this);
		G.Sys.tilemap.delSpecialTile (type, transform.position);
	}
}

