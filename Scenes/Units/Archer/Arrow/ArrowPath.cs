using Godot;

public partial class ArrowPath : Path2D, IDestroyable
{
    private const float DefautlSpeed = 1000.0f;

    private float _speed = 0.0f;
    private float _t = 0.0f;
    private bool _rightDirection;

    private PathFollow2D ArrowPathFollow;
    private Area2D ArrowHitBox;
    private Sprite2D ArrowVisual;
    private Sprite2D ArrowBrokenVisual;
    private Timer Timer;

    public TeamType Team { get; set; }

    public override void _Ready()
    {
        ArrowPathFollow = GetNode<PathFollow2D>(nameof(ArrowPathFollow));
        ArrowHitBox = ArrowPathFollow.GetNode<Area2D>(nameof(ArrowHitBox));
        ArrowVisual = ArrowPathFollow.GetNode<Sprite2D>(nameof(ArrowVisual));
        ArrowBrokenVisual = ArrowPathFollow.GetNode<Sprite2D>(nameof(ArrowBrokenVisual));
        Timer = GetNode<Timer>(nameof(Timer));

        SetPhysicsProcess(false);

        ArrowHitBox.BodyEntered += OnHitBoxEntered;
        Timer.Timeout += OnDestroyTimerTimeout;
    }

    public override void _PhysicsProcess(double delta)
    {
        _t += _speed * (float)delta;
        if (_t < 1)
        {
            ArrowPathFollow.ProgressRatio = _t;
            Timer.Start();
        }
        else
        {
            ArrowPathFollow.ProgressRatio = 1;

            Destroy();
        }

        ArrowVisual.Scale = Vector2.One + Vector2.One * ((Mathf.Abs(ArrowPathFollow.ProgressRatio * 2 - 1) * -1 + 1) * 0.5f);
    }

    public void SetTarget(Vector2 start, Vector2 target, float radius)
    {
        Vector2 targetLocal = ToLocal(target);
        Curve = Curve.Duplicate() as Curve2D;
        Curve.ClearPoints();
        ArrowPathFollow.Position = Vector2.Zero;

        float distanceToTarget = GlobalPosition.DistanceTo(target);
        float curveHeight = start.Y > targetLocal.Y && Mathf.Abs(start.X - targetLocal.X) < 250 ? 0 : 50;
        int sign = Mathf.Sign(target.X - GlobalPosition.X);
        _rightDirection = sign > 0;

        var perpendicular = targetLocal.DirectionTo(start).Orthogonal();
        var betweenStartTarget = (start + targetLocal) / 2;
        var heightPoint = perpendicular * sign * -curveHeight;
        var middlePoint = betweenStartTarget + heightPoint;

        Curve.AddPoint(start, null, null, 0);
        Curve.AddPoint(
            middlePoint,
            middlePoint.DirectionTo(start + heightPoint) * distanceToTarget / 4,
            middlePoint.DirectionTo(targetLocal + heightPoint) * distanceToTarget / 4,
            1);
        Curve.AddPoint(targetLocal, null, null, 2);

        _speed = DefautlSpeed / Curve.GetBakedLength();
        SetPhysicsProcess(true);
    }

    public void Destroy()
    {
        SetPhysicsProcess(false);
        ArrowVisual.Visible = false;
        if (IsInstanceValid(ArrowBrokenVisual))
        {
            ArrowBrokenVisual.Visible = true;
            ArrowBrokenVisual.FlipV = !_rightDirection;
        }
        Timer.Start();
    }

    public void OnDestroyTimerTimeout()
    {
        ArrowHitBox.BodyEntered -= OnHitBoxEntered;
        if (IsInstanceValid(ArrowBrokenVisual))
        {
            ArrowBrokenVisual.QueueFree();
        }
        QueueFree();
    }

    private void OnHitBoxEntered(Node2D body)
    {
        if (body is UnitBase unit
            && unit.Team != Team)
        {
            Destroy();
            ArrowBrokenVisual.Reparent(body);

            unit.TakeDamage(1);
        }
    }
}
