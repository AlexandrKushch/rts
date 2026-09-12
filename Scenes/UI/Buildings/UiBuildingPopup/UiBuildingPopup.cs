using System;
using Godot;

public partial class UiBuildingPopup : Control
{
    [Export] private PackedScene UiBuildingPopupItem;
    [Export] private float Radius;
    [Export] private int Count;
    [Export] private Texture2D CloseTexture;

    public override void _Ready()
    {
        float offset = Mathf.Min(180, 30 * Count) / Count;
        int halfCount = Count / 2;

        for (int i = -halfCount; i <= halfCount; i++)
        {
            if (Count % 2 == 0 && i == 0)
            {
                continue;
            }

            float index = Count % 2 == 0 ? i - 0.5f * Math.Sign(i) : i;
            float angle = Mathf.DegToRad((offset * index) - 90);
            var item = UiBuildingPopupItem.Instantiate<UiBuildingPopupItem>();
            AddChild(item);

            item.Position += new Vector2(Radius * Mathf.Cos(angle), Radius * Mathf.Sin(angle));
        }

        
        var closeItem = UiBuildingPopupItem.Instantiate<UiBuildingPopupItem>();
        AddChild(closeItem);
        closeItem.Icon.Texture = CloseTexture;
    }
}
