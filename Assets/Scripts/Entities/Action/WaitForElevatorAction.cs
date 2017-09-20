using System;
using UnityEngine;

public class WaitForElevatorAction : AEntityAction<AEntity>
{
	ElevatorTile elevatorTile;
	int destinationFloor;
	bool checkElevator;
	bool canEnd;

	public WaitForElevatorAction (AEntity t, Vector3 pos, ElevatorTile tile, int destination, bool check) : base(t, ActionType.WAIT_ELEVATOR, pos)
	{
		elevatorTile = tile;
		destinationFloor = destination;
		checkElevator = check;
		canEnd = true;
        elevatorTile.peopleWaiting++;
	}

	protected override bool Start ()
	{
		elevatorTile.CallElevator ((int)pos.y);
		return false;
	}

	protected override bool Update ()
	{
		if (checkElevator) {
            var positions = G.Sys.tilemap.getSurrondingSpecialTile (entity.transform.position, TileID.ESCALATOR, G.Sys.constants.EscalatorDetectionRadius, G.Sys.constants.VerticalAmplification);
			if (positions.Count > 0) {
				canEnd = false;
				return true;
			}
		}

		elevatorTile.CallElevator ((int)pos.y);
		return !elevatorTile.IsFull() && elevatorTile.IsOnFloor (Mathf.RoundToInt(pos.y));
	}

	protected override void End ()
	{
		if (canEnd) 
        {
            elevatorTile.peopleWaiting--;
            elevatorTile.AddPersonInElevator ();
            entity.path.addAction (new GetInElevatorAction (entity, new Vector3(elevatorTile.transform.position.x, entity.transform.position.y, elevatorTile.transform.position.z) 
                                                                    + Orienter.orientationToDir3(Orienter.angleToOrientation(elevatorTile.transform.rotation.eulerAngles.y)), elevatorTile, destinationFloor));
		}
	}
}

