using Godot;
using Godot.Collections;

[GlobalClass]
public partial class UnitType : Resource
{
    [Export] public UnitTypeIds Id { get; set; }

    [Export] public string Name { get; set; }

    [Export] public Texture2D Icon { get; set; }
    
    [Export] public Dictionary<ResourceTypeIds, int> Cost { get; set; }
}
