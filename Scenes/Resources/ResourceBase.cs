using Godot;

public partial class ResourceBase : Node2D
{
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
            QueueFree();
        }
    }

    public void OnExitTree()
    {
        base._ExitTree();
        NavigationRegionController.Instance.BakeNavigationPolygon(true);
    }
}
