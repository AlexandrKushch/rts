using Godot;
using System.Linq;

public partial class UiBuildingControl : Control
{
    private NinePatchRect Banner;
    private ScrollContainer BuildingsScrollable;

    public override void _Ready()
    {
        Banner = GetNode<NinePatchRect>(nameof(Banner));
        BuildingsScrollable = GetNode<ScrollContainer>(nameof(BuildingsScrollable));

        Visible = false;
        UnitsController.Instance.Connect(UnitsController.SignalName.SelectionChanged, Callable.From(UpdateUI));
    }

    private void UpdateUI()
    {
        var visible = UnitsController.Instance.Selections.Count > 0
            && UnitsController.Instance.Selections
                .All(x => x.EffectedOn is UnitBase unit && unit.Meta.Id == UnitTypeIds.Pawn);
        BuildingsScrollable.Visible = visible;

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
