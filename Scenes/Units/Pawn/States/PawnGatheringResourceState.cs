using Godot;

public partial class PawnGatheringResourceState : PawnStateBase
{
    public override void Activate()
    {
        base.Activate();

        StateManager.Pawn.Target = null;
        StateManager.Pawn.Visual.Connect(PawnVisual.SignalName.OnInteractAnimationFinished, Callable.From(Gather));

        Gather();

        GD.Print("GATHERING ON");
    }

    public override void Deactivate()
    {
        base.Deactivate();
        StateManager.Pawn.Visual.Stop();
        StateManager.Pawn.Visual.Disconnect(PawnVisual.SignalName.OnInteractAnimationFinished, Callable.From(Gather));
        GD.Print("GATHERING OFF");
    }

    private void Gather()
    {
        if (StateManager.Pawn.ResourceToCollectData.CollectedCount >= Pawn.MaxCollectableCapacity)
        {
            StateManager.MoveToClosestBuilding();
            return;
        }

        if (!IsInstanceValid(StateManager.Pawn.TargetResource))
        {
            StateManager.MoveToClosestResourceIfNotToBuilding();
            return;
        }

        StateManager.Pawn.Visual.Interact(StateManager.Pawn.TargetResource.ResourceType.Name);
    }

    private void OnInteractAnimatioKeyReached()
    {
        if (IsInstanceValid(StateManager.Pawn.TargetResource))
        {
            StateManager.Pawn.TargetResource.CollectOne();
            StateManager.Pawn.ResourceToCollectData.CollectedCount++;
            GD.Print("Gather +1");
        }
    }
}
