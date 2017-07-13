using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class DragAndDropBin : DragAndDrop {

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
		//IsBought = true;
	}

	protected override void CheckRotation() {
		Orientation or = Orienter.angleToOrientation (tf.rotation.eulerAngles.y);
		List<Orientation> PossibleOrientations = new List<Orientation> ();

		if (G.Sys.tilemap.haveSpecialTileAt (TileID.WALL, tf.position + Vector3.forward))
			PossibleOrientations.Add (Orientation.LEFT);
		if (G.Sys.tilemap.haveSpecialTileAt (TileID.WALL, tf.position + Vector3.back))
			PossibleOrientations.Add (Orientation.RIGHT);
		if (G.Sys.tilemap.haveSpecialTileAt (TileID.WALL, tf.position + Vector3.right))
			PossibleOrientations.Add (Orientation.UP);
		if (G.Sys.tilemap.haveSpecialTileAt (TileID.WALL, tf.position + Vector3.left))
			PossibleOrientations.Add (Orientation.DOWN);

		if (PossibleOrientations.Count > 0 && (!PossibleOrientations.Contains(or) || !IsWalled))
		{
			float desiredAngle = Orienter.orientationToAngle(PossibleOrientations[0]);

			if (!IsWalled || NotWalledObject.activeInHierarchy)
			{
				IsWalled = true;
				WalledObject.SetActive (true);
				NotWalledObject.SetActive (false);
			}
			if (tf.rotation.eulerAngles.y != desiredAngle)
				RotateObject(desiredAngle);
		}
		else if (PossibleOrientations.Count == 0)
		{
			if (IsWalled || WalledObject.activeInHierarchy)
			{
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
