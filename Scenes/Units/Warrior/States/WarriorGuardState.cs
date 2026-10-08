using System.Linq;

public partial class WarriorGuardState : WarriorStateBase
{
    public override void Activate()
    {
        base.Activate();

        WarriorStateMachine.Warrior.UpdateGuardState(true);
    }

    public override void Deactivate()
    {
        base.Deactivate();
        
        WarriorStateMachine.Warrior.UpdateGuardState(false);
    }

    public override void _Process(double delta)
    {
        bool shouldGuardCuaseOfEnemy = WarriorStateMachine.Warrior.EnemyDetector.Items
                .Any(x => x is Warrior warrior && warrior.CanCharge
                    || x is Lancer && x.GlobalPosition.DistanceTo(WarriorStateMachine.Warrior.GlobalPosition) < Lancer.AttackRangeDistance + 20);
        bool shouldGuardCuaseOfArrow = WarriorStateMachine.Warrior.ArrowDetector.Items
                .Any(x => x.GetParent().GetParent<ArrowPath>().Team != WarriorStateMachine.Warrior.Team);

        bool shouldGuard = shouldGuardCuaseOfEnemy || shouldGuardCuaseOfArrow;

        if (!shouldGuard || !WarriorStateMachine.Warrior.CanGuard())
        {
            WarriorStateMachine.ChangeState(WarriorStateIds.Idle);
        }
    }
}
