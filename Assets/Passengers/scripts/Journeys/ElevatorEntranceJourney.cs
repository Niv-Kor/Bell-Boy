using System.Collections.Generic;
using UnityEngine;

public class ElevatorEntranceJourney : Journey
{
    private ElevatorID selectedID;
    private bool lookAtElevator;

    public ElevatorEntranceJourney(Passenger passenger, Floor floor, float walkSpeed) :
    base(passenger, floor, walkSpeed) {
        this.selectedID = default;
        this.lookAtElevator = false;
    }

    protected override void Travel(bool finishMovement, bool finishRotation) {
        if (finishMovement && finishRotation) ContinuePath();
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

        //find the correct elevator ID
        bool goingUp = passenger.TargetFloorNum[0] > passenger.CurrentFloorNum;
        ElevatorDirection passengerDirection = goingUp ? ElevatorDirection.Up : ElevatorDirection.Down;
        List<MobileElevator> elevators = ElevatorsManager.GetAllElevators();
        CollectionsUtil.ShuffleList(elevators);

        foreach (MobileElevator elev in elevators) {
            if (elev.ID == default || elev.CurrentFloorNum != floor.FloorNumber) continue;
            else {
                bool sameDirection = elev.Direction == ElevatorDirection.Still || elev.Direction == passengerDirection;
                bool isOpen = elev.IsOpening || elev.IsOpen;

                if (sameDirection && isOpen) {
                    selectedID = elev.ID;
                    break;
                }
            }
        }

        //found no suitable elevator - cancel journey
        if (selectedID == default) return null;

        //stand in front of the elevator
        Vector3 entrancePos = floor.GetEntrance(selectedID).transform.position;
        entrancePos.y = transform.position.y;
        entrancePos.z -= dimension.z;
        path.Enqueue(entrancePos);

        MobileElevator selectedElevator = ElevatorsManager.GetElevator(selectedID);
        Vector3 elevPos = selectedElevator.transform.position;
        Vector3 elevVolume = selectedElevator.Volume;

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
            Vector3 targetPos = floor.GetEntrance(selectedID).transform.position;
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
        animationControl.Idlize();
    }

    protected override bool LookAtElevatorOnFinish() { return false; }
}