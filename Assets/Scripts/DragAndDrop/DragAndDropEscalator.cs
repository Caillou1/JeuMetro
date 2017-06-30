using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class DragAndDropEscalator : DragAndDrop {
	
	protected override void CheckCanPlace() {
		Orientation or = Orienter.angleToOrientation (tf.rotation.eulerAngles.y);

		canPlace = true;
		Vector3 dir = Orienter.orientationToDir3 (or);

		//Pivot
		var v = G.Sys.tilemap.at (tf.position);
		if (v.Count == 0 || v [0].type != TileID.GROUND || G.Sys.tilemap.tilesOfTypeAt (tf.position, TileID.ESCALATOR).Count > 0 || G.Sys.tilemap.at (tf.position + Vector3.up * 2).Count > 0) {
			canPlace = false;
		}

		//Case  plus basse
		v = G.Sys.tilemap.at (tf.position + dir);
		if (v.Count == 0 || v [0].type != TileID.GROUND || G.Sys.tilemap.tilesOfTypeAt (tf.position + dir, TileID.ESCALATOR).Count > 0 || G.Sys.tilemap.at (tf.position + dir + Vector3.up * 2).Count > 0) {
			canPlace = false;
		}

		//Pied escalator
		v = G.Sys.tilemap.at (tf.position + 2 * dir);
		if (v.Count == 0 || v [0].type != TileID.GROUND || G.Sys.tilemap.at (tf.position + 2 * dir + Vector3.up * 2).Count > 0) {
			canPlace = false;
		}

		//Haut escalator
		v = G.Sys.tilemap.at (tf.position - dir + Vector3.up * 2);
		if (v.Count == 0 || v [0].type != TileID.GROUND) {
			canPlace = false;
		}
	}

	protected override void CheckRotation() {
		List<Orientation> PossibleOrientations = new List<Orientation> ();

		var v = G.Sys.tilemap.at (tf.position + new Vector3(0, 2, 1));
		if (v.Count > 0 && v [0].type == TileID.GROUND)
			PossibleOrientations.Add (Orientation.DOWN);

		v = G.Sys.tilemap.at (tf.position + new Vector3(0, 2, -1));
		if (v.Count > 0 && v [0].type == TileID.GROUND)
			PossibleOrientations.Add (Orientation.UP);

		v = G.Sys.tilemap.at (tf.position + new Vector3(-1, 2, 0));
		if (v.Count > 0 && v [0].type == TileID.GROUND)
			PossibleOrientations.Add (Orientation.RIGHT);

		v = G.Sys.tilemap.at (tf.position + new Vector3(1, 2, 0));
		if (v.Count > 0 && v [0].type == TileID.GROUND)
			PossibleOrientations.Add (Orientation.LEFT);

		if(PossibleOrientations.Count > 0 && !PossibleOrientations.Contains(Orienter.angleToOrientation(tf.rotation.eulerAngles.y))) {
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
		list.Add (tf.position + 2 * dir);
		list.Add (tf.position - dir + Vector3.up * 2);

		Event<ObjectPlacedEvent>.Broadcast (new ObjectPlacedEvent (list));
	}
}
