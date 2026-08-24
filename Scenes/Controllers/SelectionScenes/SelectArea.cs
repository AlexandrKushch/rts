using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class SelectArea : Area2D
{
    private const float SizePx = 64;

    private Vector2 _end;

    private Sprite2D Sprite2D;
    private CollisionShape2D CollisionShape2D;
    private RectangleShape2D CollisionRectangleShape2D;

    public Vector2 Start
    {
        get
        {
            return GlobalPosition;
        }
    }

    public Vector2 End
    {
        get
        {
            return _end;
        }
        set
        {
            _end = value;
            Sprite2D.Scale = (End - Start) / SizePx;
            CollisionRectangleShape2D.Size = (End - Start).Abs();
            CollisionShape2D.Position = (End - Start) / 2;
        }
    }

    public override void _Ready()
    {
        Sprite2D = GetNode<Sprite2D>(nameof(Sprite2D));
        CollisionShape2D = GetNode<CollisionShape2D>(nameof(CollisionShape2D));
        CollisionRectangleShape2D = CollisionShape2D.Shape as RectangleShape2D;
    }

    public HashSet<UnitBase> GetUnits()
    {
        return GetOverlappingBodies()
            .Where(x => x is UnitBase unit && unit != null)
            .Select(x => x as UnitBase)
            .ToHashSet();
    }
}
