using Godot;

public partial class UnitHasVisualBase : UnitBase
{
    protected Area2D UnitOverlapedDetector;
    protected SelectableComponent UnitSelectableComponent;    

    public UnitVisualBase Visual { get; private set; }

    public override void _Ready()
    {
        base._Ready();
        UnitOverlapedDetector = GetNode<Area2D>(nameof(UnitOverlapedDetector));
        UnitSelectableComponent = GetNode<SelectableComponent>(nameof(UnitSelectableComponent));
        Visual = GetNode<UnitVisualBase>(nameof(Visual));
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        Visual.UpdateMovement(Velocity, string.Empty);

        Visual.UpdateOutlineVisible(UnitOverlapedDetector.GetOverlappingAreas().Count > 0);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
    }
}
