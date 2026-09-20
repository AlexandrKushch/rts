using Godot;

public partial class UiBuildingsVBoxContainer : Control
{
    [Export]
    public PackedScene BuildingBlueprintScene;

    public override void _Ready()
    {
        var humanPlayer = GetParent().GetParent<UiBuildingControl>().HumanPlayer;
        var buildingItem = GetChild<UiBuildingItem>(0);

        foreach (var child in GetChildren())
        {
            child.QueueFree();
        }

        foreach (var building in humanPlayer.GlobalResources.Buildings)
        {
            var newBuildingItem = buildingItem.Duplicate() as UiBuildingItem;
            newBuildingItem.Resource = building.Value;
            newBuildingItem.Team = humanPlayer.Team;
            AddChild(newBuildingItem);
        }
    }
}
