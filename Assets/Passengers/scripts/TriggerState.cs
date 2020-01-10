using UnityEngine;

public class TriggerState : StateMachineBehaviour
{
    [Tooltip("The name of the state's corresponding parameter.")]
    [SerializeField] private string parameterName;

    private static readonly float CANCEL_AFTER_PERCENT = 98;

    private float stateTime, duration;
    private AnimationControl animationControl;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        this.animationControl = animator.GetComponent<AnimationControl>();
        this.stateTime = stateInfo.length;
        this.duration = 0;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        duration += Time.deltaTime;
        if (duration >= stateTime * CANCEL_AFTER_PERCENT / 100) animationControl.Animate(parameterName, false);
    }
}