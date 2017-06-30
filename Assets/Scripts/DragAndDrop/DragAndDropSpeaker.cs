﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class DragAndDropSpeaker : DragAndDrop {

	private bool IsWalled;

	protected override void CheckRotation() {
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
	}

	protected override void SendEvent() {
		var list = new List<Vector3> ();

		list.Add (tf.position);

		Event<ObjectPlacedEvent>.Broadcast (new ObjectPlacedEvent (list));
	}
}