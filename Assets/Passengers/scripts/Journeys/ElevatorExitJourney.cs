using System.Collections.Generic;
using UnityEngine;

public class ElevatorExitJourney : Journey
{
    private ElevatorID fromElevator;
    private TipsJar tipsCalculator;

    public ElevatorExitJourney(Passenger passenger, Floor floor, float walkSpeed) :
    base(passenger, floor, walkSpeed) {
        this.fromElevator = default;
        this.tipsCalculator = Object.FindObjectOfType<TipsJar>();
    }

    protected override void Travel(bool finishMovement, bool finishRotation) {
        if (finishMovement && finishRotation) ContinuePath();
    }

    protected override Queue<Vector3> GeneratePath() {
        Queue<Vector3> path = new Queue<Vector3>();
        Vector3 passengerPos = passenger.transform.position;

        //find the correct elevator ID
        foreach (MobileElevator elev in ElevatorsManager.GetAllElevators()) {
            if (elev.Container.bounds.Contains(passengerPos)) {
                fromElevator = elev.ID;
                break;
            }
        }

        //found no suitable elevator to exit from - cancel journey
        if (fromElevator == default) return null;

        //stand in front of the elevator
        Vector3 entrancePos = floor.GetEntrance(fromElevator).transform.position;
        entrancePos.y = transform.position.y;
        entrancePos.z -= dimension.z;
        path.Enqueue(entrancePos);

        return path;
    }

    protected override void OnFinish() {
        tipsCalculator.Tip(passenger.transform.position, passenger.BaseTipValue);
        passengerAnimator.Idlize();
    }

    protected override bool LookAtElevatorOnFinish() { return false; }
}
