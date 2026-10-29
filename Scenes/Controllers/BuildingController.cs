using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class BuildingController : Node2D
{
    public const int TileSize = 64;

    private PlayerBase _player;
    private bool _showBuildingControl;
    private Control _buildControl;

    private BuildingBlueprint _blueprint;

    [Export] private PackedScene BuildingBlueprintScene;

    public bool BlueprintActive { get; private set; }

    public override void _Ready()
    {
        _player = GetParent<PlayerBase>();
    }

    public override void _UnhandledInput(InputEvent input)
    {
        if (!BlueprintActive) return;

        if (input is InputEventMouseButton inputButton)
        {
            if (inputButton.ButtonIndex == MouseButton.Left
                && !inputButton.Pressed
                && _blueprint.ValidToDeploy)
            {
                if (_blueprint.TryDeployTo(GlobalResources.Instance.World))
                {
                    var team = _blueprint.Building.Team;
                    var player = GlobalPlayers.Instance.Players[team] as HumanPlayer;
                    var units = player.SelectionController.Selections.Select(x => x.EffectedOn as UnitBase).ToHashSet();

                    foreach (var unit in units)
                    {
                        unit.SetTarget(
                            _blueprint.Building.GlobalPosition,
                            _blueprint.Building);
                    }

                    int tileSize = _blueprint.Building.Resource.RequiresOneTileToBuild
                        ? TileSize / 2
                        : TileSize;

                    foreach (var point in _blueprint.Building.Resource.TileRequiresToBuild)
                    {
                        foreach (var ground in GlobalResources.Instance.Grounds)
                        {
                            var posOnLayer = ground.LocalToMap(ground.ToLocal(_blueprint.Building.GlobalPosition + point * tileSize));
                            GlobalResources.Instance.OccupiedTiles.Add(posOnLayer);
                        }
                    }

                    _blueprint.QueueFree();
                    BlueprintActive = false;
                    GlobalResources.Instance.BuildingGrid.Visible = BlueprintActive;
                }
            }
            else if (inputButton.ButtonIndex == MouseButton.Right
                && !inputButton.Pressed)
            {
                _blueprint.QueueFree();
                BlueprintActive = false;
                GlobalResources.Instance.BuildingGrid.Visible = BlueprintActive;
                GetViewport().SetInputAsHandled();
            }
        }
    }

    public override void _Process(double delta)
    {
        if (IsInstanceValid(_blueprint) && !_blueprint.Deployed)
        {
            foreach (var ground in GlobalResources.Instance.Grounds)
            {
                var mousePosOnTileMap = ground.LocalToMap(GetLocalMousePosition());
                var mousePos = ToGlobal(ground.MapToLocal(mousePosOnTileMap));

                if (_blueprint.Building.Resource.RequiresOneTileToBuild)
                {
                    const float offset = TileSize / 2;
                    var direction = mousePos.DirectionTo(GetLocalMousePosition());
                    direction = new Vector2(Mathf.Sign(direction.X), Mathf.Sign(direction.Y));
                    mousePos += direction * offset;
                }

                float weight = 1f - Mathf.Exp(-25 * (float)delta);
                _blueprint.GlobalPosition = _blueprint.GlobalPosition.Lerp(mousePos, weight);

                if (CheckAbleToBuild(ground))
                {
                    _blueprint.Building.Modulate = Colors.Green;
                    _blueprint.ValidToDeploy = true;
                    break;
                }
                else
                {
                    _blueprint.Building.Modulate = Colors.Red;
                    _blueprint.ValidToDeploy = false;
                }
            }
        }
    }

    public void InitBuildingBlueprint(BuildResource resource, TeamType team)
    {
        if (IsInstanceValid(_blueprint))
        {
            _blueprint.QueueFree();
        }

        _blueprint = BuildingBlueprintScene.Instantiate<BuildingBlueprint>();
        _blueprint.Resource = resource;
        _blueprint.Team = _player.Team;
        GlobalResources.Instance.World.AddChild(_blueprint);

        _blueprint.Building.Team = team;

        BlueprintActive = true;
        GlobalResources.Instance.BuildingGrid.Visible = BlueprintActive;
    }

    private bool CheckAbleToBuild(TileMapLayer ground)
    {
        var spaceState = GetWorld2D().DirectSpaceState;
        var query = new PhysicsShapeQueryParameters2D
        {
            Transform = new Transform2D(0, _blueprint.GlobalPosition),
            Shape = new ConvexPolygonShape2D()
            {
                Points = _blueprint.Building.CollisionPolygon2D.Polygon
            },
            CollideWithAreas = false,
            CollideWithBodies = true
        };

        var results = spaceState.IntersectShape(query);
        var colliders = results.Select(x => x["collider"].AsGodotObject()).ToArray();
        if (colliders.Length > 0 && !colliders.Any(x => x == null))
        {
            return false;
        }

        var occupiedTilesData = new List<int>();

        int tileSize = _blueprint.Building.Resource.RequiresOneTileToBuild
            ? TileSize / 2
            : TileSize;

        foreach (var point in _blueprint.Building.Resource.TileRequiresToBuild)
        {
            var posOnLayer = ground.LocalToMap(ground.ToLocal(_blueprint.Building.GlobalPosition + point * tileSize));

            if (GlobalResources.Instance.OccupiedTiles.Contains(posOnLayer))
            {
                return false;
            }

            var cell = ground.GetCellTileData(posOnLayer);

            if (cell == null)
            {
                return false;
            }

            int buildLayer = cell.GetCustomData("build").AsInt16();

            if (buildLayer == 0)
            {
                return false;
            }

            occupiedTilesData.Add(buildLayer);
        }

        if (occupiedTilesData.Count == 0)
        {
            return false;
        }

        var tile = occupiedTilesData[0];

        if (!occupiedTilesData.All(x => x == tile))
        {
            return false;
        }

        return true;
    }
}
