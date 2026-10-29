using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Godot;

public partial class ShelterBuildingBase : BuildingBase
{
    private int _mag = 1;
    private int _maxMag = 1;

    private UnitBase _target;
    private HashSet<PackedScene> _unitPackedScenes;
    private List<Node2D> _unitTargetObjects;

    private float _radius;
    private double _attackReloadTimer = 0.0f;

    public bool Reloaded { get; private set; } = true;
    public AllyDetector AllyDetector { get; private set; }
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

        _unitPackedScenes = new HashSet<PackedScene>();
        _unitTargetObjects = new List<Node2D>();

        AllyDetector = GetNode<AllyDetector>(nameof(AllyDetector));
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

        if (!Built)
        {
            return;
        }

        if (!IsInstanceValid(_target) && EnemyDetector.Items.Count > 0)
        {
            _target = EnemyDetector.Items.MinBy(x => x.GlobalPosition.DistanceTo(GlobalPosition)) as UnitBase;
        }

        if (Reloaded && _mag > 0 && IsInstanceValid(_target))
        {
            ShootArrow(ShootPoint.Position, _target.GlobalPosition);
        }
    }

    public void ConsumeUnit(Pawn pawn)
    {
        if (!Built)
        {
            return;
        }

        Node2D targetObjectToSave = pawn.TargetResource != null ? pawn.TargetResource : pawn.TargetBuilding;
        if (targetObjectToSave != null)
        {
            _unitTargetObjects.Add(targetObjectToSave);
        }

        var unitScene = new PackedScene();
        var result = unitScene.Pack(pawn);

        if (result != Error.Ok)
        {
            GD.PushError($"{pawn.Meta.Id} has error to be consumed by shelter");
        }

        _unitPackedScenes.Add(unitScene);
        pawn.QueueFree();
        UpdateMaxMag();
        UiUnitsCapacity.Capacity = _unitPackedScenes.Count;

        UiUnitsCapacity.Visible = !UnitsPie.Selected && UiUnitsCapacity.Capacity > 0;
    }

    protected override void OnSelectionChanged()
    {
        base.OnSelectionChanged();

        if (UiUnitsCapacity != null)
        {
            UiUnitsCapacity.Visible = !UnitsPie.Selected && UiUnitsCapacity.Capacity > 0;
        }
    }

    protected override void SetupAdditionalButtons()
    {
        base.SetupAdditionalButtons();
        AdditionalItems.Add(new UiButtonData
        {
            Icon = ResourceLoader.Load<Texture2D>("uid://dnf252vbnihiq"),
            Action = RingBell
        });
    }

    public void LetUnitsOut()
    {
        TryGetOpenSpaceAround(out var _, out var openSpacePoints);
        int index = 0;
        foreach (var unitScene in _unitPackedScenes)
        {
            var unit = unitScene.Instantiate<UnitBase>();
            unit.GlobalPosition = openSpacePoints[index];
            GlobalResources.Instance.World.AddChild(unit);
            
            if (index < _unitTargetObjects.Count
                && IsInstanceValid(_unitTargetObjects[index]))
            {
                unit.SetTarget(_unitTargetObjects[index].GlobalPosition, _unitTargetObjects[index]);
                // unit.CallDeferred(nameof(unit.SetTarget), _unitTargetObjects[index].GlobalPosition, _unitTargetObjects[index]);
            }
            
            index++;
        }

        _unitPackedScenes.Clear();
        _unitTargetObjects.Clear();

        UpdateMaxMag();
        UiUnitsCapacity.Capacity = _unitPackedScenes.Count;
    }

    private void RingBell()
    {
        if (_unitPackedScenes.Count == 0)
        {
            var pawns = AllyDetector.Items.Select(x => x as Pawn).Where(x => x != null).ToArray();
            foreach (var pawn in pawns)
            {
                pawn.MoveToShelter();
            }
        }
        else
        {
            LetUnitsOut();
        }
    }

    private void UpdateMaxMag()
    {
        _maxMag = (int)Mathf.Remap(_unitPackedScenes.Count, 0, MaxArrowsCount, 1, MaxArrowsCount);
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
