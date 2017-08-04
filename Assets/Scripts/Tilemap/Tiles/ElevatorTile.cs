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
	private Transform tf;
	private float OriginFloor;
	private int CurrentFloor;
	private int[] Floors;
	private Dictionary<int, Vector3> WaitZones;
	private int peopleInElevator;

	protected override void Awake()
	{
		tf = transform;

		type = TileID.ELEVATOR;

		FloorsToVisit = new List<int> ();
		Floors = new int[tf.childCount];
		WaitZones = new Dictionary<int, Vector3> ();

		for (int i = 0; i < tf.childCount; i++) {
			var fTf = tf.GetChild (i);

			Floors [i] = Mathf.RoundToInt (fTf.position.y);

			var fDir = Orienter.orientationToDir3 (Orienter.angleToOrientation (fTf.rotation.eulerAngles.y));
			WaitZones.Add (Floors [i], fTf.position - fDir);
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

	public bool FloorExists(int f) {
		return Floors.Contains (f);
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
		if (WaitZones.ContainsKey (f))
			return WaitZones [f];
		else {
			Debug.Log ("Asked Key for " + tf.name + " elevator : " + f);
			foreach (var a in WaitZones)
				Debug.Log ("Present Key : " + a.Key);
			return Vector3.zero;
		}
	}

	IEnumerator ElevatorRoutine() {
		while (true) {
			if (FloorsToVisit.Count == 0) {
				yield return new WaitForEndOfFrame ();
			} else {
				float time = G.Sys.constants.ElevatorComeTime * Mathf.Abs (CurrentFloor - FloorsToVisit [0]) / 2f;
				CurrentFloor = int.MinValue;
				yield return new WaitForSeconds (time);

				CurrentFloor = FloorsToVisit [0];
				FloorsToVisit.RemoveAt (0);

				yield return new WaitForSeconds (G.Sys.constants.ElevatorWaitTime);
			}
		}
	}

	public List<int> GetFloors() {
		return Floors.ToList ();
	}

	public void AddPersonInElevator() {
		peopleInElevator++;
	}

	public void RemovePersonFromElevator() {
		peopleInElevator--;
	}

	public bool IsFull() {
		return peopleInElevator >= G.Sys.constants.ElevatorMaxPeople;
	}
}