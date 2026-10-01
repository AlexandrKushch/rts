using Godot;

public partial class WarriorChargeState : WarriorStateBase
{
    public override void Activate()
    {
        base.Activate();

        GD.Print("CHARGE");
        WarriorStateMachine.Warrior.NavigationAgent2D.TargetPosition = WarriorStateMachine.Warrior.GlobalPosition;
		var direction = WarriorStateMachine.Warrior.GlobalPosition.DirectionTo(WarriorStateMachine.Warrior.AttackTarget.GlobalPosition);
		WarriorStateMachine.Warrior.Velocity += direction * 2000;
		WarriorStateMachine.Warrior.Dashed = true;
        WarriorStateMachine.Warrior.WarriorVisual.Charge(WarriorStateMachine.Warrior.AttackTarget.GlobalPosition);
    }

    public override void _Process(double delta)
    {
        var collision = WarriorStateMachine.Warrior.GetLastSlideCollision();

        if (collision != null
            && collision.GetCollider() is UnitBase)
        {
            WarriorStateMachine.Warrior.HitWithChargeTarget();
            WarriorStateMachine.ChangeState(WarriorStateIds.MoveToEnemy);
        }
    }
}
