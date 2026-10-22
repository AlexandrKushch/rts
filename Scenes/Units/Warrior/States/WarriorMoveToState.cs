public partial class WarriorMoveToState : WarriorStateBase
{
    public override void _Process(double delta)
    {
        if (Warrior.NavigationAgent2D.IsNavigationFinished())
        {
            WarriorStateMachine.ChangeState(WarriorStateIds.Idle);

            if (Warrior.Formation == null)
            {
                Warrior.Target = null;
            }
        }   
    }
}
