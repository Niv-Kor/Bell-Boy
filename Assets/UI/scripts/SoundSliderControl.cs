using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundSliderControl : MonoBehaviour
{
    [Tooltip("The mixer group to control.")]
    [SerializeField] public Genre Genre;

    [Tooltip("The percentage of the slider max value to start the game with.")]
    [SerializeField] public float InitialPercentValue;

    private static readonly float MIN_MIXER_VOLUME = -80;
    private static readonly float MAX_MIXER_VOLUME = 0;
    private static readonly string EXPOSED_PARAMETER_SUFFIX = "vol";

    private AudioMixerGroup mixerGroup;

    private void Start() {
        this.mixerGroup = AudioAccessor.Instance.GetGenreGroup(Genre);
        print("for slider " + Genre + " mixer is " + mixerGroup.name);
        Slider slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(delegate { ChangeVolume(slider.value); });
        slider.value = InitialPercentValue;
    }

    /// <summary>
    /// This method invokes when the slider changes its value.
    /// </summary>
    /// <param name="value">The value of the slider</param>
    private void ChangeVolume(float value) {
        float volume = value * (MAX_MIXER_VOLUME - MIN_MIXER_VOLUME) + MIN_MIXER_VOLUME;
        string exposedVolumeParameter = Genre.ToString() + EXPOSED_PARAMETER_SUFFIX;
        mixerGroup.audioMixer.SetFloat(exposedVolumeParameter, volume);
    }
}