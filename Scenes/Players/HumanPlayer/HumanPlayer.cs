using Godot;
using System;

public partial class HumanPlayer : PlayerBase
{    
    private SelectionController _selectionController;
    public SelectionController SelectionController
    {
        get
        {
            if (_selectionController == null)
            {
                _selectionController = GetNode<SelectionController>(nameof(SelectionController));
            }

            return _selectionController;
        }
    }

    public override void _Ready()
    {
        base._Ready();
    }
}
