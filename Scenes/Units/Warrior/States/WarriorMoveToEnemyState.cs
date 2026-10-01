using Godot;

public partial class WarriorMoveToEnemyState : WarriorStateBase
{
    public override void Activate()
    {
        base.Activate();

        if (!IsInstanceValid(WarriorStateMachine.Warrior.AttackTarget))
        {
            WarriorStateMachine.ChangeState(WarriorStateIds.Idle);
            return;
        }

        WarriorStateMachine.Warrior.SetTarget(WarriorStateMachine.Warrior.AttackTarget.GlobalPosition, WarriorStateMachine.Warrior.AttackTarget);
    }

    public override void Deactivate()
    {
        base.Deactivate();
    }

    public override void _Process(double delta)
    {
        if (WarriorStateMachine.Warrior.FarFromAttachedPoint())
        {
            WarriorStateMachine.Warrior.SetTarget(WarriorStateMachine.Warrior.AttachedToPoint, null);
        }
        if (WarriorStateMachine.Warrior.CanCharge
            && WarriorStateMachine.Warrior.CanAttack
            && WarriorStateMachine.Warrior.ChargeTargetInRange())
        {
            GD.Print("REACHED CHARGE DISTANCE");

            WarriorStateMachine.ChangeState(WarriorStateIds.Charge);
        }
        else if (WarriorStateMachine.Warrior.AttackTargetInRange())
        {
            GD.Print("REACHED ATTACK TARGET");

            WarriorStateMachine.Warrior.SetTarget(WarriorStateMachine.Warrior.GlobalPosition, null);
            WarriorStateMachine.ChangeState(WarriorStateIds.Attack);
        }
    }
}
