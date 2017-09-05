using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WasteTile : ATile
{
	bool _bigWaste = false;
	GameObject _bigWasteObject;
    GameObject _smallWasteObject;
	bool targetted = false;

	public bool Targetted {
		get {
			return targetted;
		}
		set {
			targetted = value;
		}
	}

	public bool big
	{
		get{ return _bigWaste; }
		set {
			if (_bigWaste != value) {
				_bigWaste = value;
				_bigWasteObject.SetActive (_bigWaste);
                _smallWasteObject.SetActive(!_bigWaste);
				Event<BakeNavMeshEvent>.Broadcast (new BakeNavMeshEvent ());
			}
		}
	}

	protected override void Awake()
    {

        _smallWasteObject = transform.Find("LVL1").gameObject;
		_bigWasteObject = transform.Find ("LVL2").gameObject;
		_bigWasteObject.SetActive (false);

		type = TileID.WASTE;

		G.Sys.tilemap.addTile (transform.position, this, Tilemap.LOW_PRIORITY);

		G.Sys.tilemap.addSpecialTile (type, transform.position);
    }

	protected override void OnDestroy()
	{
		G.Sys.tilemap.delTile (transform.position, this);
		G.Sys.tilemap.delSpecialTile (type, transform.position);
		Event<BakeNavMeshEvent>.Broadcast (new BakeNavMeshEvent ());
	}
}