using Godot;
using System;

public partial class SheepMoveState : SheepStateBase
{
    public override void Activate()
    {
        base.Activate();
        Vector2 moveTo = RandomExtension.GetRandomPointInCircle(100) + SheepStateMachine.Sheep.GrazePoint;
        SheepStateMachine.Sheep.SetTarget(moveTo, null);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (SheepStateMachine.Sheep.NavigationAgent2D.IsNavigationFinished())
        {
            SheepStateMachine.ChangeState(SheepStateIds.Eat);
        }
    }
}
