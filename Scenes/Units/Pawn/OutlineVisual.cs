using Godot;

public partial class OutlineVisual : Sprite2D
{
    [Export] private Sprite2D DuplicateThe;

    public override void _Ready()
    {
        base._Ready();
        UpdateVisible(false);
    }

    public override void _Process(double delta)
    {
        FlipH = DuplicateThe.FlipH;
        Hframes = DuplicateThe.Hframes;
        Frame = DuplicateThe.Frame;
        Texture = DuplicateThe.Texture;
    }

    public void UpdateVisible(bool value)
    {
        Visible = value;
        SetProcess(value);
    }
}
