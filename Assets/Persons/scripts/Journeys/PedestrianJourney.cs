using System.Collections.Generic;
using UnityEngine;

public class PedestrianJourney : Journey
{
    private PedestrianStreetMap pedStreetMap;

    public PedestrianJourney(Person person, float walkSpeed) :
    base(person, null, walkSpeed) {
        this.pedStreetMap = Object.FindObjectOfType<PedestrianStreetMap>();
    }

    protected override void Travel(bool finishMovement, bool finishRotation) {
        if (finishMovement && finishRotation) ContinuePath();
    }

    protected override Queue<Vector3> GeneratePath() {
        if (pedStreetMap != null) return pedStreetMap.Lease();
        else return pedStreetMap.Clone();
    }

    protected override void OnStart() {
        transform.position = path.Dequeue();
    }

    protected override void OnFinish() {
        pedStreetMap.Free(path);
        person.Destroy();
    }

    protected override bool LookAtElevatorOnFinish() { return false; }
}