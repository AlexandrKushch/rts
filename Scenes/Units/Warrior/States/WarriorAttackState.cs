public partial class WarriorAttackState : WarriorStateBase
{
    public override void Activate()
    {
        base.Activate();

        WarriorStateMachine.Warrior.Target = null;

        if (WarriorStateMachine.Warrior.CanAttack)
        {
            Attack();
        }
    }

    public override void Deactivate()
    {
        base.Deactivate();
        WarriorStateMachine.Warrior.WarriorVisual.Stop();
    }

    public override void _Process(double delta)
    {
        if (!IsInstanceValid(WarriorStateMachine.Warrior.AttackTarget))
        {
            WarriorStateMachine.ChangeState(WarriorStateIds.Idle);
            return;
        }

        if (!WarriorStateMachine.Warrior.AttackTargetInRange())
        {
            WarriorStateMachine.ChangeState(WarriorStateIds.MoveToEnemy);    
            return;        
        }
        
        if (WarriorStateMachine.Warrior.CanAttack)
        {
            Attack();
        }
    }

    private void Attack()
    {
        WarriorStateMachine.Warrior.WarriorVisual.Attack(WarriorStateMachine.Warrior.AttackTarget.GlobalPosition);
    }

    private void OnAttackAnimationKeyReached()
    {
        if (!IsInstanceValid(WarriorStateMachine.Warrior.AttackTarget))
        {
            WarriorStateMachine.ChangeState(WarriorStateIds.Idle);
        }
        
        WarriorStateMachine.Warrior.HitAttackTarget();
    }
}
