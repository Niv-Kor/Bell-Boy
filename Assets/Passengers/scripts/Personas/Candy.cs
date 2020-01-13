using System.Collections.Generic;
using UnityEngine;

public class Candy : Passenger
{
    [Header("Special Effect")]

    [Tooltip("The time between attempts to blow a kiss when " +
             "there are other passengers on the floor.")]
    [SerializeField] private float blowKissPulse;

    [Tooltip("The heart particles to spawn over the passengers' head when the kiss is blown.")]
    [SerializeField] private ParticleSystem heartParticles;

    private float blowKissTimer;
    private bool delayingJourney;
    private JourneyPath journeyToProceed;
    private HashSet<Passenger> affectedPassengers;

    public override void Reset() {
        base.Reset();
        this.blowKissTimer = 0;
        this.affectedPassengers = new HashSet<Passenger>();
        animationControl.Triggers[StateManchine.SPECIAL].OnFinish += TriggerState_BlowKiss;
    }

    protected override void Update() {
        base.Update();

        if (WaitingForElevator && animationControl.IsIdle) {
            List<Passenger> floorPassengers = CurrentFloor.Passengers;

            //check if any other passenger is waiting for the elevator
            foreach (Passenger person in floorPassengers) {
                if (person == this) continue;

                bool isTargetingOneFloor = person.TargetFloorNum.Count == 1;
                bool isAlreadyAffected = person.TargetFloorNum[0] == TargetFloorNum[0];
                bool isAffectable = !(person is Candy);

                if (person.WaitingForElevator && isTargetingOneFloor && !isAlreadyAffected && isAffectable)
                    affectedPassengers.Add(person);
            }

            //there are other passengers around the hall besides candy
            if (affectedPassengers.Count > 0) {
                blowKissTimer += Time.deltaTime;

                //blow a kiss in the air
                if (blowKissTimer >= blowKissPulse) {
                    if (currentJourney.Journey != null) currentJourney.Journey.Pause(true);
                    animationControl.Animate(StateManchine.SPECIAL);
                    blowKissTimer = 0;
                }
            }
            else if (currentJourney.Journey != null) currentJourney.Journey.Pause(false);
        }

        //resume a delayed commited journey
        if (delayingJourney && !animationControl.IsAnimating(StateManchine.SPECIAL)) {
            delayingJourney = false;
            CommitToJourney(journeyToProceed, CurrentFloor);
            journeyToProceed = default;
        }
    }

    /// <summary>
    /// Change all passengers' target floors to the same one as candy's.
    /// </summary>
    private void TriggerState_BlowKiss(TriggerState trigger) {
        foreach (Passenger person in affectedPassengers) {
            person.TargetFloorNum = TargetFloorNum;

            Vector3 personCenter = person.transform.position;
            personCenter.y += person.Dimension.y / 2;
            GameObject particles = Instantiate(heartParticles.gameObject);
            particles.transform.SetParent(person.transform);
            particles.transform.position = personCenter;
        }

        affectedPassengers.Clear();
    }

    public override void CommitToJourney(JourneyPath path, Floor floor) {
        if (animationControl.IsAnimating(StateManchine.SPECIAL)) {
            delayingJourney = true;
            journeyToProceed = path;
            return;
        }
        else base.CommitToJourney(path, floor);
    }

    protected override int[] GenerateTargetFloor() {
        int floor = StartingFloorNum;

        while (floor == StartingFloorNum)
            floor = FloorBuilder.Instance.GetRandomFloorNumber(true, false);

        return new int[] { floor };
    }

    public override bool CanBeSpawned() { return true; }

    public override void Destroy() {
        animationControl.Triggers[StateManchine.SPECIAL].OnFinish -= TriggerState_BlowKiss;
        base.Destroy();
    }
}