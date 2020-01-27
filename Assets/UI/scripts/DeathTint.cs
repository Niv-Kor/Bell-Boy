using UnityEngine;
using UnityEngine.UI;

public class DeathTint : MonoBehaviour
{
    [Tooltip("The amount of time (in seconds) it takes to tint in and out.")]
    [SerializeField] private float flashTime;

    private Image tintImage;
    private Color transparent, destColor;
    private float lerpedTime, threshold;
    private bool activated;

    private void Start() {
        this.tintImage = GetComponent<Image>();
        this.destColor = tintImage.color;
        this.transparent = tintImage.color;
        this.activated = false;
        this.threshold = flashTime / 2;
        destColor.a = .5f;
        transparent.a = 0;
    }

    private void Update() {
        if (!activated) return;

        lerpedTime += Time.deltaTime;
        if (lerpedTime < threshold) tintImage.color = Color.Lerp(transparent, destColor, lerpedTime / threshold);
        else tintImage.color = Color.Lerp(destColor, transparent, (lerpedTime - threshold) / threshold);

        //finish
        if (tintImage.color == transparent) activated = false;
    }

    /// <summary>
    /// Activate tint effect.
    /// </summary>
    public void Tint() {
        lerpedTime = 0;
        activated = true;
    }
}