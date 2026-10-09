using Godot;

public partial class MeatResourceBase : ResourceBase
{
    [Export] private UnitBase AttachedTo;
    [Export] private UnitVisualBase Visual;
    
    public override void CollectOne()
    {
        base.CollectOne();

        Visual.TakeDamage(1);
    }

    public override void Destroy()
    {
        AttachedTo.QueueFree();
    }
}
