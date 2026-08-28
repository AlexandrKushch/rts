using Godot;

public partial class Pawn : UnitBase
{

    private PawnGatheringStateManager GatheringStateMachine;

    public PawnVisual Visual { get; private set; }
    public ResourceType Resource { get; private set; }

    public int ResourceCollectedCount { get; set; }

    public override void _Ready()
    {
        base._Ready();
        Visual = GetNode<PawnVisual>(nameof(Visual));
        GatheringStateMachine = GetNode<PawnGatheringStateManager>(nameof(GatheringStateMachine));
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        Visual.UpdateMovement(Velocity, ResourceCollectedCount, Resource);
    }

    public override void UpdatePath()
    {
        base.UpdatePath();
    }

    public void UpdateResource()
    {
        if (TargetObject is ResourceBase resource)
        {
            Resource = resource.ResourceType;
        }
        // else
        // {
        //     Resource = null;
        // }

        if (Resource != null)
        {
            GatheringStateMachine.ChangeState(PawnGatheringStateIds.MoveToResource);
            GatheringStateMachine.SetProcess(true);
        }
    }
}
