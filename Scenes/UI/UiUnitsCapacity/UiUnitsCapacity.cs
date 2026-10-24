using Godot;

public partial class UiUnitsCapacity : Control
{
    private Vector2 _originalScale;

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
            TweenCapacityChanged();
        }
    }

    [Export] private Label _label;

    public override void _Ready()
    {
        _originalScale = Scale;
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

            tween.TweenProperty(this, "scale", _originalScale, tweenDuration);
        }
    }

    private void TweenCapacityChanged()
    {
        var tween = CreateTween().SetTrans(Tween.TransitionType.Bounce);
        float tweenDuration = 0.1f;

        tween.TweenProperty(this, "scale", _originalScale + Vector2.One * 0.2f, tweenDuration);
        tween.TweenProperty(this, "scale", _originalScale, tweenDuration);
    }
}
