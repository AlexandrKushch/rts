using System;
using System.Linq;
using Godot;

public partial class Pawn : UnitBase
{
    private UpdateMovementAnimation _updateMovementAnimation;
    private PawnStateManagerBase StateMachine;

    public PawnVisual Visual { get; private set; }
    public ResourceBase TargetResource { get; private set; }
    public BuildingBase TargetBuilding { get; private set; }

    public int ResourceCollectedCount { get; set; }

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

        _updateMovementAnimation(Velocity, ResourceCollectedCount);
    }

    public BuildingBase GetClosestResourceStorageBuilding()
    {
        return GetParent()
            .GetChildren()
            .Where(x => x is BuildingBase && x != null)
            .Select(x => x as BuildingBase)
            .MinBy(x => GlobalPosition.DistanceTo(x.GlobalPosition));
    }

    public override void UpdatePath()
    {        
        base.UpdatePath();
    }

    public void UpdateTargetObject()
    {
        if (IsInstanceValid(TargetObject))
        {
            if (TargetObject is ResourceBase resource)
            {
                TargetResource = resource;
                _updateMovementAnimation = (v, c) => { Visual.UpdateMovement(v, c, resource.ResourceType); };
            }
            else if (TargetObject is BuildingBase building)
            {
                TargetBuilding = building;

                if (!building.Build)
                {
                    _updateMovementAnimation = (v, _) => { Visual.UpdateMovement(v, "build"); };
                }
            }
            
            StateMachine.ChangeState(PawnGatheringStateIds.MoveTo);
            StateMachine.SetProcess(true);
        }
        else
        {
            TargetBuilding = null;
            StateMachine.ChangeState(PawnGatheringStateIds.None);

            if (ResourceCollectedCount == 0)
            {
                TargetResource = null;
                _updateMovementAnimation = (v, _) => { Visual.UpdateMovement(v, string.Empty); };
            }
            else
            {
                _updateMovementAnimation = (v, c) => { Visual.UpdateMovement(v, c, TargetResource.ResourceType); };                
            }
        }
    }

    public void DropResources()
    {
        GD.Print("DROP RESOURCES");
        ResourceCollectedCount = 0;
    }
}
