using System.Linq;
using Godot;

public partial class MonkIdleState : MonkStateBase
{
    public override void _Process(double delta)
    {
        var toHealUnit = Monk.AllyDetector.Items
            .Where(x => x.HP < x.Meta.MaxHP)
            .MinBy(x => x.GlobalPosition.DirectionTo(Monk.GlobalPosition));

        if (toHealUnit != null)
        {
            Monk.SetHealTarget(toHealUnit);
        }
    }
}
