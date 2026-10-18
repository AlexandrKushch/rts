using Godot;

public partial class Lancer : UnitHasVisualBase
{
    public const float AttackRangeDistance = 135;
    public const float MaxDistanceToLeaveFromAttachedPoint = 200;

    public bool IsAttacking { get; set; } = false;

    public Vector2 AttachedToPoint { get; set; }
    public Node2D AttackTarget { get; set; }
    public LancerVisual LancerVisual { get; private set; }
    public LancerStateMachine StateMachine { get; private set; }
    public EnemyDetector EnemyDetector { get; private set; }
    public Timer ToIdleStateTimer { get; set; }
    public Timer UpdateAttackTargetTimer { get; set; }

    public override void _Ready()
    {
        base._Ready();
        
        if (Meta != null && Predeployed)
		{
            return;
        }

        LancerVisual = Visual as LancerVisual;

        StateMachine = GetNode<LancerStateMachine>(nameof(StateMachine));
        EnemyDetector = GetNode<EnemyDetector>(nameof(EnemyDetector));
        ToIdleStateTimer = GetNode<Timer>(nameof(ToIdleStateTimer));
        UpdateAttackTargetTimer = GetNode<Timer>(nameof(UpdateAttackTargetTimer));

        AttachedToPoint = Target ?? GlobalPosition;
        SetTarget(Target, TargetObject);

        LancerVisual.OnAttackAnimationFinished += () => { IsAttacking = false; };
    }

    public override void _Process(double delta)
    {
        Visual.UpdateOutlineVisible(UnitOverlapedDetector.GetOverlappingAreas().Count > 0);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        
		if (StateMachine.GetCurrentStateType() == LancerStateIds.MoveTo)
        {
            Node2D target;
            Vector2? point;

            if (Target != null
                && TryGetClosestTarget(Target.Value, null, ObstacleCollisionMasks[nameof(UnitBase)] | ObstacleCollisionMasks[nameof(BuildingBase)], out target, out point))
            {
                point += point.Value.DirectionTo(GlobalPosition) * 25;
                SetTarget(point, null);
            }
        }
    }

    public override void SetTarget(Vector2? targetPosition, Node2D targetObject)
    {
        base.SetTarget(targetPosition, targetObject);
        StateMachine.ChangeState(LancerStateIds.MoveTo);
    }

    public bool FarFromAttachedPoint()
    {
        if (Target.HasValue && Target.Value == AttachedToPoint)
        {
            return false;
        }
        return GlobalPosition.DistanceTo(AttachedToPoint) > MaxDistanceToLeaveFromAttachedPoint;
    }

    public bool ReachedAttachedPoint()
    {
        return GlobalPosition.DistanceTo(AttachedToPoint) < 10;
    }

    public bool IsMoving()
    {
        return StateMachine.GetCurrentStateType() == LancerStateIds.MoveTo;
    }

    public void Attack()
    {
        if (AttackTarget is UnitBase unit)
        {
            unit.TakeDamageWithDash(GlobalPosition, 0, 1.5f);
        }
        else
        {
            (AttackTarget as IDestroyableWithHp).TakeDamage(1);
        }
        IsAttacking = true;
        LancerVisual.Attack();
    }

    public override void TakeDamageWithDash(Vector2 from, int value, float power)
    {
        base.TakeDamageWithDash(from, value, power);
        StateMachine.ChangeState(LancerStateIds.MoveTo);
    }
}
