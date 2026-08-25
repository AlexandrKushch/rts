using Godot;

public partial class PawnStateBase : Node
{
    public virtual void Activate()
    {
        SetProcess(true);        
    }

    public virtual void Deactivate()
    {
        SetProcess(false);
    }
}