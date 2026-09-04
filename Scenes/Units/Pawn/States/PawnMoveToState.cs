using Godot;

public partial class PawnMoveToState : PawnStateBase
{
    private const float ResourceDesiredDistance = 40;
    private const float BuildingDesiredDistance = 80;
    private float _defaultTargetDesiredDistance;

    public override void Activate()
    {
        base.Activate();

        _defaultTargetDesiredDistance = PawnStateMachine.Pawn.NavigationAgent2D.TargetDesiredDistance;
        PawnStateMachine.Pawn.NavigationAgent2D.TargetDesiredDistance = PawnStateMachine.Pawn.TargetObject is ResourceBase ? ResourceDesiredDistance : BuildingDesiredDistance;
    }

    public override void Deactivate()
    {
        base.Deactivate();

        PawnStateMachine.Pawn.NavigationAgent2D.TargetDesiredDistance = _defaultTargetDesiredDistance;
    }

    public override void _Process(double delta)
    {
        if (PawnStateMachine.Pawn.NavigationAgent2D.IsNavigationFinished())
        {
            if (PawnStateMachine.Pawn.TargetObject is ResourceBase)
            {
                PawnStateMachine.ChangeState(PawnStateIds.GatheringResource);
            }
            else if (PawnStateMachine.Pawn.TargetObject is BuildingBase building)
            {
                if (!building.Build)
                {
                    PawnStateMachine.ChangeState(PawnStateIds.BuildOrRepair);
                }
                else
                {
                    if (PawnStateMachine.Pawn.ResourceToCollectData != null
                        && PawnStateMachine.Pawn.ResourceToCollectData.CollectedCount > 0)
                    {
                        PawnStateMachine.Pawn.DropResources();
                        
                        if (IsInstanceValid(PawnStateMachine.Pawn.TargetResource))
                        {
                            UnitsController.Instance.MoveToNodeCommand(PawnStateMachine.Pawn, PawnStateMachine.Pawn.TargetResource);
                        }
                        else
                        {
                            PawnStateMachine.MoveToClosestResourceIfNotToBuilding();
                        }
                    }
                    else
                    {
                        PawnStateMachine.Pawn.SetTarget(null, null);
                        PawnStateMachine.ChangeState(PawnStateIds.None);
                    }
                }
            }
        }
    }
}
