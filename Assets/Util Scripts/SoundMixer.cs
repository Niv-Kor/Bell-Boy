using UnityEngine;

[RequireComponent(typeof(Jukebox))]
public class SoundMixer : StateMachine
{
    private Jukebox jukebox;

    private void Awake() {
        this.jukebox = GetComponent<Jukebox>();
    }

    public override void Activate(string param, bool flag) {
        Tune tune = jukebox.Get(param);

        if (tune != null) {
            if (flag) tune.Play();
            else tune.Stop();
        }
    }

    public override bool IsAtState(string state) {
        Tune tune = jukebox.Get(state);
        return tune != null && tune.IsPlaying();
    }
}