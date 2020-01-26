using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SoundMixer))]
[RequireComponent(typeof(Button))]
public class UIButtonSFX : MonoBehaviour
{
    [Tooltip("The name of the tune to activate on click.")]
    [SerializeField] private string tuneName;

    private void Start() {
        SoundMixer soundMixer = GetComponent<SoundMixer>();
        Button button = GetComponent<Button>();
        button.onClick.AddListener(delegate () { soundMixer.Activate(tuneName); });
    }
}