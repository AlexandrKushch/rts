using Godot;

public partial class GoldBase : ResourceBase
{
    private const string IdleAnimationName = "idle";

    private double _timer;
    private Node2D Visual;

    [Export]
    private AnimationPlayer AnimationPlayer;

    public override void _Ready()
    {
        Visual = GetNode<Node2D>(nameof(Visual));

        _timer = RandomExtension.RandomDouble() * 10;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        _timer -= delta;

        if (_timer <= 0)
        {
            _timer = RandomExtension.RandomDouble() * 10;

            AnimationPlayer.Play(IdleAnimationName);
        }
    }


    public override void CollectOne()
    {
        base.CollectOne();

        var tween = CreateTween()
            .SetTrans(Tween.TransitionType.Bounce);
        float tweenDuration = 0.15f;
        tween.TweenProperty(Visual, "scale", new Vector2(1.1f, 0.9f), tweenDuration * 0.5f);
        tween.TweenProperty(Visual, "scale", new Vector2(1.0f, 1.0f), tweenDuration * 0.5f);
    }
}
