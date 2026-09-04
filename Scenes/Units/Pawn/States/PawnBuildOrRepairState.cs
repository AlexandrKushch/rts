using Godot;

public partial class PawnBuildOrRepairState : PawnStateBase
{
    public override void Activate()
    {
        base.Activate();

        PawnStateMachine.Pawn.Target = null;
        PawnStateMachine.Pawn.Visual.Connect(PawnVisual.SignalName.OnInteractAnimationFinished, Callable.From(Build));

        Build();
    }

    public override void Deactivate()
    {
        base.Deactivate();
        PawnStateMachine.Pawn.Visual.Stop();
        PawnStateMachine.Pawn.Visual.Disconnect(PawnVisual.SignalName.OnInteractAnimationFinished, Callable.From(Build));
    }

    private void Build()
    {
        if (!IsInstanceValid(PawnStateMachine.Pawn.TargetBuilding))
        {
            return;
        }

        if (PawnStateMachine.Pawn.TargetBuilding.Build)
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
                    PawnStateMachine.Pawn.SetTarget(null, null);
                    PawnStateMachine.ChangeState(PawnStateIds.None);
                }
            }
            return;
        }

        PawnStateMachine.Pawn.Visual.Interact("build");
    }

    private void OnInteractAnimatioKeyReached()
    {
        if (IsInstanceValid(PawnStateMachine.Pawn.TargetBuilding))
        {
            PawnStateMachine.Pawn.TargetBuilding.TryBuildProgressOne();
        }
    }
}
