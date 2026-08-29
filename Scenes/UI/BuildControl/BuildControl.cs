using Godot;

public partial class BuildControl : Control
{
    private NinePatchRect Background;

    private HBoxContainer Buildings;

    [Export]
    public BuildResource[] AvailableBuildings { get; set; }

    public override void _Ready()
    {
        Background = GetNode<NinePatchRect>(nameof(Background));
        Buildings = GetNode<HBoxContainer>(nameof(Buildings));

        int size = AvailableBuildings.Length;
        Size = new Vector2(Size.X * (size - 0.5f), Size.Y);

        var buildingItem = Buildings.GetChild<BuildingItem>(0);

        foreach (var building in AvailableBuildings)
        {
            var newBuildingItem = buildingItem.Duplicate() as BuildingItem;
            Buildings.AddChild(newBuildingItem);
            newBuildingItem.SetResource(building);
        }

        buildingItem.QueueFree();

        GlobalPosition = GetGlobalMousePosition() - Size / 2;

        var tween = CreateTween().SetTrans(Tween.TransitionType.Bounce);
        float tweenDuration = 0.25f;
        tween.TweenProperty(Background, "scale", new Vector2(1.2f, 0.8f), tweenDuration * 0.5f);
        tween.TweenProperty(Background, "scale", new Vector2(1, 1), tweenDuration * 0.5f);
    }

    public void OnBuldingChoosen(BuildResource building)
    {
        
    }
}
