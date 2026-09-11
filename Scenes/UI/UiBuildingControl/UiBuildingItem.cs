using Godot;

public partial class UiBuildingItem : Control, IHasTooltip
{
    private Vector2 _highlightScale;

    private TextureRect Background { get; set; }
    private TextureRect Highlight { get; set; }
    private TextureRect Icon { get; set; }
    private Button Button { get; set; }

    public BuildResource Resource { get; set; }

    public override void _Ready()
    {
        Background = GetNode<TextureRect>(nameof(Background));
        Highlight = GetNode<TextureRect>(nameof(Highlight));
        Icon = GetNode<TextureRect>(nameof(Icon));
        Button = GetNode<Button>(nameof(Button));

        Highlight.Visible = false;
        _highlightScale = Highlight.Scale;

        if (Resource != null)
        {
            Icon.Texture = Resource.Icon;
        }

        Button.MouseEntered += OnMouseEntered;
        Button.MouseExited += OnMouseExited;
    }

    public void OnClick()
    {
        foreach (var cost in Resource.Cost)
        {
            if (!ResourceController.Instance.CheckSpent(cost.Key, cost.Value))
            {
                return;
            }
        }
        
        BuildingController.Instance.InitBuildingBlueprint(Resource);
        UpdateHighlight(false);
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

    public void OnMouseEntered()
    {
        UiCursorTooltip.Instance.UpdateVisibilityAndContent(
            true,
            new TooltipContent
            {
                Title = Resource.Name.Capitalize(),
                ResourcesCost = Resource.Cost
            });
    }

    public void OnMouseExited()
    {
        UiCursorTooltip.Instance.UpdateVisibilityAndContent(false);
    }
}
