using System;
using UnityEngine;
using UnityEngine.AI;

public class GetInElevatorAction : AEntityAction<AEntity>
{
	ElevatorTile elevatorTile;
	int destinationFloor;

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
		return elevatorTile.IsOnFloor (destinationFloor);
	}

	protected override void End ()
	{
		Debug.Log ("On destination Floor");
	}
}

