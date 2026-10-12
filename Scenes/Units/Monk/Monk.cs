using Godot;

public partial class Monk : UnitHasVisualBase
{
    public const float RangeHealDistance = 100;
    public const float MaxDistanceToLeaveFromAttachedPoint = 200;

    public MonkVisual MonkVisual { get; private set; }
    public UnitBase HealTarget { get; private set; }
    public MonkStateMachine StateMachine { get; private set; }
    public AllyDetector AllyDetector { get; private set; }

    public override void _Ready()
    {
        base._Ready();
        
		if (Meta != null && !Setup)
		{
            return;
        }

        MonkVisual = Visual as MonkVisual;

        StateMachine = GetNode<MonkStateMachine>(nameof(StateMachine));
        AllyDetector = GetNode<AllyDetector>(nameof(AllyDetector));

        SetTarget(Target, TargetObject);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        
		if (StateMachine.GetCurrentStateType() == MonkStateIds.MoveTo)
        {
            Node2D target;
            Vector2? point;

            if (Target != null
                && TryGetClosestTarget(Target.Value, null, ObstacleCollisionMasks[nameof(UnitBase)], out target, out point))
            {
                point += point.Value.DirectionTo(GlobalPosition) * 25;
                SetTarget(point, null);
            }
        }
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
