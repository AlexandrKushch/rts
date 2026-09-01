using Godot;

public partial class PawnGatheringResourceState : PawnStateBase
{
    private const int MaxCollectableCapacity = 5;

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
        if (StateManager.Pawn.ResourceCollectedCount >= MaxCollectableCapacity)
        {
            var building = StateManager.Pawn.GetClosestResourceStorageBuilding();
            UnitsController.Instance.MoveToObstacleCommand(StateManager.Pawn, building);
            return;
        }

        if (!IsInstanceValid(StateManager.Pawn.TargetResource))
        {
            // TO-DO find another same resource
            // var building = StateManager.Pawn.GetClosestResourceStorageBuilding();
            // UnitsController.Instance.MoveToObstacleCommand(StateManager.Pawn, building);
            return;
        }

        StateManager.Pawn.Visual.Interact(StateManager.Pawn.TargetResource.ResourceType.Name);
    }

    private void OnInteractAnimatioKeyReached()
    {
        if (IsInstanceValid(StateManager.Pawn.TargetResource)
            && StateManager.Pawn.TargetResource.TryCollectOne())
        {
            StateManager.Pawn.ResourceCollectedCount++;
            GD.Print("Gather +1");
        }        
    }
}
