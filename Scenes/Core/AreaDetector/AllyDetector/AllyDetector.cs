using Godot;
using System;
using System.Linq;

public partial class AllyDetector : AreaDetector<UnitBase>
{
    private Node2D _attachedToUnit;
    private TeamType _team;

    public override void _Ready()
    {
        base._Ready();

        _attachedToUnit = GetParent<Node2D>();

        if (_attachedToUnit == null || _attachedToUnit is not IHasTeam team)
        {
            throw new Exception($"{Name} should be attached to {nameof(Node2D)} wit team");
        }

        _team = team.Team;
    }

    public override void _Process(double delta)
    {
        Items = Items
            .Where(unit => unit.Team == _team && unit.GetInstanceId() != _attachedToUnit.GetInstanceId())
            .ToHashSet();
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
