using Godot;
using System.Text.RegularExpressions;

public partial class Archer : UnitBase
{
    public UnitVisualBase Visual { get; private set; }

    public override void _Ready()
    {
        base._Ready();
        Visual = GetNode<UnitVisualBase>(nameof(Visual));
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        Visual.UpdateMovement(Velocity, string.Empty);
    }
}
