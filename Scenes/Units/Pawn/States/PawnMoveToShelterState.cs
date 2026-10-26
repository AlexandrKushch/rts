using Godot;
using System;

public partial class PawnMoveToShelterState : PawnStateBase
{
    private const float BuildingDesiredDistance = 80;

    public override void Activate()
    {
        base.Activate();
        Pawn.NavigationAgent2D.TargetDesiredDistance = BuildingDesiredDistance;
    }

    public override void _Process(double delta)
    {
        if (PawnStateMachine.Pawn.NavigationAgent2D.IsNavigationFinished()
            && IsInstanceValid(Pawn.TargetShelter))
        {
            Pawn.TargetShelter.ConsumeUnit(Pawn);
            PawnStateMachine.ChangeState(PawnStateIds.None);
        }

        if (!IsInstanceValid(Pawn.TargetShelter))
        {
            Pawn.MoveToShelter();
        }
    }
}
