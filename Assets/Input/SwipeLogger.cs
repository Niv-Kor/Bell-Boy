using UnityEngine;

public class SwipeLogger : MonoBehaviour
{
    private void Awake() {
        SwipeDetector.OnSwipe += SwipeDetector_OnSwipe;
    }

    private void SwipeDetector_OnSwipe(SwipeData data) {
        float horDistance = Mathf.Abs(data.EndPosition.x - data.StartPosition.x);
        float verDistance = Mathf.Abs(data.EndPosition.y - data.StartPosition.y);

        Debug.Log("Swipe in Direction: " + data.Direction + ". " +
                  "Horizontal distance: " + horDistance + ", " +
                  "Vectical Distance: " + verDistance + "\n");
    }
}