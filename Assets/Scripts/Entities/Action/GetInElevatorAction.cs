using System;
using UnityEngine;
using UnityEngine.AI;

public class GetInElevatorAction : AEntityAction<AEntity>
{
	ElevatorTile elevatorTile;
	int destinationFloor;
	bool wait = false;

	public GetInElevatorAction (AEntity t, Vector3 pos, ElevatorTile tile, int destination) : base(t, ActionType.WAIT_ELEVATOR, pos)
	{
		elevatorTile = tile;
		destinationFloor = destination;
	}

	protected override bool Start ()
	{
		elevatorTile.CallElevator (destinationFloor);

		Debug.Log ("called to destination floor");
		return false;
	}

	protected override bool Update ()
	{
		if (wait)
			return true;
		if (elevatorTile.IsOnFloor (destinationFloor)) {
			wait = true;
			entity.transform.position = elevatorTile.GetWaitZone (destinationFloor);
			entity.GetComponent<NavMeshAgent> ().enabled = false;
		}
		return false;
	}

	protected override void End ()
	{
		entity.GetComponent<NavMeshAgent> ().enabled = true;
	}
}

