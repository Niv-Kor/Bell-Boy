using System.Collections.Generic;
using UnityEngine;

public class FloorEntranceJourney : Journey
{
    private bool needCallElevator;

    public FloorEntranceJourney(Person person, Floor floor, float walkSpeed) :
    base(person, floor, walkSpeed) {
        this.needCallElevator = false;
    }

    protected override void Travel(bool finishMovement, bool finishRotation) {
        if (finishMovement && finishRotation) ContinuePath();
    }

    public override void ContinuePath() {
        //check if character needs to call the elevator and postpone its last path point
        if (path.Count == 1 && needCallElevator) {
            person.ForceJourney(JourneyPath.ElevatorCall, floor);
            needCallElevator = false;
            path.Clear(); //let the elevator call journey travel to the last point instead
        }
        else base.ContinuePath();
    }

    protected override Queue<Vector3> GeneratePath() {
        person.LeasedPath = floorplan.Lease();
        return person.LeasedPath;
    }

    protected override void OnStart() {
        transform.position = path.Dequeue();
    }

    protected override void OnEnteringHall() {
        Passenger passenger = (Passenger) person;
        passenger.WaitingForElevator = true;
        passenger.TargetMark.Activate(true);

        if (!floor.ElevatorButton.IsOn && !floor.ElevatorBeingCalled) {
            needCallElevator = true;
            floor.ElevatorBeingCalled = true;
        }
    }

    protected override bool LookAtElevatorOnFinish() { return true; }
}