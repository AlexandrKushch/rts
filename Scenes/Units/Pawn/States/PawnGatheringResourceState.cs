using Godot;

public partial class PawnGatheringResourceState : PawnStateBase
{
    public override void Activate()
    {
        base.Activate();

        PawnStateMachine.Pawn.Target = null;
        PawnStateMachine.Pawn.Visual.Connect(PawnVisual.SignalName.OnInteractAnimationFinished, Callable.From(Gather));

        Gather();
    }

    public override void Deactivate()
    {
        base.Deactivate();
        PawnStateMachine.Pawn.Visual.Stop();
        PawnStateMachine.Pawn.Visual.Disconnect(PawnVisual.SignalName.OnInteractAnimationFinished, Callable.From(Gather));
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

        PawnStateMachine.Pawn.Visual.Interact(PawnStateMachine.Pawn.TargetResource.ResourceType.Name);
    }

    private void OnInteractAnimatioKeyReached()
    {
        if (IsInstanceValid(PawnStateMachine.Pawn.TargetResource))
        {
            PawnStateMachine.Pawn.TargetResource.CollectOne();
            PawnStateMachine.Pawn.ResourceToCollectData.CollectedCount++;
            GD.Print("Gather +1");
        }
    }
}
