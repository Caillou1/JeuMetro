using System;
using UnityEngine;
using UnityEngine.AI;

public class GetInElevatorAction : AEntityAction<AEntity>
{
	ElevatorTile elevatorTile;
	int destinationFloor;
	bool wait = false;

	public GetInElevatorAction (AEntity t, Vector3 pos, ElevatorTile tile, int destination, int priority) : base(t, ActionType.WAIT_ELEVATOR, pos, priority)
	{
		elevatorTile = tile;
		destinationFloor = destination;
	}

	protected override bool Start ()
	{
		elevatorTile.CallElevator (destinationFloor);

		return false;
	}

	protected override bool Update ()
	{
		if (wait)
			return true;
		if (elevatorTile.IsOnFloor (destinationFloor)) {
			wait = true;
			var tf = entity.transform;
			tf.position = new Vector3 (tf.position.x, tf.position.y + destinationFloor - Mathf.RoundToInt (tf.position.y), tf.position.z);
			entity.GetComponent<NavMeshAgent> ().enabled = false;
		}
		return false;
	}

	protected override void End ()
	{
		entity.GetComponent<NavMeshAgent> ().enabled = true;
		elevatorTile.RemovePersonFromElevator ();
	}
}

