using Godot;
using Godot.Collections;

public partial class BuildingBlueprint : Node2D
{
    public BuildingBase Building { get; private set; }

    public bool ValidToDeploy { get; private set; } = false;
    public BuildResource Resource { get; set; }

    [Export]
    public Dictionary<string, PackedScene> BuildingsDictionary { get; set; }

    public override void _Ready()
    {
        var itemScene = BuildingsDictionary[Resource.Name];

        if (itemScene != null)
        {
            Building = itemScene.Instantiate<BuildingBase>();
            AddChild(Building);
            SetAsBlueprint();
        }
    }

    public override void _Process(double delta)
    {
        GlobalPosition = GetGlobalMousePosition();

        if (NavigationRegionController.Instance.IsPointInside(GlobalPosition))
        {
            Building.Modulate = Colors.Green;
            ValidToDeploy = true;
        }
        else
        {
            Building.Modulate = Colors.Red;
            ValidToDeploy = false;
        }
    }

    public void DeployTo(Node2D to)
    {
        SetProcess(false);

        Building.Modulate = Colors.White;
        Building.SetProcess(true);
        Building.CollisionShape2D.Disabled = false;

        foreach (var obstacle in Building.Obstacles)
        {
            obstacle.AvoidanceEnabled = true;
        }
        Building.Reparent(to);
        NavigationRegionController.Instance.BakeNavigationPolygon(true);
    }

    public void SetAsBlueprint()
    {
        Building.SetProcess(false);
        Building.CollisionShape2D.Disabled = true;

        foreach (var obstacle in Building.Obstacles)
        {
            obstacle.AvoidanceEnabled = false;
        }
    }
}
