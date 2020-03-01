using UnityEngine;
using UnityEngine.UI;

public class StartMenuButtons : MonoBehaviour
{
    [Tooltip("The time it takes the buttons to fade in (in seconds).\n")]
    [SerializeField] private float fadeInTime;

    private static readonly Color TRANSPARENT = new Color(0xFF, 0xFF, 0xFF, 0);
    private static readonly Color APPARENT_TEXT = Color.white;

    private Text[] texts;
    private Image[] buttonFrames;
    private BinaryButton[] buttons;
    private UIButtonSFX[] soundEffects;
    private float timeLerped;

    private void Start() {
        this.timeLerped = fadeInTime;
        this.buttonFrames = GetComponentsInChildren<Image>();
        this.texts = GetComponentsInChildren<Text>();
        this.buttons = GetComponentsInChildren<BinaryButton>();
        this.soundEffects = GetComponentsInChildren<UIButtonSFX>();

        //let all buttons start as transparent
        foreach (Image image in buttonFrames) {
            Color buttonColor = image.color;
            buttonColor.a = 0;
            image.color = buttonColor;
        }

        foreach (Text text in texts) text.color = TRANSPARENT;
        foreach (BinaryButton button in buttons) button.ClickEnabled = false;
        foreach (UIButtonSFX SFX in soundEffects) SFX.ClickEnabled = false;
    }

    private void Update() {
        if (timeLerped < fadeInTime) {
            timeLerped += Time.deltaTime;

            //images
            foreach (Image image in buttonFrames) {
                Color buttonColor = image.color;
                buttonColor.a = Mathf.Lerp(0, 1, timeLerped / fadeInTime);
                image.color = buttonColor;
            }

            //text
            foreach (Text text in texts)
                text.color = Color.Lerp(TRANSPARENT, APPARENT_TEXT, timeLerped / fadeInTime);
        }
    }

    /// <summary>
    /// Let all buttons slowly fade in.
    /// </summary>
    public void FadeIn() {
        timeLerped = 0;

        //enable button's function and SFX
        foreach (BinaryButton button in buttons) button.ClickEnabled = true;
        foreach (UIButtonSFX SFX in soundEffects) SFX.ClickEnabled = true;
    }
}