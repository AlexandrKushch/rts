using Godot;
using System;

public partial class AllyDetector : AreaDetector<UnitBase>
{
    private UnitBase _attachedToUnit;
    private TeamType _team;

    public override void _Ready()
    {
        base._Ready();
        
        _attachedToUnit = GetParent<UnitBase>();

        if (_attachedToUnit == null)
        {
            throw new Exception($"{Name} should be attached to {nameof(UnitBase)}");
        }

        _team = _attachedToUnit.Team;
    }
    
    public override void OnBodyEntered(Node2D body)
    {
        if (body is UnitBase unit
            && unit.Team == _team
            && unit.GetInstanceId() != _attachedToUnit.GetInstanceId())
        {
            Items.Add(unit);
        }
    }
    
    public override void OnBodyExited(Node2D body)
    {
        if (body is UnitBase unit
            && unit.Team == _team
            && unit.GetInstanceId() != _attachedToUnit.GetInstanceId())
        {
            Items.Remove(unit);
        }
    }
}
