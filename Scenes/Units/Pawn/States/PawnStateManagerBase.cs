using Godot;

public partial class PawnStateManagerBase : StateMachineBase<PawnStateIds>
{
    [Export] public Pawn Pawn { get; private set; }
    
    public override void ChangeState(PawnStateIds state)
    {
        if (CurrentState != null)
        {
            CurrentState.Deactivate();
            CurrentState = null;
        }

        if (state != PawnStateIds.None)
        {
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
        UnitsController.Instance.MoveToNodeCommand(Pawn, building);
    }

    public void MoveToClosestResourceIfNotToBuilding()
    {
        var resource = Pawn.GetNextResource();

        if (resource != null)
        {
            UnitsController.Instance.MoveToNodeCommand(Pawn, resource);
        }
        else
        {
            var building = Pawn.GetClosestResourceStorageBuilding();
            UnitsController.Instance.MoveToNodeCommand(Pawn, building);
        }
    }
}
