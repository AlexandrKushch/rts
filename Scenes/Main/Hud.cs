using Godot;
using System;

public partial class Hud : CanvasLayer
{
    public static Hud Instance { get; private set; }

    public override void _Ready()
    {
        if (!IsInstanceValid(Instance))
        {
            Instance = this;
        }
    }

}
