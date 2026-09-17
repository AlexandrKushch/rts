using Godot;
using System;

public partial class Warrior : UnitBase
{
    public WarriorVisual Visual { get; private set; }

    public override void _Ready()
    {
        base._Ready();
        Visual = GetNode<WarriorVisual>(nameof(Visual));

        Visual.SetupColor(Team, Meta);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        Visual.UpdateMovement(Velocity, string.Empty);
    }
}
