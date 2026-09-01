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
            // Deactivate();
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
