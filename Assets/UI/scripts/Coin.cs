using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Timing")]

    [Tooltip("The time it takes the coin to be tossed into the tips jar.")]
    [SerializeField] private float tossTime;

    [Tooltip("The time it takes the coin to be tossed into the tips jar.")]
    [SerializeField] private float shrinkTime;

    private static readonly float DISNTANCE_IGNORED = .1f;
    private static readonly float SIZE_IGNORED = .01f;
    private static readonly string TOSS_SOUND = "toss";
    private static readonly string DROP_SOUND = "drop";

    private TipsJar jar;
    private CoinsPool pool;
    private SoundMixer soundMixer;
    private float tossLerpedTime, shrinkLerpedTime;
    private bool toss, shrink;
    private Vector3 originScale;
    private Vector3 startingPosition;
    
    public int Value { get; set; }

    private void Awake() {
        this.jar = ScoreSystem.Instance.TipsJar;
        this.pool = ScoreSystem.Instance.CoinsPool;
        this.soundMixer = GetComponent<SoundMixer>();
        this.originScale = transform.localScale;
    }

    private void Reset() {
        this.tossLerpedTime = 0;
        this.shrinkLerpedTime = 0;
    }

    private void Update() {
        if (toss) UpdateToss();
        if (shrink) UpdateShrink();
    }

    /// <summary>
    /// Update the toss process where the coin flies towards the tips jar.
    /// </summary>
    private void UpdateToss() {
        tossLerpedTime += Time.deltaTime;
        Vector3 endPosition = jar.transform.position;
        transform.position = Vector3.Lerp(startingPosition, endPosition, tossLerpedTime / tossTime);

        if (VectorSensitivity.EffectivelyReached(transform.position, endPosition, DISNTANCE_IGNORED)) {
            soundMixer.Activate(DROP_SOUND);
            toss = false;
            shrink = true;
        }
    }

    /// <summary>
    /// Update the shrink process where the coin shrinks to size zero.
    /// </summary>
    private void UpdateShrink() {
        shrinkLerpedTime += Time.deltaTime;
        transform.localScale = Vector3.Lerp(originScale, Vector3.zero, shrinkLerpedTime / shrinkTime);

        if (VectorSensitivity.EffectivelyReached(transform.localScale, Vector3.zero, SIZE_IGNORED)) {
            jar.TotalValue += Value;
            shrink = false;
            pool.Free(this);
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Let the coin be tossed towards the jar.
    /// </summary>
    /// <param name="tossFrom">The position from which the coin should be tossed</param>
    public void Toss(Vector3 tossFrom) {
        Reset();
        soundMixer.Activate(TOSS_SOUND);
        transform.localScale = originScale;
        transform.position = tossFrom;
        startingPosition = tossFrom;
        toss = true;
    }
}