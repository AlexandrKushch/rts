using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Pawn : UnitHasVisualBase
{
    public const int MaxCollectableCapacity = 15;
    public const int GatherDistanceRange = 75;

    private UpdateMovementAnimation _updateMovementAnimation;
    private PawnStateManagerBase StateMachine;

    public PawnVisual PawnVisual { get; private set; }
    public ResourceBase TargetResource { get; private set; }
    public BuildingBase TargetBuilding { get; private set; }

    public PawnResourceToCollectData ResourceToCollectData { get; private set; }

    private delegate void UpdateMovementAnimation(Vector2 velocity, int collected);

    public override void _Ready()
    {
        base._Ready();
        
		if (Meta != null && !Setup)
		{
            return;
        }
        
        StateMachine = GetNode<PawnStateManagerBase>(nameof(StateMachine));
		SetTarget(Target, TargetObject);

        PawnVisual = Visual as PawnVisual;
    }

    public override void _Process(double delta)
    {
        if (!Dashed)
        {
            _updateMovementAnimation(Velocity, ResourceToCollectData?.CollectedCount ?? 0);
        }

        Visual.UpdateOutlineVisible(UnitOverlapedDetector.GetOverlappingAreas().Count(x => x.GetInstanceId() != UnitSelectableComponent.GetInstanceId()) > 0);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (StateMachine.GetCurrentStateType() == PawnStateIds.MoveTo)
        {
            Node2D target;
            Vector2? point;

            if (IsInstanceValid(TargetResource)
                && TryGetClosestTarget(TargetResource.GlobalPosition, TargetResource, ObstacleCollisionMasks[TargetResource.GetType().ToString()], out target, out point)
                && target is ResourceBase resource)
            {
                SetTarget(point, resource);
            }
            else if (IsInstanceValid(TargetBuilding)
                && TryGetClosestTarget(TargetBuilding.GlobalPosition, TargetBuilding, ObstacleCollisionMasks[nameof(BuildingBase)], out target, out point)
                && target is BuildingBase building)
            {
                SetTarget(point, building);
            }
            else if (Target != null
                && TryGetClosestTarget(Target.Value, null, ObstacleCollisionMasks[nameof(UnitBase)], out target, out point))
            {
                point += point.Value.DirectionTo(GlobalPosition) * 25;
                SetTarget(point, null);
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
        return QueryResource()
            ?.MinBy(x => ResourceToCollectData.Position.DistanceTo(x.GlobalPosition));
    }

    public ResourceBase GetNextResourceWithSpace()
    {
        return QueryResource()
            ?.Where(x => x.GetInstanceId() != TargetResource.GetInstanceId() && x.CurrentCollectingCount < 2)
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

        // if (!NavigationAgent2D.IsTargetReachable())
        // {
        //     if (TargetBuilding == null && TargetResource == null)
        //     {
        //         GD.Print("NOT REACHABLE");
        //         NavigationAgent2D.TargetPosition = GlobalPosition;
        //         SetTarget(null, null);
        //     }
        // }
    }

    public void SetTargetResourceWithoutChangingState(ResourceBase resource)
    {
        if (StateMachine.GetCurrentStateType() == PawnStateIds.GatheringResource)
        {
            TargetObject = resource;
            TargetResource = resource;
        }
    }

    public void UpdateTargetObject()
    {
        if (IsInstanceValid(TargetObject))
        {
            StateMachine.ChangeState(PawnStateIds.MoveTo);
            StateMachine.SetProcess(true);

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
                else
                {
                    ResourceToCollectData.Position = resource.GlobalPosition;
                }

                _updateMovementAnimation = (v, c) => { PawnVisual.UpdateMovement(v, c, resource.ResourceType); };
            }
            else if (TargetObject is BuildingBase building)
            {
                TargetBuilding = building;
                TargetResource = null;

                if (!building.BuiltAndNotDamaged)
                {
                    _updateMovementAnimation = (v, _) => { PawnVisual.UpdateMovement(v, "build"); };
                }
            }
        }
        else
        {
            StateMachine.ChangeState(PawnStateIds.MoveTo);
            TargetBuilding = null;

            if (ResourceToCollectData == null
                || ResourceToCollectData.CollectedCount == 0)
            {
                TargetResource = null;
                ResourceToCollectData = null;
                _updateMovementAnimation = (v, _) => { PawnVisual.UpdateMovement(v, string.Empty); };
            }
            else
            {
                _updateMovementAnimation = (v, c) => { PawnVisual.UpdateMovement(v, c, ResourceToCollectData.ResourceType); };
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

        GlobalPlayers.Instance.Players[Team].ResourceController.Collect(ResourceToCollectData.ResourceType.Type, ResourceToCollectData.CollectedCount);
        ResourceToCollectData.CollectedCount = 0;
    }

    private IEnumerable<ResourceBase> QueryResource()
    {
        if (ResourceToCollectData == null) return null;
        float distanceLimit = 300;

        return GetParent()
            .FindChildren("*")
            .Where(x => x is ResourceBase && x != null)
            .Select(x => x as ResourceBase)
            .Where(x => !x.Destroyed
                && x.ResourceType.Name.Equals(ResourceToCollectData.ResourceType.Name, StringComparison.OrdinalIgnoreCase)
                && ResourceToCollectData.Position.DistanceTo(x.GlobalPosition) <= distanceLimit);

    }
}
