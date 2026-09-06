using Godot;

public partial class UiSelectedUnitItem : Control
{
    private Vector2 _highlightScale;

    public int Id { get; set; }
    public TextureRect Highlight { get; set; }
    public TextureRect Icon { get; set; }
    public Label Count { get; set; }

    public override void _Ready()
    {
        Highlight = GetNode<TextureRect>(nameof(Highlight));
        Icon = GetNode<TextureRect>(nameof(Icon));
        Count = GetNode<Label>(nameof(Count));

        _highlightScale = Highlight.Scale;
    }

    public void UpdateHighlight(bool value)
    {
        var tweenScale = CreateTween().SetTrans(Tween.TransitionType.Bounce);
        var tweenRotation = CreateTween().SetTrans(Tween.TransitionType.Bounce);
        var tween = CreateTween().SetParallel();
        tween.TweenSubtween(tweenScale);
        tween.TweenSubtween(tweenRotation);
        float tweenShowDuration = 0.2f;
        float tweenHideDuration = 0.05f;

        if (value)
        {
            Highlight.Visible = true;
            tweenScale.TweenProperty(Highlight, "scale", _highlightScale * 1.2f, tweenShowDuration / 2);
            tweenScale.TweenProperty(Highlight, "scale", _highlightScale, tweenShowDuration / 2);
            tweenRotation.TweenProperty(Highlight, "rotation", (RandomExtension.RandomDouble() - 0.5f) * 0.3f, tweenShowDuration);
        }
        else
        {
            tweenScale.TweenProperty(Highlight, "scale", _highlightScale * 0.8f, tweenHideDuration);
            tweenRotation.TweenProperty(Highlight, "rotation", 0, tweenHideDuration);

            tween.Finished += () =>
            {
                Highlight.Visible = false;
            };
        }
    }

    public void OnClick()
    {
        if (Highlight.Visible)
        {
            UnitsController.Instance.ClearUnitsExcept(Id);
            UpdateHighlight(false);
        }
    }
}
