using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Floor))]
public class FloorPlanBlueprint : RoutesPool
{
    [Header("Decoration")]

    [Tooltip("Spots at which decorations can be placed.")]
    [SerializeField] public List<Vector3> DecorationSpots;

    private static readonly float WAITING_HALL_WIDTH_MULTIPLIER = .9f;
    private static readonly float WAITING_HALL_DEVIATION_PERCENT = .2f;

    private Floor floor;

    protected override void Start() {
        this.floor = GetComponent<Floor>();
        base.Start();
    }

    protected override void GeneratePaths() {
        base.GeneratePaths();

        int index = 0;
        foreach (Queue<Vector3> path in freePaths) {
            //add the last point at the elevators' waiting hall
            Vector3 floorPos = floor.transform.position;
            Vector3 hallCenter = floor.WaitingHallContainer.center;
            Vector3 hallSize = floor.WaitingHallContainer.bounds.size;
            float deviation;

            //x position
            float hallWidth = hallSize.x * WAITING_HALL_WIDTH_MULTIPLIER;
            float mostLeftX = floorPos.x + hallCenter.x - hallWidth / 2;
            float spaceBlock = hallWidth / floor.MaxCapacity;
            deviation = Random.Range(-1f, 1f) * spaceBlock * WAITING_HALL_DEVIATION_PERCENT;
            float waitingHallX = mostLeftX + spaceBlock * (index++ + .5f) + deviation;

            //y position
            float waitingHallY = floor.IndoorHeight;

            //z position
            deviation = Random.Range(-1f, 1f);
            float waitingHallZ = floorPos.z + hallCenter.z + (hallSize.z / 2) * deviation;

            Vector3 lastPoint = new Vector3(waitingHallX, waitingHallY, waitingHallZ);
            path.Enqueue(lastPoint);
        }
    }

    protected override int MaxRoutes() { return floor.MaxCapacity; }
    protected override Vector3 RelativePoint() { return floor.transform.position; }
    protected override float RelativeHeight() { return floor.IndoorHeight; }
}