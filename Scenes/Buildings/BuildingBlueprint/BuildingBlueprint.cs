using System.Linq;
using Godot;

public partial class BuildingBlueprint : Node2D
{
    public BuildingBase Building { get; private set; }

    public bool ValidToDeploy { get; set; } = false;
    public bool Deployed { get; private set; } = false;

    public BuildResource Resource { get; set; }

    public TeamType Team { get; set; }

    public override void _Ready()
    {
        var tileToOccupie = GetNode<Sprite2D>(nameof(Sprite2D));

        int tileSize = Resource.RequiresOneTileToBuild
            ? BuildingController.TileSize / 2
            : BuildingController.TileSize;

        foreach (var tile in Resource.TileRequiresToBuild)
        {
            var newTile = tileToOccupie.Duplicate() as Sprite2D;
            newTile.Position = tile * tileSize;
            AddChild(newTile);
        }

        tileToOccupie.QueueFree();

        var itemScene = GlobalPlayers.Instance.Players[Team].GlobalResources.BuildingScenes[Resource.Id];

        if (itemScene != null)
        {
            Building = itemScene.Instantiate<BuildingBase>();
            Building.Resource = Resource;
            Building.Team = Team;
            AddChild(Building);
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
        NavigationRegionController.Instance.Bake();
        return true;
    }

    public void SetAsBlueprint()
    {
        Building.SetProcess(false);
        Building.CollisionPolygon2D.Disabled = true;
        Building.HurtBox.CollisionShape2D.Disabled = true;

        foreach (var obstacle in Building.Obstacles)
        {
            obstacle.AvoidanceEnabled = false;
        }
    }
}
