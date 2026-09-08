using Godot;
using Godot.Collections;

[GlobalClass]
public partial class BuildResource : Resource
{
    [Export]
    public string Name { get; set; }

    [Export]
    public Texture2D Icon { get; set; }

    [Export]
    public int MaxHp { get; set; }
    
    [Export]
    public Dictionary<ResourceType, int> Cost { get; set; }
}
