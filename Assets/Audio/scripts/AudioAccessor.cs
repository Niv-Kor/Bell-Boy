using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioAccessor : Singleton<AudioAccessor>
{
    [Tooltip("The main mixer that the game works with.")]
    [SerializeField] private AudioMixer masterMixer;

    /// <summary>
    /// Get an audio mixer group that is compatible with a specified genre.
    /// </summary>
    /// <param name="genre">The genre of which to get the audio mixer group</param>
    /// <returns>The audio mixer group that is compatible with the specified genre.</returns>
    public AudioMixerGroup GetGenreGroup(Genre genre) {
        return masterMixer.FindMatchingGroups(genre.ToString())[0];
    }
}