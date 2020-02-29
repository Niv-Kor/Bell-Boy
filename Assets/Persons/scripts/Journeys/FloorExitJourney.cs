using System.Collections.Generic;
using UnityEngine;

public class FloorExitJourney : Journey
{
    public FloorExitJourney(Person person, Floor floor, float walkSpeed) :
    base(person, floor, walkSpeed) {}

    protected override void Travel(bool finishMovement, bool finishRotation) {
        if (finishMovement && finishRotation) ContinuePath();
    }

    protected override Queue<Vector3> GeneratePath() {
        Queue<Vector3> path = floorplan.Clone();

        //change the order of the path
        Stack<Vector3> tempStack = new Stack<Vector3>();
        while (path.Count > 1) tempStack.Push(path.Dequeue());
        path.Clear(); //eliminate last point
        while (tempStack.Count > 0) path.Enqueue(tempStack.Pop());

        return path;
    }

    protected override void OnFinish() {
        person.Destroy();
    }

    protected override bool LookAtElevatorOnFinish() { return false; }
}