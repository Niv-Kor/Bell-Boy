using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PersonAnimator))]
public class Person : MonoBehaviour
{
    [Tooltip("The passenger's speed.")]
    [SerializeField] protected float walkSpeed;

    [Tooltip("Running speed as a multiplier of the walking speed")]
    [SerializeField] public float RunSpeedMultiplier = 2;

    protected JourneyData currentJourney;
    protected Queue<JourneyData> journeys;
    protected PersonAnimator passengerAnimator;
    protected SoundMixer soundMixer;
    protected bool destroyed;

    public Queue<Vector3> LeasedPath { get; set; }
    public Vector3 Dimension { get; private set; }

    public virtual bool IsDestroyed {
        get { return destroyed; }
        protected set { destroyed = value; }
    }

    protected virtual void Awake() {
        this.journeys = new Queue<JourneyData>();
        this.currentJourney = new JourneyData(null, JourneyPath.Blank);
        this.soundMixer = GetComponent<SoundMixer>();
        this.passengerAnimator = GetComponent<PersonAnimator>();
        this.Dimension = GetComponent<BoxCollider>().size;
    }

    public virtual void Reset() {
        this.IsDestroyed = false;
        ClearJourneys();

        //spawn at neutral point
        transform.position = SpawnController.Instance.NeutralPoint;
    }

    protected virtual void Update() {
        Navigate();
    }

    /// <summary>
    /// Walk through the journeys in the queue.
    /// </summary>
    protected virtual void Navigate() {
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
        passengerAnimator.StrongIdlize();
        AddJourney(path, floor);
    }

    /// <summary>
    /// Destroy a passenger object (without losing a life).
    /// </summary>
    public virtual void Destroy() {
        SpawnController.Instance.CachePerson(gameObject);
        IsDestroyed = true;
    }

    /// <summary>
    /// Cancel all upcoming journeys, including the current one.
    /// </summary>
    public void ClearJourneys() {
        passengerAnimator.StrongIdlize();
        currentJourney.Clear();
        journeys.Clear();
    }
}
