using UnityEngine;

public class PitCable : MonoBehaviour
{
    [Tooltip("The elevator that's attached to this cable.")]
    [SerializeField] private ElevatorID elevatorID;

    private static readonly float ELEVATOR_SPEED_RATIO = 10;

    private MobileElevator elevator;
    private float speed;

    private void Start() {
        this.elevator = ElevatorsManager.GetElevator(elevatorID);
        this.speed = elevator.perFloorTime * ELEVATOR_SPEED_RATIO;
    }

    private void Update() {
        if (elevator.IsMoving) transform.Rotate(0, speed, 0);
    }
}