public partial class WarriorStateBase : StateBase<WarriorStateIds>
{
    protected WarriorStateMachine WarriorStateMachine;

    protected Warrior Warrior => WarriorStateMachine.Warrior;

    public override void _Ready()
    {
        base._Ready();
        WarriorStateMachine = StateMachine as WarriorStateMachine;
    }
}
