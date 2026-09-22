using Godot;

public partial class SheepEatState : SheepStateBase
{
    public override void Activate()
    {
        base.Activate();
        SheepStateMachine.Sheep.SheepVisual.Interact();

        SheepStateMachine.Sheep.SheepVisual.Connect(SheepVisual.SignalName.OnInteractAnimationFinished, Callable.From(GrassEaten));
    }

    public override void Deactivate()
    {
        base.Deactivate();
        SheepStateMachine.Sheep.SheepVisual.Disconnect(SheepVisual.SignalName.OnInteractAnimationFinished, Callable.From(GrassEaten));
    }


    private void GrassEaten()
    {
        SheepStateMachine.ChangeState(SheepStateIds.Idle);
    }
}
