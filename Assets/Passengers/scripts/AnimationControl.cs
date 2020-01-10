using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationControl : MonoBehaviour
{
    private static readonly string ABORT_PARAM = "abort";

    private Animator animator;

    public bool IsIdle { get; private set; }

    private void Awake() {
        this.animator = GetComponent<Animator>();
        this.IsIdle = true;
    }

    /// <summary>
    /// Activate an animation.
    /// </summary>
    /// <param name="state">The state to animate</param>
    public void Animate(StateManchine.State state) {
        Animate(state, true);
    }

    /// <summary>
    /// Activate an animation or stop it.
    /// </summary>
    /// <param name="state">The name of the state to animate (upper case sensitive)</param>
    /// <param name="flag">True to animate or false to stop</param>
    public void Animate(string state, bool flag) {
        Animate(StateManchine.GetState(state), flag);
    }

    /// <summary>
    /// Activate an animation or stop it.
    /// </summary>
    /// <param name="state">The state to animate</param>
    /// <param name="flag">True to animate or false to stop</param>
    public void Animate(StateManchine.State state, bool flag) {
        animator.SetBool(state.name, flag);

        //check if all states are currently off
        if (flag) IsIdle = false;
        else {
            bool allOff = true;

            foreach (StateManchine.State s in StateManchine.STATES)
                if (IsAnimating(s)) allOff = false;

            IsIdle = allOff;
        }
    }

    /// <param name="state">The state to check</param>
    /// <returns>True if the state's parameter is currently on</returns>
    public bool IsAnimating(StateManchine.State state) {
        return animator.GetBool(state.name);
    }

    /// <summary>
    /// Cancel all parameters and return to Idle state.
    /// </summary>
    public void Idlize() {
        foreach (StateManchine.State state in StateManchine.STATES)
            Animate(state, false);
    }

    /// <summary>
    /// Immediately cancel all parameters and return to Idle state, wihtout waiting for any exit times.
    /// </summary>
    public void StrongIdlize() {
        animator.SetTrigger(ABORT_PARAM);
        Idlize();
    }
}