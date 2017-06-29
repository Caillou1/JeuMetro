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

	protected override void Awake()
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
		l = l.Concat (G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.forward), TileID.STAIRS)).Concat (G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.forward), TileID.ESCALATOR)).ToList();
		if (l.Count > 0) {
			if (l [0].type == TileID.PODOTACTILE) {
				list.Add(l[0]);
				if(!neighbors.Contains(Orientation.UP))
					neighbors.Add (Orientation.UP);
			} else {
				if(!neighbors.Contains(Orientation.LEFT))
					neighbors.Add (Orientation.LEFT);
				if(!neighbors.Contains(Orientation.RIGHT))
					neighbors.Add (Orientation.RIGHT);
			}
		}

		l = G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.back), TileID.PODOTACTILE);
		l = l.Concat (G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.back), TileID.STAIRS)).Concat (G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.back), TileID.ESCALATOR)).ToList();
		if (l.Count > 0) {
			if (l [0].type == TileID.PODOTACTILE) {
				list.Add(l[0]);
				if(!neighbors.Contains(Orientation.DOWN))
					neighbors.Add (Orientation.DOWN);
			} else {
				if(!neighbors.Contains(Orientation.LEFT))
					neighbors.Add (Orientation.LEFT);
				if(!neighbors.Contains(Orientation.RIGHT))
					neighbors.Add (Orientation.RIGHT);
			}
		}

		l = G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.right), TileID.PODOTACTILE);
		l = l.Concat (G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.right), TileID.STAIRS)).Concat (G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.right), TileID.ESCALATOR)).ToList();
		if (l.Count > 0) {
			if (l [0].type == TileID.PODOTACTILE) {
				list.Add(l[0]);
				if(!neighbors.Contains(Orientation.RIGHT))
					neighbors.Add (Orientation.RIGHT);
			} else {
				if(!neighbors.Contains(Orientation.DOWN))
					neighbors.Add (Orientation.DOWN);
				if(!neighbors.Contains(Orientation.UP))
					neighbors.Add (Orientation.UP);
			}
		}

		l = G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.left), TileID.PODOTACTILE);
		l = l.Concat (G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.left), TileID.STAIRS)).Concat (G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.left), TileID.ESCALATOR)).ToList();
		if (l.Count > 0) {
			if (l [0].type == TileID.PODOTACTILE) {
				list.Add(l[0]);
				if(!neighbors.Contains(Orientation.LEFT))
					neighbors.Add (Orientation.LEFT);
			} else {
				if(!neighbors.Contains(Orientation.DOWN))
					neighbors.Add (Orientation.DOWN);
				if(!neighbors.Contains(Orientation.UP))
					neighbors.Add (Orientation.UP);
			}
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

		List<Pair<ATile,Vector3i>> list2 = new List<Pair<ATile, Vector3i>> ();
		foreach (var it in list)
			list2.Add (new Pair<ATile, Vector3i> (it, new Vector3i (it.transform.position)));
		applyConnexions (list2);
	}

	protected override void OnDestroy()
	{
		G.Sys.tilemap.delTile (tf.position, this);
		G.Sys.tilemap.delSpecialTile (TileID.PODOTACTILE, tf.position);
		foreach (var t in G.Sys.tilemap.at(transform.position))
			t.Connect ();
		foreach (var t in connectedTiles)
			t.First.targetOf.Remove (this);
		foreach (var t in targetOf.ToList())
			t.Connect ();
	}
}