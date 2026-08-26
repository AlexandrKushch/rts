using System.Diagnostics.CodeAnalysis;
using Godot;

public partial class SelectableComponent : Area2D
{
    private Node2D Visual;

    [Export]
    public Node2D EffectedOn { get; private set; }

    public override void _Ready()
    {
        Visual = GetNode<Node2D>(nameof(Visual));

        UpdateSelection(false);
    }

    public void UpdateSelection(bool value)
    {
        Visual.Visible = value;
    }
}
