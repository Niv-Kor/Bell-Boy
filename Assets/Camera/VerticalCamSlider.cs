using UnityEngine;

public class VerticalCamSlider : MonoBehaviour
{
    [Tooltip("The minimum height the game camera can reach.")]
    [SerializeField] public float minHeight;

    [Tooltip("The maximum height the game camera can reach.")]
    [SerializeField] public float maxHeight;

    [Tooltip("The speed of the camera's slide upon swiping (on mobile).")]
    [SerializeField] private float swipeSpeed = .1f;

    [Tooltip("The speed of the camera's slide upon mouse wheel scrolling.")]
    [SerializeField] private float scrollSpeed = 1f;

    [Tooltip("The height value that changes with each scroll.")]
    [SerializeField] private float scrollIntensity = 10;

    private static readonly string SCROLL_AXIS = "Mouse ScrollWheel";

    private Vector3 nextPos;
    private float speed;

    private void Awake() {
        Vector3 currentPos = transform.position;
        transform.position = new Vector3(currentPos.x, minHeight, currentPos.z);
        this.nextPos = transform.position;
        this.speed = 0;

        SwipeDetector.OnSwipe += SwipeDetector_OnSwipe;
    }

    private void Update() {
        OnScroll();

        //move the camera
        if (transform.position != nextPos) {
            Vector3 newPos = Vector3.Lerp(transform.position, nextPos, Time.deltaTime * speed);

            //clamp y value
            if (newPos.y < minHeight) newPos.y = minHeight;
            else if (newPos.y > maxHeight) newPos.y = maxHeight;
            transform.position = newPos;
        }
    }

    private void OnScroll() {
        float scrollMovement = Input.GetAxis(SCROLL_AXIS);

        if (Mathf.Abs(scrollMovement) > 0) {
            speed = scrollSpeed;
            nextPos = transform.position + new Vector3(0, scrollMovement * scrollIntensity, 0);
        }
    }

    private void SwipeDetector_OnSwipe(SwipeData data) {
        bool isUp = data.Direction == SwipeDirection.Up;
        bool isDown = data.Direction == SwipeDirection.Down;

        if (!isUp && !isDown) return;
        //calculate the camera's next position
        else {
            float slideMovement = data.EndPosition.y - data.StartPosition.y;
            nextPos = transform.position + new Vector3(0, slideMovement, 0);
            speed = swipeSpeed;
        }
    }
}