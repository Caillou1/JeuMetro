using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class DragAndDropSpeaker : DragAndDrop {

	private GameObject WalledObject;
	private GameObject NotWalledObject;

	void Start() {
		HasToCheckWall = true;
		tf = transform;
		isRotating = false;
		WalledObject = tf.Find ("Walled").gameObject;
		NotWalledObject = tf.Find ("NotWalled").gameObject;
		CheckCanPlace ();
		CheckRotation ();
		ToggleOutline (false);
	}

	protected override void CheckCanPlace ()
	{
		canPlace = true;

		var dir = Orienter.orientationToDir3 (Orienter.angleToOrientation (tf.rotation.eulerAngles.y));

		//Case centrale
		var v = G.Sys.tilemap.at (tf.position);
		if (v.Count == 0 || v [0].type != TileID.GROUND || G.Sys.tilemap.tilesOfTypeAt(tf.position, TileID.ESCALATOR).Count > 0)
			canPlace = false;

		//Case en face
		v = G.Sys.tilemap.at (tf.position + new Vector3(-dir.z, 0, dir.x));
		if (v.Count == 0 || v [0].type != TileID.GROUND || G.Sys.tilemap.tilesOfTypeAt(tf.position + new Vector3(-dir.z, 0, dir.x), TileID.ESCALATOR).Count > 0)
			canPlace = false;
	}

	protected override void CheckRotation() {
		Orientation or = Orienter.angleToOrientation (tf.rotation.eulerAngles.y);
		List<Orientation> PossibleOrientations = new List<Orientation> ();
		var dir = Orienter.orientationToDir3 (or);

		if (G.Sys.tilemap.GetTileOfTypeAt (tf.position + Vector3.forward, TileID.WALL) != null)
			PossibleOrientations.Add (Orientation.LEFT);
		if (G.Sys.tilemap.GetTileOfTypeAt (tf.position + Vector3.back, TileID.WALL) != null)
			PossibleOrientations.Add (Orientation.RIGHT);
		if (G.Sys.tilemap.GetTileOfTypeAt (tf.position + Vector3.right, TileID.WALL) != null)
			PossibleOrientations.Add (Orientation.UP);
		if (G.Sys.tilemap.GetTileOfTypeAt (tf.position + Vector3.left, TileID.WALL) != null)
			PossibleOrientations.Add (Orientation.DOWN);

		if (PossibleOrientations.Count > 0 && (!PossibleOrientations.Contains(or) || !IsWalled))
		{
			float desiredAngle = Orienter.orientationToAngle(PossibleOrientations[0]);

			if (tf.rotation.eulerAngles.y != desiredAngle)
				RotateObject(desiredAngle);
		}

		if (G.Sys.tilemap.haveSpecialTileAt (TileID.WALL, tf.position + new Vector3 (dir.z, 0, -dir.x))) {
			if (!IsWalled || NotWalledObject.activeInHierarchy) {
				IsWalled = true;
				WalledObject.SetActive (true);
				NotWalledObject.SetActive (false);
			}
		} else {
			if (IsWalled || WalledObject.activeInHierarchy) {
				IsWalled = false;
				WalledObject.SetActive (false);
				NotWalledObject.SetActive (true);
			}
		}
	}

	protected override void SendEvent() {
		var list = new List<Vector3> ();

		list.Add (tf.position);

		Event<ObjectPlacedEvent>.Broadcast (new ObjectPlacedEvent (list));
	}
}
