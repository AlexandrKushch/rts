using Globals;
using Godot;
using System.Linq;

public partial class BuildingController : Node2D
{
    private bool _showBuildingControl;
    private Control _buildControl;

    private BuildingBlueprint _blueprint;

    [Export] private PackedScene BuildControlScene;
    [Export] private PackedScene BuildingBlueprintScene;
    [Export] public Node2D World;
    [Export] public TileMapLayer Ground;

    public bool ShowBuildingControl
    {
        get
        {
            return _showBuildingControl;
        }
        set
        {
            _showBuildingControl = value;

            if (ShowBuildingControl)
            {
                _buildControl = BuildControlScene.Instantiate<Control>();
                Hud.Instance.AddChild(_buildControl);

            }
            else if (IsInstanceValid(_buildControl))
            {
                _buildControl.QueueFree();
            }
        }
    }

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
                var units = UnitsController.Instance.Selections.Select(x => x.EffectedOn as UnitBase).ToHashSet();

                foreach (var unit in units)
                {
                    UnitsController.Instance.MoveToNodeCommand(unit, _blueprint.Building);
                }

                _blueprint.DeployTo(World);
                _blueprint.QueueFree();
                BlueprintActive = false;
            }
            else if (inputButton.ButtonIndex == MouseButton.Right
                && !inputButton.Pressed)
            {
                _blueprint.QueueFree();
                BlueprintActive = false;
                GetViewport().SetInputAsHandled();
            }
        }
    }

    public override void _Process(double delta)
    {
        if (UnitsController.Instance.Selections.Count == 0)
        {
            ShowBuildingControl = false;
        }
        else if (Input.IsActionJustReleased(InputMapGlobal.BuildCommand)
            && UnitsController.Instance.Selections.Any(x => x.EffectedOn is Pawn))
        {
            ShowBuildingControl = !ShowBuildingControl;
        }

        if (IsInstanceValid(_blueprint) && !_blueprint.Deployed)
        {
            var mousePosOnTileMap = Ground.LocalToMap(GetLocalMousePosition());
            var mousePos = ToGlobal(Ground.MapToLocal(mousePosOnTileMap));
            float weight = 1f - Mathf.Exp(-25 * (float)delta);
            _blueprint.GlobalPosition = _blueprint.GlobalPosition.Lerp(mousePos, weight);

            if (_blueprint.Building.CollisionPolygon2D.Polygon.All(x => NavigationRegionController.Instance.IsPointInside(_blueprint.Building.GlobalPosition + x)))
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
        ShowBuildingControl = false;

        _blueprint = BuildingBlueprintScene.Instantiate<BuildingBlueprint>();
        _blueprint.Resource = resource;
        GetTree().Root.AddChild(_blueprint);

        BlueprintActive = true;
    }
}
