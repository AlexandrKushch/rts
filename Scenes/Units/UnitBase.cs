using Godot;

public partial class UnitBase : CharacterBody2D, IDestroyableWithHp
{
	protected virtual float MovementSpeed => 100f;

	public NavigationAgent2D NavigationAgent2D { get; private set; }

	public Vector2? Target { get; set; }

	public Node2D TargetObject { get; set; }

	[Export]
	public UnitType Meta { get; set; }

	[Export]
	public TeamType Team { get; set; }

	public bool Setup { get; set; } = false;
	public int HP { get; set; } = 5;

	public override void _Ready()
	{
		if (Meta != null && !Setup)
		{
			var unit = GlobalPlayers.Instance.Players[Team].GlobalResources.UnitScenes[Meta.Id].Instantiate<UnitBase>();
			unit.GlobalPosition = GlobalPosition;
			unit.Setup = true;
			GetParent().CallDeferred("add_child", unit);
			SetPhysicsProcess(false);
			QueueFree();
			return;
		}

		NavigationAgent2D = GetNode<NavigationAgent2D>(nameof(NavigationAgent2D));
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 currentAgentPosition = GlobalPosition;
		Vector2 nextPathPosition = NavigationAgent2D.GetNextPathPosition();
		NavigationAgent2D.SetVelocity(currentAgentPosition.DirectionTo(nextPathPosition) * MovementSpeed);

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

	public void TakeDamage(int value)
	{
		if (!IsInstanceValid(this))
		{
			return;
		}

		HP -= value;

		if (HP <= 0)
		{
			Destroy();
		}
	}

	public void Destroy()
	{
		QueueFree();
	}
}
