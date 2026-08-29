using Globals;
using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class UnitsController : Node2D
{
    private const double ClickedTimer = 0.2f;

    private double _clickTimer = 0.0f;
    private SelectArea _selectArea;
    private Control _buildControl;

    private HashSet<SelectableComponent> Selections = new HashSet<SelectableComponent>();

    public bool BuildOn { get; set; } = false;

    [Export]
    private PackedScene SelectAreaScene;

    [Export]
    private PackedScene BuildControlScene;

    public override void _UnhandledInput(InputEvent input)
    {
        if (input is InputEventMouseButton buttonInput)
        {
            if (buttonInput.ButtonIndex == MouseButton.Left)
            {
                SelectionInput(buttonInput);
            }
            if (buttonInput.ButtonIndex == MouseButton.Right)
            {
                MoveCommand(buttonInput);
            }
        }
    }

    public override void _Process(double delta)
    {
        SelectionProcess(delta);
        BuildCommand();
    }

    private void SelectionInput(InputEventMouseButton input)
    {
        if (input.Pressed)
        {
            ClearUnits();

            _selectArea = SelectAreaScene.Instantiate<SelectArea>();
            GetTree().Root.AddChild(_selectArea);

            _selectArea.GlobalPosition = GetGlobalMousePosition();

            _clickTimer = 0.0f;
        }
        else
        {
            if (_clickTimer <= ClickedTimer
                && _selectArea.Start.IsEqualApprox(_selectArea.End)
                && TryPointCastSelectable(out SelectableComponent selection))
            {
                Selections = new HashSet<SelectableComponent> { selection };
            }
            else
            {
                Selections = _selectArea.GetSelection();
            }

            SelectUnits();
            _selectArea.QueueFree();
        }
    }

    private void SelectionProcess(double delta)
    {
        if (IsInstanceValid(_selectArea))
        {
            _selectArea.End = GetGlobalMousePosition();

            _clickTimer += delta;
        }
    }

    private void MoveCommand(InputEventMouseButton input)
    {
        if (input.IsReleased())
        {
            TryPointCastSelectable(out SelectableComponent targetObject);
            var units = Selections.Select(x => x.EffectedOn as UnitBase).ToHashSet();

            int i = 0;
            foreach (var unit in units)
            {
                unit.Target = GetGlobalMousePosition() + RandomExtension.GetRandomPointInCircle(i * 20);
                unit.TargetObject = targetObject?.EffectedOn;
                unit.UpdatePath();

                if (unit is Pawn pawn
                    && targetObject != null)
                {
                    pawn.UpdateResource();
                }

                i++;
            }
        }
    }

    private void BuildCommand()
    {
        if (Selections.Count == 0)
        {
            BuildOn = false;
            if (IsInstanceValid(_buildControl))
            {
                _buildControl.QueueFree();
            }
            return;
        }

        if (Input.IsActionJustReleased(InputMapGlobal.BuildCommand))
        {
            BuildOn = !BuildOn;

            if (BuildOn)
            {
                _buildControl = BuildControlScene.Instantiate<Control>();
                Hud.Instance.AddChild(_buildControl);

            }
            else if (IsInstanceValid(_buildControl))
            {
                _buildControl.QueueFree();
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
        var hasUnits = Selections.Any(x => x.EffectedOn is UnitBase);

        if (hasUnits)
        {
            Selections = Selections.Where(x => x.EffectedOn is UnitBase && x != null).ToHashSet();
        }

        foreach (var unit in Selections)
        {
            unit.UpdateSelection(true);
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
