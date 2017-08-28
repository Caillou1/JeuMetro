using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AI;

public class PodotactileTile : ATile
{
	private Transform tf;
	private GameObject Stop;
	private GameObject TwoBranchesStraight;
	private GameObject TwoBranchesAngle;
	private GameObject ThreeBranches;
	private GameObject FourBranches;

	private bool isStop;

	protected override void Awake()
	{
		tf = transform;

		if (tf.name == "Podotactile (16)")
			Debug.Log ("tile awake" + Time.time);

		isStop = false;

		Stop = tf.Find ("Stop").gameObject;
		TwoBranchesStraight = tf.Find ("TwoStraight").gameObject;
		TwoBranchesAngle = tf.Find ("TwoAngle").gameObject;
		ThreeBranches = tf.Find ("Three").gameObject;
		FourBranches = tf.Find ("Four").gameObject;

		type = TileID.PODOTACTILE;

		G.Sys.tilemap.addTile (tf.position, this, Tilemap.LOW_PRIORITY);

		G.Sys.tilemap.addSpecialTile (type, tf.position);

		Connect (true);
    }

	public void Connect (bool CheckArround)
	{
		if (tf.name == "Podotactile (16)")
			Debug.Log ("tile connect " + Time.time);
		
		if (!G.Sys.tilemap.HasTileOfTypeAt (TileID.CONTROLELINE, transform.position)) {
			GetComponent<NavMeshModifier> ().enabled = false;
			Event<BakeNavMeshEvent>.Broadcast (new BakeNavMeshEvent ());
		}

		Vector3i pos = new Vector3i (transform.position);
		List<ATile> list = new List<ATile> ();
		List<Orientation> neighbors = new List<Orientation> ();

		if (!isStop || true) {
			var l = G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.forward), TileID.PODOTACTILE);
			l = l.Concat (G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.forward), TileID.STAIRS))
			.Concat (G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.forward), TileID.ESCALATOR))
			.Concat (G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.forward), TileID.OUT))
			.Concat (G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.forward), TileID.METRO))
			.ToList ();
			if (l.Count > 0) {
				if (l [0].type == TileID.PODOTACTILE) {
					list.Add (l [0]);
					if (!neighbors.Contains (Orientation.UP))
						neighbors.Add (Orientation.UP);
				} else if (l [0].type == TileID.STAIRS) {
					var s = l [0] as StairsTile;

					if (s != null && (l [0] as StairsTile).IsOnStairsPath (new Vector3i (tf.position))) {
						if (!neighbors.Contains (Orientation.LEFT))
							neighbors.Add (Orientation.LEFT);
						if (!neighbors.Contains (Orientation.RIGHT))
							neighbors.Add (Orientation.RIGHT);
						isStop = true;
					}
				} else {
					if (!neighbors.Contains (Orientation.UP))
						neighbors.Add (Orientation.UP);
				}
			}

			l = G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.back), TileID.PODOTACTILE);
			l = l.Concat (G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.back), TileID.STAIRS))
			.Concat (G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.back), TileID.ESCALATOR))
			.Concat (G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.back), TileID.OUT))
			.Concat (G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.back), TileID.METRO))
			.ToList ();
			if (l.Count > 0) {
				if (l [0].type == TileID.PODOTACTILE) {
					list.Add (l [0]);
					if (!neighbors.Contains (Orientation.DOWN))
						neighbors.Add (Orientation.DOWN);
				} else if (l [0].type == TileID.STAIRS) {
					var s = l [0] as StairsTile;

					if (s != null && (l [0] as StairsTile).IsOnStairsPath (new Vector3i (tf.position))) {
						if (!neighbors.Contains (Orientation.LEFT))
							neighbors.Add (Orientation.LEFT);
						if (!neighbors.Contains (Orientation.RIGHT))
							neighbors.Add (Orientation.RIGHT);
						isStop = true;
					}
				} else {
					if (!neighbors.Contains (Orientation.DOWN))
						neighbors.Add (Orientation.DOWN);
				}
			}

			l = G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.right), TileID.PODOTACTILE);
			l = l.Concat (G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.right), TileID.STAIRS))
			.Concat (G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.right), TileID.ESCALATOR))
			.Concat (G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.right), TileID.OUT))
			.Concat (G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.right), TileID.METRO))
			.ToList ();
			if (l.Count > 0) {
				if (l [0].type == TileID.PODOTACTILE) {
					list.Add (l [0]);
					if (!neighbors.Contains (Orientation.RIGHT))
						neighbors.Add (Orientation.RIGHT);
				} else if (l [0].type == TileID.STAIRS) {
					var s = l [0] as StairsTile;

					if (s != null && (l [0] as StairsTile).IsOnStairsPath (new Vector3i (tf.position))) {
						if (!neighbors.Contains (Orientation.DOWN))
							neighbors.Add (Orientation.DOWN);
						if (!neighbors.Contains (Orientation.UP))
							neighbors.Add (Orientation.UP);
						isStop = true;
					}
				} else {
					if (!neighbors.Contains (Orientation.RIGHT))
						neighbors.Add (Orientation.RIGHT);
				}
			}

			l = G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.left), TileID.PODOTACTILE);
			l = l.Concat (G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.left), TileID.STAIRS))
			.Concat (G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.left), TileID.ESCALATOR))
			.Concat (G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.left), TileID.OUT))
			.Concat (G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.left), TileID.METRO))
			.ToList ();
			if (l.Count > 0) {
				if (l [0].type == TileID.PODOTACTILE) {
					list.Add (l [0]);
					if (!neighbors.Contains (Orientation.LEFT))
						neighbors.Add (Orientation.LEFT);
				} else if (l [0].type == TileID.STAIRS) {
					var s = l [0] as StairsTile;

					if (s != null && (l [0] as StairsTile).IsOnStairsPath (new Vector3i (tf.position))) {
						if (!neighbors.Contains (Orientation.DOWN))
							neighbors.Add (Orientation.DOWN);
						if (!neighbors.Contains (Orientation.UP))
							neighbors.Add (Orientation.UP);
						isStop = true;
					}
				} else {
					if (!neighbors.Contains (Orientation.LEFT))
						neighbors.Add (Orientation.LEFT);
				}
			}

			switch (neighbors.Count) {
			case 0:
				TwoBranchesStraight.SetActive (true);
				TwoBranchesAngle.SetActive (false);
				ThreeBranches.SetActive (false);
				FourBranches.SetActive (false);
				Stop.SetActive (false);
				break;

			case 1:
				tf.rotation = Quaternion.Euler (0, Orienter.orientationToAngle (neighbors [0]), 0);
				TwoBranchesStraight.SetActive (true);
				TwoBranchesAngle.SetActive (false);
				ThreeBranches.SetActive (false);
				FourBranches.SetActive (false);
				Stop.SetActive (false);
				break;

			case 2:
				if (Orienter.IsOppositeTo (neighbors [0], neighbors [1])) {
					tf.rotation = Quaternion.Euler (0, Orienter.orientationToAngle (neighbors [0]), 0);
					TwoBranchesStraight.SetActive (!isStop);
					TwoBranchesAngle.SetActive (false);
					ThreeBranches.SetActive (false);
					FourBranches.SetActive (false);
					Stop.SetActive (isStop);
				} else {
					if (neighbors.Contains (Orientation.UP) && neighbors.Contains (Orientation.RIGHT)) {
						tf.rotation = Quaternion.Euler (0, Orienter.orientationToAngle (Orientation.RIGHT), 0);
					} else if (neighbors.Contains (Orientation.RIGHT) && neighbors.Contains (Orientation.DOWN)) {
						tf.rotation = Quaternion.Euler (0, Orienter.orientationToAngle (Orientation.DOWN), 0);
					} else if (neighbors.Contains (Orientation.DOWN) && neighbors.Contains (Orientation.LEFT)) {
						tf.rotation = Quaternion.Euler (0, Orienter.orientationToAngle (Orientation.LEFT), 0);
					} else if (neighbors.Contains (Orientation.LEFT) && neighbors.Contains (Orientation.UP)) {
						tf.rotation = Quaternion.Euler (0, Orienter.orientationToAngle (Orientation.UP), 0);
					}

					TwoBranchesStraight.SetActive (false);
					TwoBranchesAngle.SetActive (true);
					ThreeBranches.SetActive (false);
					FourBranches.SetActive (false);
					Stop.SetActive (false);
				}
				break;

			case 3:
				if (!neighbors.Contains (Orientation.DOWN))
					tf.rotation = Quaternion.Euler (0, Orienter.orientationToAngle (Orientation.DOWN), 0);
				else if (!neighbors.Contains (Orientation.UP))
					tf.rotation = Quaternion.Euler (0, Orienter.orientationToAngle (Orientation.UP), 0);
				else if (!neighbors.Contains (Orientation.LEFT))
					tf.rotation = Quaternion.Euler (0, Orienter.orientationToAngle (Orientation.LEFT), 0);
				else if (!neighbors.Contains (Orientation.RIGHT))
					tf.rotation = Quaternion.Euler (0, Orienter.orientationToAngle (Orientation.RIGHT), 0);
			
				TwoBranchesStraight.SetActive (false);
				TwoBranchesAngle.SetActive (false);
				ThreeBranches.SetActive (!isStop);
				FourBranches.SetActive (false);
				Stop.SetActive (isStop);

				if (isStop) {
					var rot = tf.rotation.eulerAngles;
					rot.y += 90;
					tf.rotation = Quaternion.Euler (rot);
				}
				break;

			case 4:
				TwoBranchesStraight.SetActive (false);
				TwoBranchesAngle.SetActive (false);
				ThreeBranches.SetActive (false);
				FourBranches.SetActive (true);
				Stop.SetActive (false);
				break;
			}

			if (CheckArround) {
				foreach (var t in list) {
                    var tile = t as PodotactileTile;
                    if(tile != null)
                        tile.Connect (false);
				}
			}
		}
	}

	protected override void OnDestroy()
	{
		G.Sys.tilemap.delTile (tf.position, this);
		G.Sys.tilemap.delSpecialTile (TileID.PODOTACTILE, tf.position);

		Vector3i pos = new Vector3i (transform.position);
		List<ATile> list = new List<ATile> ();

		var l = G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.forward), TileID.PODOTACTILE);
		if (l.Count > 0 && l [0].type == TileID.PODOTACTILE) {
			list.Add(l[0]);
		}

		l = G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.back), TileID.PODOTACTILE);
		if (l.Count > 0 && l [0].type == TileID.PODOTACTILE) {
			list.Add(l[0]);
		}

		l = G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.right), TileID.PODOTACTILE);
		if (l.Count > 0 && l [0].type == TileID.PODOTACTILE) {
			list.Add(l[0]);
		}

		l = G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.left), TileID.PODOTACTILE);
		if (l.Count > 0 && l [0].type == TileID.PODOTACTILE) {
			list.Add(l[0]);
		}

		foreach (var t in list) {
            var tile = t as PodotactileTile;
            if(tile != null)
			    tile.Connect (false);
		}
	}
}