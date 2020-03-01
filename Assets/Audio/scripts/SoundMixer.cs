using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Jukebox))]
public class SoundMixer : StateMachine
{
    private Jukebox jukebox;

    protected override void Awake() {
        this.jukebox = GetComponent<Jukebox>();
        base.Awake();
    }

    public override void Activate(string param, bool flag) {
        if (param == null || param == "") return;

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

    protected override List<string> RetrieveStates() {
        return jukebox.GetAllNames();
    }

    public override void Idlize() {}
    public override void StrongIdlize() {}
}