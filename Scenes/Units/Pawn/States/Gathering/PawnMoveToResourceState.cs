using Godot;

public partial class PawnMoveToResourceState : PawnStateBase
{
    private const float TargetDesiredDistance = 40;
    private float _defaultTargetDesiredDistance;

    public override void Activate()
    {
        base.Activate();

        _defaultTargetDesiredDistance = StateManager.Pawn.NavigationAgent2D.TargetDesiredDistance;
        StateManager.Pawn.NavigationAgent2D.TargetDesiredDistance = TargetDesiredDistance;
        
        GD.Print("Move to res ON");
    }

    public override void Deactivate()
    {
        base.Deactivate();

        StateManager.Pawn.NavigationAgent2D.TargetDesiredDistance = _defaultTargetDesiredDistance;
        GD.Print("Move to res OFF");
    }

    public override void _Process(double delta)
    {
        if (StateManager.Pawn.NavigationAgent2D.IsNavigationFinished())
        {
            StateManager.ChangeState(PawnGatheringStateIds.GatheringResource);
        }
    }
}
