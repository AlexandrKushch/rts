public partial class MonkMoveToState : MonkStateBase
{
    public override void _Process(double delta)
    {
        if (Monk.NavigationAgent2D.IsNavigationFinished())
        {
            ChangeState(MonkStateIds.Idle);
        }
    }
}
