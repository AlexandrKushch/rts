using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PawnStateManager : Node
{
    private Dictionary<PawnStates, PawnStateBase> _states;
    private PawnStateBase _currentState;

    [Export]
    public AnimationPlayer PawnAnimationPlayer { get; private set; }

    public override void _Ready()
    {
        _states = GetChildren().ToDictionary(x => Enum.Parse<PawnStates>(x.Name), x => x as PawnStateBase);
        ChangeState(PawnStates.Idle);
    }

    public override void _Process(double delta)
    {
        _currentState._Process(delta);
    }

    public void ChangeState(PawnStates state)
    {
        if (_currentState != null)
        {
            _currentState.Deactivate();
        }

        _currentState = _states[state];
        _currentState.Activate();
    }
}
