public partial class ArcherAttackState : ArcherStateBase
{
    public override void Activate()
    {
        base.Activate();

        ArcherStateMachine.Archer.ArcherVisual.OnAttackAnimationFinished += Attack;

        Attack();
    }

    public override void Deactivate()
    {
        base.Deactivate();
        ArcherStateMachine.Archer.ArcherVisual.Stop();
        ArcherStateMachine.Archer.ArcherVisual.OnAttackAnimationFinished -= Attack;
    }

    private void Attack()
    {
        if (!IsInstanceValid(ArcherStateMachine.Archer.AttackTarget))
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
