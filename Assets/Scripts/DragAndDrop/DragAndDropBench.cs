using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class DragAndDropBench : DragAndDrop {
	
	protected override void CheckCanPlace() {
		canPlace = true;
		Orientation or = Orienter.angleToOrientation (tf.rotation.eulerAngles.y);
		Vector3 dir = Orienter.orientationToDir3 (or);

		//Case centrale
		var v = G.Sys.tilemap.at (tf.position);
		if (v.Count == 0 || v [0].type != TileID.GROUND || G.Sys.tilemap.tilesOfTypeAt(tf.position, TileID.ESCALATOR).Count > 0)
			canPlace = false;

		//Case côté
		v = G.Sys.tilemap.at (tf.position + new Vector3(dir.x, 0, dir.z));
		if (v.Count == 0 || v [0].type != TileID.GROUND || G.Sys.tilemap.tilesOfTypeAt(tf.position + new Vector3(dir.x, 0, dir.z), TileID.ESCALATOR).Count > 0)
			canPlace = false;

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

		if(PossibleOrientations.Count > 0 && !PossibleOrientations.Contains(or)) {
			float desiredAngle = Orienter.orientationToAngle(PossibleOrientations[0]);
			if(tf.rotation.eulerAngles.y != desiredAngle)
				RotateObject (desiredAngle);
		}
	}

	protected override void SendEvent() {
		Orientation or = Orienter.angleToOrientation (tf.rotation.eulerAngles.y);
		Vector3 dir = Orienter.orientationToDir3 (or);
		var list = new List<Vector3> ();

		list.Add (tf.position);
		list.Add (tf.position + dir);

		Event<ObjectPlacedEvent>.Broadcast (new ObjectPlacedEvent (list));
	}
}
