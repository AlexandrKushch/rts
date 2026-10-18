using System.Linq;
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

        foreach (var player in GlobalPlayers.Instance.Players)
        {
            if (player.Value is HumanPlayer humanPlayer
                && humanPlayer.SelectionController.Selections.Any(x => x.GetInstanceId() == GetInstanceId()))
            {
                humanPlayer.SelectionController.Selections.Remove(this);
                humanPlayer.SelectionController.EmitSignal(SelectionController.SignalName.SelectionChanged);
            }
        }

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
