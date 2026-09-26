public partial class ArcherStateBase : StateBase<ArcherStateIds>
{
    protected ArcherStateMachine ArcherStateMachine;

    public override void _Ready()
    {
        base._Ready();
        ArcherStateMachine = StateMachine as ArcherStateMachine;
    }
}
