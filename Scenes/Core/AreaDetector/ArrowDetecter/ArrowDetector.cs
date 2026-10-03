using System.Collections.Generic;
using Godot;

public partial class ArrowDetector : AreaDetector<HitBox>
{
    public override void _Ready()
    {
        Enemies = new HashSet<HitBox>();

        AreaEntered += OnBodyEntered;
        AreaExited += OnBodyExited;
    }

    public override void OnBodyEntered(Node2D body)
    {
        base.OnBodyEntered(body);
    }

}
