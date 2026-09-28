using System;
using System.Collections.Generic;
using Godot;

public partial class EnemyDetector : Area2D
{
    private TeamType _team;

    public HashSet<UnitBase> Enemies { get; private set; }

    public override void _Ready()
    {
        var parent = GetParent<UnitBase>();

        if (parent == null)
        {
            throw new Exception($"{nameof(EnemyDetector)} should be attached to {nameof(UnitBase)}");
        }

        _team = parent.Team;

        Enemies = new HashSet<UnitBase>();

        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is UnitBase unit
            && unit.Team != _team)
        {
            Enemies.Add(unit);
        }
    }
    
    private void OnBodyExited(Node2D body)
    {
        if (body is UnitBase unit
            && unit.Team != _team)
        {
            Enemies.Remove(unit);
        }
    }
}
