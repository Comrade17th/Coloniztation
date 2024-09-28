using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FiniteState
{
    protected readonly FiniteStateMachine _finiteStateMachine;

    public FiniteState(FiniteStateMachine finiteStateMachine)
    {
        _finiteStateMachine = finiteStateMachine;
    }

    public virtual void Enter() {}
    public virtual void Update() {}
    public virtual void Exit() {}
}
