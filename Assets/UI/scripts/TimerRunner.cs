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
    [SerializeField] private bool playOnAwake = true;

    private static readonly int WHOLE_ROUND = 60;
    private static readonly int MAX_MINUTES = 99;
    private static readonly int MAX_SECONDS = 59;
    private static readonly int GEAR_SPIN_ANGLE = -360 / WHOLE_ROUND;
    private static readonly int PIN_OVERJUMP = GEAR_SPIN_ANGLE;
    private static readonly int PIN_UNDERJUMP = GEAR_SPIN_ANGLE / 2;
    private static readonly float FIRST_FIX_PIN_AFTER = 9f;
    private static readonly float SECOND_FIX_PIN_AFTER = 14f;
    private static readonly string TICK_SFX = "tick";

    private SpriteRenderer spriteRender;
    private SoundMixer soundMixer;
    private float score;
    private int prevSeconds, prevMinutes;
    private bool running, isMaxValue;
    private bool firstStabilize, pinStablized;

    public float Timer { get; set; }
    public int Minutes { get; private set; }
    public int Seconds { get; private set; }
    public int Milliseconds { get; private set; }

    public delegate void MinuteNotifier(int minutes);
    public event MinuteNotifier MinuteNotifierTrigger;

    private void Start() {
        this.spriteRender = GetComponent<SpriteRenderer>();
        this.soundMixer = GetComponent<SoundMixer>();
        this.prevSeconds = Seconds;
        this.prevMinutes = Minutes;
        this.firstStabilize = false;
        this.pinStablized = true;
        this.isMaxValue = false;
        this.running = false;
        this.score = 0;

        if (playOnAwake) Run();
    }

    private void Update() {
        if (running) {
            Timer += Time.deltaTime;

            //update string value
            if (!isMaxValue) {
                string timerStr = GameTimeValueToString(Timer);
                value.text = timerStr;

                //reached 99:99:99 - stop updating the string
                if (Minutes == 99 && Seconds == 99 && Milliseconds == 99) isMaxValue = true;
            }

            //stablize pin
            if (!pinStablized) {
                if (!firstStabilize && Milliseconds >= FIRST_FIX_PIN_AFTER && Milliseconds < SECOND_FIX_PIN_AFTER) {
                    pinGear.transform.Rotate(0, 0, -GEAR_SPIN_ANGLE - PIN_OVERJUMP - PIN_UNDERJUMP);
                    firstStabilize = true;
                }
                else if (Milliseconds >= SECOND_FIX_PIN_AFTER) {
                    if (firstStabilize) pinGear.transform.Rotate(0, 0, GEAR_SPIN_ANGLE + PIN_UNDERJUMP);
                    else pinGear.transform.Rotate(0, 0, -PIN_OVERJUMP); //bug - Update()'s frequency is too low
                    pinStablized = true;
                }
            }

            //update sprite
            if (prevSeconds != Seconds) {
                prevSeconds = Seconds;
                spriteRender.sprite = secondsClearance[Seconds];

                //make pin jump a little too far
                pinGear.transform.Rotate(0, 0, GEAR_SPIN_ANGLE + PIN_OVERJUMP);
                firstStabilize = false;
                pinStablized = false;
            }
            
            //grant life bit every round minute
            if (prevMinutes != Minutes) {
                prevMinutes = Minutes;
                soundMixer.Activate(TICK_SFX);
                MinuteNotifierTrigger?.Invoke(Minutes);
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