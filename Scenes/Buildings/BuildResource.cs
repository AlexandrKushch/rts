using Godot;
using Godot.Collections;

[GlobalClass]
public partial class BuildResource : Resource
{
    [Export]
    public BuildingTypeIds Id { get; set; }
    
    [Export]
    public string Name { get; set; }

    [Export]
    public Texture2D Icon { get; set; }

    [Export]
    public int MaxHp { get; set; }

    [Export]
    public Dictionary<ResourceTypeIds, int> Cost { get; set; }

    [Export]
    public Vector2[] TileRequiresToBuild { get; set; } = [Vector2.Zero];

    [Export]
    public UnitType[] Produces { get; set; }
}
