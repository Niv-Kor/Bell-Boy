using System;
using UnityEngine;

public class SwipeDetector : MonoBehaviour
{
    [Tooltip("The minimum distance of a detected swipe.")]
    [SerializeField] private float minSwipeDistance = 20f;

    [Tooltip("True to detect a swipe on after the user lifts his finger off the screen.")]
    [SerializeField] private bool detectOnlyAfterRelease = false;

    private Vector2 fingerDownPosition, fingerUpPosition;

    public static event Action<SwipeData> OnSwipe = delegate { };

    private void Update() {
        if (Input.touchCount != 1) return;

        Touch touch = Input.touches[0];

        switch (touch.phase) {
            case TouchPhase.Began:
                fingerDownPosition = touch.position;
                fingerUpPosition = touch.position;
                break;

            case TouchPhase.Moved:
                if (!detectOnlyAfterRelease) {
                    fingerUpPosition = touch.position;
                    DetectSwipe();
                }
                break;

            case TouchPhase.Ended:
                fingerUpPosition = touch.position;
                DetectSwipe();
                fingerDownPosition = touch.position;
                break;
        }
    }

    private void DetectSwipe() {
        if (SwipeDistanceCheckMet()) {
            bool positiveSwipe;

            if (IsVerticalSwipe()) {
                positiveSwipe = fingerUpPosition.y > fingerDownPosition.y;
                SwipeDirection direction = positiveSwipe ? SwipeDirection.Up : SwipeDirection.Down;
                SendSwipe(direction);
            }
            else {
                positiveSwipe = fingerUpPosition.x > fingerDownPosition.x;
                SwipeDirection direction = positiveSwipe ? SwipeDirection.Right : SwipeDirection.Left;
                SendSwipe(direction);
            }

            fingerDownPosition = fingerUpPosition;
        }
    }

    private bool IsVerticalSwipe() {
        return VerticalDistance() > HorizontalDistance();
    }

    private bool SwipeDistanceCheckMet() {
        return VerticalDistance() > minSwipeDistance || HorizontalDistance() > minSwipeDistance;
    }

    private float VerticalDistance() {
        return Mathf.Abs(fingerUpPosition.y - fingerDownPosition.y);
    }

    private float HorizontalDistance() {
        return Mathf.Abs(fingerUpPosition.x - fingerDownPosition.x);
    }

    private void SendSwipe(SwipeDirection direction) {
        SwipeData swipeData = new SwipeData() {
            Direction = direction,
            StartPosition = fingerUpPosition,
            EndPosition = fingerDownPosition
        };

        OnSwipe(swipeData);
    }
}
