using System.Collections.Generic;
using UnityEngine;

public class AnimationCollectiveTape : AnimationTape
{
    [Tooltip("A list of all derived animation controls.")]
    [SerializeField] private List<AnimationTape> animationControls;

    private List<Animator> animators;

    public new List<IDictionary<string, TriggerState>> Triggers { get; set; }

    protected override void Awake() {
        this.IsIdle = true;
        this.animators = new List<Animator>();
        this.Triggers = new List<IDictionary<string, TriggerState>>();

        foreach (AnimationTape anim in animationControls) {
            animators.Add(anim.GetComponent<Animator>());
            Triggers.Add(anim.Triggers);
        }
    }

    public override void Activate(string state) {
        foreach (AnimationTape anim in animationControls)
            anim.Activate(state);
    }

    public override void Activate(string state, bool flag) {
        foreach (AnimationTape anim in animationControls)
            anim.Activate(state, flag);

        //check if all states are currently off
        if (flag) IsIdle = false;
        else {
            foreach (AnimationTape anim in animationControls) {
                if (!anim.IsIdle) {
                    IsIdle = false;
                    break;
                }
            }
        }
    }

    public override bool IsAtState(string state) {
        foreach (AnimationTape anim in animationControls)
            if (anim.IsAtState(state)) return true;

        return false;
    }

    public override void Idlize() {
        foreach (AnimationTape anim in animationControls)
            anim.Idlize();
    }

    public override void StrongIdlize() {
        foreach (AnimationTape anim in animationControls)
            anim.StrongIdlize();
    }
}
