using Godot;

[GlobalClass]
public partial class ResourceType : Resource
{
    [Export]
    public int Id { get; set; }

    [Export]
    public int DefaultValue { get; set; }

    [Export]
    public string Name { get; set; }

    [Export]
    public Texture2D Icon { get; set; }

}