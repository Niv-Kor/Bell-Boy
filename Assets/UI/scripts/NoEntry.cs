using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(SoundMixer))]
public class NoEntry : MonoBehaviour
{
    [Tooltip("The sprite to show when going up is forbidden.")]
    [SerializeField] private Sprite upSprite;

    [Tooltip("The sprite to show when going down is forbidden.")]
    [SerializeField] private Sprite downSprite;

    [Tooltip("The time it takes the icon to fade away after popping.")]
    [SerializeField] private float fadeTime = 1;

    private static readonly string ERROR_TUNE = "error";
    private static readonly Color TRANSPARENT = new Color(0xFF, 0xFF, 0xFF, 0);

    private SoundMixer soundMixer;
    private SpriteRenderer spriteRender;
    private float timeLerped;
    private bool fade;

    private void Start() {
        this.soundMixer = GetComponent<SoundMixer>();
        this.spriteRender = GetComponent<SpriteRenderer>();
        this.timeLerped = 0;
        this.fade = false;
    }

    private void Update() {
        if (fade) {
            timeLerped += Time.deltaTime;
            spriteRender.color = Color.Lerp(spriteRender.color, TRANSPARENT, timeLerped / fadeTime);

            //finish fading
            if (timeLerped >= fadeTime) {
                timeLerped = 0;
                fade = false;
            }
        }
    }

    /// <summary>
    /// Pop up an icon that symbols the entry to the elevator is forbidden.
    /// </summary>
    /// <param name="direction">The direction in which the entry is forbidden</param>
    public void ForbidEntry(ElevatorDirection direction) {
        Sprite sprite;

        switch (direction) {
            case ElevatorDirection.Up: sprite = upSprite; break;
            case ElevatorDirection.Down: sprite = downSprite; break;
            default: return;
        }

        soundMixer.Activate(ERROR_TUNE);
        spriteRender.sprite = sprite;
        spriteRender.color = Color.white;
        timeLerped = 0;
        fade = true;
    }
}