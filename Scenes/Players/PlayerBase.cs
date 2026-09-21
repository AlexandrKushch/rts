using Godot;

public partial class PlayerBase : Node
{
    [Export] public TeamType Team { get; set; }

    private PlayerGlobalResources _globalResources;
    public PlayerGlobalResources GlobalResources
    {
        get
        {
            if (_globalResources == null)
            {
                _globalResources = GetNode<PlayerGlobalResources>(nameof(GlobalResources));
            }

            return _globalResources;
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
    
    private BuildingController _buildingController;
    public BuildingController BuildingController
    {
        get
        {
            if (_buildingController == null)
            {
                _buildingController = GetNode<BuildingController>(nameof(BuildingController));
            }

            return _buildingController;
        }
    }
}
