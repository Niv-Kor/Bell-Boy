﻿using UnityEngine;

[System.Serializable]
public class Tune
{
    [Tooltip("The name of the tune.")]
    [SerializeField] public string Name;

    [Tooltip("The audio file to play.")]
    [SerializeField] public AudioClip Clip;

    [Tooltip("The volume of the sound (0-1).")]
    [SerializeField] public float Volume = .5f;

    [Tooltip("The pitch of the sound.")]
    [SerializeField] public float Pitch = 1;

    [Tooltip("Delay to add before the sound starts (in seconds).")]
    [SerializeField] public float Delay = 0;

    [Tooltip("True to play the tune in loops.")]
    [SerializeField] public bool Loop = false;

    [Tooltip("The audio mixer that this tune belongs to.")]
    [SerializeField] public Genre Genre;

    private AudioSource source;

    /// <summary>
    /// Initialize the tune with a source file.
    /// </summary>
    /// <param name="src">An AudioSource component</param>
    public void SetSource(AudioSource src) {
        this.source = src;
        ResetSourceValues();
    }

    /// <summary>
    /// Reset the values of the source according to any changes in the tune parent.
    /// </summary>
    private void ResetSourceValues() {
        if (source == null) return;

        source.clip = Clip;
        source.volume = Volume;
        source.pitch = Pitch;
    }

    /// <summary>
    /// Play the tune.
    /// </summary>
    public void Play() {
        ResetSourceValues();
        source.PlayDelayed(Delay);
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
        Volume = vol;
    }

    /// <param name="ptch">The new pitch of the tune</param>
    public void SetPitch(float ptch) {
        source.pitch = ptch;
        Pitch = ptch;
    }
}