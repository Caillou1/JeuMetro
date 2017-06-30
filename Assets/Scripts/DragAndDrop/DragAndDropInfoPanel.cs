using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class DragAndDropInfoPanel : DragAndDrop {

	private bool IsWalled;

	protected override void CheckRotation() {
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;

		Orientation or = Orienter.angleToOrientation (tf.rotation.eulerAngles.y);
		List<Orientation> PossibleOrientations = new List<Orientation> ();

		if (G.Sys.tilemap.at (tf.position + Vector3.forward).Count == 0)
			PossibleOrientations.Add (Orientation.LEFT);
		if (G.Sys.tilemap.at (tf.position + Vector3.back).Count == 0)
			PossibleOrientations.Add (Orientation.RIGHT);
		if (G.Sys.tilemap.at (tf.position + Vector3.right).Count == 0)
			PossibleOrientations.Add (Orientation.UP);
		if (G.Sys.tilemap.at (tf.position + Vector3.left).Count == 0)
			PossibleOrientations.Add (Orientation.DOWN);

		if (PossibleOrientations.Count > 0 && (!PossibleOrientations.Contains(or) || !IsWalled))
		{
			float desiredAngle = Orienter.orientationToAngle(PossibleOrientations[0]);

			if (!IsWalled)
			{
				IsWalled = true;
			}
			if (tf.rotation.eulerAngles.y != desiredAngle)
				RotateObject(desiredAngle);
		}
		else if (PossibleOrientations.Count == 0)
		{
			if (IsWalled)
			{
				IsWalled = false;
			}
		}

		if (Physics.Raycast (ray, out hit)) {
			if (hit.transform.CompareTag ("Ground")) {
				Vector3 objPos = hit.transform.position;
				tf.position = new Vector3 (Mathf.RoundToInt (objPos.x), Mathf.RoundToInt (objPos.y), Mathf.RoundToInt (objPos.z));
			}
		} else {
			Vector3 pos = ray.origin + (ray.direction * 1000);
			tf.position = new Vector3 (Mathf.RoundToInt (pos.x), Mathf.RoundToInt (pos.y), Mathf.RoundToInt (pos.z));
		}
	}

	protected override void SendEvent() {
		var list = new List<Vector3> ();

		list.Add (tf.position);

		Event<ObjectPlacedEvent>.Broadcast (new ObjectPlacedEvent (list));
	}
}
