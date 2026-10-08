using Godot;
using System.Linq;

public partial class LancerIdleState : LancerStateBase
{
    public override void Activate()
    {
        base.Activate();

        Lancer.LancerVisual.UpdateDefence(null);

        Lancer.UpdateAttackTargetTimer.Start();
        Lancer.UpdateAttackTargetTimer.Timeout += UpdateAttackTarget;
    }

    public override void Deactivate()
    {
        base.Deactivate();

        Lancer.LancerVisual.UpdateMovement(Vector2.Zero, null);

        Lancer.UpdateAttackTargetTimer.Stop();
        Lancer.UpdateAttackTargetTimer.Timeout -= UpdateAttackTarget;
        Lancer.AttackTarget = null;
    }

    public override void _Process(double delta)
    {
        var attackTarget = IsInstanceValid(Lancer.AttackTarget)
            ? Lancer.AttackTarget
            : Lancer.EnemyDetector.Items.MinBy(x => Lancer.GlobalPosition.DistanceTo(x.GlobalPosition));

        if (attackTarget != null)
        {
            Lancer.AttackTarget = attackTarget;
            Lancer.LancerVisual.UpdateDefence(Lancer.AttackTarget.GlobalPosition);

            if (Lancer.AttackTarget.GlobalPosition.DistanceTo(Lancer.GlobalPosition) < Lancer.AttackRangeDistance
                && !Lancer.IsAttacking)
            {
                Lancer.Attack();
            }
        }
        else
        {
            Lancer.AttackTarget = null;
            Lancer.LancerVisual.UpdateDefence(null);
        }
    }

    private void UpdateAttackTarget()
    {
        Lancer.AttackTarget = Lancer.EnemyDetector.Items.MinBy(x => Lancer.GlobalPosition.DistanceTo(x.GlobalPosition));
    }
}
