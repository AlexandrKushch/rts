using Godot;

public partial class Pawn : UnitBase
{
    private PawnVisual Visual;

    public override void _Ready()
    {
        base._Ready();
        Visual = GetNode<PawnVisual>(nameof(Visual));
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        Visual.UpdateMovement(Velocity);
    }

    public override void UpdatePath()
    {
        base.UpdatePath();
    }
}
