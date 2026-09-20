

public partial class Warrior : UnitBase
{
    public WarriorVisual Visual { get; private set; }

    public override void _Ready()
    {
        base._Ready();
        Visual = GetNode<WarriorVisual>(nameof(Visual));
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        Visual.UpdateMovement(Velocity, string.Empty);
    }
}
