using Godot;

public partial class PawnBuildOrRepairState : PawnStateBase
{
    public override void Activate()
    {
        base.Activate();

        StateManager.Pawn.Target = null;
        StateManager.Pawn.Visual.Connect(PawnVisual.SignalName.OnInteractAnimationFinished, Callable.From(Build));

        Build();

        GD.Print("BUILDING ON");
    }

    public override void Deactivate()
    {
        base.Deactivate();
        StateManager.Pawn.Visual.Stop();
        StateManager.Pawn.Visual.Disconnect(PawnVisual.SignalName.OnInteractAnimationFinished, Callable.From(Build));
        GD.Print("BUILDING OFF");
    }

    private void Build()
    {
        if (!IsInstanceValid(StateManager.Pawn.TargetBuilding))
        {
            // Deactivate();
            return;
        }

        if (StateManager.Pawn.TargetBuilding.Build)
        {
            if (StateManager.Pawn.ResourceToCollectData != null
                && StateManager.Pawn.ResourceToCollectData.CollectedCount > 0)
            {
                StateManager.Pawn.DropResources();

                if (IsInstanceValid(StateManager.Pawn.TargetResource))
                {
                    UnitsController.Instance.MoveToNodeCommand(StateManager.Pawn, StateManager.Pawn.TargetResource);
                }
                else
                {
                    StateManager.Pawn.SetTarget(null, null);
                    StateManager.ChangeState(PawnGatheringStateIds.None);
                }
            }
            return;
        }

        StateManager.Pawn.Visual.Interact("build");
    }

    private void OnInteractAnimatioKeyReached()
    {
        if (IsInstanceValid(StateManager.Pawn.TargetBuilding))
        {
            StateManager.Pawn.TargetBuilding.TryBuildProgressOne();
        }
    }
}
