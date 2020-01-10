using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PassengerNavigator : MonoBehaviour
{
    [Tooltip("The layers that the cursor should recognize as passenger and as elevator object.")]
    [SerializeField] private LayerMask clickableLayers;

    private static readonly float MAX_RAY_LENGTH = 100;

    private Passenger passenger;
    private BoxCollider container;
    private MobileElevator selectedElevator;
    private LineRenderer lineRenderer;
    private IHighlightable passengerHighlight;
    private bool hold;

    private void Start() {
        this.lineRenderer = GetComponent<LineRenderer>();
        this.passenger = GetComponent<Passenger>();
        this.container = GetComponent<BoxCollider>();
        this.passengerHighlight = GetComponent<PassengerHighlighter>();
        SwipeDetector.OnSwipe += SwipeDetector_OnSwipe;
    }

    private void SwipeDetector_OnSwipe(SwipeData data) {
        ///TEMP
    }

    private void Update() {
        if (hold && !passenger.WaitingForElevator) CancelAllImpact();
    }

    void OnGUI() {
        if (!passenger.WaitingForElevator) return;
        EventType mouseAction = Event.current.type;

        if (mouseAction == EventType.MouseDown) OnMousePress();
        else if (mouseAction == EventType.MouseUp) OnMouseRelease();
        else if (mouseAction == EventType.MouseDrag || hold) OnDragMouse();
    }

    /// <summary>
    /// Executed when the mouse is pressed.
    /// </summary>
    private void OnMousePress() {
        if (!hold && MouseOnPassenger(MousePosition(true))) {
            Vector3 passengerPosition = PassengerCenter();
            lineRenderer.SetPosition(0, passengerPosition);
            lineRenderer.SetPosition(1, passengerPosition);
            passengerHighlight.Highlight(true);
            lineRenderer.enabled = true;
            hold = true;
        }
    }

    /// <summary>
    /// Executed when the mouse is released.
    /// </summary>
    private void OnMouseRelease() {
        if (selectedElevator != null) selectedElevator.ReceivePassenger(passenger);
        CancelAllImpact();
    }

    /// <summary>
    /// Executed when the mouse is being dragged.
    /// </summary>
    private void OnDragMouse() {
        if (!hold) return;

        Vector3 endPosition;
        Vector3 mousePosition = MousePosition(false);
        MobileElevator candidateElevator = MouseOnElevator(mousePosition);

        if (candidateElevator == null) {
            HighlightEntrance(selectedElevator, false);
            endPosition = mousePosition;
        }
        else {
            if (candidateElevator != selectedElevator) {
                HighlightEntrance(selectedElevator, false);
                HighlightEntrance(candidateElevator, true);
            }

            endPosition = candidateElevator.transform.position;
            endPosition.z = candidateElevator.Entrance.transform.position.z;
        }

        selectedElevator = candidateElevator;
        lineRenderer.SetPosition(0, PassengerCenter());
        lineRenderer.SetPosition(1, endPosition);
    }

    /// <summary>
    /// Cancel all highlights and line rendering.
    /// </summary>
    private void CancelAllImpact() {
        HighlightEntrance(selectedElevator, false);
        passengerHighlight.Highlight(false);
        lineRenderer.enabled = false;
        selectedElevator = null;
        hold = false;
    }

    /// <summary>
    /// Get mouse position on the screen in world coordinates.
    /// This method gets the position on a flat transparent plane at the front of the scene,
    /// but is also able to get the world coordinates of predefined clickable layers.
    /// Set the "clickableObjectsDepth" parameter to 'True' in order to be able to get the depth
    /// coordinates of objects at the mentioned layers, when the mouse is pointing at them.
    /// </summary>
    /// <param name="clickableObjectsDepth">True to get the real depth coordinates of clickable objects</param>
    /// <returns>The current mouse position.</returns>
    private Vector3 MousePosition(bool clickableObjectsDepth) {
        RaycastHit hit;
        Ray ray = Monitor.Instance.MainCamera.ScreenPointToRay(Input.mousePosition);

        if (!clickableObjectsDepth) Physics.Raycast(ray, out hit, MAX_RAY_LENGTH);
        else Physics.Raycast(ray, out hit, MAX_RAY_LENGTH, clickableLayers);
        return hit.point;
    }

    /// <param name="mousePos">Current mouse position</param>
    /// <returns>True if the mouse is pointing at the passenger.</returns>
    private bool MouseOnPassenger(Vector3 mousePos) {
        return container.bounds.Contains(mousePos);
    }

    /// <param name="mousePos">Current mouse position</param>
    /// <returns>True if the mouse is pointing at an available elevator in the current floor.</returns>
    private MobileElevator MouseOnElevator(Vector3 mousePos) {
        List<MobileElevator> elevators = ElevatorsManager.GetAllElevators();
        List<MobileElevator> targetElevators = new List<MobileElevator>();
        ElevatorDirection passengerDirection = passenger.Direction;

        foreach (MobileElevator elevator in elevators)
            if (elevator.CurrentFloorNum == passenger.CurrentFloorNum &&
               (elevator.IsOpen || elevator.IsOpening) &&
               (elevator.Direction == ElevatorDirection.Still ||
                elevator.Direction == passengerDirection))
                targetElevators.Add(elevator);

        foreach (MobileElevator elevator in targetElevators) {
            mousePos.z = elevator.transform.position.z;
            if (elevator.Container.bounds.Contains(mousePos)) return elevator;
        }

        return null;
    }

    /// <returns>The coordinates in the center of the passenger.</returns>
    private Vector3 PassengerCenter() {
        Vector3 passengerPos = passenger.transform.position;
        passengerPos.y += passenger.Dimension.y / 2;
        return passengerPos;
    }

    /// <summary>
    /// Highlight or revert the color of an entrance.
    /// </summary>
    /// <param name="elevator">The elevator that the entrance belongs to</param>
    /// <param name="flag">True to highlight or false to revert</param>
    private void HighlightEntrance(MobileElevator elevator, bool flag) {
        if (elevator == null) return;

        IHighlightable entranceHighlight = elevator.Entrance.GetComponent<EntranceHighlighter>();
        entranceHighlight.Highlight(flag);
    }
}