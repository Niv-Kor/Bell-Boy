using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Floor : MonoBehaviour
{
    [Header("Prefabs")]

    [Tooltip("The object of the ceiling.")]
    [SerializeField] private GameObject ceilingConcrete;

    [Tooltip("The object of the floor.")]
    [SerializeField] private GameObject floorConcrete;

    [Tooltip("The left elevator entrance of the floor.")]
    [SerializeField] private GameObject leftEntrance;

    [Tooltip("The right elevator entrance of the floor.")]
    [SerializeField] private GameObject rightEntrance;

    [Tooltip("The button that calls the elevator.")]
    [SerializeField] private GameObject elevatorButton;

    [Tooltip("Left shatterable window.")]
    [SerializeField] public GameObject LeftWindow;

    [Tooltip("Right shatterable window.")]
    [SerializeField] public GameObject RightWindow;

    [Tooltip("The number above the elavator entrances.")]
    [SerializeField] private TextMeshPro floorText;

    [Header("Capacity")]

    [Tooltip("Maximum people waiting for the elevator at a single moment.")]
    [SerializeField] public int MaxCapacity;

    private string floorName;

    public int FloorNumber { get; set; }
    public Vector3 Volume { get; private set; }
    public float IndoorHeight { get; private set; }
    public bool ElevatorBeingCalled { get; set; }
    public BoxCollider WaitingHallContainer { get; private set; }
    public List<Passenger> Passengers { get; } = new List<Passenger>();
    public ElevatorButton ElevatorButton { get { return GetComponentInChildren<ElevatorButton>(); } }

    public string FloorName {
        get { return floorName; }
        set {
            floorName = value;
            if (floorText != null) floorText.SetText(value);
        }
    }

    public float FloorHeight {
        get {
            BoxCollider floorBox = floorConcrete.GetComponent<BoxCollider>();
            return floorConcrete.transform.position.y - floorBox.bounds.extents.y; 
        }
    }

    public float RoofHeight {
        get {
            BoxCollider ceilingBox = ceilingConcrete.GetComponent<BoxCollider>();
            return ceilingConcrete.transform.position.y + ceilingBox.bounds.extents.y;
        }
    }

    private void Awake() {
        BoxCollider ceilingBox = ceilingConcrete.GetComponent<BoxCollider>();
        BoxCollider floorBox = floorConcrete.GetComponent<BoxCollider>();
        this.WaitingHallContainer = GetComponent<BoxCollider>();

        float width = Mathf.Max(ceilingBox.size.x, floorBox.size.x);
        float depth = Mathf.Max(ceilingBox.size.z, floorBox.size.z);
        float height = RoofHeight - FloorHeight;
        this.Volume = new Vector3(width, height, depth);

        if (WaitingHallContainer != null) {
            float hallCenter = WaitingHallContainer.center.y;
            float hallExtent = WaitingHallContainer.bounds.extents.y;
            this.IndoorHeight = transform.position.y + hallCenter - hallExtent;
        }
    }

    /// <param name="ID">The ID of the entrance's elevator</param>
    /// <returns>The specified entrance.</returns>
    public StationaryElevator GetEntrance(ElevatorID id) {
        if (leftEntrance == null || rightEntrance == null) return null;

        switch (id) {
            case ElevatorID.E1: return leftEntrance.GetComponent<StationaryElevator>();
            case ElevatorID.E2: return rightEntrance.GetComponent<StationaryElevator>();
            default: return null;
        }
    }

    /// <summary>
    /// Check if a point is inside the elevators waiting hall.
    /// </summary>
    /// <param name="point">The point to check</param>
    /// <returns>True if the point is in the waiting hall.</returns>
    public bool IsInWaitingHall(Vector3 point) {
        return WaitingHallContainer.bounds.Contains(point);
    }

    /// <returns>True if the floor is at full capacity of people.</returns>
    public bool IsAtFullCapacity() { return Passengers.Count >= MaxCapacity; }

    /// <summary>
    /// Check if the floor has an available elevator for a certain direction.
    /// </summary>
    /// <param name="direction">The target direction to check</param>
    /// <returns>True if the floor contains at least one available elevator for the specified taget direction.</returns>
    public bool HasElevator(ElevatorDirection direction) {
        foreach (MobileElevator elevator in ElevatorsManager.GetAllElevators()) {
            bool atFloor = elevator.CurrentFloorNum == FloorNumber;
            bool isAvailable = elevator.IsOpening || elevator.IsOpen;
            bool sameDirection = elevator.Direction == ElevatorDirection.Still || elevator.Direction == direction;
            if (atFloor && isAvailable && sameDirection) return true;
        }

        return false;
    }

    /// <summary>
    /// Update the floor's location.
    /// </summary>
    /// <param name="position">The new location of the floor</param>
    public void UpdateLocation(Vector3 position) {
        transform.position = position;

        if (WaitingHallContainer != null) {
            float hallCenter = WaitingHallContainer.center.y;
            float hallExtent = WaitingHallContainer.bounds.extents.y;
            this.IndoorHeight = transform.position.y + hallCenter - hallExtent;
        }
    }

    private void OnDrawGizmos() {
        if (FloorNumber != 2) return;

        float z = transform.position.z - Volume.z / 4;

        Vector3 floor = new Vector3(transform.position.x, FloorHeight, z);
        Vector3 roof = new Vector3(transform.position.x, RoofHeight, z);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(floor, roof);

        Vector3 indoor = new Vector3(transform.position.x + .5f, IndoorHeight, z);
        roof = new Vector3(transform.position.x + .5f, RoofHeight, z);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(indoor, roof);
    }
}