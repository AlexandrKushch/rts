public partial class SheepStateBase : StateBase<SheepStateIds>
{
    protected SheepStateMachine SheepStateMachine;

    public override void _Ready()
    {
        base._Ready();
        SheepStateMachine = StateMachine as SheepStateMachine;
    }
}
