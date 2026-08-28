using Godot;

[GlobalClass]
public partial class ResourceType : Resource
{
    [Export]
    public string Name { get; set; }

    [Export]
    public string InstrumentName { get; set; }

    [Export]
    public Texture2D Icon { get; set; }
}