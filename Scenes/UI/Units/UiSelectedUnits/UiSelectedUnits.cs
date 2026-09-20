using Godot;

public partial class UiSelectedUnits : Control
{
    private HumanPlayer _humanPlayer;

    private NinePatchRect BGBanner;

    public HumanPlayer HumanPlayer
    {
        get
        {
            if (_humanPlayer == null)
            {
                _humanPlayer = GetParent().GetParent<HumanPlayer>();
            }

            return _humanPlayer;
        }
    }

    public override void _Ready()
    {
        BGBanner = GetNode<NinePatchRect>(nameof(BGBanner));

        var bannerRegionRect = BGBanner.RegionRect;
        int offset = 128;
        bannerRegionRect.Position = new Vector2(bannerRegionRect.Position.X, bannerRegionRect.Position.Y + offset * (int)HumanPlayer.Team);
        BGBanner.RegionRect = bannerRegionRect;
    }
}
