using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class BuildingBase : StaticBody2D, IDestroyableWithHp, IHasTeam
{
    private bool _active = true;

    protected UiPie UnitsPie;
    protected UiPie AdditionalItemsPie;
    protected UiQueue UiQueue;

    protected List<UiButtonData> AdditionalItems;

    private PlayerBase _player;

    public int MaxHp { get; set; }
    public int HP { get; set; }

    public Vector2[] SpaceAroundPoints { get; private set; }

    private bool _built = false;
    public bool BuiltAndNotDamaged
    {
        get
        {
            return _built && HP >= MaxHp;
        }
        private set
        {
            _built = value;
        }
    }

    public bool Built
    {
        get
        {
            return _built;
        }
        private set
        {
            _built = value;
        }
    }
    public int BuildersCount { get; set; } = 0;
    public Vector2? TargetToSpawnedUnits { get; set; }
    public Node2D TargetObjectToSpawnedUnits { get; set; }

    public BuildingBaseVisual Visual { get; private set; }
    public CollisionPolygon2D CollisionPolygon2D { get; private set; }
    public HurtBox HurtBox { get; private set; }
    public NavigationObstacle2D[] Obstacles { get; private set; }
    public ProducingQueueManager ProducingQueueManager { get; private set; }

    [Export] public BuildResource Resource { get; set; }

    [Export] private PackedScene MarkerScene;
    [Export] private PackedScene UiQuableItemScene;

    [Export]
    public TeamType Team { get; set; }

    [Export]
    public bool Predeployed { get; set; } = false;

    public override void _Ready()
    {
        UnitsPie = GetNode<UiPie>(nameof(UnitsPie));
        AdditionalItemsPie = GetNode<UiPie>(nameof(AdditionalItemsPie));
        UiQueue = GetNode<UiQueue>(nameof(UiQueue));
        ProducingQueueManager = GetNode<ProducingQueueManager>(nameof(ProducingQueueManager));
        Visual = GetNode<BuildingBaseVisual>(nameof(Visual));
        CollisionPolygon2D = GetNode<CollisionPolygon2D>(nameof(CollisionPolygon2D));
        HurtBox = GetNode<HurtBox>(nameof(HurtBox));
        Obstacles = GetChildren().Where(x => x is NavigationObstacle2D).Select(x => x as NavigationObstacle2D).ToArray();

        Visual.Sprite2D.Texture = Resource.Icon;

        MaxHp = Resource.MaxHp;

        if (Predeployed)
        {
            Resource = GlobalPlayers.Instance.Players[Team].GlobalResources.Buildings[Resource.Id];
            Visual.Sprite2D.Texture = Resource.Icon;
            HP = Resource.MaxHp;
            Deploy();
            HaveBeenBuilt();
            Visual.UpdateProgress(Mathf.Remap(HP, 0, MaxHp, 0, 1));
        }

        SetupAdditionalButtons();

        UnitsPie.Visible = false;
        AdditionalItemsPie.Visible = false;
        UiQueue.Visible = false;
        UiQueue.ProducingQueueManager = ProducingQueueManager;
        UiQueue.ProducingQueueManager.ProgressComplete += UiQueue.ProgressComplete;

        foreach (var item in UnitsPie.GetChildren())
        {
            item.QueueFree();
        }
        foreach (var item in AdditionalItemsPie.GetChildren())
        {
            item.QueueFree();
        }
        foreach (var item in UiQueue.QueueContainer.GetChildren())
        {
            item.QueueFree();
        }
        foreach (var produce in Resource.Produces)
        {
            var pieItem = UiQuableItemScene.Instantiate<UiQuableItem>();
            pieItem.Id = produce.Id;
            var icon = pieItem.GetNode<TextureRect>("Icon");
            icon.Texture = produce.Icon;
            pieItem.ProducingQueueManager = ProducingQueueManager;
            UnitsPie.AddChild(pieItem);

            var queueItem = UiQuableItemScene.Instantiate<UiQuableItem>();
            queueItem.Id = produce.Id;
            queueItem.Readonly = true;
            var queueIcon = queueItem.GetNode<TextureRect>("Icon");
            queueIcon.Texture = produce.Icon;
            queueItem.ProducingQueueManager = ProducingQueueManager;
            UiQueue.QueueContainer.AddChild(queueItem);
        }
        var uiRoundButton = ResourceLoader.Load<PackedScene>("uid://dxcguemvfqkjc");
        foreach (var additionalItem in AdditionalItems)
        {
            var pieItem = uiRoundButton.Instantiate<Control>();
            var icon = pieItem.GetNode<TextureRect>("Icon");
            icon.Texture = additionalItem.Icon;
            pieItem.GetNode<UiButton>(nameof(UiButton)).ButtonDown += additionalItem.Action;
            AdditionalItemsPie.AddChild(pieItem);
        }

        ProducingQueueManager.ProgressComplete += SpawnUnit;
        Visual.DestroyParticles01.Finished += QueueFree;
        TreeExited += OnExitTree;
    }

    public void Deploy()
    {
        Modulate = Colors.White;
        SetProcess(true);
        CollisionPolygon2D.Disabled = false;
        HurtBox.CollisionShape2D.Disabled = false;

        foreach (var obstacle in Obstacles)
        {
            obstacle.AvoidanceEnabled = true;
        }

        SpaceAroundPoints = GetSpaceAround();

        Visual.UpdateProgress(0.0f);
        Visual.Deploy();

        _player = GlobalPlayers.Instance.Players[Team];
        if (_player is HumanPlayer humanPlayer)
        {
            humanPlayer.SelectionController.SelectionChanged += OnSelectionChanged;
        }
    }

    public void SpawnUnit(UnitTypeIds id)
    {
        TryGetOpenSpaceAround(out var _, out var openSpacePoints);

        var player = GlobalPlayers.Instance.Players[Team];
        var unit = player.GlobalResources.UnitScenes[id].Instantiate<UnitBase>();
        unit.GlobalPosition = openSpacePoints[0];
        unit.Target = TargetObjectToSpawnedUnits?.GlobalPosition ?? TargetToSpawnedUnits;
        unit.TargetObject = TargetObjectToSpawnedUnits;
        GlobalResources.Instance.World.AddChild(unit);
    }

    public bool TryBuildProgressOne()
    {
        HP++;

        if (Built)
        {
            Visual.UpdateFireParticles(HP, MaxHp);
        }
        else
        {
            Visual.UpdateProgress(Mathf.Remap(HP, 0, MaxHp, 0, 1));
        }

        if (HP >= MaxHp)
        {
            if (!Built)
            {
                HaveBeenBuilt();
            }
            return false;
        }

        return true;
    }

    public void TakeDamage(int value)
    {
        if (!_active)
        {
            return;
        }

        HP -= value;

        if (Built)
        {
            Visual.UpdateFireParticles(HP, MaxHp);
        }

        if (HP <= 0)
        {
            Destroy();
        }
    }

    public void TakeDamageWithDash(Vector2 from, int value, float power)
    {
        throw new NotImplementedException();
    }

    public override void _EnterTree()
    {
        base._EnterTree();

        if (IsNodeReady())
        {
            ProducingQueueManager.ProgressComplete += SpawnUnit;

            if (_player is HumanPlayer humanPlayer)
            {
                humanPlayer.SelectionController.SelectionChanged += OnSelectionChanged;
            }
        }
    }

    public override void _ExitTree()
    {
        base._ExitTree();

        ProducingQueueManager.ProgressComplete -= SpawnUnit;

        if (_player is HumanPlayer humanPlayer)
        {
            humanPlayer.SelectionController.SelectionChanged -= OnSelectionChanged;
        }
    }

    public void OnExitTree()
    {
        if (_active)
        {
            return;
        }
        NavigationRegionController.Instance.Bake();
    }

    public void Destroy()
    {
        if (IsInstanceValid(UnitsPie))
        {
            UnitsPie.QueueFree();
        }
        if (IsInstanceValid(AdditionalItemsPie))
        {
            AdditionalItemsPie.QueueFree();
        }
        if (IsInstanceValid(UiQueue))
        {
            UiQueue.QueueFree();
        }

        foreach (var point in Resource.TileRequiresToBuild)
        {
            foreach (var ground in GlobalResources.Instance.Grounds)
            {
                var posOnLayer = ground.LocalToMap(ground.ToLocal(GlobalPosition + point * BuildingController.TileSize));
                GlobalResources.Instance.OccupiedTiles.Remove(posOnLayer);
            }
        }

        // TO-do
        // check is on screen
        // Shake camera

        Visual.Destroy();
        _active = false;

        _player.ResourceController.Spent(ResourceTypeIds.UnitsCapacity, Resource.RestoreUnitsCapacity);
    }

    protected virtual void HaveBeenBuilt()
    {
        BuiltAndNotDamaged = true;
        HP = MaxHp;
        Visual.Built();
        OnSelectionChanged();

        _player.ResourceController.Collect(ResourceTypeIds.UnitsCapacity, Resource.RestoreUnitsCapacity);

        var bushesCount = SpaceAroundPoints.Length / 10;
        for (int i = 0; i < bushesCount; i++)
        {
            var pointIndex = RandomExtension.RandomLong(SpaceAroundPoints.Length);
            var point = SpaceAroundPoints[pointIndex];
            var bushIndex = RandomExtension.RandomLong(4);

            var bush = GlobalResources.Instance.DecorationScenes[(DecorationType)bushIndex].Instantiate<Node2D>();
            bush.GlobalPosition = GlobalPosition + point;
            GlobalResources.Instance.World.CallDeferred("add_child", bush);
        }
    }

    protected virtual void OnSelectionChanged()
    {
        if (!Built || !_active || _player is not HumanPlayer humanPlayer)
        {
            return;
        }

        UnitsPie.Selected = humanPlayer.SelectionController.Selections.Count == 1 && humanPlayer.SelectionController.Selections.Any(x => x.EffectedOn.GetInstanceId() == GetInstanceId());

        if (UnitsPie.Selected)
        {
            UnitsPie.Expand();
            AdditionalItemsPie.Expand();
            UiQueue.Colapse();
        }
        else
        {
            UnitsPie.Colapse();
            AdditionalItemsPie.Colapse();
            UiQueue.Expand();
        }
    }

    protected virtual void SetupAdditionalButtons()
    {
        AdditionalItems = new List<UiButtonData>
        {
            new UiButtonData
            {
                Icon = ResourceLoader.Load<Texture2D>("uid://cbrd3fpll0nn"),
                Action = Destroy
            }
        };
    }

    protected void TryGetOpenSpaceAround(out List<Vector2> occupiedPoints, out List<Vector2> openSpacePoints)
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
            var point = points[i] + CollisionPolygon2D.Position;
            var nextPoint = points[(i + 1) % points.Length] + CollisionPolygon2D.Position;
            var direction = point.DirectionTo(nextPoint);
            float step = 25;

            for (Vector2 start = point + direction * step; point.DistanceTo(start) < point.DistanceTo(nextPoint); start += direction * step)
            {
                var globalStart = ToGlobal(start);
                if (NavigationRegionController.Instance.IsPointInside(globalStart))
                {
                    aroundPoints.Add(start);
                    // var marker = MarkerScene.Instantiate<Node2D>();
                    // AddChild(marker);
                    // marker.GlobalPosition = globalStart;
                }
            }
        }

        return aroundPoints.ToArray();
    }
}
