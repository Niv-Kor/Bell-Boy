using System.Collections.Generic;
using UnityEngine;

public class ElevatorCallJourney : Journey
{
    private bool pressing;

    public ElevatorCallJourney(Passenger passenger, Floor floor, float walkSpeed) :
    base(passenger, floor, walkSpeed) {
        this.pressing = true;
        animationControl.Triggers[StateManchine.PRESS].OnFinish += TriggerState_CallElevator;
    }

    protected override void Travel(bool finishMovement, bool finishRotation) {
        if (pressing && !animationControl.IsAnimating(StateManchine.PRESS)) {
            //press elevator button
            if (finishMovement && finishRotation) {
                animationControl.Animate(StateManchine.PRESS);
                pressing = false;
            }
        }
        else if (finishMovement && finishRotation && animationControl.IsIdle && floor.ElevatorButton.IsOn) {
            usingSpecialRotation = false;
            ContinuePath();
        }
    }

    /// <summary>
    /// Call The elevator to the floor.
    /// </summary>
    private void TriggerState_CallElevator(TriggerState trigger) {
        floor.ElevatorButton.Call();
    }

    protected override Queue<Vector3> GeneratePath() {
        Queue<Vector3> path = new Queue<Vector3>();

        //swap floor paths
        floorplan.Free(passenger.LeasedPath);
        passenger.LeasedPath = floorplan.Lease();

        //go to elevator
        Vector3 buttonPos = floor.ElevatorButton.transform.position;
        buttonPos.y = transform.position.y;
        specialRotationTarget = Vector3.Scale(buttonPos, Vector3.one);
        buttonPos.z -= dimension.z;
        usingSpecialRotation = true;
        path.Enqueue(buttonPos);

        //a random spot at the elevators waiting hall
        Queue<Vector3> leasedPath = passenger.LeasedPath;
        Vector3 lastPoint = passenger.LeasedPath.ToArray()[leasedPath.Count - 1];
        path.Enqueue(lastPoint);

        return path;
    }

    protected override void OnStart() {
        floor.ElevatorBeingCalled = true;
    }

    protected override void OnFinish() {
        animationControl.Triggers[StateManchine.PRESS].OnFinish -= TriggerState_CallElevator;
        floor.ElevatorBeingCalled = false;
        animationControl.Idlize();
    }

    protected override bool LookAtElevatorOnFinish() { return true; }
}