public partial class WarriorMoveToState : WarriorStateBase
{
    public override void _Process(double delta)
    {
        if (WarriorStateMachine.Warrior.NavigationAgent2D.IsNavigationFinished())
        {
            WarriorStateMachine.ChangeState(WarriorStateIds.Idle);
        }   
    }
}
