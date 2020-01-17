using UnityEngine;

[RequireComponent(typeof(Jukebox))]
public class SoundMixer : StateMachine
{
    private Jukebox jukebox;

    private void Awake() {
        this.jukebox = GetComponent<Jukebox>();
    }

    public override void Activate(string param, bool flag) {
        if (param == "crash") print("asked to play crash");
        Tune tune = jukebox.Get(param);

        if (tune != null) {
            if (flag) {
                if (tune.Name == "crash") { print("played crash"); }
                tune.Play();
            }
            else tune.Stop();
        }
    }

    public override bool IsAtState(string state) {
        Tune tune = jukebox.Get(state);
        return tune != null && tune.IsPlaying();
    }
}