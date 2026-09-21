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
    
    private ResourceController _resourceController;
    public ResourceController ResourceController
    {
        get
        {
            if (_resourceController == null)
            {
                _resourceController = GetNode<ResourceController>(nameof(ResourceController));
            }

            return _resourceController;
        }
    }

    public override void _Ready()
    {
        base._Ready();
    }
}
