using System;
using Godot;

public partial class HurtBox : Area2D
{
    public UnitBase EffectedOn { get; set; }

    public override void _Ready()
    {
        var parent = GetParent();

        if (parent is not UnitBase)
        {
            throw new Exception($"HitBox should be attached to {nameof(UnitBase)}");
        }

        EffectedOn = parent as UnitBase;
    }
}
