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

		G.Sys.tilemap.addTile (tf.position, this, Tilemap.LOW_PRIORITY);

		G.Sys.tilemap.addSpecialTile (type, tf.position);

		Connect ();
    }

	public void Connect ()
	{
		Vector3i pos = new Vector3i (transform.position);
		List<ATile> list = new List<ATile> ();
		List<Orientation> neighbors = new List<Orientation> ();

		var l = G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.forward), TileID.PODOTACTILE);
		l = l.Concat (G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.forward), TileID.STAIRS))
			.Concat (G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.forward), TileID.ESCALATOR))
			.Concat (G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.forward), TileID.IN))
			.Concat (G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.forward), TileID.OUT))
			.Concat (G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.forward), TileID.METRO))
			.ToList();
		if (l.Count > 0) {
			if (l [0].type == TileID.PODOTACTILE) {
				list.Add (l [0]);
				if (!neighbors.Contains (Orientation.UP))
					neighbors.Add (Orientation.UP);
			} else if ((l [0].type == TileID.ESCALATOR) || (l[0].type == TileID.STAIRS )) {
				if ((new Vector3i (l [0].transform.position)).equal (new Vector3i (tf.position + Vector3.forward * 2)) || (new Vector3i (l [0].transform.position)).equal (new Vector3i (tf.position + Vector3.forward + Vector3.down * 2))) {
					if (!neighbors.Contains (Orientation.LEFT))
						neighbors.Add (Orientation.LEFT);
					if (!neighbors.Contains (Orientation.RIGHT))
						neighbors.Add (Orientation.RIGHT);
				}
			} else {
				if (!neighbors.Contains (Orientation.UP))
					neighbors.Add (Orientation.UP);
			}
		}

		l = G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.back), TileID.PODOTACTILE);
		l = l.Concat (G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.back), TileID.STAIRS))
			.Concat (G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.back), TileID.ESCALATOR))
			.Concat (G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.back), TileID.IN))
			.Concat (G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.back), TileID.OUT))
			.Concat (G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.back), TileID.METRO))
			.ToList();
		if (l.Count > 0) {
			if (l [0].type == TileID.PODOTACTILE) {
				list.Add(l[0]);
				if(!neighbors.Contains(Orientation.DOWN))
					neighbors.Add (Orientation.DOWN);
			} else if ((l[0].type == TileID.ESCALATOR) || (l[0].type == TileID.STAIRS)) {
				if ((new Vector3i (l [0].transform.position)).equal (new Vector3i (tf.position + Vector3.back * 2)) || (new Vector3i (l [0].transform.position)).equal (new Vector3i (tf.position + Vector3.back + Vector3.down * 2))) {
					if (!neighbors.Contains (Orientation.LEFT))
						neighbors.Add (Orientation.LEFT);
					if (!neighbors.Contains (Orientation.RIGHT))
						neighbors.Add (Orientation.RIGHT);
				}
			} else {
				if (!neighbors.Contains (Orientation.DOWN))
					neighbors.Add (Orientation.DOWN);
			}
		}

		l = G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.right), TileID.PODOTACTILE);
		l = l.Concat (G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.right), TileID.STAIRS))
			.Concat (G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.right), TileID.ESCALATOR))
			.Concat (G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.right), TileID.IN))
			.Concat (G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.right), TileID.OUT))
			.Concat (G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.right), TileID.METRO))
			.ToList();
		if (l.Count > 0) {
			if (l [0].type == TileID.PODOTACTILE) {
				list.Add(l[0]);
				if(!neighbors.Contains(Orientation.RIGHT))
					neighbors.Add (Orientation.RIGHT);
			} else if ((l[0].type == TileID.ESCALATOR) || (l[0].type == TileID.STAIRS)) {
				if ((new Vector3i (l [0].transform.position)).equal (new Vector3i (tf.position + Vector3.right * 2)) || (new Vector3i (l [0].transform.position)).equal (new Vector3i (tf.position + Vector3.right + Vector3.down * 2))) {
					if (!neighbors.Contains (Orientation.DOWN))
						neighbors.Add (Orientation.DOWN);
					if (!neighbors.Contains (Orientation.UP))
						neighbors.Add (Orientation.UP);
				}
			} else {
				if (!neighbors.Contains (Orientation.RIGHT))
					neighbors.Add (Orientation.RIGHT);
			}
		}

		l = G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.left), TileID.PODOTACTILE);
		l = l.Concat (G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.left), TileID.STAIRS))
			.Concat (G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.left), TileID.ESCALATOR))
			.Concat (G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.left), TileID.IN))
			.Concat (G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.left), TileID.OUT))
			.Concat (G.Sys.tilemap.tilesOfTypeAt (pos + new Vector3i (Vector3.left), TileID.METRO))
			.ToList();
		if (l.Count > 0) {
			if (l [0].type == TileID.PODOTACTILE) {
				list.Add(l[0]);
				if(!neighbors.Contains(Orientation.LEFT))
					neighbors.Add (Orientation.LEFT);
			} else if ((l[0].type == TileID.ESCALATOR) || (l[0].type == TileID.STAIRS)) {
				if ((new Vector3i (l [0].transform.position)).equal (new Vector3i (tf.position + Vector3.left * 2)) || (new Vector3i (l [0].transform.position)).equal (new Vector3i (tf.position + Vector3.left + Vector3.down * 2))) {
					if (!neighbors.Contains (Orientation.DOWN))
						neighbors.Add (Orientation.DOWN);
					if (!neighbors.Contains (Orientation.UP))
						neighbors.Add (Orientation.UP);
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


	}

	protected override void OnDestroy()
	{
		G.Sys.tilemap.delTile (tf.position, this);
		G.Sys.tilemap.delSpecialTile (TileID.PODOTACTILE, tf.position);
	}
}