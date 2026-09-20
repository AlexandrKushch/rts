using Godot;
using System.Linq;

public partial class UiBuildingControl : Control
{
    private NinePatchRect Banner;
    private NinePatchRect Ribbon;
    private NinePatchRect RibbonClose;
    private ScrollContainer BuildingsScrollable;

    private HumanPlayer _humanPlayer;

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
        Banner = GetNode<NinePatchRect>(nameof(Banner));
        Ribbon = Banner.GetChild<NinePatchRect>(0);
        RibbonClose = Banner.GetChild<NinePatchRect>(1);
        BuildingsScrollable = GetNode<ScrollContainer>(nameof(BuildingsScrollable));

        var ribbonRegionRect = Ribbon.RegionRect;
        int offset = 128;
        float colorY = ribbonRegionRect.Position.Y + offset * (int)HumanPlayer.Team;
        ribbonRegionRect.Position = new Vector2(ribbonRegionRect.Position.X, colorY);
        Ribbon.RegionRect = ribbonRegionRect;

        var ribbonCloseRegionRect = RibbonClose.RegionRect;
        ribbonCloseRegionRect.Position = new Vector2(ribbonCloseRegionRect.Position.X, colorY);
        RibbonClose.RegionRect = ribbonCloseRegionRect;

        Visible = false;
        HumanPlayer.UnitsController.Connect(UnitsController.SignalName.SelectionChanged, Callable.From(UpdateUI));
    }

    private void UpdateUI()
    {
        bool oldVisible = Visible;
        var visible = HumanPlayer.UnitsController.Selections.Count > 0
            && HumanPlayer.UnitsController.Selections
                .All(x => x.EffectedOn is UnitBase unit && unit.Meta.Id == UnitTypeIds.Pawn);
        BuildingsScrollable.Visible = visible;

        if (oldVisible == visible)
        {
            return;
        }

        if (visible)
        {
            Visible = visible;
            var tweenObject = Banner;
            var tweenSize = CreateTween().SetTrans(Tween.TransitionType.Bounce);
            float tweenSizeDuration = 0.2f;
            tweenSize.TweenProperty(tweenObject, "scale", new Vector2(0.9f, 1.1f), tweenSizeDuration / 2);
            tweenSize.TweenProperty(tweenObject, "scale", Vector2.One, tweenSizeDuration / 2);
        }
        else
        {
            var tweenObject = Banner;
            var tweenSize = CreateTween().SetTrans(Tween.TransitionType.Bounce);
            float tweenSizeDuration = 0.2f;
            tweenSize.TweenProperty(tweenObject, "scale", new Vector2(0.7f, 0.9f), tweenSizeDuration / 2);
            tweenSize.Finished += () => { Visible = visible; };
        }
    }
}
