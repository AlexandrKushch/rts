using Godot;

public partial class PawnBuildOrRepairState : PawnStateBase
{
    public override void Activate()
    {
        base.Activate();

        Pawn.Target = null;
        Pawn.PawnVisual.Connect(PawnVisual.SignalName.OnInteractAnimationFinished, Callable.From(Build));
        Pawn.TargetBuilding.BuildersCount++;

        Build();
    }

    public override void Deactivate()
    {
        base.Deactivate();
        PawnStateMachine.Pawn.PawnVisual.Stop();
        PawnStateMachine.Pawn.PawnVisual.Disconnect(PawnVisual.SignalName.OnInteractAnimationFinished, Callable.From(Build));
        Pawn.TargetBuilding.BuildersCount--;
    }

    private void Build()
    {
        if (!IsInstanceValid(PawnStateMachine.Pawn.TargetBuilding))
        {
            return;
        }

        if (PawnStateMachine.Pawn.TargetBuilding.BuiltAndNotDamaged)
        {
            if (PawnStateMachine.Pawn.ResourceToCollectData != null
                && PawnStateMachine.Pawn.ResourceToCollectData.CollectedCount > 0)
            {
                PawnStateMachine.Pawn.DropResources();

                if (IsInstanceValid(PawnStateMachine.Pawn.TargetResource))
                {
                    PawnStateMachine.Pawn.SetTarget(
                        PawnStateMachine.Pawn.TargetResource.GlobalPosition,
                        PawnStateMachine.Pawn.TargetResource);
                }
                else
                {
                    PawnStateMachine.ChangeState(PawnStateIds.None);
                    PawnStateMachine.Pawn.SetTarget(null, null);
                }
            }
            else
            {
                PawnStateMachine.ChangeState(PawnStateIds.None);
                PawnStateMachine.Pawn.SetTarget(null, null);
            }
            return;
        }

        PawnStateMachine.Pawn.PawnVisual.Interact("build", PawnStateMachine.Pawn.TargetObject?.GlobalPosition);
    }

    private void OnInteractAnimatioKeyReached()
    {
        if (IsInstanceValid(PawnStateMachine.Pawn.TargetBuilding))
        {
            PawnStateMachine.Pawn.TargetBuilding.TryBuildProgressOne();
        }
    }
}
