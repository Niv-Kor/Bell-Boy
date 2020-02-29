using UnityEngine;
using UnityEngine.UI;

public class ScreenFlicker : MonoBehaviour
{
    [Tooltip("A set of flicker images to switch between.")]
    [SerializeField] private Sprite[] images;

    [Tooltip("The time (in seconds) it takes one flicker image to change.")]
    [SerializeField] private float perImageTime = .2f;

    [Tooltip("The maximum amount of time (in seconds) the whole flicker may take.")]
    [SerializeField] private float maximumFlickerTime = .5f;

    private static readonly Color TRANSPARENT = new Color(0xFF, 0xFF, 0xFF, 0);

    private Image image;
    private bool flickering;
    private int imageCounter;
    private float flickerTimer, imageTimer;

    private void Start() {
        this.image = GetComponent<Image>();
        this.flickering = false;
        image.color = TRANSPARENT;
    }

    private void Update() {
        if (!flickering) return;

        //time the whole flicker session
        flickerTimer += Time.deltaTime;
        if (imageCounter >= images.Length || flickerTimer >= maximumFlickerTime) Stop();
        else {
            image.sprite = images[imageCounter];

            //time image changing
            imageTimer += Time.deltaTime;

            if (imageTimer >= perImageTime) {
                imageTimer = 0;
                imageCounter++;
            }
        }
    }

    /// <summary>
    /// Flicker the screen.
    /// </summary>
    public void Flicker() {
        flickering = true;
        image.color = Color.white;
        imageCounter = 0;
        flickerTimer = 0;
    }

    /// <summary>
    /// Stop flickering the screen.
    /// </summary>
    public void Stop() {
        image.sprite = null;
        flickering = false;
        image.color = TRANSPARENT;
    }
}
