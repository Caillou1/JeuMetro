using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class DragAndDropDistrib : DragAndDrop {
	public bool isTicket;

	protected override void OnAwake ()
	{
		Space = 1;
	}

	protected override void CheckCanPlace ()
	{
		canPlace = true;

		var dir = Orienter.orientationToDir3 (Orienter.angleToOrientation (tf.rotation.eulerAngles.y));

		//Case centrale
		var v = G.Sys.tilemap.at (tf.position);
		if (v.Count == 0 || (v [0].type != TileID.GROUND && !HasTileOfType(v, TileID.WAIT_ZONE)) || G.Sys.tilemap.tilesOfTypeAt(tf.position, TileID.ESCALATOR).Count > 0)
			canPlace = false;

		//Case en face
		v = G.Sys.tilemap.at (tf.position + new Vector3(-dir.z, 0, dir.x));
		if (v.Count == 0 || (v [0].type != TileID.GROUND && !HasTileOfType(v, TileID.WAIT_ZONE)) || G.Sys.tilemap.tilesOfTypeAt(tf.position + new Vector3(-dir.z, 0, dir.x), TileID.ESCALATOR).Count > 0)
			canPlace = false;
	}

	protected override void CheckRotation() {
		Orientation or = Orienter.angleToOrientation (tf.rotation.eulerAngles.y);
		List<Orientation> PossibleOrientations = new List<Orientation> ();

		if (G.Sys.tilemap.GetTileOfTypeAt (tf.position + Vector3.forward, TileID.WALL) != null || G.Sys.tilemap.IsEmpty(tf.position + Vector3.forward))
			PossibleOrientations.Add (Orientation.LEFT);
		if (G.Sys.tilemap.GetTileOfTypeAt (tf.position + Vector3.back, TileID.WALL) != null || G.Sys.tilemap.IsEmpty(tf.position + Vector3.back))
			PossibleOrientations.Add (Orientation.RIGHT);
		if (G.Sys.tilemap.GetTileOfTypeAt (tf.position + Vector3.right, TileID.WALL) != null || G.Sys.tilemap.IsEmpty(tf.position + Vector3.right))
			PossibleOrientations.Add (Orientation.UP);
		if (G.Sys.tilemap.GetTileOfTypeAt (tf.position + Vector3.left, TileID.WALL) != null || G.Sys.tilemap.IsEmpty(tf.position + Vector3.left))
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
        var tile = tf.GetComponent<ATile>();
        if(tile != null)
           Event<ObjectPlacedEvent>.Broadcast (new ObjectPlacedEvent (list, tile.type));
	}

	protected override void OnBuy ()
	{
		G.Sys.AddDisposable (isTicket ? TileID.TICKETDISTRIBUTEUR : TileID.FOODDISTRIBUTEUR);
	}
}
