using System.Collections.Generic;
using UnityEngine;

public class ElevatorCallJourney : Journey
{
    private bool pressing;

    public ElevatorCallJourney(Person person, Floor floor, float walkSpeed) :
    base(person, floor, walkSpeed) {
        this.pressing = true;
    }

    protected override void Travel(bool finishMovement, bool finishRotation) {
        if (pressing && !passengerAnimator.IsAtState(PersonAnimator.PRESS)) {
            //press elevator button
            if (finishMovement && finishRotation) {
                passengerAnimator.Activate(PersonAnimator.PRESS);
                pressing = false;
            }
        }
        else if (finishMovement && finishRotation && passengerAnimator.IsIdle) {
            usingSpecialRotation = false;
            ContinuePath();
        }
    }

    /// <summary>
    /// Call The elevator to the floor.
    /// </summary>
    private void TriggerState_CallElevator() {
        floor.ElevatorButton.Call();
    }

    protected override Queue<Vector3> GeneratePath() {
        Queue<Vector3> path = new Queue<Vector3>();

        //swap floor paths
        floorplan.Free(person.LeasedPath);
        person.LeasedPath = floorplan.Lease();

        //go to elevator
        Vector3 buttonPos = floor.ElevatorButton.transform.position;
        buttonPos.y = transform.position.y;
        specialRotationTarget = Vector3.Scale(buttonPos, Vector3.one);
        buttonPos.z -= dimension.z;
        usingSpecialRotation = true;
        path.Enqueue(buttonPos);

        //a random spot at the elevators waiting hall
        Queue<Vector3> leasedPath = person.LeasedPath;
        Vector3 lastPoint = person.LeasedPath.ToArray()[leasedPath.Count - 1];
        path.Enqueue(lastPoint);

        return path;
    }

    protected override void OnStart() {
        passengerAnimator.AnimationTape.Triggers["press"].Subscribe(TriggerState_CallElevator);
        floor.ElevatorBeingCalled = true;
    }

    protected override void OnFinish() {
        passengerAnimator.AnimationTape.Triggers["press"].Unsubscribe(TriggerState_CallElevator);
        floor.ElevatorBeingCalled = false;
        passengerAnimator.Idlize();
    }

    protected override bool LookAtElevatorOnFinish() { return true; }
}