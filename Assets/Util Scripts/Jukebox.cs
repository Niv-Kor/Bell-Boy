using System;
using UnityEngine;

public class Jukebox : MonoBehaviour
{
    [Tooltip("An array containing all of the object's tunes.")]
    [SerializeField] private Tune[] tunes;

    private static readonly string PARENT_NAME = "Audio";

    private void Awake() {
        GameObject audioParent = new GameObject(PARENT_NAME);
        audioParent.transform.SetParent(transform);

        //create an audio source component for each tune
        foreach (Tune tune in tunes) {
            AudioSource audioSource = audioParent.AddComponent<AudioSource>();
            tune.SetSource(audioSource);
        }
    }

    /// <param name="name">The name of the tune</param>
    /// <returns>The correct tune, or null if it doesn't exist.</returns>
    public Tune Get(string name) {
        return Array.Find(tunes, tune => tune.Name == name);
    }
}