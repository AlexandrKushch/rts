using Godot;

public partial class Archer : UnitHasVisualBase
{
	protected override float MovementSpeed => 150f;
    public UnitBase AttackTarget { get; private set; }

    public ArcherVisual ArcherVisual { get; private set; }
    public ArcherStateMachine StateMachine { get; private set; }
    public Area2D EnemyDetector { get; private set; }
    public Marker2D ShootPoint { get; set; }

    [Export] public PackedScene ArrowScene { get; private set; }

    public override void _Ready()
    {
        base._Ready();

        ArcherVisual = Visual as ArcherVisual;
        StateMachine = GetNode<ArcherStateMachine>(nameof(StateMachine));
        EnemyDetector = GetNode<Area2D>(nameof(EnemyDetector));
        ShootPoint = GetNode<Marker2D>(nameof(ShootPoint));

        StateMachine.ChangeState(ArcherStateIds.Idle);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
    }

    public override void SetTarget(Vector2? targetPosition, Node2D targetObject)
    {
        var changeToState = ArcherStateIds.MoveTo;

        if (targetObject is UnitBase)
        {
            changeToState = ArcherStateIds.MoveToEnemy;
        }

        base.SetTarget(targetPosition, targetObject);
        StateMachine.ChangeState(changeToState);
    }

    public void SetAttackTarget(UnitBase unit)
    {
        AttackTarget = unit;
        StateMachine.ChangeState(ArcherStateIds.Attack);
    }

    public float GetRadius()
    {
        return (EnemyDetector.GetNode<CollisionShape2D>(nameof(CollisionShape2D)).Shape as CircleShape2D).Radius;
    }

    public void ShootArrow()
    {
        var arrowPath = ArrowScene.Instantiate<ArrowPath>();
        arrowPath.GlobalPosition = GlobalPosition;
        arrowPath.Team = Team;
        GlobalResources.Instance.World.AddChild(arrowPath);

        float radius = GetRadius();

        var changeShootPointPosition = ShootPoint.Position;
        changeShootPointPosition.X = Mathf.Abs(changeShootPointPosition.X);
        changeShootPointPosition.X = Mathf.Sign(AttackTarget.GlobalPosition.X - GlobalPosition.X) > 0
            ? changeShootPointPosition.X
            : changeShootPointPosition.X * -1;
        ShootPoint.Position = changeShootPointPosition;

        var randomness = Mathf.Remap(
            GlobalPosition.DistanceTo(AttackTarget.GlobalPosition),
            0,
            radius,
            25,
            100);
        var randomTargetPosition = AttackTarget.GlobalPosition + RandomExtension.GetRandomPointInCircle(randomness);
        arrowPath.SetTarget(ShootPoint.Position, randomTargetPosition, radius);
    }
}
