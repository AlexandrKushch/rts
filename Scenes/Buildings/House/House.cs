using Godot;

public partial class House : BuildingBase
{
    public override void _Ready()
    {
        base._Ready();

        string name = $"{Resource.Name}{RandomExtension.RandomLong(3) + 1}".Capitalize().Replace(" ", string.Empty);
        Visual.Sprite2D.Texture = ResourceLoader.Load<Texture2D>($"{GlobalResources.Instance.Teams[Team].BuildingsPath}/{name}.png");
    }
}
