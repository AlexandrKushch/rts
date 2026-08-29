using Godot;
using System;

[GlobalClass]
public partial class BuildResource : Resource
{
    [Export]
    public string Name { get; set; }

    [Export]
    public Texture2D Icon { get; set; }
}
