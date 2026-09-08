using Godot;

[GlobalClass]
public partial class ResourceInfo : Resource
{
    [Export]
    public ResourceType Type { get; set; }

    [Export]
    public int DefaultValue { get; set; }

    [Export]
    public string Name { get; set; }

    [Export]
    public Texture2D Icon { get; set; }
}