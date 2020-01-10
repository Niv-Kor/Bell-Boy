using UnityEngine;

public class ElevatorTask
{
    private static readonly float REACH_TOLERANCE = .1f;

    private MobileElevator elevator;
    private Transform transform;
    Vector3 startingPosition;
    private bool openDelay, openingDelay;
    private float openDelayTimer, openingDelayTimer;
    private float timeLerped, totalTransportTime;

    public int TargetFloor { get; private set; }
    public Vector3 Destination { get; private set; }
    public bool Started { get; private set; }

    /// <param name="elev">The elevator that this task belongs to</param>
    /// <param name="target">The target floor of the task</param>
    /// <param name="dest">The final position that the elevator should be brought to</param>
    public ElevatorTask(MobileElevator elev, int target, Vector3 dest) {
        this.elevator = elev;
        this.transform = elev.transform;
        this.TargetFloor = target;
        this.Destination = dest;
        this.Started = false;
        this.openDelay = false;
        this.openingDelay = false;
        this.openDelayTimer = 0;
        this.openingDelayTimer = 0;
        this.timeLerped = 0;
    }

    /// <summary>
    /// Prepare the elevator for transportation.
    /// </summary>
    private void Start() {
        elevator.Close();
        elevator.Entrance.Close();
        elevator.IsMoving = true;
        startingPosition = transform.position;
        int floorsAmount = Mathf.Abs(TargetFloor - elevator.CurrentFloorNum);
        totalTransportTime = floorsAmount * elevator.perFloorTime;
        Started = true;
    }

    /// <summary>
    /// Transport the elevator to its destination, and then open its doors.
    /// </summary>
    public void ProcessTask() {
        if (!Started) Start();

        //delay the door opening after reaching the floor
        if (openingDelay) {
            if (openingDelayTimer >= elevator.secondsTillOpen) {
                elevator.Open();
                elevator.Entrance.Open();
                openingDelay = false;
                openDelay = true;
            }
            else openingDelayTimer += Time.deltaTime;
        }
        //delay the time the doors are open before taking the next task
        else if (openDelay) {
            if (openDelayTimer >= elevator.secondsTillClose) Finish();
            else openDelayTimer += Time.deltaTime;
        }
        //move elevator towards destination
        else if (!IsWaiting()) {
            timeLerped += Time.deltaTime;
            transform.position = Vector3.Lerp(startingPosition, Destination, timeLerped / totalTransportTime);
            elevator.CurrentFloorNum = FloorBuilder.Instance.GetFloorNumberByHeight(transform.position.y);

            //check if reached target floor
            if (VectorSensitivity.EffectivelyReached(transform.position, Destination, REACH_TOLERANCE)) {
                elevator.CurrentFloorNum = TargetFloor;
                elevator.IsMoving = false;
                openingDelay = true;
            }
        }
    }

    /// <summary>
    /// Check if the elevator is currently busy with doors action.
    /// </summary>
    /// <returns>True if the elevator is not yet ready for transportation.</returns>
    private bool IsWaiting() {
        return elevator.Entrance.IsOpening ||
               elevator.Entrance.IsClosing ||
               elevator.IsOpening ||
               elevator.IsClosing ||
               elevator.IsOpen;
    }

    /// <summary>
    /// Terminate this task as soon as its job is done.
    /// </summary>
    private void Finish() { elevator.FinishTask(this); }
}