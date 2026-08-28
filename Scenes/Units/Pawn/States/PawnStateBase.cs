using Godot;

public partial class PawnStateBase : Node
{
    protected PawnGatheringStateManager StateManager;

    public override void _Ready()
    {
        StateManager = GetParent<PawnGatheringStateManager>();
        SetProcess(false);
    }

    public virtual void Activate()
    {
        SetProcess(true);        
    }

    public virtual void Deactivate()
    {
        SetProcess(false);
    }
}