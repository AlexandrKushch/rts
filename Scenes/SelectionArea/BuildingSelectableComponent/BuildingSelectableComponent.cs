
using Godot;

public partial class BuildingSelectableComponent : SelectableComponent
{
    private Vector2 _originalScale;

    [Export]
    private NinePatchRect Visual;

    public override void _Ready()
    {
        base._Ready();
        _originalScale = Visual.Scale;
    }

    public override void UpdateSelection(bool value)
    {
        base.UpdateSelection(value);

        if (value)
        {
            var tween = CreateTween()
                .SetTrans(Tween.TransitionType.Expo);
            float tweenDuration = 0.15f;

            tween.TweenProperty(Visual, "scale", _originalScale + Vector2.One * _originalScale.Length() / 10, tweenDuration * 0.5f);
            tween.TweenProperty(Visual, "scale", _originalScale, tweenDuration * 0.5f);
        }
    }
}
