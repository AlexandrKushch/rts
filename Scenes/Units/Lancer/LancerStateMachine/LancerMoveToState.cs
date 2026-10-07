using Godot;

public partial class LancerMoveToState : LancerStateBase
{
    public override void Activate()
    {
        base.Activate();

        Lancer.ToIdleStateTimer.Timeout += ToIdleState;
    }

    public override void Deactivate()
    {
        base.Deactivate();

        Lancer.ToIdleStateTimer.Timeout -= ToIdleState;
    }

    public override void _Process(double delta)
    {
        if (!Lancer.Dashed)
        {
            Lancer.LancerVisual.UpdateMovement(Lancer.Velocity, null);
        }
        
        if (Lancer.NavigationAgent2D.IsNavigationFinished()
            && Lancer.ToIdleStateTimer.TimeLeft == 0)
        {
            Lancer.ToIdleStateTimer.Start();
        }
        else if (!Lancer.NavigationAgent2D.IsNavigationFinished()
            && Lancer.ToIdleStateTimer.TimeLeft > 0)
        {
            Lancer.ToIdleStateTimer.Stop();
        }
    }

    private void ToIdleState()
    {
        ChangeState(LancerStateIds.Idle);        
    }
}
