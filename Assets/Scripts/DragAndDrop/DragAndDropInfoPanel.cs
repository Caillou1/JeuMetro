using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class DragAndDropInfoPanel : DragAndDrop {

	private GameObject WalledObject;
	private GameObject NotWalledObject;

	protected override void OnAwake ()
	{
		Space = 1;
	}

	void Start() {
		HasToCheckWall = true;
		tf = transform;
		isRotating = false;
		CheckCanPlace ();
		CheckRotation ();
		if(bought)
			ToggleOutline (false);
	}

	protected override void CheckRotation() {
		Orientation or = Orienter.angleToOrientation (tf.rotation.eulerAngles.y);
		List<Orientation> PossibleOrientations = new List<Orientation> ();

		if (G.Sys.tilemap.at (tf.position + Vector3.forward).Count == 0) {
			PossibleOrientations.Add (Orientation.LEFT);
		}

		if (G.Sys.tilemap.at (tf.position + Vector3.back).Count == 0) {
			PossibleOrientations.Add (Orientation.RIGHT);
		}

		if (G.Sys.tilemap.at (tf.position + Vector3.right).Count == 0) {
			PossibleOrientations.Add (Orientation.UP);
		}

		if (G.Sys.tilemap.at (tf.position + Vector3.left).Count == 0) {
			PossibleOrientations.Add (Orientation.DOWN);
		}

		if (PossibleOrientations.Count > 0 && !PossibleOrientations.Contains(or))
		{
			float desiredAngle = Orienter.orientationToAngle(PossibleOrientations[0]);

			if (tf.rotation.eulerAngles.y != desiredAngle)
				RotateObject(desiredAngle);
		}
	}

	protected override void SendEvent(bool wasBought) {
		var list = new List<Vector3> ();

		list.Add (tf.position);

		var tile = tf.GetComponent<ATile>();
		if (tile != null)
            Event<ObjectPlacedEvent>.Broadcast(new ObjectPlacedEvent(list, tile.type, wasBought));
	}

	protected override void OnBuy ()
	{
		G.Sys.AddDisposable (TileID.INFOPANEL);
	}
}
