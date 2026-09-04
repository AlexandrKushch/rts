using System;
using Godot;

public partial class StateBase<T> : Node
    where T : struct, Enum
{
    protected StateMachineBase<T> StateMachine;

    public override void _Ready()
    {
        StateMachine = GetParent<StateMachineBase<T>>();
        SetProcess(false);
    }

    public virtual void Activate()
    {
        SetProcess(true);        
    }

    public virtual void Deactivate()
    {
        SetProcess(false);
    }
}
