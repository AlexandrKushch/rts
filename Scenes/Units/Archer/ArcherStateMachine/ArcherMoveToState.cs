using Godot;

public partial class ArcherMoveToState : ArcherStateBase
{
    public override void _Process(double delta)
    {
        if (ArcherStateMachine.Archer.NavigationAgent2D.IsNavigationFinished())
        {
            ArcherStateMachine.ChangeState(ArcherStateIds.Idle);
        }
    }
}
