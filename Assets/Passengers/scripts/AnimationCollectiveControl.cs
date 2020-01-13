using System.Collections.Generic;
using UnityEngine;

public class AnimationCollectiveControl : AnimationControl
{
    [Tooltip("A list of all derived animation controls.")]
    [SerializeField] private List<AnimationControl> animationControls;

    private List<Animator> animators;

    public new bool IsIdle { get; private set; }
    public new List<IDictionary<StateManchine.State, TriggerState>> Triggers { get; set; }

    protected override void Awake() {
        this.IsIdle = true;
        this.animators = new List<Animator>();
        this.Triggers = new List<IDictionary<StateManchine.State, TriggerState>>();

        foreach (AnimationControl anim in animationControls) {
            animators.Add(anim.GetComponent<Animator>());
            Triggers.Add(anim.Triggers);
        }
    }

    public override void Animate(StateManchine.State state) {
        foreach (AnimationControl anim in animationControls)
            anim.Animate(state);
    }

    public override void Animate(string state, bool flag) {
        foreach (AnimationControl anim in animationControls)
            anim.Animate(state, flag);
    }

    public override void Animate(StateManchine.State state, bool flag) {
        foreach (AnimationControl anim in animationControls)
            anim.Animate(state, flag);

        //check if all states are currently off
        if (flag) IsIdle = false;
        else {
            foreach (AnimationControl anim in animationControls) {
                if (!anim.IsIdle) {
                    IsIdle = false;
                    break;
                }
            }
        }
    }

    public override bool IsAnimating(StateManchine.State state) {
        foreach (AnimationControl anim in animationControls)
            if (anim.IsAnimating(state)) return true;

        return false;
    }

    public override void Idlize() {
        foreach (AnimationControl anim in animationControls)
            anim.Idlize();
    }

    public override void StrongIdlize() {
        foreach (AnimationControl anim in animationControls)
            anim.StrongIdlize();
    }
}
