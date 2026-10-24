using Godot;
using System;

public partial class PawnMoveToShelterState : PawnStateBase
{
    public override void _Process(double delta)
    {
        if (PawnStateMachine.Pawn.NavigationAgent2D.IsNavigationFinished()
            && PawnStateMachine.Pawn.TargetObject is BuildingBase building
            && building is ShelterBuildingBase shelter
            && (shelter.BuiltAndNotDamaged || shelter.EnemyDetector.Items.Count > 0))
        {
            shelter.ConsumeUnit(Pawn);
            PawnStateMachine.ChangeState(PawnStateIds.None);
        }
        else
        {
            PawnStateMachine.ChangeState(PawnStateIds.MoveTo);            
        }
    }
}
