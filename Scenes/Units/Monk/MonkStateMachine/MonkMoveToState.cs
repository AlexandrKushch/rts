public partial class MonkMoveToState : MonkStateBase
{
    public override void _Process(double delta)
    {
        if (MonkStateMachine.Monk.NavigationAgent2D.IsNavigationFinished())
        {
            MonkStateMachine.ChangeState(MonkStateIds.Idle);
        }
    }
}
