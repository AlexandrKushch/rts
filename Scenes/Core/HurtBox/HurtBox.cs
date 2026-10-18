using System;
using Godot;

public partial class HurtBox : Area2D
{
    public IDestroyableWithHp EffectedOn { get; set; }

    public CollisionShape2D CollisionShape2D { get; private set; }

    public override void _Ready()
    {
        var parent = GetParent();

        if (parent is not IDestroyableWithHp)
        {
            throw new Exception($"HitBox should be attached to {nameof(IDestroyableWithHp)}");
        }

        EffectedOn = parent as IDestroyableWithHp;
        CollisionShape2D = GetNode<CollisionShape2D>(nameof(CollisionShape2D));
    }
}
