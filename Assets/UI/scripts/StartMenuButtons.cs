using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class StartMenuButtons : MonoBehaviour
{
    [Tooltip("The time it takes the buttons to fade in (in seconds).")]
    [SerializeField] private float fadeInTime;

    private CanvasGroup canvasGroup;
    private float timeLerped;
    private bool transition;

    private void Start() {
        this.canvasGroup = GetComponent<CanvasGroup>();
        this.timeLerped = 0;
        this.transition = false;

        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }

    private void Update() {
        if (transition) {
            if (timeLerped < fadeInTime) {
                timeLerped += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(0, 1, timeLerped / fadeInTime);
            }
            else {
                //enable button's function and SFX
                canvasGroup.blocksRaycasts = true;
                canvasGroup.interactable = true;
                transition = false;
            }
        }
    }

    /// <summary>
    /// Let all buttons slowly fade in.
    /// </summary>
    public void FadeIn() { transition = true; }
}