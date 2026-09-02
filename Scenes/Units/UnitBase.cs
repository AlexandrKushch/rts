using Godot;

public partial class UnitBase : CharacterBody2D
{
	private const float _movementSpeed = 100f;

	public NavigationAgent2D NavigationAgent2D { get; private set; }

	public Vector2? Target { get; set; }

	public Node2D TargetObject { get; set; }

	[Export]
	public UnitResource Meta { get; private set; }

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

	public virtual void SetTarget(Vector2? targetPosition, Node2D targetObject)
	{
		Target = targetPosition;
		TargetObject = targetObject;
		UpdatePath();
	}
}
