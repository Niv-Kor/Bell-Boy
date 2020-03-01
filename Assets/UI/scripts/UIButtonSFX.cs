using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SoundMixer))]
[RequireComponent(typeof(Button))]
public class UIButtonSFX : MonoBehaviour
{
    [Tooltip("The name of the tune to activate on click.")]
    [SerializeField] private string tuneName;

    private SoundMixer soundMixer;

    public bool ClickEnabled { get; set; }

    private void Start() {
        this.ClickEnabled = true;
        this.soundMixer = GetComponent<SoundMixer>();
        Button button = GetComponent<Button>();
        button.onClick.AddListener(delegate () { MakeSound(); });
    }

    private void MakeSound() {
        if (!ClickEnabled) return;
        else soundMixer.Activate(tuneName);
    }
}