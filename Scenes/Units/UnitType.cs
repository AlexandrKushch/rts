using Godot;

[GlobalClass]
public partial class UnitType : Resource
{
    [Export] public int Id { get; set; }

    [Export] public string Name { get; set; }

    [Export] public Texture2D Icon { get; set; }
}
