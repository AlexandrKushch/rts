public partial class ArcherStateBase : StateBase<ArcherStateIds>
{
    protected ArcherStateMachine ArcherStateMachine;

    public Archer Archer => ArcherStateMachine.Archer;

    public override void _Ready()
    {
        base._Ready();
        ArcherStateMachine = StateMachine as ArcherStateMachine;
    }
}
