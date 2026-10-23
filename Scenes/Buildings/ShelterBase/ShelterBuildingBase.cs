using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class ShelterBuildingBase : BuildingBase
{
    private int _mag = 1;
    private int _maxMag = 1;

    private UnitBase _target;
    private HashSet<UnitBase> _units;

    private float _radius;
    private double _attackReloadTimer = 0.0f;

    public bool Reloaded { get; private set; } = true;
    public EnemyDetector EnemyDetector { get; private set; }
    public Marker2D ShootPoint { get; private set; }
    public Timer ReloadArrowTimer { get; private set; }
    public Timer ReloadMagTimer { get; private set; }
    public UiUnitsCapacity UiUnitsCapacity { get; private set; }
    
    [Export] public PackedScene ArrowScene { get; private set; }
    [Export] public int MaxArrowsCount = 5;

    public override void _Ready()
    {
        base._Ready();

        _units = new HashSet<UnitBase>();

        EnemyDetector = GetNode<EnemyDetector>(nameof(EnemyDetector));
        ShootPoint = GetNode<Marker2D>(nameof(ShootPoint));
        ReloadArrowTimer = GetNode<Timer>(nameof(ReloadArrowTimer));
        ReloadMagTimer = GetNode<Timer>(nameof(ReloadMagTimer));
        UiUnitsCapacity = GetNode<UiUnitsCapacity>(nameof(UiUnitsCapacity));

        _radius = (EnemyDetector.GetNode<CollisionShape2D>(nameof(CollisionShape2D)).Shape as CircleShape2D).Radius;

        ReloadArrowTimer.Timeout += OnReloaded;
        ReloadMagTimer.Timeout += OnMagReloaded;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (!IsInstanceValid(_target) && EnemyDetector.Items.Count > 0)
        {
            _target = EnemyDetector.Items.MinBy(x => x.GlobalPosition.DistanceTo(GlobalPosition)) as UnitBase;
        }

        if (Reloaded && _mag > 0 && IsInstanceValid(_target))
        {
            ShootArrow(ShootPoint.Position, _target.GlobalPosition);
        }
    }

    public void ConsumeUnit(UnitBase unit)
    {
        if (!Built)
        {
            return;
        }

        _units.Add(unit);
        unit.QueueFree();
        UpdateMaxMag();
        UiUnitsCapacity.Capacity = _units.Count;
    }

    protected override void OnSelectionChanged()
    {
        base.OnSelectionChanged();

        if (UiUnitsCapacity != null && UiUnitsCapacity.Capacity > 0)
        {
            UiUnitsCapacity.Visible = !UiBuildingPopup.Selected;
        }
    }

    public void LetUnitsOut()
    {
        foreach (var unit in _units)
        {
            TryGetOpenSpaceAround(out var _, out var openSpacePoints);
            unit.GlobalPosition = openSpacePoints[0];
            unit.Target = TargetObjectToSpawnedUnits?.GlobalPosition ?? TargetToSpawnedUnits;
            unit.TargetObject = TargetObjectToSpawnedUnits;
            GlobalResources.Instance.World.AddChild(unit);
        }
        
        UpdateMaxMag();
        UiUnitsCapacity.Capacity = _units.Count;
    }

    private void UpdateMaxMag()
    {
        _maxMag = (int)Mathf.Remap(_units.Count, 0, MaxArrowsCount, 1, MaxArrowsCount);
    }

    private void ShootArrow(Vector2 shootPoint, Vector2 attackTargetPosition)
    {
        _mag--;
        
        if (_mag <= 0)
        {
            ReloadMagTimer.Start();
        }

        Reloaded = false;
        ReloadArrowTimer.Start();

        var arrowPath = ArrowScene.Instantiate<ArrowPath>();
        arrowPath.GlobalPosition = GlobalPosition;
        arrowPath.Team = Team;
        GlobalResources.Instance.World.AddChild(arrowPath);

        var changeShootPointPosition = shootPoint;
        changeShootPointPosition.X = Mathf.Abs(changeShootPointPosition.X);
        changeShootPointPosition.X = Mathf.Sign(attackTargetPosition.X - GlobalPosition.X) > 0
            ? changeShootPointPosition.X
            : changeShootPointPosition.X * -1;
        shootPoint = changeShootPointPosition;

        var randomness = Mathf.Remap(
            GlobalPosition.DistanceTo(attackTargetPosition),
            0,
            _radius,
            25,
            100);
        var randomTargetPosition = attackTargetPosition + RandomExtension.GetRandomPointInCircle(randomness);
        arrowPath.SetTarget(shootPoint, randomTargetPosition, _radius);
    }

    private void OnMagReloaded()
    {
        _mag = _maxMag;
    }

    private void OnReloaded()
    {
        Reloaded = true;
    }
}
