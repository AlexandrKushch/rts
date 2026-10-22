using Godot;
using System.Linq;

public partial class UnitsController : Node2D
{
    private HumanPlayer HumanPlayer;

    public override void _Ready()
    {
        var player = GetParent<PlayerBase>();

        if (player is HumanPlayer humanPlayer)
        {
            HumanPlayer = humanPlayer;
        }

        if (HumanPlayer == null)
        {
            SetProcessUnhandledInput(false);
        }
    }

    public override void _UnhandledInput(InputEvent input)
    {
        if (HumanPlayer.BuildingController.BlueprintActive) return;

        if (input is InputEventMouseButton buttonInput)
        {
            if (buttonInput.ButtonIndex == MouseButton.Right)
            {
                InputMoveCommand(buttonInput);
            }
        }
    }

    private void InputMoveCommand(InputEventMouseButton input)
    {
        if (input.IsReleased())
        {
            TryPointCastSelectable(out SelectableComponent targetObject);
            var formations = HumanPlayer.SelectionController.Selections.Select(x => x.EffectedOn as Formation).Where(x => x != null).ToHashSet();
            var units = HumanPlayer.SelectionController.Selections.Select(x => x.EffectedOn as UnitBase).Where(x => x != null).ToHashSet();
            var buildings = HumanPlayer.SelectionController.Selections.Select(x => x.EffectedOn as BuildingBase).Where(x => x != null).ToHashSet();

            int i = 0;

            foreach (var formation in formations)
            {
                formation.SetTarget(GetGlobalMousePosition(), null);
            }

            foreach (var unit in units)
            {
                var hasFormationUnit = unit as IHasFormationUnit;
                if (targetObject != null)
                {
                    unit.SetTarget(
                        GetGlobalMousePosition(),
                        targetObject.EffectedOn as Node2D);

                    if (hasFormationUnit != null)
                    {
                        hasFormationUnit.AttachTo(targetObject.EffectedOn as Node2D);
                    }
                }
                else
                {
                    unit.SetTarget(GetGlobalMousePosition() + RandomExtension.GetRandomPointInCircle(i * 20), null);
                    
                    if (hasFormationUnit != null)
                    {
                        hasFormationUnit.Detach();
                    }
                }

                if (unit is Warrior warrior)
                {
                    warrior.AttachedToPoint = warrior.Target.Value;
                }

                if (unit is Lancer lancer)
                {
                    lancer.AttachedToPoint = lancer.Target.Value;
                }

                i++;
            }

            if (units.Count > 1
                && units.All(x => x.Meta.Id == units.First().Meta.Id && x is IHasFormationUnit formationUnit && formationUnit.Formation == null))
            {
                var formation = Formation.Create();
                foreach (var unit in units)
                {
                    formation.AddUnit(unit as IHasFormationUnit);
                }

                formation.Ready += () =>
                {
                    HumanPlayer.SelectionController.SelectFormation(formation);
                    InputMoveCommand(input);
                };
                return;
            }

            foreach (var building in buildings)
            {
                building.TargetToSpawnedUnits = GetGlobalMousePosition();
                building.TargetObjectToSpawnedUnits = targetObject?.EffectedOn is ResourceBase resource ? resource : null;
            }

            if (HumanPlayer.SelectionController.Selections.All(x => x.EffectedOn is UnitBase || x.EffectedOn is Formation))
            {
                HumanPlayer.SelectionController.ClearSelections();                
		        HumanPlayer.SelectionController.EmitSignal(SelectionController.SignalName.SelectionChanged);
            }
        }
    }

    private bool TryPointCastSelectable(out SelectableComponent collider)
    {
        collider = null;

        var spaceState = GetWorld2D().DirectSpaceState;
        var query = new PhysicsPointQueryParameters2D
        {
            Position = GetGlobalMousePosition(),
            CollideWithAreas = true,
            CollideWithBodies = false,
            CollisionMask = int.MaxValue ^ 256
        };

        var results = spaceState.IntersectPoint(query);

        foreach (var result in results)
        {
            var selectable = result["collider"].As<SelectableComponent>();
            if (selectable != null)
            {
                collider = selectable;
                return true;
            }
        }

        return false;
    }
}
