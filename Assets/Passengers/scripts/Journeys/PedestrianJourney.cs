using System.Collections.Generic;
using UnityEngine;

public class PedestrianJourney : Journey
{
    private StreetsMap streetsMap;

    public PedestrianJourney(Passenger passenger, float walkSpeed) :
    base(passenger, null, walkSpeed) {
        this.streetsMap = Object.FindObjectOfType<StreetsMap>();
    }

    protected override void Travel(bool finishMovement, bool finishRotation) {
        if (finishMovement && finishRotation) ContinuePath();
    }

    protected override Queue<Vector3> GeneratePath() { return streetsMap.Clone(); }

    protected override void OnStart() {
        passenger.TargetMark.gameObject.gameObject.SetActive(false);
        transform.position = path.Dequeue();
    }

    protected override void OnFinish() {
        streetsMap.Free(path);
        passenger.TargetMark.gameObject.SetActive(true);
        passenger.Destroy();
    }

    protected override bool LookAtElevatorOnFinish() { return false; }
}