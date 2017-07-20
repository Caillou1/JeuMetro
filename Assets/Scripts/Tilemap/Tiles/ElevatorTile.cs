using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using DG.Tweening;

public class ElevatorTile : ATile
{
	public int Floors = 2;
	public float TimeBetweenFloors = 1f;

	private List<int> FloorsToVisit;
	private bool isMoving = false;
	private Transform tf;
	private float OriginFloor;
	private int CurrentFloor;

	protected override void Awake()
    {
		FloorsToVisit = new List<int> ();
		tf = transform;
		OriginFloor = tf.position.y;
		CurrentFloor = 0;

		type = TileID.ELEVATOR;
		Vector3 pos = transform.position;
		for (int i = 0; i < Floors; i++) {
			pos += i * 2 * Vector3.up;

			G.Sys.tilemap.addTile (pos, this, true, false, Tilemap.ELEVATOR_PRIORITY);

			G.Sys.tilemap.addSpecialTile (type, pos);
		}
    }

	protected override void OnDestroy()
	{
		Vector3 pos = transform.position;

		for (int i = 0; i < Floors; i++) {
			G.Sys.tilemap.delTile (pos, this);

			G.Sys.tilemap.delSpecialTile (type, pos);
		}
	}

	public void CallElevator(int floor) {
		if (!FloorsToVisit.Contains (floor)) {
			FloorsToVisit.Add (floor);
		}
		MoveElevator ();
	}

	private void MoveElevator() {
		if (!isMoving) {
			isMoving = true;
			tf.DOLocalMoveY (OriginFloor + FloorsToVisit [0] * 2, TimeBetweenFloors * Mathf.Abs((FloorsToVisit[0] - CurrentFloor)), false).OnComplete (() => {
				CurrentFloor = FloorsToVisit[0];
				FloorsToVisit.RemoveAt(0);
				isMoving = false;
				if(FloorsToVisit.Count > 0)
					MoveElevator();
			});
		}
	}
}