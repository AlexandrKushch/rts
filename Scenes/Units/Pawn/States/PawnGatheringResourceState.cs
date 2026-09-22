using Godot;

public partial class PawnGatheringResourceState : PawnStateBase
{
    public override void Activate()
    {
        base.Activate();

        // PawnStateMachine.Pawn.TargetResource.CurrentCollectingCount++;
        PawnStateMachine.Pawn.Target = null;
        PawnStateMachine.Pawn.PawnVisual.Connect(PawnVisual.SignalName.OnInteractAnimationFinished, Callable.From(Gather));

        Gather();
    }

    public override void Deactivate()
    {
        base.Deactivate();
        // PawnStateMachine.Pawn.TargetResource.CurrentCollectingCount--;
        PawnStateMachine.Pawn.PawnVisual.Stop();
        PawnStateMachine.Pawn.PawnVisual.Disconnect(PawnVisual.SignalName.OnInteractAnimationFinished, Callable.From(Gather));
    }

    private void Gather()
    {
        if (PawnStateMachine.Pawn.ResourceToCollectData.CollectedCount >= Pawn.MaxCollectableCapacity)
        {
            PawnStateMachine.MoveToClosestBuilding();
            return;
        }

        if (!IsInstanceValid(PawnStateMachine.Pawn.TargetResource))
        {
            PawnStateMachine.MoveToClosestResourceIfNotToBuilding();
            return;
        }

        PawnStateMachine.Pawn.PawnVisual.Interact(PawnStateMachine.Pawn.TargetResource.ResourceType.Name, PawnStateMachine.Pawn.TargetObject?.GlobalPosition);
    }

    private void OnInteractAnimatioKeyReached()
    {
        if (IsInstanceValid(PawnStateMachine.Pawn.TargetResource))
        {
            PawnStateMachine.Pawn.TargetResource.CollectOne();
            PawnStateMachine.Pawn.ResourceToCollectData.CollectedCount++;
        }
    }
}
