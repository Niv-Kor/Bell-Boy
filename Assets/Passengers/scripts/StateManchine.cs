using System;

public class StateManchine
{
    public struct State {
        public string name;

        /// <param name="name">The name of the state as written in the state machine</param>
        public State(string name) {
            this.name = name;
        }
    }

    public static readonly State WALK = new State("walk");
    public static readonly State PRESS = new State("press");
    public static readonly State RUN = new State("run");
    public static readonly State JUMP = new State("jump");
    public static readonly State FALL = new State("fall");
    public static readonly State CRASH = new State("crash");
    public static readonly State[] STATES = { WALK, PRESS, RUN, JUMP, FALL, CRASH };

    /// <param name="paramName">Parameter name</param>
    /// <returns>The state that corresponds with the parameter.</returns>
    /// <throws>NullReferenceException if no state is using the specified parameter.</throws>
    public static State GetState(string paramName) {
        foreach (State state in STATES)
            if (state.name == paramName) return state;

        throw new NullReferenceException();
    }
}