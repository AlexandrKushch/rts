using Godot;

public partial class UnitBase : CharacterBody2D
{
	private const float _movementSpeed = 100f;

    protected NavigationAgent2D NavigationAgent2D;

	public Vector2? Target { get; set; }

	public Node2D TargetObject { get; set; }

    public override void _Ready()
    {
        NavigationAgent2D = GetNode<NavigationAgent2D>(nameof(NavigationAgent2D));
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

	public virtual void UpdatePath()
	{
		if (Target == null) return;
		NavigationAgent2D.TargetPosition = Target.Value;
	}
}
