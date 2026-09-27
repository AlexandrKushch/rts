using Godot;
using System;

public partial class HitBox : Area2D
{
    private CollisionShape2D CollisionShape2D;

    public override void _Ready()
    {
        CollisionShape2D = GetNode<CollisionShape2D>(nameof(CollisionShape2D));
    }

    public void Disable()
    {
        CollisionShape2D.Disabled = true;
    }
}
