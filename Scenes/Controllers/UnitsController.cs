using Globals;
using Godot;
using System.Collections.Generic;

public partial class UnitsController : Node2D
{
    private SelectArea _selectArea;

    private HashSet<UnitBase> SelectedUnits = new HashSet<UnitBase>();

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
            foreach (var unit in SelectedUnits)
            {
                unit.Target = GetGlobalMousePosition();
                unit.UpdatePath();
            }
        }
    }

    private void ClearUnits()
    {
        foreach (var unit in SelectedUnits)
        {
            unit.UpdateSelection(false);
        }

        SelectedUnits.Clear();        
    }

    private void SelectUnits()
    {
        SelectedUnits = _selectArea.GetUnits();
        
        foreach (var unit in SelectedUnits)
        {
            unit.UpdateSelection(true);
        }
    }
}
