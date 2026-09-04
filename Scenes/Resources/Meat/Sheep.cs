using Godot;

public partial class Sheep : UnitBase
{
	public Vector2 GrazePoint { get; private set; }
	public SheepVisual Visual { get; set; }

    protected override float MovementSpeed => 25;
    
	public override void _Ready()
	{
		base._Ready();
		Visual = GetNode<SheepVisual>(nameof(Visual));
		GrazePoint = GlobalPosition;
	}
    
	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		Visual.UpdateMovement(Velocity);
	}
}
