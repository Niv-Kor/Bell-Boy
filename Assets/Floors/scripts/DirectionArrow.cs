using UnityEngine;

public class DirectionArrow : MonoBehaviour
{
    [Header("Prefabs")]

    [Tooltip("A sprite of an arrow pointing upwards.")]
    [SerializeField] private Sprite upArrow;

    [Tooltip("A sprite of an arrow pointing downwards.")]
    [SerializeField] private Sprite downArrow;

    [Header("Configurations")]

    [Tooltip("The ID of the elevator tell the direction of.")]
    [SerializeField] private ElevatorID ID;

    private MobileElevator elevator;
    private SpriteRenderer spriteRenderer;
    private ElevatorDirection direction;

    private void Start() {
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        this.elevator = ElevatorsManager.GetElevator(ID);
    }

    private void Update() {
        ElevatorDirection elevDirection = elevator.Direction;
        if (elevDirection == direction) return;

        switch (elevDirection) {
            case ElevatorDirection.Up: spriteRenderer.sprite = upArrow; break;
            case ElevatorDirection.Down: spriteRenderer.sprite = downArrow; break;
            case ElevatorDirection.Still: spriteRenderer.sprite = null; break;
        }

        direction = elevDirection;
    }
}