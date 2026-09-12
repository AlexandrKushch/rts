using Godot;

public partial class UiBuildingPopupItem : Control
{
    public TextureRect Icon { get; set; }

    public override void _Ready()
    {
        Icon = GetNode<TextureRect>(nameof(Icon));
    }
}
