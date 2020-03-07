using UnityEngine;
using UnityEngine.UI;

public class GameLogo : MonoBehaviour
{
    [Tooltip("The time it takes until the logo starts bulking up (in seconds).")]
    [SerializeField] private float bulkDelay = 1;

    [Tooltip("The time it takes until the logo starts fading (in seconds).")]
    [SerializeField] private float fadeDelay = 5;

    [Tooltip("The time it takes to logo to pop into the screen (in seconds).")]
    [SerializeField] private float bulkTime = .5f;

    [Tooltip("The time it takes the logo to fade (in seconds).")]
    [SerializeField] private float fadeTime = .5f;

    [Tooltip("How big the logo should become when it bulks, as a function of its own size's percentage.")]
    [SerializeField] [Range(0f, 1f)] private float bulkPercent;

    private static readonly Color TRANSPARENT = new Color(0xFF, 0xFF, 0xFF, 0);
    private static readonly string BOING_SFX = "boing";

    private Image image;
    private Color originColor;
    private Vector3 originScale, fullBulk;
    private SoundMixer soundMixer;
    private RectTransform rectTransform;
    private float bulkTimer, fadeTimer, timeLerped;
    private bool playedSound, bulked, finish;

    private void Start() {
        this.bulkTimer = 0;
        this.fadeTimer = 0;
        this.timeLerped = 0;
        this.bulked = false;
        this.finish = false;
        this.playedSound = false;
        this.image = GetComponent<Image>();
        this.originColor = image.color;
        this.soundMixer = GetComponent<SoundMixer>();
        this.rectTransform = GetComponent<RectTransform>();
        this.originScale = rectTransform.localScale;

        float xBulk = originScale.x + originScale.x * bulkPercent;
        float yBulk = originScale.y + originScale.y * bulkPercent;
        float zBulk = originScale.z + originScale.z * bulkPercent;
        this.fullBulk = Vector3.Scale(Vector3.one, new Vector3(xBulk, yBulk, zBulk));
    }

    private void Update() {
        if (finish) return;

        //bulk up
        if (!bulked) {
            if (bulkTimer < bulkDelay) bulkTimer += Time.deltaTime;
            else {
                //make boing sound
                if (!playedSound) {
                    soundMixer.Activate(BOING_SFX);
                    playedSound = true;
                }

                float halfBulkTime = bulkTime / 2;
                timeLerped += Time.deltaTime;

                if (timeLerped < halfBulkTime) //bulk up
                    rectTransform.localScale = Vector3.Lerp(originScale, fullBulk, timeLerped / halfBulkTime);
                else //bulk down
                    rectTransform.localScale = Vector3.Lerp(fullBulk, originScale, (timeLerped - halfBulkTime) / halfBulkTime);

                if (timeLerped >= bulkTime) {
                    timeLerped = 0;
                    bulked = true;
                }
            }
        }

        //fade
        if (fadeTimer < fadeDelay) fadeTimer += Time.deltaTime;
        else {
            timeLerped += Time.deltaTime;
            image.color = Color.Lerp(originColor, TRANSPARENT, timeLerped / fadeTime);

            if (timeLerped >= fadeTime) finish = true;
        }
    }
}