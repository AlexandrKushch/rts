using Godot;

public partial class SheepEatState : SheepStateBase
{
    public override void Activate()
    {
        base.Activate();
        SheepStateMachine.Sheep.Visual.Interact();

        SheepStateMachine.Sheep.Visual.Connect(SheepVisual.SignalName.OnInteractAnimationFinished, Callable.From(GrassEaten));
    }

    public override void Deactivate()
    {
        base.Deactivate();
        SheepStateMachine.Sheep.Visual.Disconnect(SheepVisual.SignalName.OnInteractAnimationFinished, Callable.From(GrassEaten));
    }


    private void GrassEaten()
    {
        SheepStateMachine.ChangeState(SheepStateIds.Idle);
    }
}
