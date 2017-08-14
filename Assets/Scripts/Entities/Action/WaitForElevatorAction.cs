using System;
using UnityEngine;

public class WaitForElevatorAction : AEntityAction<AEntity>
{
	ElevatorTile elevatorTile;
	int destinationFloor;

	public WaitForElevatorAction (AEntity t, Vector3 pos, ElevatorTile tile, int destination, int priority) : base(t, ActionType.WAIT_ELEVATOR, pos, priority)
	{
		elevatorTile = tile;
		destinationFloor = destination;
	}

	protected override bool Start ()
	{
		elevatorTile.CallElevator ((int)pos.y);
		return false;
	}

	protected override bool Update ()
	{
		elevatorTile.CallElevator ((int)pos.y);
		return !elevatorTile.IsFull() && elevatorTile.IsOnFloor (Mathf.RoundToInt(pos.y));
	}

	protected override void End ()
	{
		elevatorTile.AddPersonInElevator ();
		entity.path.addAction (new GetInElevatorAction (entity, new Vector3(elevatorTile.transform.position.x - 1f, entity.transform.position.y, elevatorTile.transform.position.z), elevatorTile, destinationFloor, priority - 1));
	}
}

