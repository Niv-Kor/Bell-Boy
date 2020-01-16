using UnityEngine;

[RequireComponent(typeof(AnimationTape))]
[RequireComponent(typeof(SoundMixer))]
public class PassengerAnimator : MonoBehaviour
{
    public static readonly AnimationState WALK = new AnimationState { animationParam = "walk", tuneName = "step" };
    public static readonly AnimationState RUN = new AnimationState { animationParam = "run", tuneName = "run" };
    public static readonly AnimationState JUMP = new AnimationState { animationParam = "jump", tuneName = "jump" };
    public static readonly AnimationState FALL = new AnimationState { animationParam = "fall", tuneName = "fall" };
    public static readonly AnimationState CRASH = new AnimationState { animationParam = "crash", tuneName = "crash" };
    public static readonly AnimationState PRESS = new AnimationState { animationParam = "press", tuneName = "press" };
    public static readonly AnimationState SPECIAL = new AnimationState { animationParam = "special", tuneName = "special" };

    public AnimationTape AnimationTape { get; private set; }
    public SoundMixer SoundMixer { get; private set; }
    public bool IsIdle { get { return AnimationTape.IsIdle; } }

    private void Awake() {
        this.AnimationTape = GetComponent<AnimationTape>();
        this.SoundMixer = GetComponent<SoundMixer>();
    }

    public void Activate(AnimationState state, bool flag) {
        AnimationTape.Activate(state.animationParam, flag);
        SoundMixer.Activate(state.tuneName, flag);
    }

    public void Activate(AnimationState state) {
        AnimationTape.Activate(state.animationParam);
        SoundMixer.Activate(state.tuneName);
    }

    public bool IsAtState(AnimationState state) {
        return AnimationTape.IsAtState(state.animationParam);
    }

    public void Idlize() {
        AnimationTape.Idlize();
        SoundMixer.Idlize();
    }

    public void StrongIdlize() {
        AnimationTape.StrongIdlize();
        SoundMixer.StrongIdlize();
    }
}