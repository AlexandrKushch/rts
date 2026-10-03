using System.Linq;

public partial class ArcherMoveToEnemyState : ArcherStateBase
{
    public override void _Process(double delta)
    {
        var enemy = ArcherStateMachine.Archer.EnemyDetector.Items
            .FirstOrDefault(x => x.GetInstanceId() == ArcherStateMachine.Archer.TargetObject?.GetInstanceId());

        if (enemy != null)
        {
            ArcherStateMachine.Archer.SetTarget(ArcherStateMachine.Archer.GlobalPosition, null);
            ArcherStateMachine.Archer.SetAttackTarget(enemy);
        }
    }
}
