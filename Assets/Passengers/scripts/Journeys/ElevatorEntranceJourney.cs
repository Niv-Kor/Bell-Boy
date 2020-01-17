using System.Collections.Generic;
using UnityEngine;

public class ElevatorEntranceJourney : Journey
{
    private ElevatorID elevatorID;
    private MobileElevator selectedElevator;

    public ElevatorEntranceJourney(Passenger passenger, Floor floor, float walkSpeed) :
    base(passenger, floor, walkSpeed) {
        this.elevatorID = default;
        this.lookAtElevator = false;
    }

    protected override void Travel(bool finishMovement, bool finishRotation) {
        if (finishMovement && finishRotation) ContinuePath();

        //don't look at the elevator's doors if they are closing and about to change location
        if (lookAtElevator && (selectedElevator.IsClosing || !selectedElevator.IsOpen))
            ContinuePath();
    }

    protected override bool Move() {
        if (lookAtElevator) return true;
        else return base.Move();
    }

    public override bool Activate() {
        bool isActivated = base.Activate();
        if (isActivated) floorplan.Free(passenger.LeasedPath);
        return isActivated;
    }

    protected override Queue<Vector3> GeneratePath() {
        Queue<Vector3> path = new Queue<Vector3>();
        selectedElevator = passenger.TargetElevatorBuffer;
        Vector3 elevPos = selectedElevator.transform.position;
        Vector3 elevVolume = selectedElevator.Volume;
        elevatorID = selectedElevator.ID;

        //stand in front of the elevator
        Vector3 entrancePos = floor.GetEntrance(elevatorID).transform.position;
        entrancePos.y = transform.position.y;
        entrancePos.z -= dimension.z;
        path.Enqueue(entrancePos);

        //move the position to the elevator's (0, 0) point
        elevPos.x -= elevVolume.x / 2 - dimension.x / 2;
        elevPos.y = transform.position.y;
        elevPos.z -= elevVolume.z / 2 - dimension.z / 2;

        //generate a location in the elevator
        elevPos.x += Random.Range(dimension.x, elevVolume.x - dimension.x);
        elevPos.z += Random.Range(dimension.z, elevVolume.z - dimension.z);
        path.Enqueue(elevPos);

        return path;
    }

    public override void ContinuePath() {
        //look towards the entrance of the elevator from the inside
        if (path.Count == 0 && !lookAtElevator) {
            Vector3 targetPos = floor.GetEntrance(elevatorID).transform.position;
            targetPos.y = passenger.transform.position.y;
            specialRotationTarget = targetPos;
            usingSpecialRotation = true;
            nextPoint = targetPos;
            lookAtElevator = true;
        }
        else {
            lookAtElevator = false;
            usingSpecialRotation = false;
            base.ContinuePath();
        }
    }

    protected override void OnStart() {
        passenger.TargetMark.Activate(false);
        passenger.WaitingForElevator = false;
        floor.Passengers.Remove(passenger);
    }

    protected override void OnFinish() {
        passengerAnimator.Idlize();
    }

    protected override bool LookAtElevatorOnFinish() { return false; }
}