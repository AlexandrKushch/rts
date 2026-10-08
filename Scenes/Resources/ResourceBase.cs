using Godot;

public partial class ResourceBase : Node2D
{
    public bool Destroyed { get; set; } = false;
    public int CurrentCollectingCount { get; set; }

    [Export]
    public ResourceInfo ResourceType { get; set; }

    [Export]
    public int Quantity { get; set; }

    public override void _Ready()
    {
        base._Ready();
        TreeExited += OnExitTree;
    }

    public virtual void CollectOne()
    {
        Quantity -= 1;

        if (Quantity <= 0)
        {
            Destroy();
        }
    }

    public virtual void Destroy()
    {
        Destroyed = true;
        QueueFree();
    }

    public virtual void OnExitTree()
    {
        NavigationRegionController.Instance.Bake();
    }
}
