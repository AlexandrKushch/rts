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
        bool shouldGuard = WarriorStateMachine.Warrior.EnemyDetector.Enemies
            .Any(x => x is Warrior warrior && warrior.CanCharge);

        if (!shouldGuard)
        {
            WarriorStateMachine.ChangeState(WarriorStateIds.Idle);
        }
    }
}
