using Godot;

public partial class SelectableComponent : Area2D
{
    private Node2D Visual;
    private CollisionShape2D CollisionShape2D;

    public bool Selected { get; private set; }

    [Export]
    public Node2D EffectedOn { get; private set; }

    public override void _Ready()
    {
        Visual = GetNode<Node2D>(nameof(Visual));
        CollisionShape2D = GetNode<CollisionShape2D>(nameof(CollisionShape2D));

        UpdateSelection(false);

        MouseEntered += () => { CustomMouseCursors.Instance.UpdateCursorPointerCount(+1); };
        MouseExited += () => { CustomMouseCursors.Instance.UpdateCursorPointerCount(-1); };
    }

    public override void _ExitTree()
    {
        base._ExitTree();

        CustomMouseCursors.Instance.UpdateCursorPointerCount(-1);
    }

    public virtual void UpdateSelection(bool value)
    {
        if (!IsInstanceValid(Visual))
        {
            return;
        }
        Selected = value;
        Visual.Visible = value;
    }
}
