using Godot;

public partial class BushDecoration : Node2D
{
    public Area2D UnitEnteredSquishArea { get; private set; }

    public override void _Ready()
    {
        UnitEnteredSquishArea = GetNode<Area2D>(nameof(UnitEnteredSquishArea));
        UnitEnteredSquishArea.BodyEntered += Squish;

        Scale = Vector2.Zero;

        var tween = CreateTween().SetTrans(Tween.TransitionType.Bounce);
        float tweenDuration = 0.3f;

        tween.TweenProperty(this, "scale", Vector2.One, tweenDuration);
    }

    private void Squish(Node2D _)
    {
        var tween = CreateTween().SetTrans(Tween.TransitionType.Bounce);
        float tweenDuration = 0.3f;

        tween.TweenProperty(this, "scale", new Vector2(1.1f, 0.9f), tweenDuration * 0.5f);
        tween.TweenProperty(this, "scale", Vector2.One, tweenDuration * 0.5f);
    }
}
