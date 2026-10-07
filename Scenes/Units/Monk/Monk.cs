using Godot;

public partial class Monk : UnitHasVisualBase
{
    public const float RangeHealDistance = 100;
    public const float MaxDistanceToLeaveFromAttachedPoint = 200;

    public Vector2 AttachedToPoint { get; set; }
    public MonkVisual MonkVisual { get; private set; }
    public UnitBase HealTarget { get; private set; }
    public MonkStateMachine StateMachine { get; private set; }
    public AllyDetector AllyDetector { get; private set; }

    public override void _Ready()
    {
        base._Ready();

        MonkVisual = Visual as MonkVisual;

        StateMachine = GetNode<MonkStateMachine>(nameof(StateMachine));
        AllyDetector = GetNode<AllyDetector>(nameof(AllyDetector));

        AttachedToPoint = GlobalPosition;

        StateMachine.ChangeState(MonkStateIds.Idle);
    }

    public override void SetTarget(Vector2? targetPosition, Node2D targetObject)
    {
        var changeToState = MonkStateIds.MoveTo;

        if (targetObject is UnitBase)
        {
            changeToState = MonkStateIds.MoveToUnit;
        }

        base.SetTarget(targetPosition, targetObject);

        if (changeToState != StateMachine.GetCurrentStateType())
        {
            StateMachine.ChangeState(changeToState);
        }
    }

    public void SetHealTarget(UnitBase unit)
    {
        HealTarget = unit;
        StateMachine.ChangeState(MonkStateIds.MoveToUnit);
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

    public bool HealTargetInRange()
    {
        if (!IsInstanceValid(HealTarget))
        {
            StateMachine.ChangeState(MonkStateIds.Idle);
            return false;
        }

        var distanceToAttackTarget = GlobalPosition.DistanceTo(HealTarget.GlobalPosition);

        return distanceToAttackTarget < RangeHealDistance;
    }

    public void HealTheTarget()
    {
        HealTarget.Heal(1);   
    }
}
