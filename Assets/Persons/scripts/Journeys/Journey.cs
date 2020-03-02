using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Journey
{
    protected static readonly float WALK_TO_ROTATION_RATIO = 150;
    protected static readonly float LERP_TOLERANCE = .1f;

    protected Floor floor;
    protected Queue<Vector3> path;
    protected PersonAnimator passengerAnimator;
    protected FloorPlanBlueprint floorplan;
    protected Person person;
    protected Transform transform;
    protected Vector3 dimension;
    protected Vector3 nextPoint, specialRotationTarget;
    protected bool stopped, paused, usingSpecialRotation, lookAtElevator;
    private bool enteredHall;
    private float speed, rotationSpeed;

    protected float Speed {
        get { return speed; }
        set {
            speed = value;
            rotationSpeed = value * WALK_TO_ROTATION_RATIO; ;
        }
    }

    /// <param name="person">The person that this journey belongs to</param>
    /// <param name="navigator">The floor's navigator object</param>
    /// <param name="speed">The walking speed of the passenger</param>
    protected Journey(Person person, Floor floor, float speed) {
        this.person = person;
        this.transform = person.transform;
        this.floor = floor;
        this.Speed = speed;
        this.stopped = false;
        this.enteredHall = false;
        this.lookAtElevator = false;
        this.floorplan = (floor != null) ? floor.GetComponent<FloorPlanBlueprint>() : null;
        this.passengerAnimator = person.GetComponent<PersonAnimator>();
        this.dimension = person.GetComponent<BoxCollider>().size;
    }

    /// <summary>
    /// Create an instance of a journey.
    /// </summary>
    /// <param name="path">The type of path to create</param>
    /// <param name="person">The person to send to do the journey</param>
    /// <param name="floor">The floor at which the journey occurs (null if none)</param>
    /// <param name="walkSpeed">The normal walking speed of the person</param>
    /// <returns></returns>
    public static Journey Create(JourneyPath path, Person person, Floor floor, float walkSpeed) {
        switch (path) {
            case JourneyPath.FloorEntrance: return new FloorEntranceJourney(person, floor, walkSpeed);
            case JourneyPath.FloorExit: return new FloorExitJourney(person, floor, walkSpeed);
            case JourneyPath.ElevatorCall: return new ElevatorCallJourney(person, floor, walkSpeed);
            case JourneyPath.ElevatorEntrance: return new ElevatorEntranceJourney(person, floor, walkSpeed);
            case JourneyPath.ElevatorExit: return new ElevatorExitJourney(person, floor, walkSpeed);
            case JourneyPath.WindowJump: return new WindowJumpJourney(person, floor, walkSpeed);
            case JourneyPath.Pedestrian: return new PedestrianJourney(person, walkSpeed);
            default: return null;
        }
    }

    /// <summary>
    /// Update the passenger's walk thoughout the path.
    /// </summary>
    /// <returns>True if the path has ended.</returns>
    public bool Update() {
        if (paused) return false;

        //passenger is entering the hall for the first time in this journey
        if (floor != null && !enteredHall && floor.IsInWaitingHall(person)) {
            enteredHall = true;
            OnEnteringHall();
        }

        if (path != null) {
            Travel(Move(), Rotate());
            return false;
        }
        else return true;
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
        passengerAnimator.Activate(PersonAnimator.WALK, !reached);

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
            if (generatedPath == null) {
                Debug.Log("No path for " + person.name);
                throw new Exception();
            }
            else path = generatedPath;

            OnStart();
            ContinuePath();
            return true;
        }
        catch (Exception) { return false; }
    }

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

                StationaryElevator entrance = floor.GetEntrance(ID);
                Vector3 entrancePos = entrance.transform.position;
                float distance = Vector3.Distance(transform.position, entrancePos);

                if (distance < minDistance) {
                    minDistance = distance;
                    targetEntrance = entrancePos;
                }
            }

            targetEntrance.y = transform.position.y; //maintain same height as passenger
            nextPoint = targetEntrance;
            lookAtElevator = true;
        }
        else Stop();
    }

    /// <summary>
    /// Pause the journey.
    /// </summary>
    /// <param name="flag">True to pause or false to resume</param>
    public void Pause(bool flag) { paused = flag; }

    /// <summary>
    /// Stop the journey.
    /// </summary>
    public void Stop() {
        if (stopped) return;

        OnFinish();
        path = null;
        stopped = true;
    }

    /// <returns>A path for the passenger to walk through.</returns>
    protected abstract Queue<Vector3> GeneratePath();

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