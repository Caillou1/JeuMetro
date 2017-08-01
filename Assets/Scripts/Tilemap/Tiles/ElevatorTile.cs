using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using DG.Tweening;

public class ElevatorTile : ATile
{
	private List<int> FloorsToVisit;
	private bool isMoving = false;
	private Transform tf;
	private float OriginFloor;
	private int CurrentFloor;
	private int[] Floors;

	protected override void Awake()
	{
		tf = transform;

		type = TileID.ELEVATOR;

		FloorsToVisit = new List<int> ();
		Floors = new int[tf.childCount];

		for (int i = 0; i < tf.childCount; i++) {
			Floors [i] = (int)tf.GetChild (i).position.y;
		}

		var dir = Orienter.orientationToDir3 (Orienter.angleToOrientation (tf.rotation.eulerAngles.y + -90f));
		var pos = new Vector3 (tf.position.x, 0, tf.position.z);

		foreach (var f in Floors) {
			G.Sys.tilemap.addTile (pos + new Vector3(0, f, 0), this, Tilemap.ELEVATOR_PRIORITY);
			G.Sys.tilemap.addTile (pos + dir + new Vector3(0, f, 0), this, Tilemap.ELEVATOR_PRIORITY);

			G.Sys.tilemap.addSpecialTile (TileID.ELEVATOR, pos + dir + new Vector3 (0, f, 0));
			G.Sys.tilemap.addSpecialTile (TileID.ELEVATOR, pos + new Vector3 (0, f, 0));
		}

		StartCoroutine (ElevatorRoutine ());
    }

	protected override void OnDestroy()
	{
		var dir = Orienter.orientationToDir3 (Orienter.angleToOrientation (tf.rotation.eulerAngles.y + -90f));
		var pos = new Vector3 (tf.position.x, 0, tf.position.z);

		foreach (var f in Floors) {
			G.Sys.tilemap.delTile (pos + new Vector3 (0, f, 0), this);
			G.Sys.tilemap.delTile (pos + dir + new Vector3(0, f, 0), this);

			G.Sys.tilemap.delSpecialTile (TileID.ELEVATOR, pos + dir + new Vector3 (0, f, 0));
			G.Sys.tilemap.delSpecialTile (TileID.ELEVATOR, pos + new Vector3 (0, f, 0));
		}

		StopAllCoroutines ();
	}

	public void CallElevator(int Floor) {
		if (CurrentFloor != Floor) {
			if (!FloorsToVisit.Contains (Floor)) {
				FloorsToVisit.Add (Floor);
			}
		}
	}

	public bool IsOnFloor(int f) {
		return CurrentFloor == f;
	}

	public Vector3 GetWaitZone(int f) {
		var dir = Orienter.orientationToDir3 (Orienter.angleToOrientation (tf.rotation.eulerAngles.y));
		var pos = new Vector3 (tf.position.x, 0, tf.position.z);

		return pos - dir + Vector3.up * f;
	}

	IEnumerator ElevatorRoutine() {
		while (true) {
			if (FloorsToVisit.Count == 0) {
				yield return new WaitForEndOfFrame ();
			} else {
				yield return new WaitForSeconds (G.Sys.constants.ElevatorComeTime * Mathf.Abs (CurrentFloor - FloorsToVisit [0]) / 2f);
				CurrentFloor = FloorsToVisit [0];
				FloorsToVisit.RemoveAt (0);
				Debug.Log ("Elevator arrived at floor " + CurrentFloor);
				yield return new WaitForSeconds (G.Sys.constants.ElevatorWaitTime);
			}
		}
	}
}