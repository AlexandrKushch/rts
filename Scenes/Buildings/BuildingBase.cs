using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class BuildingBase : StaticBody2D, IDestroyableWithHp
{
    private UiBuildingPopup UiBuildingPopup;
    private ProducingQueueManager ProducingQueueManager;

    public int MaxHp { get; set; }
    public int HP { get; set; }

    public Vector2[] SpaceAroundPoints { get; private set; }

    public bool Built { get; private set; } = false;
    public CollisionPolygon2D CollisionPolygon2D { get; private set; }
    public NavigationObstacle2D[] Obstacles { get; private set; }

    [Export] public BuildResource Resource { get; private set; }

    [Export] private PackedScene MarkerScene;

    public TeamType Team { get; set; }

    public override void _Ready()
    {
        UiBuildingPopup = GetNode<UiBuildingPopup>(nameof(UiBuildingPopup));
        ProducingQueueManager = GetNode<ProducingQueueManager>(nameof(ProducingQueueManager));
        CollisionPolygon2D = GetNode<CollisionPolygon2D>(nameof(CollisionPolygon2D));
        Obstacles = GetChildren().Where(x => x is NavigationObstacle2D).Select(x => x as NavigationObstacle2D).ToArray();

        MaxHp = Resource.MaxHp;

        ProducingQueueManager.ProgressComplete += SpawnUnit;
        UnitsController.Instance.SelectionChanged += OnSelectionChanged;
    }

    public void Deploy()
    {
        Modulate = Colors.White;
        SetProcess(true);
        CollisionPolygon2D.Disabled = false;

        foreach (var obstacle in Obstacles)
        {
            obstacle.AvoidanceEnabled = true;
        }

        SpaceAroundPoints = GetSpaceAround();
    }

    public void SpawnUnit(UnitTypeIds id)
    {
        TryGetOpenSpaceAround(out var occupiedPoints, out var openSpacePoints);

        var unit = GlobalResources.Instance.UnitScenes[id].Instantiate<UnitBase>();
        GD.Print(openSpacePoints[0]);
        unit.GlobalPosition = openSpacePoints[0];
        BuildingController.Instance.World.AddChild(unit);
    }

    public bool TryBuildProgressOne()
    {
        if (HP + 1 > MaxHp)
        {
            Built = true;
            return false;
        }

        HP++;
        return true;
    }

    public void TakeDamage(int value)
    {
        HP -= value;

        if (HP <= 0)
        {
            Destroy();
        }
    }

    public void Destroy()
    {
        QueueFree();
    }

    private void OnSelectionChanged()
    {
        bool wasSelected = UiBuildingPopup.Selected;
        UiBuildingPopup.Selected = UnitsController.Instance.Selections.Count == 1 && UnitsController.Instance.Selections.Any(x => x.EffectedOn == this);

        if (UiBuildingPopup.Selected)
        {
            UiBuildingPopup.Expand(wasSelected != UiBuildingPopup.Selected);
        }
        else
        {
            UiBuildingPopup.Colapse();
        }
    }

    private void TryGetOpenSpaceAround(out List<Vector2> occupiedPoints, out List<Vector2> openSpacePoints)
    {
        occupiedPoints = [];
        openSpacePoints = [];

        var spaceState = GetWorld2D().DirectSpaceState;
        var query = new PhysicsPointQueryParameters2D
        {
            CollideWithAreas = false,
            CollideWithBodies = true
        };

        foreach (var point in SpaceAroundPoints)
        {
            query.Position = ToGlobal(point);
            var results = spaceState.IntersectPoint(query);

            if (results.Count == 0)
            {
                openSpacePoints.Add(query.Position);
            }
            else
            {
                occupiedPoints.Add(query.Position);
            }
        }
    }

    private Vector2[] GetSpaceAround()
    {
        List<Vector2> aroundPoints = new List<Vector2>();
        var points = CollisionPolygon2D.Polygon.Select(x => x + Vector2.Zero.DirectionTo(x) * 45).ToArray();

        for (int i = 0; i < points.Length; i++)
        {
            var point = points[i];
            var nextPoint = points[(i + 1) % points.Length];
            var direction = point.DirectionTo(nextPoint);
            float step = 25;

            for (Vector2 start = point + direction * step; point.DistanceTo(start) < point.DistanceTo(nextPoint); start += direction * step)
            {
                var globalStart = ToGlobal(start);
                if (NavigationRegionController.Instance.IsPointInside(globalStart))
                {
                    aroundPoints.Add(start);
                    var marker = MarkerScene.Instantiate<Node2D>();
                    AddChild(marker);
                    marker.GlobalPosition = globalStart;
                }
            }
        }

        return aroundPoints.ToArray();
    }

    private bool IsEqualApprox(Vector2 a, Vector2 b, float tolerance = 0.1f)
    {
        if (Mathf.IsEqualApprox(a.X, b.X, tolerance))
        {
            return Mathf.IsEqualApprox(a.Y, b.Y, tolerance);
        }

        return false;
    }
}
