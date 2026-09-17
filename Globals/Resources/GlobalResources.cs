using Godot;
using Godot.Collections;

public partial class GlobalResources : Node
{
    [Export] public Dictionary<UnitTypeIds, UnitType> Units { get; set; }
    [Export] public Dictionary<ResourceTypeIds, ResourceInfo> GatheringResources { get; set; }
    [Export] public Dictionary<BuildingTypeIds, BuildResource> Buildings { get; set; }
    [Export] public Dictionary<TeamType, TeamResource> Teams { get; set; }

    [Export] public Dictionary<UnitTypeIds, PackedScene> UnitScenes { get; set; }
    [Export] public Dictionary<BuildingTypeIds, PackedScene> BuildingScenes { get; set; }

    public static GlobalResources Instance { get; private set; }

    public override void _Ready()
    {
        if (!IsInstanceValid(Instance))
        {
            Instance = this;   
        }
    }
}
