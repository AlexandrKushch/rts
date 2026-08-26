using Globals;
using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class UnitsController : Node2D
{
    private SelectArea _selectArea;

    private HashSet<SelectableComponent> Selections = new HashSet<SelectableComponent>();

    [Export]
    private PackedScene SelectAreaScene;

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed(InputMapGlobal.Lmb))
        {
            ClearUnits();

            _selectArea = SelectAreaScene.Instantiate<SelectArea>();
            GetTree().Root.AddChild(_selectArea);

            _selectArea.GlobalPosition = GetGlobalMousePosition();
        }
        else if (Input.IsActionPressed(InputMapGlobal.Lmb))
        {
            _selectArea.End = GetGlobalMousePosition();
        }
        else if (Input.IsActionJustReleased(InputMapGlobal.Lmb))
        {
            SelectUnits();
            _selectArea.QueueFree();
        }

        if (Input.IsActionJustReleased(InputMapGlobal.Rmb))
        {
            TryPointCastSelectable(out Node2D targetObject);
            var units = Selections.Where(x => x.EffectedOn is UnitBase && x != null).Select(x => x.EffectedOn as UnitBase).ToHashSet();

            int i = 0;
            foreach (var unit in units)
            {
                unit.Target = GetGlobalMousePosition() + RandomExtension.GetRandomPointInCircle(i * 20);
                unit.TargetObject = targetObject;
                unit.UpdatePath();
                i++;
            }
        }
    }

    private void ClearUnits()
    {
        foreach (var unit in Selections)
        {
            unit.UpdateSelection(false);
        }

        Selections.Clear();
    }

    private void SelectUnits()
    {
        Selections = _selectArea.GetSelection();

        foreach (var unit in Selections)
        {
            unit.UpdateSelection(true);
        }
    }

    private bool TryPointCastSelectable(out Node2D collider)
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
                collider = selectable.EffectedOn;
                return true;
            }
        }

        return false;
    }
}
