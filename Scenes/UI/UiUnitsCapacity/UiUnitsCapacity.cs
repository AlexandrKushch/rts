using Godot;

public partial class UiUnitsCapacity : Control
{
    private int _capacity;
    public int Capacity
    {
        get
        {
            return _capacity;
        }
        set
        {
            _capacity = value;
            _label.Text = _capacity.ToString();

            Visible = _capacity > 0;
        }
    }

    [Export] private Label _label;

    public override void _Ready()
    {
        Capacity = 0;
        VisibilityChanged += TweenVisibility;
    }

    private void TweenVisibility()
    {
        if (Visible)
        {
            Scale = Vector2.Zero;
            var tween = CreateTween();
            float tweenDuration = 0.15f;

            tween.TweenProperty(this, "scale", Vector2.One, tweenDuration);
        }
    }
}
