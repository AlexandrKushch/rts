public partial class LancerStateBase : StateBase<LancerStateIds>
{
    protected LancerStateMachine LancerStateMachine;

    protected Lancer Lancer => LancerStateMachine.Lancer;

    public override void _Ready()
    {
        base._Ready();
        LancerStateMachine = StateMachine as LancerStateMachine;
    }
}
