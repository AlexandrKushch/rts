using System;
using Godot;

public partial class EnemyDetector : AreaDetector<Node2D>
{
    private TeamType _team;

    public override void _Ready()
    {
        base._Ready();
        
        var parent = GetParent<IHasTeam>();

        if (parent == null)
        {
            throw new Exception($"{Name} should be attached to {nameof(IHasTeam)}");
        }

        _team = parent.Team;
    }
    
    public override void OnBodyEntered(Node2D body)
    {
        if ((body is UnitBase unit && unit.Team != _team)
            || (body is BuildingBase building && building.Team != _team))
        {
            Items.Add(body);
        }
    }
    
    public override void OnBodyExited(Node2D body)
    {
        if ((body is UnitBase unit && unit.Team != _team)
            || (body is BuildingBase building && building.Team != _team))
        {
            Items.Remove(body);
        }
    }
}