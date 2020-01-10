using System.Collections.Generic;
using UnityEngine;

public class ElevatorSelectionArrow : MonoBehaviour
{
    [Tooltip("The speed of the arrow's bouncy movement.")]
    [SerializeField] private float bounceSpeed;

    [Tooltip("The amount of time (in seconds) it takes the arrow to reach the ground after launch.")]
    [SerializeField] private float fireDuration;

    [Tooltip("The distance from the arrow's maximum height to the arrow's minimum height.")]
    [SerializeField] private float verticalAxis;

    private static readonly Color TRANSPARENT = new Color(0, 0, 0, 0);
    private static readonly float BOUNCE_TOLERANCE = 10;
    private static readonly float FIRE_TOLERANCE = 1;

    private Floor floor;
    private ElevatorID ID;
    private SpriteRenderer spriteRenderer;
    private List<ElevatorSelectionArrow> parallelArrows;
    private Vector3 medianPoint, topBounce, bottomBounce, nextBouncePoint;
    private Color originColor;
    private bool activated, fire;
    private float fireTimer;

    private void Start() {
        this.floor = GetComponentInParent<Floor>();
        this.ID = GetComponentInParent<StationaryElevator>().ID;
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        this.originColor = spriteRenderer.color;
        spriteRenderer.enabled = false;

        this.medianPoint = transform.position;
        Vector3 epsilon = new Vector3(0, verticalAxis / 2, 0);
        this.topBounce = medianPoint + epsilon;
        this.bottomBounce = medianPoint - epsilon;
        this.nextBouncePoint = topBounce;
        this.fireTimer = 0;
        this.activated = false;
        this.fire = false;

        //trigger elevator call event
        floor.ElevatorButton.OnElevatorCall += ElevatorButton_OnElevatorCall;
    }

    private void Update() {
        //init parallel arrows list
        if (parallelArrows == null) FindParallelArrows();

        if (activated) {
            Bounce();

            //no one is waiting for the elevator
            if (floor.Passengers.Count == 0) {
                floor.ElevatorButton.Switch(false);
                Activate(false);

                foreach (ElevatorSelectionArrow arrow in parallelArrows)
                    arrow.Activate(false);
            }
        }
        //fire arrow downwards
        else if (fire) {
            if (spriteRenderer.color != TRANSPARENT || transform.position.y > 0) Launch();
            //finish
            else {
                fire = false;
                spriteRenderer.enabled = false;
            }
        }
    }

    private void OnMouseDown() {
        if (activated) OnTap();
    }

    /// <summary>
    /// Find all other arrows on the same floor.
    /// </summary>
    private void FindParallelArrows() {
        int floorNum = floor.FloorNumber;
        ElevatorSelectionArrow[] arrowsArr = FindObjectsOfType<ElevatorSelectionArrow>();
        List<ElevatorSelectionArrow> arrowsList = new List<ElevatorSelectionArrow>(arrowsArr);
        this.parallelArrows = arrowsList.FindAll((x) => x.floor.FloorNumber == floorNum && x.ID != ID);
    }

    /// <summary>
    /// Executes when the user taps on the arrow (via mouse click or touch).
    /// </summary>
    private void OnTap() {
        if (parallelArrows == null) FindParallelArrows();

        MobileElevator elevator = ElevatorsManager.GetElevator(ID);
        elevator.SendToFloor(floor.FloorNumber);

        Fire(); //launch the arrow to the ground
        foreach (ElevatorSelectionArrow arrow in parallelArrows)
            arrow.Activate(false);

        //desired elevator is already open at the current floor
        if (elevator.CurrentFloorNum == floor.FloorNumber && elevator.IsOpen)
            floor.ElevatorButton.Switch(false);
    }

    /// <summary>
    /// Process the arrow's bouncing animation.
    /// </summary>
    private void Bounce() {
        //bounce arrow
        transform.position = Vector3.Lerp(transform.position, nextBouncePoint, bounceSpeed * Time.deltaTime);

        //change direction
        if (VectorSensitivity.EffectivelyReached(transform.position, nextBouncePoint, verticalAxis / 2, BOUNCE_TOLERANCE))
            nextBouncePoint = Equals(nextBouncePoint, bottomBounce) ? topBounce : bottomBounce;
    }

    /// <summary>
    /// Process the arrow's fire animation.
    /// </summary>
    private void Launch() {
        fireTimer += Time.deltaTime;
        float lerpSpeed = fireTimer / fireDuration;

        //fire arrow downwards and change its color
        Vector3 floorPoint = new Vector3(transform.position.x, 0, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, floorPoint, lerpSpeed);
        spriteRenderer.color = Color.Lerp(spriteRenderer.color, TRANSPARENT, lerpSpeed);

        //cancel effect
        if (VectorSensitivity.EffectivelyReached(transform.position, floorPoint, medianPoint.y, FIRE_TOLERANCE)) {
            fire = false;
            fireTimer = 0;
        }
    }

    /// <summary>
    /// Bounce the arrow up and down, pointing at the elevator entrance.
    /// </summary>
    public void Activate(bool flag) {
        if (flag) {
            transform.position = medianPoint;
            nextBouncePoint = topBounce;
            spriteRenderer.color = originColor;
            activated = true;
        }

        spriteRenderer.enabled = flag;
    }

    /// <summary>
    /// Fire the arrow rapidly downwards and disable it.
    /// </summary>
    public void Fire() {
        activated = false;
        fire = true;
    }

    private void ElevatorButton_OnElevatorCall(ElevatorButton button) { Activate(true); }
}