using Godot;
using System.Linq;

public partial class Warrior : UnitHasVisualBase
{
    private const double ChargeTimeout = 3f;
    private const int MaxGuardingCount = 3;
    private double _chargeTimer = 0.0f;

    private UpdateMovementAnimation _updateMovementAnimation;

    public bool CanAttack { get; private set; } = true;
    public bool CanCharge { get; private set; } = false;
    public int GuardingCount { get; private set; } = MaxGuardingCount;

    public const float RangeChargeDistance = 130;
    public const float RangeAttackDistance = 100;
    public const float MaxDistanceToLeaveFromAttachedPoint = 200;

    public Vector2 AttachedToPoint { get; set; }
    public Node2D AttackTarget { get; private set; }
    public WarriorVisual WarriorVisual { get; private set; }
    public ArrowDetector ArrowDetector { get; private set; }
    public EnemyDetector EnemyDetector { get; private set; }
    public WarriorStateMachine StateMachine { get; private set; }
    public Timer AttackReloadTimer { get; private set; }
    public Timer GuardReloadTimer { get; private set; }

    private delegate void UpdateMovementAnimation(Vector2 velocity);

    public override void _Ready()
    {
        base._Ready();

        if (Meta != null && !Setup)
        {
            return;
        }

        WarriorVisual = Visual as WarriorVisual;
        ArrowDetector = GetNode<ArrowDetector>(nameof(ArrowDetector));
        EnemyDetector = GetNode<EnemyDetector>(nameof(EnemyDetector));
        StateMachine = GetNode<WarriorStateMachine>(nameof(StateMachine));
        AttackReloadTimer = GetNode<Timer>(nameof(AttackReloadTimer));
        GuardReloadTimer = GetNode<Timer>(nameof(GuardReloadTimer));

        AttachedToPoint = Target ?? GlobalPosition;
        SetTarget(Target, TargetObject);

        AttackReloadTimer.Timeout += () => { CanAttack = true; };
        GuardReloadTimer.Timeout += () => { GuardingCount = MaxGuardingCount; };

        _updateMovementAnimation = (v) => { WarriorVisual.UpdateMovement(v, string.Empty); };
    }

    public override void _Process(double delta)
    {
        if (!Dashed)
        {
            _updateMovementAnimation(Velocity);
        }

        if (!InGuard()
            && CanGuard())
        {
            bool shouldGuardCuaseOfEnemy = EnemyDetector.Items
                .Any(x => x is Lancer && x.GlobalPosition.DistanceTo(GlobalPosition) < Lancer.AttackRangeDistance + 20);
            bool shouldGuardCauseOfArrow = ArrowDetector.Items
                .Any(x => x.GetParent().GetParent<ArrowPath>().Team != Team);

            if (shouldGuardCuaseOfEnemy || shouldGuardCauseOfArrow)
            {
                StateMachine.ChangeState(WarriorStateIds.Guard);
            }
        }

        Visual.UpdateOutlineVisible(UnitOverlapedDetector.GetOverlappingAreas().Count > 0);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (Velocity.Length() > 0)
        {
            _chargeTimer += delta;

            if (_chargeTimer > ChargeTimeout)
            {
                CanCharge = true;
            }
        }
        else if (Velocity.Length() == 0)
        {
            CanCharge = false;
            _chargeTimer = 0.0f;
        }

        if (StateMachine.GetCurrentStateType() == WarriorStateIds.MoveTo
            || StateMachine.GetCurrentStateType() == WarriorStateIds.MoveToEnemy)
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
        var changeToState = WarriorStateIds.MoveTo;

        if (targetObject is IHasTeam node && node.Team != Team)
        {
            AttackTarget = targetObject;
            changeToState = WarriorStateIds.MoveToEnemy;
        }

        base.SetTarget(targetPosition, targetObject);

        if (StateMachine.GetCurrentStateType() != changeToState)
        {
            StateMachine.ChangeState(changeToState);
        }
    }

    public void SetAttackTarget(Node2D unit)
    {
        AttackTarget = unit;
        StateMachine.ChangeState(WarriorStateIds.MoveToEnemy);
    }

    public bool AttackTargetInRange()
    {
        if (!IsInstanceValid(AttackTarget))
        {
            StateMachine.ChangeState(WarriorStateIds.Idle);
            return false;
        }

        var distanceToAttackTarget = GlobalPosition.DistanceTo(AttackTarget.GlobalPosition);

        return distanceToAttackTarget < RangeAttackDistance;
    }

    public bool ChargeTargetInRange()
    {
        if (!IsInstanceValid(AttackTarget))
        {
            StateMachine.ChangeState(WarriorStateIds.Idle);
            return false;
        }

        if (AttackTarget is BuildingBase)
        {
            return false;
        }

        var distanceToAttackTarget = GlobalPosition.DistanceTo(AttackTarget.GlobalPosition);

        return distanceToAttackTarget < RangeChargeDistance;
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
        return GlobalPosition.IsEqualApprox(AttachedToPoint, 50);
    }

    public void HitAttackTarget()
    {
        CanAttack = false;
        AttackReloadTimer.Start();

        if (!AttackTargetInRange())
        {
            return;
        }

        (AttackTarget as IDestroyableWithHp).TakeDamage(1);
    }

    public void HitWithChargeTarget()
    {
        CanAttack = false;
        AttackReloadTimer.Start();

        if (!AttackTargetInRange())
        {
            return;
        }

        (AttackTarget as UnitBase).TakeDamageWithDash(GlobalPosition, 1, 2.2f);
        CanCharge = false;
    }

    public void UpdateGuardState(bool guard)
    {
        if (guard)
        {
            _updateMovementAnimation = WarriorVisual.Guard;
            MovementSpeed /= 2;
        }
        else
        {
            _updateMovementAnimation = (v) => { WarriorVisual.UpdateMovement(v, string.Empty); };
            MovementSpeed *= 2;
        }
    }

    public bool InGuard()
    {
        return StateMachine.GetCurrentStateType() == WarriorStateIds.Guard;
    }

    public bool CanGuard()
    {
        return GuardingCount > 0;
    }

    public override void TakeDamageWithDash(Vector2 from, int value, float power)
    {
        bool inGuard = InGuard();

        if (inGuard)
        {
            GuardingCount -= 1;
            GuardReloadTimer.Stop();
            GuardReloadTimer.Start();
        }

        base.TakeDamageWithDash(from, !inGuard ? value : 0, !inGuard ? power : power / 2);

        StateMachine.ChangeState(WarriorStateIds.Idle);
    }
}
