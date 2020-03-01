﻿using UnityEngine;

[RequireComponent(typeof(SoundMixer))]
public class BGM : MonoBehaviour
{
    [Tooltip("The title of the song as it appears in the jukebox.")]
    [SerializeField] private string songTitle;

    [Tooltip("True to start playing automatically as the game starts.")]
    [SerializeField] private bool playOnAwake = true;

    private SoundMixer soundMixer;

    private void Start() {
        this.soundMixer = GetComponent<SoundMixer>();
        if (playOnAwake) Play();
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