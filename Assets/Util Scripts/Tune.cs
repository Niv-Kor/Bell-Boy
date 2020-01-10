using UnityEngine;

[System.Serializable]
public class Tune
{
    [Tooltip("The name of the tune.")]
    [SerializeField] public string name;

    [Tooltip("The audio file to play.")]
    [SerializeField] public AudioClip clip;

    [Tooltip("The volume of the sound (0-1).")]
    [SerializeField] [Range(0, 1f)] public float volume = .5f;

    [Tooltip("The pitch of the sound.")]
    [SerializeField] [Range(.1f, 3)] public float pitch = 1;

    private AudioSource source;

    /// <summary>
    /// Initialize the tune with a source file.
    /// </summary>
    /// <param name="src">An AudioSource component</param>
    public void SetSource(AudioSource src) {
        this.source = src;
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
    }

    /// <summary>
    /// Play the tune.
    /// </summary>
    public void Play() {
        Debug.Log("asked to play " + name);
        source.Play();
    }

    /// <summary>
    /// Stop the tune.
    /// </summary>
    public void Stop() { source.Stop(); }

    /// <returns>True if the tune is currently playing.</returns>
    public bool IsPlaying() { return source.isPlaying; }

    /// <param name="vol">The new volume of the tune</param>
    public void SetVolume(float vol) {
        source.volume = vol;
        volume = vol;
    }

    /// <param name="ptch">The new pitch of the tune</param>
    public void SetPitch(float ptch) {
        source.pitch = ptch;
        pitch = ptch;
    }
}