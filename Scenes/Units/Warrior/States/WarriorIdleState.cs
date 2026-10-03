using Godot;
using System.Linq;

public partial class WarriorIdleState : WarriorStateBase
{
    private double _timerBeforeAttack = 0.0f;
    private UnitBase _enemy;

    public override void Activate()
    {
        base.Activate();

        _timerBeforeAttack = 0;
        _enemy = null;

        var enemy = GetEnemyOrGoToBasePoint();

        if (enemy != null)
        {
            WarriorStateMachine.Warrior.SetAttackTarget(enemy);
        }
    }

    public override void _Process(double delta)
    {
        bool shouldGuard = WarriorStateMachine.Warrior.EnemyDetector.Items
            .Any(x => x is Warrior warrior && warrior.CanCharge);

        if (shouldGuard)
        {
            WarriorStateMachine.ChangeState(WarriorStateIds.Guard);
            return;
        }

        if (_enemy != null)
        {
            _timerBeforeAttack += delta;

            if (_timerBeforeAttack > 1)
            {
                WarriorStateMachine.Warrior.SetAttackTarget(_enemy);
            }
            return;
        }

        var enemy = GetEnemyOrGoToBasePoint();

        if (enemy != null)
        {
            _enemy = enemy;
        }
    }

    private UnitBase GetEnemyOrGoToBasePoint()
    {
        var enemy = WarriorStateMachine.Warrior.EnemyDetector.Items.MinBy(x => WarriorStateMachine.Warrior.GlobalPosition.DistanceTo(x.GlobalPosition));
     
        if ((enemy == null && !WarriorStateMachine.Warrior.ReachedAttachedPoint())
            || WarriorStateMachine.Warrior.FarFromAttachedPoint())
        {
            GD.Print("GO HOME WALTER");
            WarriorStateMachine.Warrior.SetTarget(WarriorStateMachine.Warrior.AttachedToPoint, null);
            return null;
        }

        return enemy;
    }
}
