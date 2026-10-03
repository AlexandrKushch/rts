public partial class MonkHealState : MonkStateBase
{
    private const double MaxAccumulatedPower = 2.0f;
    private double _accumulatedPower = 0.0;

    public override void Activate()
    {
        base.Activate();
        SetProcess(false);

        Monk.Target = null;
        Monk.MonkVisual.UpdateHealing(true);
    }

    public override void Deactivate()
    {
        base.Deactivate();
        Monk.MonkVisual.Stop();
    }

    public override void _Process(double delta)
    {
        if (!IsInstanceValid(Monk.HealTarget))
        {
            ChangeState(MonkStateIds.Idle);
            return;
        }

        if (Monk.HealTarget.HP == Monk.HealTarget.Meta.MaxHP)
        {
            ChangeState(MonkStateIds.Idle);
            return;
        }

        _accumulatedPower += delta;

        if (_accumulatedPower > MaxAccumulatedPower)
        {
            SetProcess(false);
            _accumulatedPower = 0;
            Monk.MonkVisual.UpdateHealing(false);
        }
    }

    private void OnHealStartAnimationKeyReached()
    {
        SetProcess(true);        
    }

    private void OnHealEndAnimationKeyReached()
    {
        Monk.HealTheTarget();
        ChangeState(MonkStateIds.Idle);
    }
}
