using Godot;

public partial class UnitBase : CharacterBody2D
{
	private const float _movementSpeed = 100f;

    private NavigationAgent2D NavigationAgent2D;
	private Node2D Selection;

	public Vector2? Target { get; set; }

    public override void _Ready()
    {
        NavigationAgent2D = GetNode<NavigationAgent2D>(nameof(NavigationAgent2D));
		Selection = GetNode<Node2D>(nameof(Selection));

		UpdateSelection(false);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (NavigationAgent2D.IsNavigationFinished())
		{
			return;
		}
        
		Vector2 currentAgentPosition = GlobalPosition;
		Vector2 nextPathPosition = NavigationAgent2D.GetNextPathPosition();
		NavigationAgent2D.SetVelocity(currentAgentPosition.DirectionTo(nextPathPosition) * _movementSpeed);
		
		MoveAndSlide();
    }

	public void OnVelocityComputed(Vector2 safeVelocity)
	{
		Velocity = safeVelocity;
	}

	public void UpdatePath()
	{
		if (Target == null) return;
		NavigationAgent2D.TargetPosition = Target.Value;
	}

	public void UpdateSelection(bool value)
	{
		Selection.Visible = value;
	}
}
