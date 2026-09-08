using System.Linq;
using Godot;

public partial class UiBuildingsVBoxContainer : Control
{
    [Export]
    public BuildResource[] AvailableBuildings { get; set; }

    [Export]
    public PackedScene BuildingBlueprintScene;

    public override void _Ready()
    {
        var buildingItem = GetChild<UiBuildingItem>(0);

        foreach (var child in GetChildren())
        {
            child.QueueFree();
        }

        foreach (var building in AvailableBuildings)
        {
            var newBuildingItem = buildingItem.Duplicate() as UiBuildingItem;
            newBuildingItem.Resource = building;
            AddChild(newBuildingItem);
        }
    }
}
