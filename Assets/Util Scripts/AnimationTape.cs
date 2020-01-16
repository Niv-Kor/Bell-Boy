using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationTape : StateMachine
{
    protected static readonly string ABORT_PARAM = "abort";

    protected Animator animator;

    public bool IsIdle { get; protected set; }
    public IDictionary<string, TriggerState> Triggers { get; set; }

    protected virtual void Awake() {
        this.animator = GetComponent<Animator>();
        this.IsIdle = true;
        this.Triggers = new Dictionary<string, TriggerState>();
        
        //wrap all trigger behaviours in a dictionary
        TriggerState[] triggerStates = animator.GetBehaviours<TriggerState>();
        foreach (TriggerState state in triggerStates)
            Triggers.Add(state.ParameterName, state);
    }

    public override void Activate(string state, bool flag) {
        animator.SetBool(state, flag);

        //check if all states are currently off
        if (flag) IsIdle = false;
        else {
            bool allOff = true;

            foreach (string s in states) {
                if (IsAtState(s)) {
                    allOff = false;
                    break;
                }
            }

            IsIdle = allOff;
        }
    }

    public override bool IsAtState(string state) {
        return animator.GetBool(state);
    }

    public override void StrongIdlize() {
        animator.SetTrigger(ABORT_PARAM);
        animator.ResetTrigger(ABORT_PARAM);
        Idlize();
    }
}