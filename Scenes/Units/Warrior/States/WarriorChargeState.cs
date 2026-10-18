using Godot;

public partial class WarriorChargeState : WarriorStateBase
{
    public override void Activate()
    {
        base.Activate();

        WarriorStateMachine.Warrior.NavigationAgent2D.TargetPosition = WarriorStateMachine.Warrior.GlobalPosition;
		var direction = WarriorStateMachine.Warrior.GlobalPosition.DirectionTo(WarriorStateMachine.Warrior.AttackTarget.GlobalPosition);
		WarriorStateMachine.Warrior.Velocity += direction * 2000;
		WarriorStateMachine.Warrior.Dashed = true;
        WarriorStateMachine.Warrior.WarriorVisual.Charge(WarriorStateMachine.Warrior.AttackTarget.GlobalPosition);
    }

    public override void _Process(double delta)
    {
        var collision = WarriorStateMachine.Warrior.GetLastSlideCollision();

        if (collision != null)
        {
            if (collision.GetCollider() is UnitBase)
            {
                WarriorStateMachine.Warrior.HitWithChargeTarget();
            }
            else
            {
                WarriorStateMachine.Warrior.HitAttackTarget();
            }
            
            WarriorStateMachine.ChangeState(WarriorStateIds.MoveToEnemy);
        }
    }
}
