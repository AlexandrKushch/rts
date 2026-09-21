using System.Linq;
using Godot;

public partial class BuildingBlueprint : Node2D
{
    public BuildingBase Building { get; private set; }

    public bool ValidToDeploy { get; set; } = false;
    public bool Deployed { get; private set; } = false;

    public BuildResource Resource { get; set; }

    public override void _Ready()
    {
        var itemScene = GlobalResources.Instance.BuildingScenes[Resource.Id];

        if (itemScene != null)
        {
            Building = itemScene.Instantiate<BuildingBase>();
            Building.Resource = Resource;
            AddChild(Building);
            Building.Visual.Texture = Resource.Icon;
            SetAsBlueprint();
        }
    }

    public bool TryDeployTo(Node2D to)
    {
        if (!GlobalPlayers.Instance.Players[Building.Team].ResourceController.TrySpentCost(Resource.Cost.ToDictionary()))
        {
            return false;
        }

        Deployed = true;
        Building.Deploy();
        Building.Reparent(to);
        NavigationRegionController.Instance.BakeNavigationPolygon(true);
        return true;
    }

    public void SetAsBlueprint()
    {
        Building.SetProcess(false);
        Building.CollisionPolygon2D.Disabled = true;

        foreach (var obstacle in Building.Obstacles)
        {
            obstacle.AvoidanceEnabled = false;
        }
    }
}
