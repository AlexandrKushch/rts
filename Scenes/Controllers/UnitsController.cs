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

    public void MoveToNodeCommand(UnitBase unit, Node2D targetObject)
    {
        unit.SetTarget(
            targetObject?.GlobalPosition ?? null,
            targetObject ?? null);
    }

    private void InputMoveCommand(InputEventMouseButton input)
    {
        if (input.IsReleased())
        {
            TryPointCastSelectable(out SelectableComponent targetObject);
            var units = HumanPlayer.SelectionController.Selections.Select(x => x.EffectedOn as UnitBase).Where(x => x != null).ToHashSet();

            int i = 0;
            foreach (var unit in units)
            {
                if (targetObject != null)
                {
                    MoveToNodeCommand(unit, targetObject.EffectedOn);
                }
                else
                {
                    unit.SetTarget(GetGlobalMousePosition() + RandomExtension.GetRandomPointInCircle(i * 20), null);
                }

                i++;
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
            CollideWithBodies = false
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
