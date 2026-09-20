using Godot;

public partial class UiResources : Control
{
    private NinePatchRect Banner;

    public override void _Ready()
    {
        Banner = GetNode<NinePatchRect>(nameof(Banner));

        var player = GetParent().GetParent<PlayerBase>();
        var bannerRegionRect = Banner.RegionRect;
        int offset = 128;
        bannerRegionRect.Position = new Vector2(bannerRegionRect.Position.X, bannerRegionRect.Position.Y + offset * (int)player.Team);
        Banner.RegionRect = bannerRegionRect;
    }
}
