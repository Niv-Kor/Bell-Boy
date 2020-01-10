using TMPro;
using UnityEngine;

public class ElevatorLocation : MonoBehaviour
{
    [Header("Configurations")]

    [Tooltip("The ID of the elevator tell the direction of.")]
    [SerializeField] private ElevatorID ID;

    private MobileElevator elevator;
    private TextMeshPro textMesh;
    private int floor;

    private void Start() {
        this.textMesh = GetComponent<TextMeshPro>();
        this.elevator = ElevatorsManager.GetElevator(ID);
        this.floor = 0;
    }

    private void Update() {
        int elevFloor = elevator.CurrentFloorNum;

        if (elevFloor != floor) {
            textMesh.SetText("" + elevFloor);
            floor = elevFloor;
        }
    }
}