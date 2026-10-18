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
    
    private CustomMouseCursors _customMouseCursors;
    public CustomMouseCursors CustomMouseCursors
    {
        get
        {
            if (_customMouseCursors == null)
            {
                _customMouseCursors = GetNode<CustomMouseCursors>(nameof(CustomMouseCursors));
            }

            return _customMouseCursors;
        }
    }
    
    private CameraController _cameraController;
    public CameraController CameraController
    {
        get
        {
            if (_cameraController == null)
            {
                _cameraController = GetNode<CameraController>(nameof(CameraController));
            }

            return _cameraController;
        }
    }
}
