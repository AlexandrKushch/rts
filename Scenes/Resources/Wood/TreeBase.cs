using Godot;

public partial class TreeBase : ResourceBase
{
    private const string IdleAnimationName = "idle";

    private double _timer;

    private Node2D Visual;

    [Export]
    private AnimationPlayer AnimationPlayer;

    public override void _Ready()
    {
        Visual = GetNode<Node2D>(nameof(Visual));

        _timer = RandomExtension.RandomDouble();
    }

    public override void _Process(double delta)
    {
        _timer -= delta;

        if (_timer <= 0)
        {
            AnimationPlayer.Play(IdleAnimationName);
        }
    }

    public override bool TryCollectOne()
    {
        var result = base.TryCollectOne();

        var tween = CreateTween()
            .SetTrans(Tween.TransitionType.Bounce);
        float tweenDuration = 0.25f;
        tween.TweenProperty(Visual, "scale", new Vector2(0.9f, 1.1f), tweenDuration * 0.5f);
        tween.TweenProperty(Visual, "scale", new Vector2(1.0f, 1.0f), tweenDuration * 0.5f);

        return result;
    }
}
