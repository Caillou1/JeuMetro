using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PodotactileTile : ATile
{
	private Transform tf;
	private GameObject TwoBranchesStraight;
	private GameObject TwoBranchesAngle;
	private GameObject ThreeBranches;
	private GameObject FourBranches;

	void Awake()
	{
		tf = transform;

		TwoBranchesStraight = tf.Find ("TwoStraight").gameObject;
		TwoBranchesAngle = tf.Find ("TwoAngle").gameObject;
		ThreeBranches = tf.Find ("Three").gameObject;
		FourBranches = tf.Find ("Four").gameObject;

		type = TileID.PODOTACTILE;

		G.Sys.tilemap.addTile (tf.position, this, true, false, Tilemap.LOW_PRIORITY);

		foreach (var t in G.Sys.tilemap.at(tf.position))
			t.Connect ();

		G.Sys.tilemap.addSpecialTile (type, tf.position);
    }

	public override void Connect ()
	{
		Vector3i pos = new Vector3i (transform.position);
		List<ATile> list = new List<ATile> ();
		List<Orientation> neighbors = new List<Orientation> ();

		var l = G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.forward), TileID.PODOTACTILE);
		if (l.Count > 0) {
			Add (l [0], list);
			neighbors.Add (Orientation.UP);
		}

		l = G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.back), TileID.PODOTACTILE);
		if (l.Count > 0) {
			Add (l [0], list);
			neighbors.Add (Orientation.DOWN);
		}

		l = G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.right), TileID.PODOTACTILE);
		if (l.Count > 0) {
			Add (l [0], list);
			neighbors.Add (Orientation.RIGHT);
		}

		l = G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.left), TileID.PODOTACTILE);
		if (l.Count > 0) {
			Add (l [0], list);
			neighbors.Add (Orientation.LEFT);
		}

		switch (neighbors.Count) {
		case 0:
			TwoBranchesStraight.SetActive (true);
			TwoBranchesAngle.SetActive (false);
			ThreeBranches.SetActive (false);
			FourBranches.SetActive (false);
			break;

		case 1:
			tf.rotation = Quaternion.Euler(0, Orienter.orientationToAngle(neighbors[0]), 0);
			TwoBranchesStraight.SetActive (true);
			TwoBranchesAngle.SetActive (false);
			ThreeBranches.SetActive (false);
			FourBranches.SetActive (false);
			break;

		case 2:
			if (Orienter.IsOppositeTo (neighbors [0], neighbors [1])) {
				tf.rotation = Quaternion.Euler (0, Orienter.orientationToAngle (neighbors [0]), 0);
				TwoBranchesStraight.SetActive (true);
				TwoBranchesAngle.SetActive (false);
				ThreeBranches.SetActive (false);
				FourBranches.SetActive (false);
			} else {
				if (neighbors.Contains (Orientation.UP) && neighbors.Contains (Orientation.RIGHT)) {
					tf.rotation = Quaternion.Euler (0, Orienter.orientationToAngle(Orientation.RIGHT), 0);
				} else if (neighbors.Contains (Orientation.RIGHT) && neighbors.Contains (Orientation.DOWN)) {
					tf.rotation = Quaternion.Euler (0, Orienter.orientationToAngle(Orientation.DOWN), 0);
				} else if (neighbors.Contains (Orientation.DOWN) && neighbors.Contains (Orientation.LEFT)) {
					tf.rotation = Quaternion.Euler (0, Orienter.orientationToAngle(Orientation.LEFT), 0);
				} else if (neighbors.Contains (Orientation.LEFT) && neighbors.Contains (Orientation.UP)) {
					tf.rotation = Quaternion.Euler (0, Orienter.orientationToAngle(Orientation.UP), 0);
				}

				TwoBranchesStraight.SetActive (false);
				TwoBranchesAngle.SetActive (true);
				ThreeBranches.SetActive (false);
				FourBranches.SetActive (false);
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
			ThreeBranches.SetActive (true);
			FourBranches.SetActive (false);
			break;

		case 4:
			TwoBranchesStraight.SetActive (false);
			TwoBranchesAngle.SetActive (false);
			ThreeBranches.SetActive (false);
			FourBranches.SetActive (true);
			break;
		}

		applyConnexions (list);
	}

	void OnDestroy()
	{
		G.Sys.tilemap.delTile (tf.position, this);
		foreach (var t in G.Sys.tilemap.at(transform.position))
			t.Connect ();
		foreach (var t in connectedTiles)
			t.targetOf.Remove (this);
		foreach (var t in targetOf.ToList())
			t.Connect ();
	}
}