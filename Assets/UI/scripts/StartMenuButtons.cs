using UnityEngine;
using UnityEngine.UI;

public class StartMenuButtons : MonoBehaviour
{
    [Tooltip("The time it takes the buttons to fade in (in seconds).\n")]
    [SerializeField] private float fadeInTime;

    private static readonly Color TRANSPARENT = new Color(0xFF, 0xFF, 0xFF, 0);
    private static readonly Color APPARENT_BUTTON = Color.white;
    private static readonly Color APPARENT_TEXT = Color.white;

    private Text[] texts;
    private Image[] buttons;
    private float timeLerped;

    private void Start() {
        this.timeLerped = fadeInTime;
        this.buttons = GetComponentsInChildren<Image>();
        this.texts = GetComponentsInChildren<Text>();

        //let all buttons start transparent
        foreach (Image image in buttons) image.color = TRANSPARENT;
        foreach (Text text in texts) text.color = TRANSPARENT;
    }

    private void Update() {
        if (timeLerped < fadeInTime) {
            timeLerped += Time.deltaTime;

            //images
            foreach (Image image in buttons)
                image.color = Color.Lerp(TRANSPARENT, APPARENT_BUTTON, timeLerped / fadeInTime);

            //text
            foreach (Text text in texts)
                text.color = Color.Lerp(TRANSPARENT, APPARENT_TEXT, timeLerped / fadeInTime);
        }
    }

    /// <summary>
    /// Let all buttons slowly fade in.
    /// </summary>
    public void FadeIn() { timeLerped = 0; }
}