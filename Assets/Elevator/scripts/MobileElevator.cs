using System.Collections.Generic;
using UnityEngine;

public class MobileElevator : StationaryElevator
{
    [Header("Transportation")]

    [Tooltip("The time it takes the elevator to transport past a single floor.")]
    [SerializeField] [Range(0f, 10)] public float perFloorTime;

    [Tooltip("The elevator's height above the ground when it lands in front of the entrance.")]
    [SerializeField] private float landingHeight = 0;

    [Header("Delay")]

    [Tooltip("Amount of time between the arrival of the elevator to the floor, and its opening.")]
    [SerializeField] public float secondsTillOpen = 1;

    [Tooltip("Amount of time between the opening and the closing of the elevator.")]
    [SerializeField] public float secondsTillClose = 1;

    private List<ElevatorTask> tasksQueue;
    private Stack<int> pendingTasks;
    private HashSet<Passenger> passengers, leavingPassengers;
    private bool countingPassengers;

    public bool IsMoving { get; set; }
    public int CurrentFloorNum { get; set; }
    public StationaryElevator Entrance { get { return GetEntrance(CurrentFloorNum); } }
    public BoxCollider Container { get; set; }
    public Vector3 Volume { get { return Container.bounds.size; } }

    public ElevatorDirection Direction {
        get {
            int nextFloor;

            if (tasksQueue.Count == 0) return ElevatorDirection.Still;
            else if (tasksQueue.Count == 1) nextFloor = tasksQueue[0].TargetFloor;
            else {
                if (tasksQueue[0].TargetFloor != CurrentFloorNum) nextFloor = tasksQueue[0].TargetFloor;
                else nextFloor = tasksQueue[1].TargetFloor;
            }

            if (nextFloor == CurrentFloorNum) return ElevatorDirection.Still;
            else return (nextFloor > CurrentFloorNum) ? ElevatorDirection.Up : ElevatorDirection.Down;
        }
    }

    protected override void Start() {
        base.Start();

        this.Container = GetComponent<BoxCollider>();
        this.passengers = new HashSet<Passenger>();
        this.leavingPassengers = new HashSet<Passenger>();
        this.tasksQueue = new List<ElevatorTask>();
        this.pendingTasks = new Stack<int>();
        this.IsMoving = false;
        this.countingPassengers = false;
        this.CurrentFloorNum = 0;
    }

    protected override void Update() {
        base.Update();
        MoveElevator();
    }

    /// <summary>
    /// Move the elevator based on the most recent request.
    /// </summary>
    private void MoveElevator() {
        if (tasksQueue.Count > 0) {
            ElevatorTask task = tasksQueue[0];
            if (task.Started || !IsWaitingForPassengers()) {
                //proceed
                float previousY = transform.position.y;
                task.ProcessTask();
                float nextY = transform.position.y;

                //translate all passengers in the elevator
                if (previousY != nextY)
                    foreach (Passenger passenger in passengers)
                        passenger.transform.position += Vector3.up * (nextY - previousY);
            }
        }
        //close elevator when idle
        else if (!IsMoving && IsOpen && !IsWaitingForPassengers()) {
            Entrance.Close();
            Close();
        }
    }

    /// <summary>
    /// Request the elevator's transportation to a specific floor.
    /// This request is enqueued and will be executed at the most optimized timing.
    /// </summary>
    /// <param name="floorNum">The floor number to send the elevator to</param>
    /// <returns>True if the request is valid and been noted.</returns>
    public bool SendToFloor(int floorNum) {
        if (IsTask(floorNum)) return false;

        Floor[] floors = FloorBuilder.Instance.Floors;
        if (floorNum < 0 || floorNum > floors.Length) return false;

        //calculate task parameters
        int targetFloor = floorNum;
        Floor destFloor = floors[floorNum];
        float destHeight = destFloor.FloorHeight + landingHeight;
        Vector3 currentPos = transform.position;
        Vector3 destination = new Vector3(currentPos.x, destHeight, currentPos.z);

        //enqueue task
        ElevatorTask task = new ElevatorTask(this, targetFloor, destination);
        int taskIndex = FindTaskQueueIndex(floorNum);
        if (taskIndex == tasksQueue.Count) tasksQueue.Add(task);
        else tasksQueue.Insert(taskIndex, task);

        PrintQueue();
        return true;
    }

    /// <summary>
    /// Get the elevator entrance at a certain floor, on this elevator's side.
    /// </summary>
    /// <param name="floorNum">The number of the floor</param>
    /// <returns>The elevator entrance at the specified floor.</returns>
    private StationaryElevator GetEntrance(int floorNum) {
        Floor floor = FloorBuilder.Instance.Floors[floorNum];
        return floor.GetEntrance(ID);
    }

    /// <summary>
    /// Check if the elevator should stop at a certain floor.
    /// </summary>
    /// <param name="floorNum">The floor number to check</param>
    /// <returns>True if the elevator should eventually stop at the specified floor.</returns>
    private bool IsTask(int floorNum) {
        foreach (ElevatorTask task in tasksQueue)
            if (task.TargetFloor == floorNum) return true;

        return false;
    }

    /// <summary>
    /// Find the correct index of a task in the tasks queue.
    /// Each task is placed in the queue in a way that utilizes the elevator's time optimizely.
    /// This method does not enqueue the task, but rather returns its index,
    /// so it can later be added to the queue with Insert(int, Task).
    /// </summary>
    /// <param name="taskFloor">The task that needs to be enqueued</param>
    /// <returns>The correct index for the task in the queue.</returns>
    private int FindTaskQueueIndex(int taskFloor) {
        if (tasksQueue.Count == 0) return 0;

        int nextFloor = tasksQueue[0].TargetFloor;
        if (Direction == ElevatorDirection.Still) return tasksQueue.Count;
        else return (nextFloor > taskFloor) ? InsertLowerTask(Direction, taskFloor) : InsertHigherTask(Direction, taskFloor);
    }

    /// <summary>
    /// Get the correct index of a task in the tasks queue.
    /// This is an auxiliary method for the FindTaskQueueIndex(int) method.
    /// This method find the correct index of a task if its target floor is higher than the current one.
    /// </summary>
    /// <param name="direction">The vertical direction of the elevator</param>
    /// <param name="taskFloor">The task that needs to be enqueued</param>
    /// <returns>The correct index for the task in the queue.</returns>
    private int InsertHigherTask(ElevatorDirection direction, int taskFloor) {
        int tasksAmount = tasksQueue.Count;

        switch (direction) {
            case ElevatorDirection.Up:
                for (int i = 0; i < tasksAmount; i++) {
                    int linkFloor = tasksQueue[i].TargetFloor;

                    //regular link
                    if (linkFloor > taskFloor || (i > 0 && linkFloor < tasksQueue[i - 1].TargetFloor))
                        return i;

                    //last link
                    else if (i == tasksAmount - 1) return i + 1;
                }
                break;

            case ElevatorDirection.Down:
                for (int i = 0; i < tasksAmount; i++) {
                    int linkFloor = tasksQueue[i].TargetFloor;

                    //regular link
                    if (linkFloor > taskFloor && linkFloor > tasksQueue[i - 1].TargetFloor)
                        return i;

                    //last link
                    else if (i == tasksAmount - 1) return i + 1;
                }
                break;
        }

        return -1; //formal return statement
    }

    /// <summary>
    /// Get the correct index of a task in the tasks queue.
    /// This is an auxiliary method for the FindTaskQueueIndex(int) method.
    /// This method find the correct index of a task if its target floor is lower than the current one.
    /// </summary>
    /// <param name="direction">The vertical direction of the elevator</param>
    /// <param name="taskFloor">The task that needs to be enqueued</param>
    /// <returns>The correct index for the task in the queue.</returns>
    private int InsertLowerTask(ElevatorDirection direction, int taskFloor) {
        int tasksAmount = tasksQueue.Count;

        switch (direction) {
            case ElevatorDirection.Up:
                for (int i = 0; i < tasksAmount; i++) {
                    int linkFloor = tasksQueue[i].TargetFloor;

                    //regular link
                    if (linkFloor < taskFloor && linkFloor < tasksQueue[i - 1].TargetFloor)
                        return i;

                    //last link
                    else if (i == tasksAmount - 1) return i + 1;
                }
                break;

            case ElevatorDirection.Down:
                for (int i = 0; i < tasksAmount; i++) {
                    int linkFloor = tasksQueue[i].TargetFloor;

                    //regular link
                    if (linkFloor < taskFloor || (i > 0 && linkFloor > tasksQueue[i - 1].TargetFloor))
                        return i;

                    //last link
                    if (i == tasksAmount - 1) return i + 1;
                }
                break;
        }

        return -1; //formal return statement
    }

    /// <summary>
    /// Insert a passenger to the elevator.
    /// </summary>
    /// <param name="passenger">The passenger to insert</param>
    public void Enter(Passenger passenger) { passengers.Add(passenger); }

    /// <summary>
    /// Let out the passengers that reached their target floor.
    /// </summary>
    private void RemovePassengers() {
        Queue<Passenger> removedPassengers = new Queue<Passenger>();
        Floor currentFloorComponent = FloorBuilder.Instance.Floors[CurrentFloorNum];

        //find passengers that need to leave the elevator
        foreach (Passenger passenger in passengers)
            if (passenger.TargetFloorNum.Contains(CurrentFloorNum))
                removedPassengers.Enqueue(passenger);

        //activate semaphore
        countingPassengers = true;

        //tell the passengers to leave and remove them from the lists
        while (removedPassengers.Count > 0) {
            Passenger passenger = removedPassengers.Dequeue();
            passenger.AddJourney(JourneyPath.ElevatorExit, currentFloorComponent);
            passenger.AddJourney(JourneyPath.FloorExit, currentFloorComponent);
            leavingPassengers.Add(passenger);
            passengers.Remove(passenger);
        }

        countingPassengers = false;
    }

    /// <summary>
    /// Invite a passenger inside the elevator.
    /// </summary>
    /// <returns>True if the elevator can accept the passenger.</returns>
    public bool ReceivePassenger(Passenger passenger) {
        if ((!IsOpen && !IsOpening) || passengers.Contains(passenger)) return false;

        //push the passenger's desired buttons
        foreach (int targetFloor in passenger.TargetFloorNum) pendingTasks.Push(targetFloor);

        Floor currentFloorComponent = FloorBuilder.Instance.Floors[CurrentFloorNum];
        passenger.TargetElevatorBuffer = this;
        passenger.CommitToJourney(JourneyPath.ElevatorEntrance, currentFloorComponent);
        currentFloorComponent.Passengers.Remove(passenger);
        passengers.Add(passenger);
        return true;
    }

    /// <returns>
    /// True if the elevator cannot be closed yet,
    /// bacause it's waiting for passengers to enter or leave.
    /// </returns>
    private bool IsWaitingForPassengers() {
        if (countingPassengers) return true;

        //wait for passengers that need to leave the elevator
        foreach (Passenger leaving in leavingPassengers)
            if (IsPassengerInsideElevator(leaving) && !leaving.IsDestroyed) return true;

        //all passengers that need to leave have left
        leavingPassengers.Clear();

        //wait for passengers that need to enter the elevator
        foreach (Passenger entering in passengers)
            if (!IsPassengerInsideElevator(entering) && !entering.IsDestroyed) return true;

        return false;
    }

    /// <summary>
    /// Push all the buttons that the passengers inside the elevator need.
    /// </summary>
    private void TakePassengersRequests() {
        //push all pending buttons as a bulk
        if (tasksQueue.Count == 0 && pendingTasks.Count > 0) AddBulkTasks(pendingTasks.ToArray());
        //push all pending buttons in their order
        else while (pendingTasks.Count > 0) SendToFloor(pendingTasks.Pop());

        pendingTasks.Clear();
    }

    /// <param name="passenger">The passenger to check</param>
    /// <returns>
    /// True if the passenger is inside the elevator's boundaries.
    /// </returns>
    private bool IsPassengerInsideElevator(Passenger passenger) {
        return Container.bounds.Contains(passenger.transform.position);
    }

    /// <summary>
    /// Sort the tasks automatically to form a reasonable order.
    /// Tasks are normally ordered via algorithm at runtime,
    /// but reordering them might be helpful when they are all added in a bulk
    /// while the elevator is not moving.
    /// </summary>
    private void AddBulkTasks(int[] tasks) {
        int minDistance = FloorBuilder.Instance.Floors.Length;
        int closestFloorIndex = 0;

        //find the closest floor
        for (int i = 0; i < tasks.Length; i++) {
            int distance = Mathf.Abs(tasks[i] - CurrentFloorNum);

            if (distance < minDistance) {
                minDistance = distance;
                closestFloorIndex = i;
            }
        }

        tasksQueue.Clear();
        SendToFloor(tasks[closestFloorIndex]);

        //add all other tasks
        for (int i = 0; i < tasks.Length; i++)
            if (i != closestFloorIndex) SendToFloor(tasks[i]);
    }

    /// <summary>
    /// Dequeue a task from the queue.
    /// This method should be used from inside the task itself, as soon as its job is done.
    /// </summary>
    /// <param name="task">The task to dequeue</param>
    public void FinishTask(ElevatorTask task) { tasksQueue.Remove(task); }

    private void PrintQueue() {
        string line = name + ": ";

        foreach (ElevatorTask t in tasksQueue)
            line += t.TargetFloor + "|";

        print(line + " Going " + Direction);
    }

    protected override void OnFullyOpen() {
        RemovePassengers();
    }

    protected override void OnOpening() {
        Floor currentFloorComponent = FloorBuilder.Instance.Floors[CurrentFloorNum];
        ElevatorButton button = currentFloorComponent.ElevatorButton;
        button.Switch(false);
    }

    protected override void OnClosing() {
        TakePassengersRequests();
    }

    public override bool Close() {
        if (IsWaitingForPassengers()) return false;
        else return base.Close();
    }
}