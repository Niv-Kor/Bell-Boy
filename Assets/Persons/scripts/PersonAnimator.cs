using UnityEngine;

[RequireComponent(typeof(AnimationTape))]
[RequireComponent(typeof(SoundMixer))]
public class PersonAnimator : MonoBehaviour
{
    public static readonly string ABORT_PARAM = "abort";
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

    /// <summary>
    /// Activate or deactivate a state.
    /// </summary>
    /// <param name="state">The state's name</param>
    /// <param name="flag">True to activate or false to deactivate</param>
    public void Activate(string state, bool flag) {
        AnimationTape.Activate(state, flag);
        SoundMixer.Activate(state, flag);
    }

    /// <summary>
    /// Activate or deactivate a state.
    /// </summary>
    /// <param name="state">The state to manage</param>
    /// <param name="flag">True to activate or false to deactivate</param>
    public void Activate(AnimationState state, bool flag) {
        AnimationTape.Activate(state.animationParam, flag);
        SoundMixer.Activate(state.tuneName, flag);
    }

    /// <summary>
    /// Activate a state.
    /// </summary>
    /// <param name="state">The state to manage</param>
    public void Activate(AnimationState state) {
        AnimationTape.Activate(state.animationParam);
        SoundMixer.Activate(state.tuneName);
    }

    /// <summary>
    /// Check if the passenger is currently at a specified state.
    /// </summary>
    /// <param name="state">The state to check</param>
    /// <returns>True if the passenger is currently at that state.</returns>
    public bool IsAtState(AnimationState state) {
        return AnimationTape.IsAtState(state.animationParam);
    }

    /// <summary>
    /// Return a passenger to idle state.
    /// This method might wait until some processes end before idlizing.
    /// </summary>
    public void Idlize() {
        AnimationTape.Idlize();
        SoundMixer.Idlize();
    }

    /// <summary>
    /// Return a passenger to idle state.
    /// This method forces all current processes to stop.
    /// </summary>
    public void StrongIdlize() {
        AnimationTape.StrongIdlize();
        SoundMixer.StrongIdlize();
    }
}