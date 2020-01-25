using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationTape : StateMachine
{
    protected Animator animator;

    public bool IsIdle { get; protected set; }
    public IDictionary<string, TriggerState> Triggers { get; set; }

    protected override void Awake() {
        this.animator = GetComponent<Animator>();
        this.IsIdle = true;
        this.Triggers = new Dictionary<string, TriggerState>();
        
        //wrap all trigger behaviours in a dictionary
        foreach (TriggerState state in animator.GetBehaviours<TriggerState>())
            Triggers.Add(state.ParameterName, state);

        base.Awake();
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
        Idlize();
        animator.SetTrigger(PassengerAnimator.ABORT_PARAM);
    }

    protected override List<string> RetrieveStates() {
        List<string> statesList = new List<string>();

        foreach (var parameter in animator.parameters)
            statesList.Add(parameter.name);

        return statesList;
    }
}