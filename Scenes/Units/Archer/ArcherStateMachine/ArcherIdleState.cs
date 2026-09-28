using System.Linq;

public partial class ArcherIdleState : ArcherStateBase
{
    public override void _Process(double delta)
    {
        var enemies = ArcherStateMachine.Archer.EnemyDetector.Enemies.ToArray();

        if (enemies.Length > 0)
        {
            var enemy = enemies[RandomExtension.RandomLong(enemies.Length)];
            ArcherStateMachine.Archer.SetAttackTarget(enemy);
        }
    }
}
