using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class DragAndDropBin : DragAndDrop {

	private GameObject NotWalledObject;

	void Start() {
		HasToCheckWall = true;
		tf = transform;
		isRotating = false;
		NotWalledObject = tf.Find ("NotWalled").gameObject;
		CheckCanPlace ();
		CheckRotation ();
		if(bought)
			ToggleOutline (false);
		//IsBought = true;
	}

	protected override void OnAwake ()
	{
		Space = 1;
	}

	protected override void CheckRotation() {
		Orientation or = Orienter.angleToOrientation (tf.rotation.eulerAngles.y);
		List<Pair<Orientation, bool>> PossibleOrientations = new List<Pair<Orientation, bool>> ();

		if (G.Sys.tilemap.GetTileOfTypeAt (tf.position + Vector3.forward, TileID.WALL) != null) {
			PossibleOrientations.Add (new Pair<Orientation, bool>(Orientation.LEFT, true));
		} else if(G.Sys.tilemap.HasEmptyWallAt (tf.position + Vector3.forward)) {
			PossibleOrientations.Add (new Pair<Orientation, bool>(Orientation.LEFT, false));
		}

		if (G.Sys.tilemap.GetTileOfTypeAt (tf.position + Vector3.back, TileID.WALL) != null) {
			PossibleOrientations.Add (new Pair<Orientation, bool>(Orientation.RIGHT, true));
		} else if (G.Sys.tilemap.HasEmptyWallAt (tf.position + Vector3.back)) {
			PossibleOrientations.Add (new Pair<Orientation, bool>(Orientation.RIGHT, false));
		}

		if (G.Sys.tilemap.GetTileOfTypeAt (tf.position + Vector3.right, TileID.WALL) != null) {
			PossibleOrientations.Add (new Pair<Orientation, bool>(Orientation.UP, true));
		} else if(G.Sys.tilemap.HasEmptyWallAt (tf.position + Vector3.right)) {
			PossibleOrientations.Add (new Pair<Orientation, bool>(Orientation.UP, false));
		}

		if (G.Sys.tilemap.GetTileOfTypeAt (tf.position + Vector3.left, TileID.WALL) != null) {
			PossibleOrientations.Add (new Pair<Orientation, bool>(Orientation.DOWN, true));
		} else if(G.Sys.tilemap.HasEmptyWallAt (tf.position + Vector3.left)) {
			PossibleOrientations.Add (new Pair<Orientation, bool>(Orientation.DOWN, false));
		}

		if (PossibleOrientations.Count > 0 && !PossibleOrientations.Exists(x => { return x.First == or; }))
		{
			float desiredAngle = Orienter.orientationToAngle(PossibleOrientations[0].First);

			if (tf.rotation.eulerAngles.y != desiredAngle)
				RotateObject(desiredAngle);
		}
	}

	protected override void SendEvent() {
		var list = new List<Vector3> ();

		list.Add (tf.position);

		Event<ObjectPlacedEvent>.Broadcast (new ObjectPlacedEvent (list));
	}

	protected override void OnBuy ()
	{
		G.Sys.AddDisposable (TileID.BIN);
	}
}
