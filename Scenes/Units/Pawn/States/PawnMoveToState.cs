using Godot;

public partial class PawnMoveToState : PawnStateBase
{
    private const float ResourceDesiredDistance = 40;
    private const float BuildingDesiredDistance = 80;
    private float _defaultTargetDesiredDistance;

    public override void Activate()
    {
        base.Activate();

        _defaultTargetDesiredDistance = StateManager.Pawn.NavigationAgent2D.TargetDesiredDistance;
        StateManager.Pawn.NavigationAgent2D.TargetDesiredDistance = StateManager.Pawn.TargetObject is ResourceBase ? ResourceDesiredDistance : BuildingDesiredDistance;
        
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
            if (StateManager.Pawn.TargetObject is ResourceBase)
            {
                StateManager.ChangeState(PawnGatheringStateIds.GatheringResource);
            }
            else if (StateManager.Pawn.TargetObject is BuildingBase building)
            {
                if (!building.Build)
                {
                    StateManager.ChangeState(PawnGatheringStateIds.BuildOrRepair);
                }
                else
                {
                    if (StateManager.Pawn.ResourceCollectedCount > 0)
                    {
                        StateManager.Pawn.DropResources();
                        
                        if (IsInstanceValid(StateManager.Pawn.TargetResource))
                        {
                            UnitsController.Instance.MoveToObstacleCommand(StateManager.Pawn, StateManager.Pawn.TargetResource);
                        }
                        else
                        {
                            UnitsController.Instance.MoveToObstacleCommand(StateManager.Pawn, null);
                            StateManager.ChangeState(PawnGatheringStateIds.None);
                        }
                    }
                    else
                    {
                        UnitsController.Instance.MoveToObstacleCommand(StateManager.Pawn, null);
                        StateManager.ChangeState(PawnGatheringStateIds.None);
                    }
                }
            }
        }
    }
}
