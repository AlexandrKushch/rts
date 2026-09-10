using System.Linq;
using Godot;

public partial class UiBuildingsVBoxContainer : Control
{
    [Export]
    public PackedScene BuildingBlueprintScene;

    public override void _Ready()
    {
        var buildingItem = GetChild<UiBuildingItem>(0);

        foreach (var child in GetChildren())
        {
            child.QueueFree();
        }

        foreach (var building in GlobalResources.Instance.Buildings)
        {
            var newBuildingItem = buildingItem.Duplicate() as UiBuildingItem;
            newBuildingItem.Resource = building.Value;
            AddChild(newBuildingItem);
        }
    }
}
