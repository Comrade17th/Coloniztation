using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiniteStateMachine
{
    private FiniteState _stateCurrent;

    private Dictionary<Type, FiniteState> _states = new();

    public void AddState(FiniteState state)
    {
        _states.Add(state.GetType(), state);
    }

    public void SetState<T>() where T : FiniteState
    {
        var type = typeof(T);
        
        if(_stateCurrent.GetType() == type)
            return;

        if (_states.TryGetValue(type, out var newState))
        {
            _stateCurrent?.Exit();
            _stateCurrent = newState;
            _stateCurrent.Enter();
        }
    }

    public void Update()
    {
        _stateCurrent?.Update();
    }
}
