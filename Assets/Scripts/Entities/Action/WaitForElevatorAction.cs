using System;
using UnityEngine;

public class WaitForElevatorAction : AEntityAction<AEntity>
{
	ElevatorTile elevatorTile;
	int destinationFloor;

	public WaitForElevatorAction (AEntity t, Vector3 pos, ElevatorTile tile, int destination) : base(t, ActionType.WAIT_ELEVATOR, pos)
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
		return elevatorTile.IsOnFloor ((int)pos.y);
	}

	protected override void End ()
	{
		entity.path.addAction (new GetInElevatorAction (entity, new Vector3(elevatorTile.transform.position.x, entity.transform.position.y, elevatorTile.transform.position.z), elevatorTile, destinationFloor));
	}
}

