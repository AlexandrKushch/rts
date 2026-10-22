using System;
using System.Collections.Generic;
using Godot;

public partial class UnitBase : CharacterBody2D, IDestroyableWithHp, IHasTeam
{
	private PackedScene _deadScene;

	public virtual float MovementSpeed { get; protected set; } = 100f;
	public bool Dashed { get; set; } = false;

	private NavigationAgent2D _navigation;
	public NavigationAgent2D NavigationAgent2D
	{
		get
		{
			if (!IsInstanceValid(_navigation) && IsInstanceValid(this))
			{
				_navigation = GetNode<NavigationAgent2D>(nameof(NavigationAgent2D));
			}

			return _navigation;
		}
	}

	public SelectableComponent SelectableComponent { get; private set; }

	public Vector2? Target { get; set; }

	public Node2D TargetObject { get; set; }

	public int HP { get; set; }

	[Export]
	public UnitType Meta { get; set; }

	[Export]
	public TeamType Team { get; set; }
	
	[Export]
	public bool Predeployed { get; set; } = false;
	
    protected static Dictionary<string, uint> ObstacleCollisionMasks = new Dictionary<string, uint>
    {
        [nameof(UnitBase)] = 2,
        [nameof(BuildingBase)] = 16,
        [nameof(TreeBase)] = 32,
        [nameof(GoldBase)] = 64,
        [nameof(MeatResourceBase)] = 128
    };

	public override void _Ready()
	{
		if (Meta != null && Predeployed)
		{
			var unit = GlobalPlayers.Instance.Players[Team].GlobalResources.UnitScenes[Meta.Id].Instantiate<UnitBase>();
			unit.GlobalPosition = GlobalPosition;
			unit.Target = Target ?? GlobalPosition;
			unit.TargetObject = TargetObject;
			GetParent().CallDeferred("add_child", unit);
			SetPhysicsProcess(false);
			QueueFree();
			return;
		}

		if (Meta != null)
		{
			HP = Meta.MaxHP;
		}

		if (HasNode(nameof(SelectableComponent)))
		{
			SelectableComponent = GetNode<SelectableComponent>(nameof(SelectableComponent));
		}
		
		_deadScene = ResourceLoader.Load<PackedScene>("uid://d0k60hnpsewgk");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Dashed)
		{
			float weight = 1f - Mathf.Exp(-15 * (float)delta);
			Velocity = Velocity.Lerp(Vector2.Zero, weight);
			if (Velocity.IsEqualApprox(Vector2.Zero, 0.15f))
			{
				Dashed = false;
			}
		}
		else
		{
			Vector2 currentAgentPosition = GlobalPosition;
			Vector2 nextPathPosition = NavigationAgent2D.GetNextPathPosition();
			NavigationAgent2D.SetVelocity(currentAgentPosition.DirectionTo(nextPathPosition) * MovementSpeed);
		}
		MoveAndSlide();
	}

	public void OnVelocityComputed(Vector2 safeVelocity)
	{
		if (!Dashed)
		{
			Velocity = safeVelocity;
		}
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

	public virtual void TakeDamage(int value)
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

	public virtual void TakeDamageWithDash(Vector2 from, int value, float power)
	{
		TakeDamage(value);

		var direction = from.DirectionTo(GlobalPosition);
		Velocity += direction * 500 * power;
		Dashed = true;
	}

    public virtual void Heal(int value)
	{
		HP = Math.Min(HP + value, Meta.MaxHP);
	}

	public virtual void Destroy()
	{
		var dead = _deadScene.Instantiate<Dead>();
		dead.GlobalPosition = GlobalPosition;
		GetParent().AddChild(dead);
		GlobalPlayers.Instance.Players[Team].ResourceController.Collect(ResourceTypeIds.UnitsCapacity, Meta.Cost[ResourceTypeIds.UnitsCapacity]);
		QueueFree();
	}
	
    protected bool TryGetClosestTarget(Vector2 to, Node2D target, uint collissionMask, out Node2D collider, out Vector2? collisionPoint)
    {
        collider = null;
        collisionPoint = null;

        var spaceState = GetWorld2D().DirectSpaceState;
        var query = new PhysicsRayQueryParameters2D
        {
            From = GlobalPosition,
            To = to,
            CollideWithAreas = false,
            CollideWithBodies = true,
            CollisionMask = collissionMask
        };

        var excluded = new Godot.Collections.Array<Rid>();

        while (true)
        {
            var result = spaceState.IntersectRay(query);

            if (result.Count == 0)
                break;

            collider = result.ContainsKey("collider") ? result["collider"].As<Node2D>() : null;
            collisionPoint = result.ContainsKey("position") ? result["position"].As<Vector2>() : null;

            if (target != null
				&& collider is BuildingBase building
                && building.GetInstanceId() != target.GetInstanceId())
            {
                excluded.Add(building.GetRid());
                query.Exclude = excluded;
                continue;
            }
            else if (collider is UnitBase unit
                && !unit.GlobalPosition.IsEqualApprox(to, 1f))
            {
                excluded.Add(unit.GetRid());
                query.Exclude = excluded;
                continue;                
            }

            if (collider != null && collisionPoint != null)
            {
                return true;
            }

            break;
        }

        return false;
    }
}
