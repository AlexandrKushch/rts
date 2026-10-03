using Godot;

public partial class MonkMoveToUnitState : MonkStateBase
{
    public override void Activate()
    {
        base.Activate();

        if (!IsInstanceValid(Monk.HealTarget))
        {
            ChangeState(MonkStateIds.Idle);
            return;
        }

        Monk.SetTarget(Monk.HealTarget.GlobalPosition, Monk.HealTarget);
    }

    public override void _Process(double delta)
    {
        if (Monk.FarFromAttachedPoint())
        {
            Monk.SetTarget(Monk.AttachedToPoint, null);
        }
        else if (Monk.HealTargetInRange())
        {
            Monk.SetTarget(Monk.GlobalPosition, null);
            ChangeState(MonkStateIds.Heal);
        }
    }
}
