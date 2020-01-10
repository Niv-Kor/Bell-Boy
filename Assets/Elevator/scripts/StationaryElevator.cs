using UnityEngine;

public class StationaryElevator : MonoBehaviour
{
    [Header("Prefabs")]

    [Tooltip("The left door of the elevator.")]
    [SerializeField] protected Transform leftDoor;

    [Tooltip("The right door of the elevator.")]
    [SerializeField] protected Transform rightDoor;

    [Header("Doors")]

    [Tooltip("The speed of the doors.")]
    [SerializeField] protected float doorSpeed = .5f;

    [Tooltip("Percentage of the door that sticks out when its fully open.")]
    [SerializeField] [Range(0f, 100)] protected float stickOutPercent = 10;

    [Tooltip("The percentage of the total distance the doors should make,"
           + "That will be ignored in order the elevator ready for transportation.")]
    [SerializeField] [Range(.001f, 100)] public float tolerancePercent;

    [Header("Identity")]

    [Tooltip("Unique ID of the elevator.")]
    [SerializeField] public ElevatorID ID;

    private static readonly float HALFWAY_DOOR_SPEED_MULTIPLIER = 3;

    protected float initLeftX, initRightX;
    protected float currentDoorSpeed;
    protected float axleWidth;
    protected bool finishMovement;

    public bool IsOpen { get; private set; }
    public bool IsOpening { get; private set; }
    public bool IsClosing { get; private set; }
    public ElevatorSelectionArrow SelectionArrow { get; private set; }

    protected virtual void Start() {
        this.SelectionArrow = GetComponentInChildren<ElevatorSelectionArrow>();
        this.finishMovement = false;
        this.IsOpening = false;
        this.IsClosing = false;
        this.IsOpen = false;
        this.initLeftX = leftDoor.position.x;
        this.initRightX = rightDoor.position.x;
        this.currentDoorSpeed = doorSpeed;

        float leftDoorWidth = leftDoor.GetComponent<MeshRenderer>().bounds.size.x;
        float rightDoorWidth = rightDoor.GetComponent<MeshRenderer>().bounds.size.x;
        this.axleWidth = (leftDoorWidth + rightDoorWidth) / 2 * (1 - stickOutPercent / 100);
    }

    protected virtual void Update() {
        MoveDoors();
    }

    /// <summary>
    /// Open or close the elevator doors based on the most recent request.
    /// </summary>
    protected virtual void MoveDoors() {
        if (!IsOpening && !IsClosing) return;

        //calculate left door
        int leftDirection = IsOpening ? -1 : 1;
        float leftFinalXPos = IsOpening ? initLeftX + axleWidth * leftDirection : initLeftX;
        Vector3 leftCurrentPos = leftDoor.position;
        Vector3 leftFinalPos = new Vector3(leftFinalXPos, leftCurrentPos.y, leftCurrentPos.z);

        //calculate right door
        int rightDirection = IsOpening ? 1 : -1;
        float rightFinalXPos = IsOpening ? initRightX + axleWidth * rightDirection : initRightX;
        Vector3 rightCurrentPos = rightDoor.position;
        Vector3 rightFinalPos = new Vector3(rightFinalXPos, rightCurrentPos.y, rightCurrentPos.z);

        //fasten the speed if the doors are halfway
        if (VectorSensitivity.EffectivelyReached(rightDoor.position, rightFinalPos, axleWidth, 50))
            currentDoorSpeed = doorSpeed * HALFWAY_DOOR_SPEED_MULTIPLIER;
        else
            currentDoorSpeed = doorSpeed;

        //move doors
        leftDoor.position = Vector3.Lerp(leftCurrentPos, leftFinalPos, Time.deltaTime * currentDoorSpeed);
        rightDoor.position = Vector3.Lerp(rightCurrentPos, rightFinalPos, Time.deltaTime * currentDoorSpeed);

        //check if the door movement is effectively over
        if (!finishMovement && VectorSensitivity.EffectivelyReached(rightDoor.position, rightFinalPos, axleWidth, tolerancePercent)) {
            rightDoor.position = rightFinalPos;
            leftDoor.position = leftFinalPos;

            IsOpen = IsOpening;
            if (IsOpen) OnFullyOpen();
            else OnFullyClose();

            IsOpening = false;
            IsClosing = false;
            finishMovement = true;
        }
    }

    /// <summary>
    /// Open the elevator doors.
    /// This method is safe to use no matter the current state of the doors.
    /// </summary>
    public virtual void Open() {
        IsClosing = false;
        IsOpening = true;
        finishMovement = false;
        OnOpening();
    }

    /// <summary>
    /// Close the elevator doors.
    /// This method is safe to use no matter the current state of the doors.
    /// </summary>
    /// <returns>True if the elevator can be closed.</returns>
    public virtual bool Close() {
        IsOpening = false;
        IsClosing = true;
        IsOpen = false;
        finishMovement = false;
        OnClosing();
        return true;
    }

    /// <summary>
    /// Execute when the elevator is opening.
    /// </summary>
    protected virtual void OnOpening() {}

    /// <summary>
    /// Execute when the elevator is closing.
    /// </summary>
    protected virtual void OnClosing() {}

    /// <summary>
    /// Execute when the elevator is fully open.
    /// </summary>
    protected virtual void OnFullyOpen() {}

    /// <summary>
    /// Execute when the elevator is fully close.
    /// </summary>
    protected virtual void OnFullyClose() {}
}