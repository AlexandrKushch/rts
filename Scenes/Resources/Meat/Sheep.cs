using Godot;

public partial class Sheep : UnitHasVisualBase
{
    public SheepVisual SheepVisual { get; private set; }
	public Vector2 GrazePoint { get; private set; }

    protected override float MovementSpeed => 25;
    
	public override void _Ready()
	{
		base._Ready();
		GrazePoint = GlobalPosition;
		SheepVisual = Visual as SheepVisual;
	}
    
	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
	}
}
