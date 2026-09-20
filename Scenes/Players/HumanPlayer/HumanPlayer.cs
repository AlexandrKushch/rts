using Godot;
using System;

public partial class HumanPlayer : PlayerBase
{
    private UnitsController _unitsController;
    public UnitsController UnitsController
    {
        get
        {
            if (_unitsController == null)
            {
                _unitsController = GetNode<UnitsController>(nameof(UnitsController));
            }

            return _unitsController;
        }
    }

    public override void _Ready()
    {
        base._Ready();
    }
}
