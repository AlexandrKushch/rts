using Godot;

public partial class ResourceBase : Node2D
{
    [Export]
    public ResourceType ResourceType { get; set; }

    [Export]
    public int Quantity { get; set; }

    public void CollectOne()
    {
        Quantity -= 1;

        if (Quantity <= 0)
        {
            QueueFree();
        }
    }
}
