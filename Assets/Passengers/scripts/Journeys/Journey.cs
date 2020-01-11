using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(AnimationControl))]
public abstract class Journey
{
    protected static readonly float WALK_TO_ROTATION_RATIO = 150;
    protected static readonly float LERP_TOLERANCE = .1f;

    protected Floor floor;
    protected Queue<Vector3> path;
    protected AnimationControl animationControl;
    protected FloorPlanBlueprint floorplan;
    protected Passenger passenger;
    protected Transform transform;
    protected Vector3 dimension;
    protected Vector3 nextPoint, specialRotationTarget;
    protected bool usingSpecialRotation;
    private bool enteredHall, lookAtElevator;
    private float speed, rotationSpeed;

    protected float Speed {
        get { return speed; }
        set {
            speed = value;
            rotationSpeed = value * WALK_TO_ROTATION_RATIO; ;
        }
    }

    /// <param name="passenger">The passenger that this journey belongs to</param>
    /// <param name="navigator">The floor's navigator object</param>
    /// <param name="speed">The walking speed of the passenger</param>
    protected Journey(Passenger passenger, Floor floor, float speed) {
        this.passenger = passenger;
        this.transform = passenger.transform;
        this.floor = floor;
        this.Speed = speed;
        this.enteredHall = false;
        this.lookAtElevator = false;
        this.floorplan = floor.GetComponent<FloorPlanBlueprint>();
        this.animationControl = passenger.GetComponent<AnimationControl>();
        this.dimension = passenger.GetComponent<BoxCollider>().size;
    }

    /// <summary>
    /// Create an instance of a journey.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="passenger"></param>
    /// <param name="floor"></param>
    /// <param name="walkSpeed"></param>
    /// <returns></returns>
    public static Journey Create(JourneyPath path, Passenger passenger, Floor floor, float walkSpeed) {
        switch (path) {
            case JourneyPath.FloorEntrance: return new FloorEntranceJourney(passenger, floor, walkSpeed);
            case JourneyPath.FloorExit: return new FloorExitJourney(passenger, floor, walkSpeed);
            case JourneyPath.ElevatorCall: return new ElevatorCallJourney(passenger, floor, walkSpeed);
            case JourneyPath.ElevatorEntrance: return new ElevatorEntranceJourney(passenger, floor, walkSpeed);
            case JourneyPath.ElevatorExit: return new ElevatorExitJourney(passenger, floor, walkSpeed);
            case JourneyPath.WindowJump: return new WindowJumpJourney(passenger, floor, walkSpeed);
            default: return null;
        }
    }

    /// <summary>
    /// Update the passenger's walk thoughout the path.
    /// </summary>
    /// <returns>True if the path has ended.</returns>
    public bool Update() {
        //passenger is entering the hall for the first time in this journey
        if (!enteredHall && PassengerInHall()) {
            enteredHall = true;
            OnEnteringHall();
        }

        if (path != null) {
            Travel(Move(), Rotate());
            return false;
        }
        else return true;
    }

    /// <returns>True if the passenger in inside the floor's waiting hall.</returns>
    private bool PassengerInHall() {
        Vector3 passengerPos = passenger.transform.position;
        Vector3 passengerCenter = passengerPos + new Vector3(0, passenger.Dimension.y / 2, 0);
        return floor.IsInWaitingHall(passengerCenter);
    }

    /// <summary>
    /// Move and rotate the passenger towards the next target.
    /// This method must contain a call to ContinuePath() in order to proceed the path.
    /// </summary>
    /// <param name="finishMovement">True if the passenger reached the target point</param>
    /// <param name="finishRotation">True if the passenger is fully rotated towards the target point</param>
    protected abstract void Travel(bool finishMovement, bool finishRotation);

    /// <summary>
    /// Move towards the next point of the path.
    /// </summary>
    /// <returns>True if the passenger reached the target point.</returns>
    protected virtual bool Move() {
        if (lookAtElevator) return true;

        transform.position = Vector3.MoveTowards(transform.position, nextPoint, Speed * Time.deltaTime);
        bool reached = VectorSensitivity.EffectivelyReached(transform.position, nextPoint, LERP_TOLERANCE);

        //animate
        animationControl.Animate(StateManchine.WALK, !reached);

        return reached;
    }

    /// <summary>
    /// Rotate towards the next point of the path.
    /// </summary>
    /// <returns>True if the passenger is fully rotated towards the target point.</returns>
    protected virtual bool Rotate() {
        Vector3 rotationTarget = usingSpecialRotation ? specialRotationTarget : nextPoint;
        Vector3 targetPoint = rotationTarget - transform.position;
        if (targetPoint == Vector3.zero) return true;

        Quaternion targetRotation = Quaternion.LookRotation(targetPoint, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        return VectorSensitivity.EffectivelyReached(transform.rotation.eulerAngles, targetRotation.eulerAngles, LERP_TOLERANCE);
    }

    /// <summary>
    /// Start this passenger's movement by generating a random path from the navigator.
    /// </summary>
    /// <returns>True if the journey had been activated successfully.</returns>
    public virtual bool Activate() {
        Queue<Vector3> generatedPath = GeneratePath();

        try {
            if (generatedPath == null) throw new Exception();
            else path = generatedPath;

            OnStart();
            ContinuePath();
            return true;
        }
        catch (Exception) { return false; }
    }

    protected abstract Queue<Vector3> GeneratePath();

    /// <summary>
    /// Continue towards to the next point of the path.
    /// </summary>
    public virtual void ContinuePath() {
        if (path.Count > 0) nextPoint = path.Dequeue();
        else if (path.Count == 0 && LookAtElevatorOnFinish() && !lookAtElevator) {
            //find the closest entrance to look at
            float minDistance = float.MaxValue;
            Vector3 targetEntrance = Vector3.zero; //formal assignment

            foreach (ElevatorID ID in Enum.GetValues(typeof(ElevatorID))) {
                if (ID == default) continue;

                StationaryElevator entrace = floor.GetEntrance(ID);
                Vector3 entrancePos = entrace.transform.position;
                float distance = Vector3.Distance(transform.position, entrancePos);

                if (distance < minDistance) {
                    minDistance = distance;
                    targetEntrance = entrancePos;
                }
            }

            targetEntrance.y = transform.position.y; //maintain same height as character
            nextPoint = targetEntrance;
            lookAtElevator = true;
        }
        else Pause();
    }

    /// <summary>
    /// Pause the passenger's movement.
    /// </summary>
    public void Pause() {
        OnFinish();
        path = null;
    }

    /// <returns>True if the passenger should look at the elevator at the end of the journey.</returns>
    protected abstract bool LookAtElevatorOnFinish();

    /// <summary>
    /// Executes when the passenger starts his journey.
    /// </summary>
    protected virtual void OnStart() {}

    /// <summary>
    /// Executes when the passenger enters the elevators waiting hall,
    /// </summary>
    protected virtual void OnEnteringHall() {}

    /// <summary>
    /// Executes when the passenger finishes his journey.
    /// </summary>
    protected virtual void OnFinish() {}
}