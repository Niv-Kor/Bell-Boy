using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SoundSliderControl : MonoBehaviour
{
    [Tooltip("The mixer group to control.")]
    [SerializeField] public Genre Genre;

    [Tooltip("The percentage of the slider max value to start the game with.")]
    [SerializeField] public float InitialPercentValue;

    private void Awake() {
        Slider slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(delegate { ChangeVolume(slider.value); });
        slider.value = InitialPercentValue;
    }

    /// <summary>
    /// This method invokes when the slider changes its value.
    /// </summary>
    /// <param name="value">The value of the slider</param>
    private void ChangeVolume(float value) {
        VolumeController.Instance.SetVolume(Genre, value);
    }
}