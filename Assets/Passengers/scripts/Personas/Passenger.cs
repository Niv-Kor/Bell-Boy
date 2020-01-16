using System.Collections.Generic;
using UnityEngine;

public abstract class Passenger : MonoBehaviour
{
    [Header("Timing")]

    [Tooltip("The passenger's speed.")]
    [SerializeField] protected float walkSpeed;

    [Tooltip("Running speed as a multiplier of the walking speed")]
    [SerializeField] public float RunSpeedMultiplier = 2;

    [Tooltip("The total amount of time (in seconds) the passenger is willing to wait for the elevator.")]
    [SerializeField] public float Patience;

    [Header("Transportation")]

    [Tooltip("The type of mark to show above the passenger.")]
    [SerializeField] protected TargetMarkSymbol targetMarkSymbol;

    [Header("Debug")]

    [Tooltip("The passenger's unique ID.")]
    [SerializeField] public int ID;

    protected static readonly float DISPOSE_TIME = 90;

    protected JourneyData currentJourney;
    protected Queue<JourneyData> journeys;
    protected AnimationControl animationControl;
    protected int startingFloorNum;
    protected float failedSpawnTimer, disposeTimer;
    protected bool destroyed, failedSpawnAttempt;
    private List<int> targetFloorNum;

    public Queue<Vector3> LeasedPath { get; set; }
    public Floor StartingFloor { get { return FloorBuilder.Instance.Floors[StartingFloorNum]; } }
    public Floor CurrentFloor { get { return FloorBuilder.Instance.Floors[CurrentFloorNum]; } }
    public Floor TargetFloor { get { return FloorBuilder.Instance.Floors[TargetFloorNum[0]]; } }
    public TargetMark TargetMark { get; private set; }
    public MobileElevator TargetElevatorBuffer { get; set; }
    public int CurrentFloorNum { get; set; }
    public bool WaitingForElevator { get; set; }
    public Vector3 Dimension { get; private set; }

    public List<int> TargetFloorNum {
        get { return targetFloorNum; }
        set {
            targetFloorNum = value;

            if (targetMarkSymbol == TargetMarkSymbol.Numeral)
                TargetMark.SetFloorNumber(value[0]);
        }
    }

    public bool IsDestroyed {
        get { return destroyed; }
        private set {
            if (value == true) CurrentFloor.Passengers.Remove(this);
            destroyed = value;
        }
    }

    public int StartingFloorNum {
        get { return startingFloorNum; }
        set {
            startingFloorNum = value;
            CurrentFloorNum = value;
        }
    }

    public ElevatorDirection RealDirection {
        get {
            bool goingUp = TargetFloorNum[0] > CurrentFloorNum;
            return goingUp ? ElevatorDirection.Up : ElevatorDirection.Down;
        }
    }

    public ElevatorDirection ApparentDirection {
        get {
            if (targetMarkSymbol == TargetMarkSymbol.Numeral) return RealDirection;
            else return ElevatorDirection.Both;
        }
    }

    protected void Awake() {
        this.ID = Random.Range(0, int.MaxValue);
        this.journeys = new Queue<JourneyData>();
        this.currentJourney = new JourneyData(null, JourneyPath.Blank);
        this.TargetMark = GetComponentInChildren<TargetMark>();
        this.animationControl = GetComponent<AnimationControl>();
        this.Dimension = GetComponent<BoxCollider>().size;
    }

    public virtual void Reset() {
        this.WaitingForElevator = false;
        this.failedSpawnAttempt = false;
        this.IsDestroyed = false;
        this.failedSpawnTimer = 0;
        this.disposeTimer = 0;
        TargetMark.Reset();
        ClearJourneys();

        //set target floors and mark
        this.TargetFloorNum = new List<int>(GenerateTargetFloor());
        if (targetMarkSymbol != TargetMarkSymbol.Numeral) TargetMark.SetSymbol(targetMarkSymbol);
    }

    protected virtual void Update() {
        Navigate();

        if (!IsDestroyed) {
            //respawn attempt
            if (failedSpawnAttempt) {
                failedSpawnTimer += Time.deltaTime;
                if (failedSpawnTimer >= SpawnControl.Instance.FloorSpawnRate) {
                    AddJourney(JourneyPath.FloorEntrance, CurrentFloor);
                    failedSpawnAttempt = false;
                    failedSpawnTimer = 0;
                }
            }

            //call the elevator if needed
            Floor currentFloor = CurrentFloor;

            if (WaitingForElevator &&
               !currentFloor.ElevatorButton.IsOn &&
               !currentFloor.ElevatorBeingCalled &&
               !currentFloor.HasElevator(RealDirection)) {

                CurrentFloor.ElevatorBeingCalled = true;
                AddJourney(JourneyPath.ElevatorCall, currentFloor);
            }
        }
        else if (disposeTimer < DISPOSE_TIME) {
            disposeTimer += Time.deltaTime;

            //dispose passenger (return it to the passengers pool)
            if (disposeTimer >= DISPOSE_TIME) {
                disposeTimer = 0;
                SpawnControl.Instance.CachePassenger(gameObject);
            }
        }
    }

    /// <summary>
    /// Walk through the journeys in the queue.
    /// </summary>
    protected void Navigate() {
        //assign new next journey in the queue
        if (journeys.Count > 0 && currentJourney.Journey == null) {
            JourneyData nextJourney = journeys.Dequeue();

            if (nextJourney.Paused || nextJourney.Journey.Activate()) {
                //continue the paused path
                if (nextJourney.Paused) {
                    nextJourney.Paused = false;
                    nextJourney.Journey.ContinuePath();
                }

                currentJourney = nextJourney;
            }
            //the journey could not be activated
            else {
                switch (nextJourney.PathType) {
                    case JourneyPath.FloorEntrance: failedSpawnAttempt = true; break;
                    case JourneyPath.ElevatorCall: CurrentFloor.ElevatorBeingCalled = false; break;
                }
            }
        }
        //run current journey
        else if (currentJourney.Journey != null) {
            bool finished = currentJourney.Journey.Update();
            if (finished) currentJourney.Clear();
        }
    }

    /// <summary>
    /// Add a journey to the end of the queue.
    /// </summary>
    /// <param name="path">Type of journey to add</param>
    /// <param name="floor">The Component of the floor at which the journey occurs</param>
    public virtual void AddJourney(JourneyPath path, Floor floor) {
        Journey journey = Journey.Create(path, this, floor, walkSpeed);
        if (journey != null) journeys.Enqueue(new JourneyData(journey, path));
    }

    /// <summary>
    /// Force push a journey to the beginning of the queue.
    /// This journey will be executed before anything else, even if another one is already taking place.
    /// Any journey that had been paused as a result of this push, will resume as soon as this journey ends.
    /// </summary>
    /// <param name="path">Type of journey to add</param>
    /// <param name="floor">The Component of the floor at which the journey occurs</param>
    public virtual void ForceJourney(JourneyPath path, Floor floor) {
        Journey journey = Journey.Create(path, this, floor, walkSpeed);

        //push to the start
        if (journey != null) {
            Queue<JourneyData> newQueue = new Queue<JourneyData>();
            currentJourney.Paused = true;
            newQueue.Enqueue(currentJourney);
            currentJourney = new JourneyData(journey, path);

            //unsuccessful activation of the new journey
            if (!currentJourney.Journey.Activate()) currentJourney = newQueue.Dequeue();

            //transfer all remaining journeys between the queues
            while (journeys.Count > 0) newQueue.Enqueue(journeys.Dequeue());
            while (newQueue.Count > 0) journeys.Enqueue(newQueue.Dequeue());
        }
    }

    /// <summary>
    /// Clear the queue and immediately force a new journey.
    /// </summary>
    /// <param name="path">Type of journey to add</param>
    /// <param name="floor">The Component of the floor at which the journey occurs</param>
    public virtual void CommitToJourney(JourneyPath path, Floor floor) {
        ClearJourneys();
        AddJourney(path, floor);
    }

    /// <summary>
    /// Kill a passenger and lose a life in the game.
    /// </summary>
    public void Kill() {
        CommitToJourney(JourneyPath.WindowJump, CurrentFloor);
        IsDestroyed = true;
    }

    /// <summary>
    /// Destroy a passenger object (without losing a life).
    /// </summary>
    public virtual void Destroy() {
        SpawnControl.Instance.CachePassenger(gameObject);
        IsDestroyed = true;
    }

    /// <summary>
    /// Cancel all upcoming journeys, including the current one.
    /// </summary>
    public void ClearJourneys() {
        animationControl.StrongIdlize();
        currentJourney.Clear();
        journeys.Clear();
    }

    /// <summary>
    /// Generate a target floor for the passenger.
    /// </summary>
    /// <returns>A target floor which is different than the starting floor.</returns>
    protected abstract int[] GenerateTargetFloor();

    /// <returns>True if the passenger get be spawned at the moment.</returns>
    public abstract bool CanBeSpawned();
}