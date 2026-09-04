using Godot;

public partial class PawnStateBase : StateBase<PawnStateIds>
{
    protected PawnStateManagerBase PawnStateMachine;

    public override void _Ready()
    {
        base._Ready();
        PawnStateMachine = StateMachine as PawnStateManagerBase;
    }
}