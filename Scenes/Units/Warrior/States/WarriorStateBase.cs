public partial class WarriorStateBase : StateBase<WarriorStateIds>
{
    protected WarriorStateMachine WarriorStateMachine;

    public override void _Ready()
    {
        base._Ready();
        WarriorStateMachine = StateMachine as WarriorStateMachine;
    }
}
