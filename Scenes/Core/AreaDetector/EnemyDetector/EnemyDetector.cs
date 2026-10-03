using System;
using Godot;

public partial class EnemyDetector : AreaDetector<UnitBase>
{
    private TeamType _team;

    public override void _Ready()
    {
        base._Ready();
        
        var parent = GetParent<UnitBase>();

        if (parent == null)
        {
            throw new Exception($"{Name} should be attached to {nameof(UnitBase)}");
        }

        _team = parent.Team;
    }
    
    public override void OnBodyEntered(Node2D body)
    {
        if (body is UnitBase unit
            && unit.Team != _team)
        {
            Enemies.Add(unit);
        }
    }
    
    public override void OnBodyExited(Node2D body)
    {
        if (body is UnitBase unit
            && unit.Team != _team)
        {
            Enemies.Remove(unit);
        }
    }
}