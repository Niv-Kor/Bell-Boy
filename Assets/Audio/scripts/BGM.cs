using UnityEngine;

[RequireComponent(typeof(SoundMixer))]
public class BGM : MonoBehaviour
{
    [Tooltip("The background music's volume.")]
    [SerializeField] public float Volume = .5f;

    [Tooltip("The time it takes the background music to start playing \n" + 
             "(Only available when 'playOnAwake' is not checked).")]
    [SerializeField] private float startDelay;

    [Tooltip("True to start playing automatically as soon as the game starts.")]
    [SerializeField] private bool playOnAwake = true;

    private SoundMixer soundMixer;
    private Jukebox jukebox;
    private float startTimer;
    private string songTitle;
    private bool reachDelayTime;

    private void Start() {
        this.soundMixer = GetComponent<SoundMixer>();
        this.jukebox = GetComponent<Jukebox>();
        this.songTitle = jukebox.GetAllNames()[0];
        this.reachDelayTime = false;
        this.startTimer = 0;

        Tune song = jukebox.Get(songTitle);
        song.Volume = Volume;
        song.Loop = true;
        song.Delay = 0;
        song.Genre = Genre.BGM;
    }

    private void Update() {
        if (playOnAwake && !reachDelayTime) {
            if (startTimer < startDelay) startTimer += Time.deltaTime;
            else {
                Play();
                reachDelayTime = true;
            }
        }
    }

    /// <summary>
    /// Play the song.
    /// </summary>
    public void Play() { soundMixer.Activate(songTitle); }

    /// <summary>
    /// Stop the song.
    /// </summary>
    public void Stop() { soundMixer.Activate(songTitle, false); }

    /// <returns>True if the song is playing at the moment.</returns>
    internal bool IsPlaying() { return soundMixer.IsAtState(songTitle); }
}