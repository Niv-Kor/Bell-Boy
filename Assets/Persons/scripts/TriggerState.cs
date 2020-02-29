using UnityEngine;

public class TriggerState : StateMachineBehaviour
{
    [Tooltip("The name of the state's corresponding parameter.")]
    [SerializeField] public string ParameterName;

    [Tooltip("The percentage of time passed before activating the trigger event.")]
    [SerializeField] [Range(0f, 1f)] private float triggerAfter = .5f;

    private static readonly float CANCEL_AFTER_PERCENT = .98f;

    private bool triggered, canceled;
    private float stateTime, duration;
    private PersonAnimator passengerAnimator;

    public delegate void OnTrigger();
    private event OnTrigger OnTriggerEvent;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        this.passengerAnimator = animator.GetComponent<PersonAnimator>();
        this.stateTime = stateInfo.length;
        this.triggered = false;
        this.canceled = false;
        this.duration = 0;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        duration += Time.deltaTime;

        if (!triggered && duration >= stateTime * triggerAfter) {
            OnTriggerEvent?.Invoke();
            triggered = true;
        }
        else if (!canceled && duration >= stateTime * CANCEL_AFTER_PERCENT) {
            passengerAnimator.Activate(ParameterName, false);
            canceled = true;
        }
    }

    /// <summary>
    /// Subscribe to the trigger event.
    /// </summary>
    /// <param name="ev">A method to invoke when the event occurs</param>
    public void Subscribe(OnTrigger ev) { OnTriggerEvent += ev; }

    /// <summary>
    /// Unubscribe from the trigger event.
    /// </summary>
    /// <param name="ev">The method to remove from the event</param>
    public void Unsubscribe(OnTrigger ev) { OnTriggerEvent -= ev; }
}