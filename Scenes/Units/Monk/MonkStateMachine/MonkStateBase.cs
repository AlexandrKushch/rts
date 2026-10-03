public partial class MonkStateBase : StateBase<MonkStateIds>
{
    protected MonkStateMachine MonkStateMachine;

    protected Monk Monk => MonkStateMachine.Monk;

    public override void _Ready()
    {
        base._Ready();
        MonkStateMachine = StateMachine as MonkStateMachine;
    }
}
