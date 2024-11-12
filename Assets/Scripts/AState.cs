using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AState {
    public enum State
    {
        Idle,
        Walk,
        Dash,
        Run,
        HeavyAttack,
        Dameged,
        Attack,
        Parry,
        Execute,
        Guard,
        Jump,
        ExecuteReady,
    }
    public Enemy enemy = null;
    public abstract State state { get; }
    public abstract void StartState(Actor actor);
    public abstract void UpdateState(Actor actor);
    public abstract void EndState(Actor actor);
}
