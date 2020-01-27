using UnityEngine;

public class ElevatorStatistics : MonoBehaviour
{
    public enum TransitionDirection
    {
        Up, Down, Left, Right
    }

    [Tooltip("The elevator of which the statistics are about.")]
    [SerializeField] public ElevatorID ElevatorID;

    [Tooltip("The direction of which the panel is closing.")]
    [SerializeField] private TransitionDirection direction;

    [Tooltip("The percentage of the panel showing when its closed.")]
    [SerializeField] [Range(0f, 1f)] private float peekPercent;

    [Tooltip("The time it takes to panel to transit (open/close).")]
    [SerializeField] private float transitionTime;

    private RectTransform rectTransform;
    private Vector2 openCoord, closedCoord;
    private Vector2 startCoord, endCoord;
    private float lerpedTime;

    public bool InTransition { get; private set; }
    public bool IsOpen { get; private set; }

    private void Start() {
        this.rectTransform = GetComponent<RectTransform>();
        this.openCoord = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y);
        this.closedCoord = Vector2.Scale(Vector2.one, openCoord);
        this.lerpedTime = 0;
        this.IsOpen = true;
        this.InTransition = false;

        switch (direction) {
            case TransitionDirection.Up: closedCoord.y += Mathf.Abs(openCoord.y) * (2 - peekPercent); break;
            case TransitionDirection.Down: closedCoord.y -= Mathf.Abs(openCoord.y) * (2 - peekPercent); break;
            case TransitionDirection.Left: closedCoord.x -= Mathf.Abs(openCoord.x) * (2 - peekPercent); break;
            case TransitionDirection.Right: closedCoord.x += Mathf.Abs(openCoord.x) * (2 - peekPercent); break;
        }
    }

    private void Update() {
        if (!InTransition) return;

        lerpedTime += Time.deltaTime;
        float newX = Mathf.Lerp(startCoord.x, endCoord.x, lerpedTime / transitionTime);
        float newY = Mathf.Lerp(startCoord.y, endCoord.y, lerpedTime / transitionTime);
        rectTransform.anchoredPosition = new Vector2(newX, newY);

        //finish
        if (lerpedTime >= transitionTime) {
            IsOpen = !IsOpen;
            InTransition = false;
        }
    }

    /// <summary>
    /// Open or close the panel.
    /// </summary>
    public void Transit() {
        if (!InTransition) {
            startCoord = IsOpen ? openCoord : closedCoord;
            endCoord = IsOpen ? closedCoord : openCoord;
            InTransition = true;
            lerpedTime = 0;
        }
    }
}