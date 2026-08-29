using Godot;

public partial class BuildingItem : Control
{
    [Export]
    private TextureRect BuildingIcon;

    public BuildResource Resource { get; set; }

    public void SetResource(BuildResource resource)
    {
        Resource = resource;
        BuildingIcon.Texture = Resource.Icon;
    }

    public void OnClick()
    {
        GD.Print(Resource.Name);
    }
}
