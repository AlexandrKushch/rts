using Godot;

public partial class Lancer : UnitHasVisualBase
{
    public const float AttackRangeDistance = 135;
    public const float MaxDistanceToLeaveFromAttachedPoint = 200;

    public bool IsAttacking { get; set; } = false;

    public Vector2 AttachedToPoint { get; set; }
    public UnitBase AttackTarget { get; set; }
    public LancerVisual LancerVisual { get; private set; }
    public LancerStateMachine StateMachine { get; private set; }
    public EnemyDetector EnemyDetector { get; private set; }
    public Timer ToIdleStateTimer { get; set; }
    public Timer UpdateAttackTargetTimer { get; set; }

    public override void _Ready()
    {
        base._Ready();

        LancerVisual = Visual as LancerVisual;

        StateMachine = GetNode<LancerStateMachine>(nameof(StateMachine));
        EnemyDetector = GetNode<EnemyDetector>(nameof(EnemyDetector));
        ToIdleStateTimer = GetNode<Timer>(nameof(ToIdleStateTimer));
        UpdateAttackTargetTimer = GetNode<Timer>(nameof(UpdateAttackTargetTimer));

        AttachedToPoint = GlobalPosition;
        StateMachine.ChangeState(LancerStateIds.Idle);

        LancerVisual.OnAttackAnimationFinished += () => { IsAttacking = false; };
    }

    public override void _Process(double delta)
    {
        Visual.UpdateOutlineVisible(UnitOverlapedDetector.GetOverlappingAreas().Count > 0);
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
        AttackTarget.TakeDamageWithDash(GlobalPosition, 0, 1.5f);
        IsAttacking = true;
        LancerVisual.Attack();
    }

    public override void TakeDamageWithDash(Vector2 from, int value, float power)
    {
        base.TakeDamageWithDash(from, value, power);
        StateMachine.ChangeState(LancerStateIds.MoveTo);
    }
}
