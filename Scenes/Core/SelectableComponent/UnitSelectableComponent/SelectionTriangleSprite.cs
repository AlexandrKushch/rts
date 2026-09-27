using Godot;

public partial class SelectionTriangleSprite : Sprite2D
{
    private Vector2 _origPosition;

    public override void _Ready()
    {
        _origPosition = Position;

        VisibilityChanged += UpdateVisible;
    }

    public void UpdateVisible()
    {
        if (Visible)
        {
            var tween = CreateTween().SetParallel();
            float tweenDuration = 0.2f;

            var positionTween = CreateTween().SetTrans(Tween.TransitionType.Bounce);
            positionTween.TweenProperty(this, "position", _origPosition + Vector2.Up * 15, tweenDuration * 0.5f);
            positionTween.TweenProperty(this, "position", _origPosition, tweenDuration * 0.5f);

            var scaleTween = CreateTween();
            scaleTween.TweenProperty(this, "scale", Vector2.One * 1.6f, tweenDuration * 0.2f);
            scaleTween.TweenProperty(this, "scale", Vector2.One, tweenDuration * 0.2f);

            tween.TweenSubtween(positionTween);
            tween.TweenSubtween(scaleTween);

            tween.Finished += () =>
            {
                var loopTween = CreateTween().SetLoops();
                float tweenDuration = 1.5f;

                loopTween.TweenProperty(this, "scale", Vector2.One * 1.1f, tweenDuration * 0.33f);
                loopTween.TweenProperty(this, "scale", Vector2.One * 0.9f, tweenDuration * 0.33f);
                loopTween.TweenProperty(this, "scale", Vector2.One, tweenDuration * 0.33f);
            };
        }
    }
}
