using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class StateMachineBase<T> : Node where T : struct, Enum
{
    protected Dictionary<T, StateBase<T>> States;
    protected StateBase<T> CurrentState;
    protected T? CurrentStateType;

    public override void _Ready()
    {
        States = GetChildren().ToDictionary(x => Enum.Parse<T>(x.Name), x => x as StateBase<T>);
        SetProcess(false);
    }

    public override void _Process(double delta)
    {
        CurrentState._Process(delta);
    }

    public T? GetCurrentStateType()
    {
        return CurrentStateType;
    }

    public virtual void ChangeState(T state)
    {
        if (CurrentState != null)
        {
            CurrentState.Deactivate();
            CurrentState = null;
            CurrentStateType = null;
        }

        CurrentStateType = state;
        CurrentState = States[state];
        CurrentState.Activate();
    }
}
