using TMPro;
using UnityEngine;

public class TimerRunner : MonoBehaviour
{
    [Header("Prefabs")]

    [Tooltip("The textual value of the timer.")]
    [SerializeField] private TextMeshProUGUI value;

    [Tooltip("The pin of the timer.")]
    [SerializeField] private GameObject pinGear;

    [Tooltip("The sprites that represent each of the seconds passed.")]
    [SerializeField] private Sprite[] secondsClearance;

    [Header("Configuration")]

    [Tooltip("True to run as soon as the scene loads.")]
    [SerializeField] private bool runOnAwake = true;

    private static readonly int WHOLE_ROUND = 60;
    private static readonly int MAX_MINUTES = 99;
    private static readonly int MAX_SECONDS = 59;
    private static readonly int GEAR_SPIN_ANGLE = -6;
    private static readonly int PIN_OVERJUMP = -5;
    private static readonly float FIX_PIN_AFTER = 9f;

    private SpriteRenderer spriteRender;
    private float score;
    private int prevSeconds;
    private bool running, pinStablized;

    public float Timer { get; set; }
    public int Minutes { get; private set; }
    public int Seconds { get; private set; }
    public int Milliseconds { get; private set; }

    private void Start() {
        this.spriteRender = GetComponent<SpriteRenderer>();
        this.prevSeconds = Seconds;
        this.pinStablized = true;
        this.running = false;
        this.score = 0;
        if (runOnAwake) Run();
    }

    private void Update() {
        if (running) {
            //update value
            Timer += Time.deltaTime;
            string timerStr = GameTimeValueToString(Timer);
            value.text = timerStr;

            //stablize pin
            if (!pinStablized && Milliseconds >= FIX_PIN_AFTER) {
                pinGear.transform.Rotate(0, 0, -PIN_OVERJUMP);
                pinStablized = true;
            }

            //update sprite
            if (prevSeconds != Seconds) {
                prevSeconds = Seconds;
                spriteRender.sprite = secondsClearance[Seconds];

                //make pin jump a little too far
                pinGear.transform.Rotate(0, 0, GEAR_SPIN_ANGLE + PIN_OVERJUMP);
                pinStablized = false;
            }
        }
    }

    /// <summary>
    /// Convert the timer value to a string representation.
    /// </summary>
    /// <param name="time">The total time passed by the timer</param>
    /// <returns>A string representation of the time passed.</returns>
    private string GameTimeValueToString(float time) {
        int allSeconds = (int) time;
        Minutes = allSeconds / WHOLE_ROUND;
        Seconds = allSeconds - (WHOLE_ROUND * Minutes);
        Milliseconds = (int) ((time - allSeconds) * 100);

        if (Minutes >= MAX_MINUTES) Minutes = MAX_MINUTES;
        if (Seconds >= MAX_SECONDS) Seconds = MAX_SECONDS;

        string minPrefix = (Minutes < 10) ? "0" : "";
        string secPrefix = (Seconds < 10) ? "0" : "";
        string milPrefix = (Milliseconds < 10) ? "0" : "";

        return minPrefix + Minutes + ":" + secPrefix + Seconds + ":" + milPrefix + Milliseconds;
    }

    /// <summary>
    /// Start the timer.
    /// </summary>
    public void Run() { running = true; }

    /// <summary>
    /// Pause the timer.
    /// </summary>
    public void Pause() { running = false; }

    /// <summary>
    /// Stop the timer and save the result.
    /// </summary>
    public void Stop() {
        Pause();
        score = Timer;
    }
}