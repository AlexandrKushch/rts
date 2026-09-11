using System;
using System.Linq;
using Godot;

public partial class Pawn : UnitBase
{
    public const int MaxCollectableCapacity = 15;

    private UpdateMovementAnimation _updateMovementAnimation;
    private PawnStateManagerBase StateMachine;

    public PawnVisual Visual { get; private set; }
    public ResourceBase TargetResource { get; private set; }
    public BuildingBase TargetBuilding { get; private set; }

    public PawnResourceToCollectData ResourceToCollectData { get; private set; }

    private delegate void UpdateMovementAnimation(Vector2 velocity, int collected);

    public override void _Ready()
    {
        base._Ready();
        Visual = GetNode<PawnVisual>(nameof(Visual));
        StateMachine = GetNode<PawnStateManagerBase>(nameof(StateMachine));
        _updateMovementAnimation = (v, _) => { Visual.UpdateMovement(v, string.Empty); };
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        _updateMovementAnimation(Velocity, ResourceToCollectData?.CollectedCount ?? 0);

        if (StateMachine.GetCurrentStateType() == PawnStateIds.MoveTo)
        {
            Node2D target;
            Vector2? point;

            if (IsInstanceValid(TargetResource)
                && TryGetClosestTarget(TargetResource.GlobalPosition, out target, out point)
                && target is ResourceBase resource)
            {
                SetTarget(point, resource);
            }
            else if (IsInstanceValid(TargetBuilding)
                && TryGetClosestTarget(TargetBuilding.GlobalPosition, out target, out point)
                && target is BuildingBase building)
            {
                SetTarget(point, building);
            }
        }
    }

    public BuildingBase GetClosestResourceStorageBuilding()
    {
        return GetParent()
            .GetChildren()
            .Where(x => x is BuildingBase && x != null)
            .Select(x => x as BuildingBase)
            .MinBy(x => GlobalPosition.DistanceTo(x.GlobalPosition));
    }

    public ResourceBase GetNextResource()
    {
        if (ResourceToCollectData == null) return null;

        return GetParent()
            .FindChildren("*")
            .Where(x => x is ResourceBase && x != null)
            .Select(x => x as ResourceBase)
            .Where(x => x.ResourceType.Name.Equals(ResourceToCollectData.ResourceType.Name, StringComparison.OrdinalIgnoreCase))
            .MinBy(x => ResourceToCollectData.Position.DistanceTo(x.GlobalPosition));
    }

    public override void UpdatePath()
    {
        base.UpdatePath();
    }

    public override void SetTarget(Vector2? targetPosition, Node2D targetObject)
    {
        base.SetTarget(targetPosition, targetObject);

        UpdateTargetObject();

        if (!NavigationAgent2D.IsTargetReachable())
        {
            if (TargetBuilding == null && TargetResource == null)
            {
                GD.Print("NOT REACHABLE");
                NavigationAgent2D.TargetPosition = GlobalPosition;
                SetTarget(null, null);
            }
        }
    }

    public void UpdateTargetObject()
    {
        if (IsInstanceValid(TargetObject))
        {
            if (TargetObject is ResourceBase resource)
            {
                TargetResource = resource;
                TargetBuilding = null;

                if (ResourceToCollectData == null
                    || ResourceToCollectData.ResourceType.Name != resource.ResourceType.Name)
                {
                    ResourceToCollectData = new PawnResourceToCollectData
                    {
                        Position = resource.GlobalPosition,
                        ResourceType = resource.ResourceType,
                        CollectedCount = 0
                    };
                }

                _updateMovementAnimation = (v, c) => { Visual.UpdateMovement(v, c, resource.ResourceType); };
            }
            else if (TargetObject is BuildingBase building)
            {
                TargetBuilding = building;
                TargetResource = null;

                if (!building.Built)
                {
                    _updateMovementAnimation = (v, _) => { Visual.UpdateMovement(v, "build"); };
                }
            }

            StateMachine.ChangeState(PawnStateIds.MoveTo);
            StateMachine.SetProcess(true);
        }
        else
        {
            TargetBuilding = null;
            StateMachine.ChangeState(PawnStateIds.None);

            if (ResourceToCollectData == null
                || ResourceToCollectData.CollectedCount == 0)
            {
                TargetResource = null;
                ResourceToCollectData = null;
                _updateMovementAnimation = (v, _) => { Visual.UpdateMovement(v, string.Empty); };
            }
            else
            {
                _updateMovementAnimation = (v, c) => { Visual.UpdateMovement(v, c, ResourceToCollectData.ResourceType); };
            }
        }
    }

    public void DropResources()
    {
        var label = new Label()
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            Text = $"+{ResourceToCollectData.CollectedCount}",
            PivotOffsetRatio = Vector2.One / 2,
        };
        label.AddThemeColorOverride("font_color", new Color("63b7ba"));
        label.AddThemeConstantOverride("outline_size", 4);
        label.Position += Vector2.Up * 50;
        AddChild(label);

        var tween = CreateTween().SetParallel();
        float tweenDuration = 1f;
        tween.TweenProperty(label, "position", new Vector2((float)(RandomExtension.RandomDouble() - 0.5f) * 10, label.Position.Y + label.Position.Y / 2), tweenDuration / 2);
        tween.TweenProperty(label, "rotation", RandomExtension.RandomDouble() - 0.5f, tweenDuration / 2);
        tween.Finished += label.QueueFree;

        ResourceController.Instance.Collect(ResourceToCollectData.ResourceType.Type, ResourceToCollectData.CollectedCount);
        ResourceToCollectData.CollectedCount = 0;
    }

    private bool TryGetClosestTarget(Vector2 to, out Node2D collider, out Vector2? collisionPoint)
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
            CollisionMask = 1
        };

        var result = spaceState.IntersectRay(query);

        if (result != null)
        {
            collider = result.ContainsKey("collider") ? result["collider"].As<Node2D>() : null;
            collisionPoint = result.ContainsKey("position") ? result["position"].As<Vector2>() : null;
            
            if (collider != null && collisionPoint != null)
            {
                return true;
            }
        }

        return false;
    }
}
