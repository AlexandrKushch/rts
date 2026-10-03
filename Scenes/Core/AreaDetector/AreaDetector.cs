using System.Collections.Generic;
using Godot;

public partial class AreaDetector<T> : Area2D
{
    public HashSet<T> Items { get; protected set; }

    public override void _Ready()
    {
        Items = new HashSet<T>();

        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
    }

    public virtual void OnBodyEntered(Node2D body)
    {
        if (body is T unit)
        {
            Items.Add(unit);
        }
    }
    
    public virtual void OnBodyExited(Node2D body)
    {
        if (body is T unit)
        {
            Items.Remove(unit);
        }
    }
}
