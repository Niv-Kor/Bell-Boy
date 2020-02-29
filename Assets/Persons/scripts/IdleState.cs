using UnityEngine;

public class IdleState : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        animator.ResetTrigger(PersonAnimator.ABORT_PARAM);
    }
}
