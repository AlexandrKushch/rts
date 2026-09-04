public partial class SheepIdleState : SheepStateBase
{
    private double _timer;

    public override void Activate()
    {
        base.Activate();
        _timer = RandomExtension.RandomDouble() * 10;
    }

    public override void Deactivate()
    {
        base.Deactivate();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        _timer -= delta;

        if (_timer <= 0)
        {
            StateMachine.ChangeState(SheepStateIds.Move);
        }
    }
}
