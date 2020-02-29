using System.Collections.Generic;
using UnityEngine;

public abstract class Passenger : Person
{
    [Header("Timing")]

    [Tooltip("The total amount of time (in seconds) the passenger is willing to wait for the elevator.")]
    [SerializeField] public float Patience;

    [Header("Transportation")]

    [Tooltip("The type of mark to show above the passenger.")]
    [SerializeField] public TargetMarkSymbol DefaultTargetMark;

    protected static readonly float DISPOSE_TIME = 90;

    protected TipsJar tipsCalculator;
    protected TargetMarkSymbol targetMarkSymbol;
    protected int startingFloorNum;
    protected float disposeTimer;
    private List<int> targetFloorNum;

    public Floor StartingFloor { get { return StoreyBuilder.Instance.Storeys[StartingFloorNum]; } }
    public Floor CurrentFloor { get { return StoreyBuilder.Instance.Storeys[CurrentFloorNum]; } }
    public Floor TargetFloor { get { return StoreyBuilder.Instance.Storeys[TargetFloorNum[0]]; } }
    public int BaseTipValue { get { return tipsCalculator.PatienceToTipValue(Patience); } }
    public TargetMark TargetMark { get; private set; }
    public MobileElevator TargetElevatorBuffer { get; set; }
    public int CurrentFloorNum { get; set; }
    public bool WaitingForElevator { get; set; }

    public override bool IsDestroyed {
        get { return destroyed; }
        protected set {
            if (value == true) CurrentFloor.Passengers.Remove(this);
            base.IsDestroyed = value;
        }
    }

    public List<int> TargetFloorNum {
        get { return targetFloorNum; }
        set {
            targetFloorNum = value;

            if (TargetMarkSymbol == TargetMarkSymbol.Numeral)
                TargetMark.SetFloorNumber(value[0]);
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
            if (TargetMarkSymbol == TargetMarkSymbol.Numeral) return RealDirection;
            else return ElevatorDirection.Both;
        }
    }

    public TargetMarkSymbol TargetMarkSymbol {
        get { return targetMarkSymbol; }
        set {
            targetMarkSymbol = value;

            if (value != TargetMarkSymbol.Numeral) TargetMark.SetSymbol(value);
            else TargetMark.SetFloorNumber(TargetFloorNum[0]);
        }
    }

    protected override void Awake() {
        base.Awake();

        this.TargetMark = GetComponentInChildren<TargetMark>();
        this.tipsCalculator = FindObjectOfType<TipsJar>();
    }

    public override void Reset() {
        base.Reset();

        this.WaitingForElevator = false;
        this.disposeTimer = 0;
        TargetMark.Reset();

        //set target floors and mark
        this.TargetFloorNum = new List<int>(GenerateTargetFloor());
        this.TargetMarkSymbol = DefaultTargetMark;
    }

    protected override void Update() {
        base.Update();

        if (!IsDestroyed) {
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
                SpawnController.Instance.CachePerson(gameObject);
            }
        }
    }

    /// <summary>
    /// Kill a passenger and lose a life in the game.
    /// </summary>
    public void Kill() {
        CommitToJourney(JourneyPath.WindowJump, CurrentFloor);
        IsDestroyed = true;
    }

    /// <summary>
    /// Generate a target floor for the passenger.
    /// </summary>
    /// <returns>A target floor which is different than the starting floor.</returns>
    protected abstract int[] GenerateTargetFloor();

    /// <returns>True if the passenger get be spawned at the moment.</returns>
    public abstract bool CanBeSpawned();
}