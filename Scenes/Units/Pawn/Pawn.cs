using Godot;

public partial class Pawn : UnitBase
{
    private UpdateMovementAnimation _updateMovementAnimation;
    private PawnStateManagerBase StateMachine;

    public PawnVisual Visual { get; private set; }
    public ResourceBase Resource { get; private set; }
    public BuildingBase Building { get; private set; }

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
                Resource = resource;
                _updateMovementAnimation = (v, c) => { Visual.UpdateMovement(v, c, resource.ResourceType); };
            }
            else if (TargetObject is BuildingBase building)
            {
                Building = building;
                _updateMovementAnimation = (v, _) => { Visual.UpdateMovement(v, "build"); };
            }
            
            StateMachine.ChangeState(PawnGatheringStateIds.MoveTo);
            StateMachine.SetProcess(true);
        }
        else
        {
            Building = null;
            StateMachine.ChangeState(PawnGatheringStateIds.None);
            StateMachine.SetProcess(false);

            if (ResourceCollectedCount == 0)
            {
                Resource = null;
                _updateMovementAnimation = (v, _) => { Visual.UpdateMovement(v, string.Empty); };
            }
            else
            {
                _updateMovementAnimation = (v, c) => { Visual.UpdateMovement(v, c, Resource.ResourceType); };                
            }
        }
    }
}
