using Godot;

public partial class TreeBase : ResourceBase
{
    private const string IdleAnimationName = "idle";

    private double _timer;

    private Node2D Visual;
    private Sprite2D Sprite2D;
    private Sprite2D Stump;
    private Timer DestroyTimer;

    [Export]
    private AnimationPlayer AnimationPlayer;

    [Export]
    private CollisionShape2D[] CollisionShapes;

    public override void _Ready()
    {
        base._Ready();
        Visual = GetNode<Node2D>(nameof(Visual));
        Sprite2D = Visual.GetNode<Sprite2D>(nameof(Sprite2D));
        Stump = Visual.GetNode<Sprite2D>(nameof(Stump));
        DestroyTimer = GetNode<Timer>(nameof(DestroyTimer));

        Stump.Visible = false;
        _timer = RandomExtension.RandomDouble();

        DestroyTimer.Timeout += QueueFree;
    }

    public override void _Process(double delta)
    {
        _timer -= delta;

        if (_timer <= 0)
        {
            AnimationPlayer.Play(IdleAnimationName);
        }
    }

    public override void CollectOne(Pawn by)
    {
        base.CollectOne(by);

        var tween = CreateTween()
            .SetTrans(Tween.TransitionType.Bounce);
        float tweenDuration = 0.25f;
        tween.TweenProperty(Visual, "scale", new Vector2(0.9f, 1.1f), tweenDuration * 0.5f);
        tween.TweenProperty(Visual, "scale", new Vector2(1.0f, 1.0f), tweenDuration * 0.5f);
    }

    public override void Destroy()
    {
        Destroyed = true;
        Sprite2D.Visible = false;
        Stump.Visible = true;

        foreach (var collsion in CollisionShapes)
        {
            collsion.Disabled = true;
        }

        NavigationRegionController.Instance.Bake();
        DestroyTimer.Start();
    }

    public override void OnExitTree()
    {
    }
}
