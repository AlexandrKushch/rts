using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PawnStateManagerBase : Node
{
    private Dictionary<PawnGatheringStateIds, PawnStateBase> _states;
    private PawnStateBase _currentState;
    
    [Export] public Pawn Pawn { get; private set; }

    public override void _Ready()
    {
        _states = GetChildren().ToDictionary(x => Enum.Parse<PawnGatheringStateIds>(x.Name), x => x as PawnStateBase);
        SetProcess(false);
    }

    public override void _Process(double delta)
    {
        _currentState._Process(delta);
    }

    public void ChangeState(PawnGatheringStateIds state)
    {
        if (_currentState != null)
        {
            _currentState.Deactivate();
        }

        _currentState = _states[state];
        _currentState.Activate();
    }
}
