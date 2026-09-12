using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class BuildingController : Node2D
{
    private const int TileSize = 64;

    private bool _showBuildingControl;
    private Control _buildControl;

    private BuildingBlueprint _blueprint;

    [Export] private PackedScene BuildingBlueprintScene;
    [Export] public Node2D World;
    [Export] public TileMapLayer Ground;
    [Export] private Control BuildingGrid;

    public HashSet<Vector2> OccupiedTiles { get; private set; } = new HashSet<Vector2>();
    public bool BlueprintActive { get; private set; }

    public static BuildingController Instance { get; private set; }

    public override void _Ready()
    {
        if (!IsInstanceValid(Instance))
        {
            Instance = this;
        }
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
                if (_blueprint.TryDeployTo(World))
                {
                    var units = UnitsController.Instance.Selections.Select(x => x.EffectedOn as UnitBase).ToHashSet();

                    foreach (var unit in units)
                    {
                        UnitsController.Instance.MoveToNodeCommand(unit, _blueprint.Building);
                    }

                    foreach (var point in _blueprint.Building.Resource.TileRequiresToBuild)
                    {
                        var posOnLayer = Ground.LocalToMap(Ground.ToLocal(_blueprint.Building.GlobalPosition + point * TileSize));
                        OccupiedTiles.Add(posOnLayer);
                    }

                    _blueprint.QueueFree();
                    BlueprintActive = false;
                    BuildingGrid.Visible = BlueprintActive;
                }
            }
            else if (inputButton.ButtonIndex == MouseButton.Right
                && !inputButton.Pressed)
            {
                _blueprint.QueueFree();
                BlueprintActive = false;
                BuildingGrid.Visible = BlueprintActive;
                GetViewport().SetInputAsHandled();
            }
        }
    }

    public override void _Process(double delta)
    {
        if (IsInstanceValid(_blueprint) && !_blueprint.Deployed)
        {
            var mousePosOnTileMap = Ground.LocalToMap(GetLocalMousePosition());
            var mousePos = ToGlobal(Ground.MapToLocal(mousePosOnTileMap));
            float weight = 1f - Mathf.Exp(-25 * (float)delta);
            _blueprint.GlobalPosition = _blueprint.GlobalPosition.Lerp(mousePos, weight);

            if (CheckAbleToBuild())
            {
                _blueprint.Building.Modulate = Colors.Green;
                _blueprint.ValidToDeploy = true;
            }
            else
            {
                _blueprint.Building.Modulate = Colors.Red;
                _blueprint.ValidToDeploy = false;
            }
        }
    }

    public void InitBuildingBlueprint(BuildResource resource)
    {
        if (IsInstanceValid(_blueprint))
        {
            _blueprint.QueueFree();
        }

        _blueprint = BuildingBlueprintScene.Instantiate<BuildingBlueprint>();
        _blueprint.Resource = resource;
        World.AddChild(_blueprint);

        BlueprintActive = true;
        BuildingGrid.Visible = BlueprintActive;
    }

    private bool CheckAbleToBuild()
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

        foreach (var point in _blueprint.Building.Resource.TileRequiresToBuild)
        {
            var posOnLayer = Ground.LocalToMap(Ground.ToLocal(_blueprint.Building.GlobalPosition + point * TileSize));

            if (OccupiedTiles.Contains(posOnLayer))
            {
                return false;
            }

            var cell = Ground.GetCellTileData(posOnLayer);

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
