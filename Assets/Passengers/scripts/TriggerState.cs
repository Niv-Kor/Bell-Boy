using UnityEngine;
using UnityEngine.Animations;

public class TriggerState : StateMachineBehaviour
{
    [Tooltip("The name of the state's corresponding parameter.")]
    [SerializeField] public string ParameterName;

    [Tooltip("The percentage of time passed before activating the trigger event.")]
    [SerializeField] [Range(0f, 1f)] private float triggerAfter = .5f;

    private static readonly float CANCEL_AFTER_PERCENT = .98f;

    private float stateTime, duration;
    private AnimationControl animationControl;

    public event System.Action<TriggerState> OnFinish = delegate {};

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        this.animationControl = animator.GetComponent<AnimationControl>();
        this.stateTime = stateInfo.length;
        this.duration = 0;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateExit(animator, stateInfo, layerIndex);

        duration += Time.deltaTime;

        if (duration >= stateTime * triggerAfter) OnFinish(this);
        if (duration >= stateTime * CANCEL_AFTER_PERCENT) animationControl.Animate(ParameterName, false);
    }
}