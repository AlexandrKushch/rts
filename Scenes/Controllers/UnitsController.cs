using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class UnitsController : Node2D
{
    private const double ClickedTimer = 0.2f;

    private double _clickTimer = 0.0f;
    private SelectArea _selectArea;
    private MeshInstance2D _marker;
    [Export] private PackedScene SelectAreaScene;
    [Export] private PackedScene MarkerScene;

    [Signal] public delegate void SelectionChangedEventHandler();

    public HashSet<SelectableComponent> Selections { get; private set; } = new HashSet<SelectableComponent>();

    public static UnitsController Instance { get; private set; }

    public override void _Ready()
    {
        if (!IsInstanceValid(Instance))
        {
            Instance = this;
        }
    }

    public override void _UnhandledInput(InputEvent input)
    {
        if (BuildingController.Instance.BlueprintActive) return;

        if (input is InputEventMouseButton buttonInput)
        {
            if (buttonInput.ButtonIndex == MouseButton.Left)
            {
                SelectionInput(buttonInput);
            }
            if (buttonInput.ButtonIndex == MouseButton.Right)
            {
                InputMoveCommand(buttonInput);
            }
        }
    }

    public override void _Process(double delta)
    {
        SelectionProcess(delta);
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
        else if (IsInstanceValid(_selectArea))
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
            EmitSignal(SignalName.SelectionChanged);
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

    public void MoveToNodeCommand(UnitBase unit, Node2D targetObject)
    {
        unit.SetTarget(
            targetObject?.GlobalPosition ?? null,
            targetObject ?? null);
    }

    public void ClearUnitsExcept(UnitTypeIds unitId)
    {
        foreach (var unit in Selections)
        {
            unit.UpdateSelection(false);
        }

        Selections = Selections
            .Where(x => x.EffectedOn is UnitBase unit && unit.Meta.Id == unitId)
            .Where(x => x != null)
            .ToHashSet();

        foreach (var unit in Selections)
        {
            unit.UpdateSelection(true);
        }
        
        EmitSignal(SignalName.SelectionChanged);
    }

    private void InputMoveCommand(InputEventMouseButton input)
    {
        if (input.IsReleased())
        {
            TryPointCastSelectable(out SelectableComponent targetObject);
            var units = Selections.Select(x => x.EffectedOn as UnitBase).Where(x => x != null).ToHashSet();

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
