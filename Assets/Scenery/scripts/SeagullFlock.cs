using UnityEngine;

public class SeagullFlock : MonoBehaviour
{
    private static readonly string SEAGULLS_CALL_SFX = "seagull flock";

    private SoundMixer soundMixer;

    private void Start() {
        this.soundMixer = GetComponent<SoundMixer>();
        soundMixer.Activate(SEAGULLS_CALL_SFX);
    }
}