using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class DragAndDropDistrib : DragAndDrop {
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

		if (G.Sys.tilemap.haveSpecialTileAt (TileID.WALL, tf.position + Vector3.forward) || G.Sys.tilemap.at(tf.position + Vector3.forward).Count == 0)
			PossibleOrientations.Add (Orientation.LEFT);
		if (G.Sys.tilemap.haveSpecialTileAt (TileID.WALL, tf.position + Vector3.back) || G.Sys.tilemap.at(tf.position + Vector3.back).Count == 0)
			PossibleOrientations.Add (Orientation.RIGHT);
		if (G.Sys.tilemap.haveSpecialTileAt (TileID.WALL, tf.position + Vector3.right) || G.Sys.tilemap.at(tf.position + Vector3.right).Count == 0)
			PossibleOrientations.Add (Orientation.UP);
		if (G.Sys.tilemap.haveSpecialTileAt (TileID.WALL, tf.position + Vector3.left) || G.Sys.tilemap.at(tf.position + Vector3.left).Count == 0)
			PossibleOrientations.Add (Orientation.DOWN);

		if (PossibleOrientations.Count > 0 && (!PossibleOrientations.Contains(or)))
		{
			float desiredAngle = Orienter.orientationToAngle(PossibleOrientations[0]);

			if (tf.rotation.eulerAngles.y != desiredAngle)
				RotateObject(desiredAngle);
		}
	}

	protected override void SendEvent() {
		var list = new List<Vector3> ();

		list.Add (tf.position);

		Event<ObjectPlacedEvent>.Broadcast (new ObjectPlacedEvent (list));
	}
}
