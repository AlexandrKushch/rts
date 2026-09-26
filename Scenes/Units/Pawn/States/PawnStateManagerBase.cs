using Godot;

public partial class PawnStateManagerBase : StateMachineBase<PawnStateIds>
{
    [Export] public Pawn Pawn { get; private set; }
    public UnitsController UnitsController { get; private set; }

    public override void _Ready()
    {
        base._Ready();

        UnitsController = GlobalPlayers.Instance.Players[Pawn.Team].UnitsController;
    }

    public override void ChangeState(PawnStateIds state)
    {
        if (CurrentState != null)
        {
            CurrentState.Deactivate();
            CurrentState = null;
            CurrentStateType = null;
        }

        if (state != PawnStateIds.None)
        {
            CurrentStateType = state;
            CurrentState = States[state];
            CurrentState.Activate();
        }

        if (state == PawnStateIds.None)
        {
            SetProcess(false);
        }
    }

    public void MoveToClosestBuilding()
    {
        var building = Pawn.GetClosestResourceStorageBuilding();
        
        if (IsInstanceValid(building))
        {
            Pawn.SetTarget(
                building.GlobalPosition,
                building);
        }
    }

    public void MoveToClosestResourceIfNotToBuilding()
    {
        var resource = Pawn.GetNextResource();

        if (resource != null)
        {
            Pawn.SetTarget(
                resource.GlobalPosition,
                resource);
        }
        else
        {
            MoveToClosestBuilding();
        }
    }
}
