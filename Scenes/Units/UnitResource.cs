using Godot;

[GlobalClass]
public partial class UnitResource : Resource
{
    [Export]
    public string Name { get; set; }

    [Export]
    public Texture2D Icon { get; set; }
}
