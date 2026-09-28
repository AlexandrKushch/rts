public partial class ArcherAttackState : ArcherStateBase
{
    public override void Activate()
    {
        base.Activate();

        if (ArcherStateMachine.Archer.CanShoot)
        {
            Attack();
        }
    }

    public override void Deactivate()
    {
        base.Deactivate();
        ArcherStateMachine.Archer.ArcherVisual.Stop();
    }

    public override void _Process(double delta)
    {
        if (ArcherStateMachine.Archer.CanShoot)
        {
            Attack();
        }
    }

    private void Attack()
    {
        if (!IsInstanceValid(ArcherStateMachine.Archer.AttackTarget))
        {
            ArcherStateMachine.ChangeState(ArcherStateIds.Idle);
            return;
        }

        if (ArcherStateMachine.Archer.GlobalPosition
            .DistanceTo(ArcherStateMachine.Archer.AttackTarget.GlobalPosition)
            > ArcherStateMachine.Archer.Radius + ArcherStateMachine.Archer.Radius / 1)
        {
            ArcherStateMachine.ChangeState(ArcherStateIds.Idle);
            return;
        }

        ArcherStateMachine.Archer.ArcherVisual.Attack(ArcherStateMachine.Archer.AttackTarget.GlobalPosition);
    }

    private void OnAttackAnimationKeyReached()
    {
        if (!IsInstanceValid(ArcherStateMachine.Archer.AttackTarget))
        {
            ArcherStateMachine.ChangeState(ArcherStateIds.Idle);
            return;
        }

        ArcherStateMachine.Archer.ShootArrow();
    }
}
