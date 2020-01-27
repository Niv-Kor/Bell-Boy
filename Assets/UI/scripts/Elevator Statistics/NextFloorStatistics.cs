using UnityEngine;

public class NextFloorStatistics : ElevatorStatisticsScreen
{
    [Header("Prefabs")]

    [Tooltip("The sprite component.")]
    [SerializeField] private SpriteRenderer directionSprite;

    [Header("Sprites")]

    [Tooltip("A sprite of an arrow pointing upwards.")]
    [SerializeField] private Sprite upArrow;

    [Tooltip("A sprite of an arrow pointing downwards.")]
    [SerializeField] private Sprite downArrow;

    protected override void UpdateScreen() {
        int nextFloor = elevator.NextFloorNum;
        string nextFloorStr = (nextFloor != elevator.CurrentFloorNum) ? nextFloor.ToString() : "";
        primaryText[0].text = nextFloorStr;

        Sprite direction;
        switch (elevator.Direction) {
            case ElevatorDirection.Up: direction = upArrow; break;
            case ElevatorDirection.Down: direction = downArrow; break;
            default: direction = null; break;
        }

        directionSprite.sprite = direction;
    }
}