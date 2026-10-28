using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class Formation : Node
{
    public const int MaxFormationUnitCount = 9;
    private const int UnitsOffset = 40;

    private readonly Vector2[] _directions = [
      new Vector2(-1, 0),
      new Vector2(1, 0),
      new Vector2(0, 1),
      new Vector2(0, -1),
      new Vector2(-1, -1),
      new Vector2(1, -1),
      new Vector2(-1, 1),
      new Vector2(1, 1)
    ];

    private SelectableComponent _unitSelectableComponent;
    public SelectableComponent UnitSelectableComponent
    {
        get
        {
            if (!IsInstanceValid(_unitSelectableComponent))
            {
                _unitSelectableComponent = GetNode<SelectableComponent>(nameof(UnitSelectableComponent));
            }
            return _unitSelectableComponent;
        }
        set
        {
            _unitSelectableComponent = value;
        }
    }

    public HashSet<IHasFormationUnit> Units { get; set; } = new HashSet<IHasFormationUnit>();

    public override void _Process(double delta)
    {
        var unitsPosition = Units.Select(x => (x as UnitBase).GlobalPosition).ToArray();

        Vector2 avgPosition = Vector2.Zero;

        foreach (var position in unitsPosition)
        {
            avgPosition += position;
        }

        avgPosition /= Units.Count;

        UnitSelectableComponent.GlobalPosition = avgPosition;
    }

    public void AddUnit(IHasFormationUnit unit)
    {
        if (Units.Count >= MaxFormationUnitCount)
        {
            return;
        }

        Units.Add(unit);
        unit.Formation = this;
    }

    public void RemoveUnit(IHasFormationUnit unit)
    {
        unit.Formation = null;
        Units.Remove(unit);

        if (Units.Count == 1)
        {
            Units.First().Formation = null;
            QueueFree();
        }
    }

    public void SetTarget(Vector2 target, Node2D targetObject)
    {
        if (!Check())
        {
            return;
        }

        if (targetObject is IHasFormationUnit hasFormation
            && targetObject.GetType() == Units.First().GetType())
        {
            CombineWith(target, hasFormation.Formation);
            hasFormation.Formation.SetTarget(target, null);
            return;
        }

        var firstUnit = Units.First() as UnitBase;
        firstUnit.SetTarget(target, targetObject);

        if (firstUnit is Warrior warrior)
        {
            warrior.AttachedToPoint = target;
        }

        var surroundUnits = Units.Skip(1).ToArray();
        var layersCount = (int)Mathf.Ceil(surroundUnits.Length / 8.0);

        for (int i = 0; i < surroundUnits.Length; i++)
        {
            var unit = surroundUnits[i] as UnitBase;
            var index = i % 8;

            var positionInFormation = _directions[index] * UnitsOffset * layersCount + target;

            unit.SetTarget(
                positionInFormation,
                targetObject);

            if (unit is Warrior warrior1)
            {
                warrior1.AttachedToPoint = positionInFormation;
            }
        }
    }

    private bool Check()
    {
        if (Units.Count == 0)
        {
            QueueFree();
            return false;
        }

        return true;
    }

    public static Formation Create()
    {
        var formation = ResourceLoader.Load<PackedScene>("uid://kn5afehh6n2a").Instantiate<Formation>();
        GlobalResources.Instance.World.CallDeferred("add_child", formation);
        return formation;
    }

    public void CombineWith(Vector2 target, Formation formation)
    {
        var units = Units.ToArray();

        foreach (var unit in units)
        {
            unit.Detach();
            unit.AttachTo(formation.Units.First() as Node2D);
        }
    }
}
