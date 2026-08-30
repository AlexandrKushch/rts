using Godot;

public partial class PawnStateBase : Node
{
    protected PawnStateManagerBase StateManager;

    public override void _Ready()
    {
        StateManager = GetParent<PawnStateManagerBase>();
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