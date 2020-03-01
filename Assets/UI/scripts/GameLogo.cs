using UnityEngine;
using UnityEngine.UI;

public class GameLogo : MonoBehaviour
{
    [Tooltip("The time it takes until the logo starts fading (in seconds).\n")]
    [SerializeField] private float fadeDelay;

    [Tooltip("The time it takes the logo to fade (in seconds).\n")]
    [SerializeField] private float fadeTime;

    private static readonly Color TRANSPARENT = new Color(0xFF, 0xFF, 0xFF, 0);

    private Image image;
    private Color originColor;
    private float fadeTimer, timeLerped;
    private bool finish;

    private void Start() {
        this.fadeTimer = 0;
        this.timeLerped = 0;
        this.finish = false;
        this.image = GetComponent<Image>();
        this.originColor = image.color;
    }

    private void Update() {
        if (finish) return;

        if (fadeTimer < fadeDelay) fadeTimer += Time.deltaTime;
        //start fading
        else {
            timeLerped += Time.deltaTime;
            image.color = Color.Lerp(originColor, TRANSPARENT, timeLerped / fadeTime);

            if (timeLerped >= fadeTime) finish = true;
        }
    }
}