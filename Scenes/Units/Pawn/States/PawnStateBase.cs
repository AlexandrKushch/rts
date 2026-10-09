using Godot;

public partial class PawnStateBase : StateBase<PawnStateIds>
{
    protected PawnStateManagerBase PawnStateMachine;

    protected Pawn Pawn => PawnStateMachine.Pawn;

    public override void _Ready()
    {
        base._Ready();
        PawnStateMachine = StateMachine as PawnStateManagerBase;
    }
}