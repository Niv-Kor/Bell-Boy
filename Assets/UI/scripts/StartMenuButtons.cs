using UnityEngine;
using UnityEngine.UI;

public class StartMenuButtons : MonoBehaviour
{
    [Tooltip("The time it takes the buttons to fade in (in seconds).\n")]
    [SerializeField] private float fadeInTime;

    private Text[] texts;
    private Image[] buttonFrames;
    private float[] textsAlphaValues, buttonFramesAlphaValues;
    private BinaryButton[] buttons;
    private UIButtonSFX[] soundEffects;
    private float timeLerped;

    private void Start() {
        this.timeLerped = fadeInTime;
        this.buttonFrames = GetComponentsInChildren<Image>();
        this.texts = GetComponentsInChildren<Text>();
        this.buttons = GetComponentsInChildren<BinaryButton>();
        this.soundEffects = GetComponentsInChildren<UIButtonSFX>();
        this.buttonFramesAlphaValues = new float[buttonFrames.Length];
        this.textsAlphaValues = new float[texts.Length];

        for (int i = 0; i < buttonFramesAlphaValues.Length; i++)
            buttonFramesAlphaValues[i] = buttonFrames[i].color.a;

        for (int i = 0; i < textsAlphaValues.Length; i++)
            textsAlphaValues[i] = texts[i].color.a;

        //let all buttons start as transparent
        foreach (Image image in buttonFrames) {
            Color buttonColor = image.color;
            buttonColor.a = 0;
            image.color = buttonColor;
        }

        //let all texts start as transparent
        foreach (Text text in texts) {
            Color textColor = text.color;
            textColor.a = 0;
            text.color = textColor;
        }

        foreach (BinaryButton button in buttons) button.ClickEnabled = false;
        foreach (UIButtonSFX SFX in soundEffects) SFX.ClickEnabled = false;
    }

    private void Update() {
        if (timeLerped < fadeInTime) {
            timeLerped += Time.deltaTime;

            //images
            for (int i = 0; i < buttonFrames.Length; i++) {
                Image image = buttonFrames[i];
                float originAlphaValue = buttonFramesAlphaValues[i];
                Color buttonColor = image.color;
                buttonColor.a = Mathf.Lerp(0, originAlphaValue, timeLerped / fadeInTime);
                image.color = buttonColor;
            }

            //texts
            for (int i = 0; i < texts.Length; i++) {
                Text text = texts[i];
                float originAlphaValue = textsAlphaValues[i];
                Color textColor = text.color;
                textColor.a = Mathf.Lerp(0, originAlphaValue, timeLerped / fadeInTime);
                text.color = textColor;
            }
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