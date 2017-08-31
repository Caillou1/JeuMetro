using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using DG.Tweening;

public class ElevatorTile : ATile
{
    float closedOffset = 69;
    float openedOffset = 142;
    float openedoffset2 = 65;
    float openTime = 0.6f;

	private List<int> FloorsToVisit;
	private Transform tf;
	private int CurrentFloor;
	private int[] Floors;
    private Dictionary<int, Vector3> WaitZones;
    private Dictionary<int, Pair<Transform, Transform>> doors = new Dictionary<int, Pair<Transform, Transform>>();
	private int peopleInElevator;
    private List<Pair<GameObject, int>> travelers = new List<Pair<GameObject, int>>();
    public int peopleWaiting = 0;

	protected override void Awake()
	{
		tf = transform;

		type = TileID.ELEVATOR;

		FloorsToVisit = new List<int> ();

        List<Transform> validFloors = new List<Transform>();
        for (int i = 0; i < tf.childCount; i++)
        {
			var fTf = tf.GetChild(i);
			if (!fTf.gameObject.name.StartsWith("Floor", StringComparison.Ordinal))
				continue;
            validFloors.Add(fTf);
        }

		Floors = new int[validFloors.Count()];
		WaitZones = new Dictionary<int, Vector3> ();

        for (int i = 0; i < validFloors.Count(); i++)
        {
            var fTf = validFloors[i];

			Floors [i] = Mathf.RoundToInt (fTf.position.y);

            var fDir = Orienter.orientationToDir3(Orienter.angleToOrientation(fTf.rotation.eulerAngles.y+90)) * 1.5f;
			WaitZones.Add (Floors [i], fTf.position - fDir);
            doors.Add(Floors[i], new Pair<Transform, Transform>(fTf.Find("porte_01"), fTf.Find("porte_02")));
		}

		var dir = Orienter.orientationToDir3 (Orienter.angleToOrientation (tf.rotation.eulerAngles.y + -90f));
		var pos = new Vector3 (tf.position.x, 0, tf.position.z);

		foreach (var f in Floors) {
			G.Sys.tilemap.addTile (pos + new Vector3(0, f, 0), this, Tilemap.ELEVATOR_PRIORITY);
			G.Sys.tilemap.addTile (pos + dir + new Vector3(0, f, 0), this, Tilemap.ELEVATOR_PRIORITY);

			G.Sys.tilemap.addSpecialTile (TileID.ELEVATOR, pos + dir + new Vector3 (0, f, 0));
			G.Sys.tilemap.addSpecialTile (TileID.ELEVATOR, pos + new Vector3 (0, f, 0));
		}

        CurrentFloor = WaitZones.First().Key;

        openDoors(CurrentFloor);

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
                closeDoors(CurrentFloor);
				float time = G.Sys.constants.ElevatorComeTime * Mathf.Abs (CurrentFloor - FloorsToVisit [0]) / 2f;
				CurrentFloor = int.MinValue;
				yield return new WaitForSeconds (time);

				CurrentFloor = FloorsToVisit [0];
				FloorsToVisit.RemoveAt (0);
                sendTravelersToTarget();
                openDoors(CurrentFloor);

				yield return new WaitForSeconds (G.Sys.constants.ElevatorWaitTime);
			}
		}
	}

    void openDoors(int floor)
    {
        if (!doors.ContainsKey(floor))
            return;
        var d = doors[floor];
        d.First.DOLocalMoveX(openedOffset, openTime);
        d.Second.DOLocalMoveX(openedoffset2, openTime);
    }

    void closeDoors(int floor)
    {
		if (!doors.ContainsKey(floor))
			return;
		var d = doors[floor];
        d.First.DOLocalMoveX(closedOffset, openTime);
        d.Second.DOLocalMoveX(-closedOffset, openTime);
    }

    void sendTravelersToTarget()
    {
        for (int i = 0; i < travelers.Count(); i++)
        {
            if(travelers[i].Second == CurrentFloor)
                travelers[i].First.SetActive(true);
        }
        travelers.RemoveAll(t => t.Second == CurrentFloor);
    }

	public List<int> GetFloors() {
		return Floors.ToList ();
	}

    public void AddPersonInElevator() {
		peopleInElevator++;
	}

    public void SetTravelerinElevator(GameObject traveler, int targetFloor)
    {
		travelers.Add(new Pair<GameObject, int>(traveler, targetFloor));
		traveler.SetActive(false);
    }

	public void RemovePersonFromElevator() {
		peopleInElevator--;
	}

	public bool IsFull() {
		return peopleInElevator >= G.Sys.constants.ElevatorMaxPeople;
	}
}